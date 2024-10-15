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

    /// <summary>
    /// This sanitizes the DateTime object by removing the DateTimeKind.
    /// This is necessary because the TimeZoneInfo.ConvertTimeBySystemTimeZoneId method
    /// does not work correctly with DateTimeKind.Local or DateTimeKind.Utc.
    /// Additionally, this method removes "Z" from the end of the DateTime string.
    /// </summary>
    private static DateTime SanitizeDateTime(DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
}
