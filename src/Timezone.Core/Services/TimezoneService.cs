namespace Timezone.Core.Services;

using Models;

public class TimezoneService : ITimezoneService
{
    public IEnumerable<TimeZoneInfo> GetTimezones() => TimeZoneInfo.GetSystemTimeZones();

    public TimeZoneInfo? GetTimezone(string timezoneId) => TimezoneHelper.GetTimezone(timezoneId);

    public TimeZoneConversionResultDomain ConvertFromOneTimezoneToAnother(TimeZoneConversionDomain request)
    {
        ValidateRequest(request);

        var conversionResult = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
            SanitizeDateTime(request.DateTime),
            request.FromTimezone.Id,
            request.ToTimezone.Id);

        return new TimeZoneConversionResultDomain { DateTime = SanitizeDateTime(conversionResult) };
    }

    private static void ValidateRequest(TimeZoneConversionDomain request)
    {
        if (request.DateTime == DateTime.MinValue)
        {
            throw new ArgumentException("DateTime is required.");
        }

        if (request.FromTimezone == null)
        {
            throw new ArgumentException("FromTimezone provided is invalid. Please provide a valid timezone.");
        }

        if (request.ToTimezone is null)
        {
            throw new ArgumentException("ToTimezone provided is invalid. Please provide a valid timezone.");
        }
    }

    public IEnumerable<TimezoneAbbreviationDomainModel> GetAllTimezoneAbbreviations()
    {
        var timeZones = TimeZoneInfo.GetSystemTimeZones();

        var daylightSavingsAbbreviations = timeZones
            .Where(x => x.DaylightName != x.StandardName)
            .Select(timezone =>
            {
                var rules = timezone.GetAdjustmentRules();

                foreach (var rule in rules)
                {
                    if (rule.DateEnd > DateTime.UtcNow)
                    {
                        return new TimezoneAbbreviationDomainModel
                        {
                            Abbreviation = timezone.DaylightName,
                            // based on https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo.adjustmentrule.daylightdelta?view=net-10.0#remarks
                            UtcOffset = timezone.BaseUtcOffset + rule.DaylightDelta + rule.BaseUtcOffsetDelta,
                            TimezoneId = timezone.Id
                        };
                    }
                }

                return null;
            })
            .Where(x => x != null);

        var nonDaylightSavingsAbbreviations = timeZones.Select(x =>
            new TimezoneAbbreviationDomainModel
            {
                Abbreviation = x.StandardName,
                UtcOffset = x.BaseUtcOffset,
                TimezoneId = x.Id
            });

        return nonDaylightSavingsAbbreviations.Concat(daylightSavingsAbbreviations)!;
    }

    /// <summary>
    /// This sanitizes the DateTime object by removing the DateTimeKind.
    /// This is necessary because the TimeZoneInfo.ConvertTimeBySystemTimeZoneId method
    /// does not work correctly with DateTimeKind.Local or DateTimeKind.Utc.
    /// Additionally, this method removes "Z" from the end of the DateTime string.
    /// </summary>
    private static DateTime SanitizeDateTime(DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
}
