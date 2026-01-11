namespace Timezone.Core.Models;

public class TimezoneResponse
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public TimeSpan BaseOffset { get; set; }

}
