namespace Timezone.FunctionalTests.StepDefinitions;

using System.Globalization;
using System.Text.Json;
using Core.Models;
using FluentAssertions;
using RestSharp;
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
        var request = new RestRequest("/convert", Method.Post) { RequestFormat = DataFormat.Json };

        request.AddBody(new TimeZoneConversionRequest
        {
            DateTime = DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture),
            FromTimezone = fromTimezone,
            ToTimezone = toTimezone
        });

        // Add to context
        Context.Set(request);
    }

    [Then(@"the response should correctly convert the time into '""([^""]*)""'")]
    public void ThenTheResponseShouldCorrectlyConvertTheTimeInto(string dateTime)
    {
        // Arrange
        var expected = DateTime.Parse(dateTime, CultureInfo.InvariantCulture);

        // Act
        var rawResponse = Context.Get<RestResponse>();
        var response = JsonSerializer.Deserialize<TimeZoneConversionResultResponse>
            (rawResponse.Content, JsonSerializerOptions);

        response.DateTime.Should().Be(expected);
    }
}
