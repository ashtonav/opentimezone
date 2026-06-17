namespace Timezone.FunctionalTests.StepDefinitions;

using System.Globalization;
using System.Text;
using System.Text.Json;
using Core.Models;
using NUnit.Framework;
using Support;

[Binding]
public class ConvertStepDefinitions(ScenarioContext context) : TestBase(context)
{
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [Given(@"I have a request to convert date '""([^""]*)""' from '""([^""]*)""' to '""([^""]*)""'")]
    public void GivenIHaveARequestToConvertDateFromTo(string dateTimeString, string fromTimezone, string toTimezone)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/convert")
        {
            Content = new StringContent(JsonSerializer.Serialize(new TimeZoneConversionRequest
            {
                DateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture),
                FromTimezone = fromTimezone,
                ToTimezone = toTimezone
            }), Encoding.UTF8, "application/json")
        };

        // Add to context
        Context.Set<HttpRequestMessage?>(request);
    }

    [Then(@"the response should correctly convert the time into '""([^""]*)""'")]
    public async Task ThenTheResponseShouldCorrectlyConvertTheTimeInto(string dateTime)
    {
        // Arrange
        var expected = DateTime.Parse(dateTime, CultureInfo.InvariantCulture);

        // Act
        var rawResponse = Context.Get<HttpResponseMessage?>();
        var response = JsonSerializer.Deserialize<TimeZoneConversionResultResponse>
            (await rawResponse.Content.ReadAsStringAsync(), JsonSerializerOptions);

        Assert.That(response.DateTime, Is.EqualTo(expected));
    }
}
