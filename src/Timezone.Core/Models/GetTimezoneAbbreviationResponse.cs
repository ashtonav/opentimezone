namespace Timezone.Core.Models;

public class GetTimezoneAbbreviationResponse
{
    public required IEnumerable<TimezoneAbbreviationResponse> Abbreviations { get; set; }
}
