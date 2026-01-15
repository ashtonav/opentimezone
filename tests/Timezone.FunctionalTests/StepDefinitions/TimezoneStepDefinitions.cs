namespace Timezone.FunctionalTests.StepDefinitions;

using System.Globalization;
using System.Text.Json;
using Core.Models;
using NUnit.Framework;
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
        Context.Set<RestRequest?>(request);
    }

    [Then(@"I should get exactly (.*) timezones")]
    public void ThenIShouldGetExactlyTimezones(int minimumExpectedNumberOfTimezones)
    {
        // Arrange
        var response = Context.Get<RestResponse?>();
        var listOfTimezones = JsonSerializer.Deserialize<IEnumerable<TimezoneResponse>>
            (response.Content, JsonSerializerOptions);

        // Act
        // Assert
        Assert.That(listOfTimezones.Count(), Is.EqualTo(minimumExpectedNumberOfTimezones));
    }

    [When(@"I send the request")]
    public void WhenISendTheRequest()
    {
        var request = Context.Get<RestRequest?>();
        var response = TestContainer.Client.Execute(request);
        // Add to context
        Context.Set<RestResponse?>(response);
    }

    [Given(@"I have a request to get '(.*)' timezone")]
    public void GivenIHaveARequestToGetTimezone(string timezoneName)
    {
        var request = new RestRequest($"/timezone/{timezoneName}");
        // Add to context
        Context.Set<RestRequest?>(request);
    }

    [Then(@"the response should return '(.*)' status code")]
    public void ThenTheResponseShouldReturnStatusCode(string statusCode)
    {
        // Arrange
        var response = Context.Get<RestResponse?>();
        var expectedStatusCode = int.Parse(statusCode, CultureInfo.InvariantCulture);
        var actualStatusCode = (int)response.StatusCode;

        // Assert
        Assert.That(actualStatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Then(@"the response should contain timezone information:")]
    public void ThenTheResponseShouldContainTimezoneInformation(Table table)
    {
        // Arrange
        var response = Context.Get<RestResponse?>();
        var expectedName = table.Rows[0]["Name"];
        var expectedOffset = int.Parse(table.Rows[0]["Offset"], CultureInfo.InvariantCulture);

        // Act
        var actualTimezone = JsonSerializer.Deserialize<TimezoneResponse>
            (response.Content, JsonSerializerOptions);

        // Assert
        Assert.That(actualTimezone.DisplayName, Is.EqualTo(expectedName));
        Assert.That(actualTimezone.BaseOffset.Hours, Is.EqualTo(expectedOffset));
    }


    [Then(@"the list of timezone response should be:")]
    public void ThenTheListOfTimezoneResponseShouldBe(Table table)
    {
        // Arrange
        var response = Context.Get<RestResponse?>();
        var actual = JsonSerializer.Deserialize<IEnumerable<TimezoneResponse>>
            (response.Content, JsonSerializerOptions).ToList();

        var expected = table.Rows.Select(r => new TimezoneResponse
        {
            Id = r["Id"].Trim(),
            DisplayName = r["Name"].Trim(),
            BaseOffset = TimeSpan.Parse(r["Offset"].Trim(), CultureInfo.InvariantCulture)
        }).ToList();

        // Assert
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.That(actual[i].Id, Is.EqualTo(expected[i].Id));
            Assert.That(actual[i].DisplayName, Is.EqualTo(expected[i].DisplayName));
            Assert.That(actual[i].BaseOffset, Is.EqualTo(expected[i].BaseOffset));
        }
    }

}
