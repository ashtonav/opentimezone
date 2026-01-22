namespace Timezone.Core.Models;

public class TimezoneAbbreviationDomainModel
{
    public required string Abbreviation { get; set; }
    public required TimeSpan UtcOffset { get; set; }
    public required string TimezoneId { get; set; }
}
