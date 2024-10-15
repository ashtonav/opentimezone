namespace Timezone.Core.Services;

using Models;

public interface ITimezoneService
{
    TimeZoneConversionResultDomain ConvertFromOneTimezoneToAnother(TimeZoneConversionDomain request);
    IEnumerable<TimeZoneInfo> GetTimezones();
    TimeZoneInfo? GetTimezone(string timezoneId);
}
