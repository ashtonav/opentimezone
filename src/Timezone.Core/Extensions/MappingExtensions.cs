namespace Timezone.Core.Extensions;

using Models;
using Services;

public static class MappingExtensions
{
    public static TimeZoneConversionDomain ToDomain(this TimeZoneConversionRequest request) => new()
    {
        DateTime = request.DateTime,
        FromTimezone = TimezoneHelper.GetTimezone(request.FromTimezone),
        ToTimezone = TimezoneHelper.GetTimezone(request.ToTimezone)
    };

    public static TimezoneResponse ToResponse(this TimeZoneInfo timezone) => new()
    {
        Id = timezone.Id,
        DisplayName = timezone.DisplayName,
        BaseOffset = timezone.BaseUtcOffset
    };

    public static TimeZoneConversionResultResponse ToResponse(this TimeZoneConversionResultDomain domain) => new()
    {
        DateTime = domain.DateTime
    };

    public static IEnumerable<TimezoneResponse> ToResponse(this IEnumerable<TimeZoneInfo> timezones) =>
        timezones.Select(x => x.ToResponse());

    public static GetTimezoneAbbreviationResponse ToResponse(this IEnumerable<TimezoneAbbreviationDomainModel> items)
    {
        var timezoneAbbreviations = items
            .GroupBy(x => new { x.Abbreviation, x.UtcOffset })
            .Select(g => new TimezoneAbbreviationResponse
            {
                Abbreviation = g.Key.Abbreviation,
                UtcOffset = g.Key.UtcOffset,
                TimezoneIds =
                [
                    .. g
                        .Select(x => x.TimezoneId)
                        .Distinct(StringComparer.InvariantCultureIgnoreCase)
                        .OrderBy(id => id)
                ]
            })
            .OrderBy(x => x.UtcOffset);


        return new GetTimezoneAbbreviationResponse { Abbreviations = timezoneAbbreviations };
    }
}
