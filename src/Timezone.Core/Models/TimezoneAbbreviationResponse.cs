namespace Timezone.Core.Models;

public class TimezoneAbbreviationResponse
{
    public required string Abbreviation { get; set; }
    public required TimeSpan UtcOffset { get; set; }
    public required IEnumerable<string> TimezoneIds { get; set; }
}
