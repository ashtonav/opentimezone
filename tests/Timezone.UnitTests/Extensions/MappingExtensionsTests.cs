namespace Timezone.UnitTests.Extensions;
using NUnit.Framework;
using Timezone.Core.Extensions;
using Timezone.Core.Models;
using Timezone.Core.Services;

public class MappingExtensionTests
{
    [Test]
    public void TimeZoneConversionRequestToDomainMapsCorrectly()
    {
        // Arrange
        var request = new TimeZoneConversionRequest
        {
            DateTime = DateTime.Now,
            FromTimezone = "Australia/Sydney",
            ToTimezone = "Europe/London"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.DateTime, Is.EqualTo(request.DateTime));
            Assert.That(result.FromTimezone.Id, Is.EqualTo(TimezoneHelper.GetTimezone(request.FromTimezone).Id));
            Assert.That(result.ToTimezone.Id, Is.EqualTo(TimezoneHelper.GetTimezone(request.ToTimezone).Id));
        });
    }

    [Test]
    public void TimeZoneInfoToResponseMapsCorrectly()
    {
        // Arrange
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // Act
        var response = timezone.ToResponse();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(timezone.Id));
            Assert.That(response.DisplayName, Is.EqualTo(timezone.DisplayName));
            Assert.That(response.BaseOffset, Is.EqualTo(timezone.BaseUtcOffset));
        });
    }

    [Test]
    public void TimeZoneConversionResultDomainToResponseMapsCorrectly()
    {
        // Arrange
        var dateTime = DateTime.Now;
        var domain = new TimeZoneConversionResultDomain
        {
            DateTime = dateTime
        };

        // Act
        var response = domain.ToResponse();

        // Assert
        Assert.That(response.DateTime, Is.EqualTo(dateTime));
    }

    [Test]
    public void IEnumerableTimeZoneInfoToResponseMapsCorrectly()
    {
        // Arrange
        var timeZones = new List<TimeZoneInfo>
        {
            TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"),
            TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
        };

        // Act
        var response = timeZones.ToResponse().ToList();

        // Assert
        Assert.That(response, Has.Count.EqualTo(timeZones.Count));
        for (var i = 0; i < response.Count; i++)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response[i].Id, Is.EqualTo(timeZones[i].Id));
                Assert.That(response[i].DisplayName, Is.EqualTo(timeZones[i].DisplayName));
                Assert.That(response[i].BaseOffset, Is.EqualTo(timeZones[i].BaseUtcOffset));
            });
        }
    }

    [Test]
    public void EmptyTimeZoneInfoListToResponse()
    {
        // Arrange
        var timezones = new List<TimeZoneInfo>();

        // Act
        var results = timezones.ToResponse();

        // Assert
        Assert.That(results, Is.Not.Null);
        Assert.That(results.Any(), Is.False);
    }

    [Test]
    public void ToDomainMapsCorrectlyWithValidTimezones()
    {
        // Arrange
        var request = new TimeZoneConversionRequest
        {
            DateTime = DateTime.Now,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.DateTime, Is.EqualTo(request.DateTime));
            Assert.That(result.FromTimezone.StandardName, Is.EqualTo(TimezoneHelper.GetTimezone(request.FromTimezone).StandardName));
            Assert.That(result.ToTimezone.StandardName, Is.EqualTo(TimezoneHelper.GetTimezone(request.ToTimezone).StandardName));
        });
    }

    [Test]
    public void MappingPreservesDateTimeKindUnspecified()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var request = new TimeZoneConversionRequest
        {
            DateTime = dateTime,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.DateTime.Kind, Is.EqualTo(DateTimeKind.Unspecified));
    }

    [Test]
    public void MappingPreservesDateTimeKindUtc()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Utc);
        var request = new TimeZoneConversionRequest
        {
            DateTime = dateTime,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.DateTime.Kind, Is.EqualTo(DateTimeKind.Utc));
    }

    [Test]
    public void MappingPreservesDateTimeKindLocal()
    {
        // Arrange
        var dateTime = new DateTime(2023, 5, 15, 0, 0, 0, DateTimeKind.Local);
        var request = new TimeZoneConversionRequest
        {
            DateTime = dateTime,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.DateTime.Kind, Is.EqualTo(DateTimeKind.Local));
    }

    [Test]
    public void MappingHandlesDateTimeMinValue()
    {
        // Arrange
        var request = new TimeZoneConversionRequest
        {
            DateTime = DateTime.MinValue,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.DateTime, Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void MappingHandlesDateTimeMaxValue()
    {
        // Arrange
        var request = new TimeZoneConversionRequest
        {
            DateTime = DateTime.MaxValue,
            FromTimezone = "GMT Standard Time",
            ToTimezone = "Asia/Kolkata"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result.DateTime, Is.EqualTo(DateTime.MaxValue));
    }
}
