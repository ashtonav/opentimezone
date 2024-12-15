namespace Timezone.UnitTests.Services;

using Core.Models;
using Moq;
using NUnit.Framework;
using Timezone.Core.Services;

[TestFixture]
public class TimezoneServiceTests
{
    private Mock<ITimezoneService> _mockTimezoneService;
    private TimezoneService _timezoneService;

    [SetUp]
    public void SetUp()
    {
        _mockTimezoneService = new Mock<ITimezoneService>();
        _timezoneService = new TimezoneService();
    }

    [Test]
    public void GetTimezonesReturnsCorrectResult()
    {
        // Act
        var result = _timezoneService.GetTimezones();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public void GetTimezoneReturnsCorrectResult()
    {
        // Arrange
        var timezoneId = "UTC";

        // Act
        var result = _timezoneService.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(timezoneId));
    }

    [Test]
    public void GetTimezonesReturnsListOfTimeZones()
    {
        // Arrange
        var expected = TimeZoneInfo.GetSystemTimeZones();
        _mockTimezoneService.Setup(service => service.GetTimezones()).Returns(expected);

        // Act
        var timezones = _timezoneService.GetTimezones();

        // Assert
        Assert.That(timezones, Is.Not.Null, "Expected a non-null object");
        Assert.That(timezones.Count, Is.GreaterThan(0), "Expected more than zero time zones");
    }

    [Test]
    public void GetTimezoneReturnsCorrectTimeZone()
    {
        // Arrange
        var timezoneId = TimeZoneInfo.Local.Id;
        _mockTimezoneService.Setup(service => service.GetTimezone(timezoneId)).Returns(TimeZoneInfo.Local);

        // Act
        var timeZoneInfo = _timezoneService.GetTimezone(timezoneId);

        // Assert
        Assert.That(timeZoneInfo.Id, Is.EqualTo(timezoneId), "Expected timezone ID to match");
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherReturnsConvertedTime()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Utc
        };

        var expected = request.DateTime.ToUniversalTime();

        // Act
        var response = _timezoneService.ConvertFromOneTimezoneToAnother(request);

        // Assert
        Assert.That(response.DateTime, Is.EqualTo(expected));
    }

    [Test]
    public void ValidateRequestThrowsExceptionWhenDateTimeIsMinValue()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.MinValue,
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Utc
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }

    [Test]
    public void ValidateRequestThrowsExceptionWhenFromTimezoneIsNull()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = null,
            ToTimezone = TimeZoneInfo.Utc
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }

    [Test]
    public void ValidateRequestThrowsExceptionWhenToTimezoneIsNull()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = null
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }

    [Test]
    public void GetTimezoneReturnsNullForInvalidTimezoneId()
    {
        // Arrange
        var invalidTimezoneId = "Invalid Timezone Id";

        // Act
        var result = _timezoneService.GetTimezone(invalidTimezoneId);

        // Assert
        Assert.That(result, Is.Null);
    }


    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsArgumentExceptionForInvalidFromTimeZone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = TimeZoneInfo.FindSystemTimeZoneById("Not a Timezone ID"),
                ToTimezone = TimeZoneInfo.Utc
            };

            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsArgumentExceptionForInvalidToTimeZone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = TimeZoneInfo.Local,
                ToTimezone = TimeZoneInfo.FindSystemTimeZoneById("Not a Timezone ID")
            };
            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void ConvertFromOneTimezoneToAnotherConvertsCorrectlyWhenTimezonesAreSame()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Local // Same as FromTimezone
        };

        // Act
        var response = _timezoneService.ConvertFromOneTimezoneToAnother(request);

        // Assert
        // Since the FromTimezone and ToTimezone are the same, there shouldn't be any conversion.
        Assert.That(response.DateTime, Is.EqualTo(SanitizeDateTime(request.DateTime)));
    }

    private static DateTime SanitizeDateTime(DateTime dateTime) => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

    [Test]
    public void GetTimezoneReturnsNullForInvalidTimezone()
    {
        // Arrange
        var result = _timezoneService.GetTimezone("Invalid Timezone");
        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsForInvalidFromTimezone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var invalidFromTimezone = TimeZoneInfo.FindSystemTimeZoneById("Invalid Timezone");

            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = invalidFromTimezone,
                ToTimezone = TimeZoneInfo.Utc
            };

            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsForInvalidToTimezone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var invalidToTimezone = TimeZoneInfo.FindSystemTimeZoneById("Invalid Timezone");

            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = TimeZoneInfo.Local,
                ToTimezone = invalidToTimezone
            };
            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void GetTimezonesReturnsExpectedTimezones()
    {
        // Arrange
        var expectedTimezones = TimeZoneInfo.GetSystemTimeZones();

        // Act
        var timezones = _timezoneService.GetTimezones();

        // Assert
        Assert.That(timezones, Is.EquivalentTo(expectedTimezones));
    }

    [Test]
    public void GetTimezoneReturnsNullForNonExistingTimeZone()
    {
        // Act
        var result = _timezoneService.GetTimezone("NonExisting_TimeZone");
        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsExceptionForNonExistingFromTimeZone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = TimeZoneInfo.FindSystemTimeZoneById("NonExisting_TimeZone"),
                ToTimezone = TimeZoneInfo.Utc
            };

            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsExceptionForNonExistingToTimeZone() =>
        Assert.Throws<TimeZoneNotFoundException>(() =>
        {
            var request = new TimeZoneConversionDomain
            {
                DateTime = DateTime.Now,
                FromTimezone = TimeZoneInfo.Local,
                ToTimezone = TimeZoneInfo.FindSystemTimeZoneById("NonExisting_TimeZone")
            };
            _timezoneService.ConvertFromOneTimezoneToAnother(request);
        });

    [Test]
    public void ConvertFromOneTimezoneToAnotherConvertsCorrectlyForFarFutureDates()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now.AddYears(1000), // DateTime far in the future
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Utc
        };

        // Act
        var response = _timezoneService.ConvertFromOneTimezoneToAnother(request);

        // Assert
        var expected = TimeZoneInfo.ConvertTimeToUtc(request.DateTime);
        Assert.That(response.DateTime, Is.EqualTo(expected));
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherConvertsCorrectlyForFarPastDates()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now.AddYears(-1000), // DateTime far in the past
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Utc
        };

        // Act
        var response = _timezoneService.ConvertFromOneTimezoneToAnother(request);

        // Assert
        var expected = TimeZoneInfo.ConvertTimeToUtc(request.DateTime);
        Assert.That(response.DateTime, Is.EqualTo(expected));
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherConvertsCorrectlyForNonUtcNonLocalTimezones()
    {
        // Arrange
        var fromTimezone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        var toTimezone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        var request = new TimeZoneConversionDomain
        {
            DateTime = new DateTime(2023, 1, 1, 12, 0, 0), // 12:00 PM
            FromTimezone = fromTimezone,
            ToTimezone = toTimezone
        };

        // Act
        var response = _timezoneService.ConvertFromOneTimezoneToAnother(request);

        // Assert
        var expected = TimeZoneInfo.ConvertTimeToUtc(request.DateTime, fromTimezone);
        expected = TimeZoneInfo.ConvertTimeFromUtc(expected, toTimezone);
        Assert.That(response.DateTime, Is.EqualTo(expected));
    }
    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsExceptionWhenDateTimeIsNull()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            // Null DateTime.
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = TimeZoneInfo.Utc
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsExceptionWhenFromTimezoneIsNull()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = null, // Null FromTimezone.
            ToTimezone = TimeZoneInfo.Utc
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }

    [Test]
    public void ConvertFromOneTimezoneToAnotherThrowsExceptionWhenToTimezoneIsNull()
    {
        // Arrange
        var request = new TimeZoneConversionDomain
        {
            DateTime = DateTime.Now,
            FromTimezone = TimeZoneInfo.Local,
            ToTimezone = null // Null ToTimezone.
        };

        // Assert
        Assert.Throws<ArgumentException>(() => _timezoneService.ConvertFromOneTimezoneToAnother(request));
    }
}
