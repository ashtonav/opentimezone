namespace Timezone.FunctionalTests.StepDefinitions;

using System.Globalization;
using System.Text.Json;
using Core.Models;
using NUnit.Framework;
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
        var request = new HttpRequestMessage(HttpMethod.Get, "/timezones");
        // Add to context
        Context.Set<HttpRequestMessage?>(request);
    }

    [Then(@"I should get exactly (.*) timezones")]
    public async Task ThenIShouldGetExactlyTimezones(int expectedNumberOfTimezones)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var listOfTimezones = JsonSerializer.Deserialize<IEnumerable<TimezoneResponse>>
            (await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        // Act
        // Assert
        Assert.That(listOfTimezones.Count(), Is.EqualTo(expectedNumberOfTimezones));
    }

    [When(@"I send the request")]
    public async Task WhenISendTheRequest()
    {
        var request = Context.Get<HttpRequestMessage?>();
        var response = await TestContainer.Client.SendAsync(request);
        // Add to context
        Context.Set<HttpResponseMessage?>(response);
    }

    [Given(@"I have a request to get '(.*)' timezone")]
    public void GivenIHaveARequestToGetTimezone(string timezoneName)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/timezone/{timezoneName}");
        // Add to context
        Context.Set<HttpRequestMessage?>(request);
    }

    [Then(@"the response should return '(.*)' status code")]
    public void ThenTheResponseShouldReturnStatusCode(string statusCode)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var expectedStatusCode = int.Parse(statusCode, CultureInfo.InvariantCulture);
        var actualStatusCode = (int)response.StatusCode;

        // Assert
        Assert.That(actualStatusCode, Is.EqualTo(expectedStatusCode));
    }

    [Then(@"the response should contain timezone information:")]
    public async Task ThenTheResponseShouldContainTimezoneInformation(Table table)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var expectedName = table.Rows[0]["Name"];
        var expectedOffset = int.Parse(table.Rows[0]["Offset"], CultureInfo.InvariantCulture);

        // Act
        var actualTimezone = JsonSerializer.Deserialize<TimezoneResponse>
            (await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        // Assert
        Assert.That(actualTimezone.DisplayName, Is.EqualTo(expectedName));
        Assert.That(actualTimezone.BaseOffset.Hours, Is.EqualTo(expectedOffset));
    }


    [Then(@"the list of timezone response should be:")]
    public async Task ThenTheListOfTimezoneResponseShouldBe(Table table)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var actual = JsonSerializer.Deserialize<IEnumerable<TimezoneResponse>>
            (await response.Content.ReadAsStringAsync(), JsonSerializerOptions).ToList();

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

    [Given("I have a request to get all timezone abbreviations with includeNumeric = false")]
    public void GivenIHaveARequestToGetAllTimezoneAbbreviationsWithIncludeNumericFalse()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "timezones/abbreviations/search?includeNumeric=false");
        Context.Set<HttpRequestMessage?>(request);
    }

    [Given("I have a request to get all timezone abbreviations")]
    public void GivenIHaveARequestToGetAllTimezoneAbbreviations()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/timezones/abbreviations");
        Context.Set<HttpRequestMessage?>(request);
    }

    [Then("I should get exactly (.*) timezone abbreviations")]
    public async Task ThenIShouldGetExactlyTimezoneAbbreviations(int expectedNumberOfTimezoneAbbreviations)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var listOfAbbreviations = JsonSerializer.Deserialize<GetTimezoneAbbreviationResponse>
            (await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        // Act
        // Assert
        Assert.That(listOfAbbreviations.Abbreviations.Count(), Is.EqualTo(expectedNumberOfTimezoneAbbreviations));
    }

    [Then("the list of timezone abbreviations response should be:")]
    public async Task ThenTheListOfTimezoneAbbreviationsResponseShouldBe(Table table)
    {
        // Arrange
        var response = Context.Get<HttpResponseMessage?>();
        var actual = JsonSerializer.Deserialize<GetTimezoneAbbreviationResponse>
            (await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

        var expected = table.Rows.Select(r => new TimezoneAbbreviationResponse
        {
            Abbreviation = r["Abbreviation"].Trim(),
            UtcOffset = TimeSpan.Parse(r["Offset"].Trim(), CultureInfo.InvariantCulture),
            TimezoneIds = [.. Array.ConvertAll(r["TimezoneIds"].Split(','), p => p.Trim())]
        }).ToList();

        // Assert
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.That(actual.Abbreviations.ElementAt(i).Abbreviation, Is.EqualTo(expected[i].Abbreviation));
            Assert.That(actual.Abbreviations.ElementAt(i).UtcOffset, Is.EqualTo(expected[i].UtcOffset));
            Assert.That(actual.Abbreviations.ElementAt(i).TimezoneIds, Is.EqualTo(expected[i].TimezoneIds));
        }
    }

    [Given("I have a request to retrieve timezone abbreviation '(.*)'")]
    public void GivenIHaveARequestToRetrieveTimezoneAbbreviation(string abbreviation)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/timezones/abbreviations/search?abbreviation={abbreviation}");
        Context.Set<HttpRequestMessage?>(request);
    }
}
