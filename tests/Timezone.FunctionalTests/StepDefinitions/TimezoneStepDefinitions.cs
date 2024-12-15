namespace Timezone.FunctionalTests.StepDefinitions;

using System.Globalization;
using System.Text.Json;
using Core.Models;
using FluentAssertions;
using RestSharp;
using Support;

[Binding]
public class TimezoneStepDefinitions(ScenarioContext context) : TestBase(context)
{
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNameCaseInsensitive = true,
    };

    [Given(@"I have a request to get all timezones")]
    public void GivenIHaveARequestToGetAllTimezones()
    {
        var request = new RestRequest("/timezones");
        // Add to context
        Context.Set(request);
    }

    [Then(@"I should get at least (.*) timezones")]
    public void ThenIShouldGetAtLeastTimezones(int minimumExpectedNumberOfTimezones)
    {
        var response = Context.Get<RestResponse>();

        // Act
        var listOfTimezones = JsonSerializer.Deserialize<IEnumerable<TimezoneResponse>>
            (response.Content, JsonSerializerOptions);

        // Assert
        listOfTimezones.Count().Should().BeGreaterOrEqualTo(minimumExpectedNumberOfTimezones);
    }

    [When(@"I send the request")]
    public void WhenISendTheRequest()
    {
        var request = Context.Get<RestRequest>();
        var response = Client.Execute(request);
        // Add to context
        Context.Set(response);
    }

    [Given(@"I have a request to get '(.*)' timezone")]
    public void GivenIHaveARequestToGetTimezone(string timezoneName)
    {
        var request = new RestRequest($"/timezone/{timezoneName}");
        // Add to context
        Context.Set(request);
    }

    [Then(@"the response should return '(.*)' status code")]
    public void ThenTheResponseShouldReturnStatusCode(string statusCode)
    {
        // Arrange
        var response = Context.Get<RestResponse>();
        var expectedStatusCode = int.Parse(statusCode, CultureInfo.InvariantCulture);
        var actualStatusCode = (int)response.StatusCode;

        // Assert
        expectedStatusCode.Should().Be(actualStatusCode);
    }

    [Then(@"the response should contain timezone information:")]
    public void ThenTheResponseShouldContainTimezoneInformation(Table table)
    {
        // Arrange
        var response = Context.Get<RestResponse>();
        var expectedName = table.Rows[0]["Name"];
        var expectedOffset = int.Parse(table.Rows[0]["Offset"], CultureInfo.InvariantCulture);

        // Act
        var actualTimezone = JsonSerializer.Deserialize<TimezoneResponse>
            (response.Content, JsonSerializerOptions);

        // Assert
        actualTimezone.DisplayName.Should().Be(expectedName);
        actualTimezone.BaseOffset.Hours.Should().Be(expectedOffset);
    }

}
