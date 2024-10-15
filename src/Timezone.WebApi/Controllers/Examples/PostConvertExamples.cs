namespace TimezoneWebApi.Controllers.Examples;
using Swashbuckle.AspNetCore.Filters;
using Timezone.Core.Models;

public class PostConvertExamples : IMultipleExamplesProvider<TimeZoneConversionRequest>
{
    public IEnumerable<SwaggerExample<TimeZoneConversionRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Example 1",
            "Example 1 - Convert UTC to New York Time",
            new TimeZoneConversionRequest
            {
                DateTime = TimeProvider.System.GetUtcNow().DateTime,
                FromTimezone = "UTC",
                ToTimezone = "America/New_York"
            });

        yield return SwaggerExample.Create(
            "Example 2",
            "Example 2 - Convert New York Time to UTC",
            new TimeZoneConversionRequest
            {
                DateTime = TimeZoneInfo.ConvertTime(TimeProvider.System.GetUtcNow().DateTime, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")),
                FromTimezone = "America/New_York",
                ToTimezone = "UTC"
            });

        yield return SwaggerExample.Create(
            "Example 3",
            "Example 3 - Convert from one timezone to another timezone",
            new TimeZoneConversionRequest
            {
                DateTime = TimeZoneInfo.ConvertTime(TimeProvider.System.GetUtcNow().DateTime, TimeZoneInfo.FindSystemTimeZoneById("America/New_York")),
                FromTimezone = "America/New_York",
                ToTimezone = "Europe/London"
            });
    }
}
