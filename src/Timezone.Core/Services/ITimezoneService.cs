namespace Timezone.Core.Services;

using Models;

public interface ITimezoneService
{
    public TimeZoneConversionResultDomain ConvertFromOneTimezoneToAnother(TimeZoneConversionDomain request);
    public IEnumerable<TimeZoneInfo> GetTimezones();
    public TimeZoneInfo? GetTimezone(string timezoneId);
    public IEnumerable<TimezoneAbbreviationDomainModel> GetAllTimezoneAbbreviations();
}
