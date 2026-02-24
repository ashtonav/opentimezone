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

        if (request.FromTimezone is null)
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
            .Select(CreateDaylightSavingsAbbreviation)
            .OfType<TimezoneAbbreviationDomainModel>();

        var nonDaylightSavingsAbbreviations = timeZones.Select(CreateNonDaylightSavingsAbbreviation);

        return nonDaylightSavingsAbbreviations.Concat(daylightSavingsAbbreviations);
    }

    private static TimezoneAbbreviationDomainModel? CreateDaylightSavingsAbbreviation(TimeZoneInfo timeZone)
    {
        var rule = timeZone.GetAdjustmentRules().FirstOrDefault(r => r.DateEnd > DateTime.UtcNow);
        if (rule is null)
        {
            return null;
        }

        return new TimezoneAbbreviationDomainModel
        {
            Abbreviation = timeZone.DaylightName,
            // based on https://learn.microsoft.com/en-us/dotnet/api/system.timezoneinfo.adjustmentrule.daylightdelta?view=net-10.0#remarks
            UtcOffset = timeZone.BaseUtcOffset + rule.DaylightDelta + rule.BaseUtcOffsetDelta,
            TimezoneId = timeZone.Id
        };
    }

    private static TimezoneAbbreviationDomainModel CreateNonDaylightSavingsAbbreviation(TimeZoneInfo timeZone) => new()
    {
        Abbreviation = timeZone.Equals(TimeZoneInfo.Utc) ? "UTC" : timeZone.StandardName,
        UtcOffset = timeZone.BaseUtcOffset,
        TimezoneId = timeZone.Id
    };

    /// <summary>
    /// This sanitizes the DateTime object by removing the DateTimeKind.
    /// This is necessary because the TimeZoneInfo.ConvertTimeBySystemTimeZoneId method
    /// does not work correctly with DateTimeKind.Local or DateTimeKind.Utc.
    /// Additionally, this method removes "Z" from the end of the DateTime string.
    /// </summary>
    private static DateTime SanitizeDateTime(DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
}
