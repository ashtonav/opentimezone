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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DateTime, Is.EqualTo(request.DateTime));
            Assert.That(result.FromTimezone.Id, Is.EqualTo(TimezoneHelper.GetTimezone(request.FromTimezone).Id));
            Assert.That(result.ToTimezone.Id, Is.EqualTo(TimezoneHelper.GetTimezone(request.ToTimezone).Id));
        }
    }

    [Test]
    public void TimeZoneInfoToResponseMapsCorrectly()
    {
        // Arrange
        var timezone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // Act
        var response = timezone.ToResponse();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response.Id, Is.EqualTo(timezone.Id));
            Assert.That(response.DisplayName, Is.EqualTo(timezone.DisplayName));
            Assert.That(response.BaseOffset, Is.EqualTo(timezone.BaseUtcOffset));
        }
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
            using (Assert.EnterMultipleScope())
            {
                Assert.That(response[i].Id, Is.EqualTo(timeZones[i].Id));
                Assert.That(response[i].DisplayName, Is.EqualTo(timeZones[i].DisplayName));
                Assert.That(response[i].BaseOffset, Is.EqualTo(timeZones[i].BaseUtcOffset));
            }
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.DateTime, Is.EqualTo(request.DateTime));
            Assert.That(result.FromTimezone.StandardName, Is.EqualTo(TimezoneHelper.GetTimezone(request.FromTimezone).StandardName));
            Assert.That(result.ToTimezone.StandardName, Is.EqualTo(TimezoneHelper.GetTimezone(request.ToTimezone).StandardName));
        }
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

    [Test]
    public void TimezoneAbbreviationDomainModelToResponseGroupsAndMapsCorrectly()
    {
        // Arrange
        var items = new List<TimezoneAbbreviationDomainModel>
        {
            new() {
                Abbreviation = "AEDT",
                UtcOffset = TimeSpan.FromHours(11),
                TimezoneId = "Australia/Sydney"
            },
            new() {
                Abbreviation = "AEDT",
                UtcOffset = TimeSpan.FromHours(11),
                TimezoneId = "Australia/Melbourne"
            },
            new() {
                Abbreviation = "AEDT",
                UtcOffset = TimeSpan.FromHours(11),
                TimezoneId = "Australia/Sydney" // duplicate should be removed
            },
        };

        // Act
        var response = items.ToResponse();
        var abbreviations = response.Abbreviations.ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(abbreviations, Has.Count.EqualTo(1));

            Assert.That(abbreviations[0].Abbreviation, Is.EqualTo("AEDT"));
            Assert.That(abbreviations[0].UtcOffset, Is.EqualTo(TimeSpan.FromHours(11)));

            // Distinct + filtered + ordered
            Assert.That(abbreviations[0].TimezoneIds.ToList(), Is.EqualTo(new List<string>
            {
                "Australia/Melbourne",
                "Australia/Sydney"
            }));
        }
    }

    [Test]
    public void TimezoneAbbreviationDomainModelToResponseDistinctsTimezoneIdsCaseInsensitive()
    {
        // Arrange
        var items = new List<TimezoneAbbreviationDomainModel>
        {
            new() {
                Abbreviation = "GMT",
                UtcOffset = TimeSpan.Zero,
                TimezoneId = "Europe/London"
            },
            new() {
                Abbreviation = "GMT",
                UtcOffset = TimeSpan.Zero,
                TimezoneId = "europe/london" // same id, different casing
            },
            new() {
                Abbreviation = "GMT",
                UtcOffset = TimeSpan.Zero,
                TimezoneId = "Etc/UTC"
            }
        };

        // Act
        var response = items.ToResponse();
        var abbreviation = response.Abbreviations.Single();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(abbreviation.Abbreviation, Is.EqualTo("GMT"));
            Assert.That(abbreviation.UtcOffset, Is.EqualTo(TimeSpan.Zero));

            // Case-insensitive distinct + ordered
            Assert.That(abbreviation.TimezoneIds.ToList(), Is.EqualTo(new List<string>
            {
                "Etc/UTC",
                "Europe/London"
            }));
        }
    }

    [Test]
    public void TimezoneAbbreviationDomainModelToResponseCreatesSeparateGroupsForSameAbbreviationDifferentOffsets()
    {
        // Arrange
        var items = new List<TimezoneAbbreviationDomainModel>
        {
            new() {
                Abbreviation = "CST",
                UtcOffset = TimeSpan.FromHours(-6),
                TimezoneId = "America/Chicago"
            },
            new() {
                Abbreviation = "CST",
                UtcOffset = TimeSpan.FromHours(8),
                TimezoneId = "Asia/Shanghai"
            }
        };

        // Act
        var response = items.ToResponse();
        var abbreviations = response.Abbreviations.ToList();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(abbreviations, Has.Count.EqualTo(2));

            // Ordered by UtcOffset (ascending)
            Assert.That(abbreviations[0].Abbreviation, Is.EqualTo("CST"));
            Assert.That(abbreviations[0].UtcOffset, Is.EqualTo(TimeSpan.FromHours(-6)));
            Assert.That(abbreviations[0].TimezoneIds.ToList(), Is.EqualTo(new List<string> { "America/Chicago" }));

            Assert.That(abbreviations[1].Abbreviation, Is.EqualTo("CST"));
            Assert.That(abbreviations[1].UtcOffset, Is.EqualTo(TimeSpan.FromHours(8)));
            Assert.That(abbreviations[1].TimezoneIds.ToList(), Is.EqualTo(new List<string> { "Asia/Shanghai" }));
        }
    }

    [Test]
    public void TimezoneAbbreviationDomainModelToResponseOrdersGroupsByUtcOffset()
    {
        // Arrange
        var items = new List<TimezoneAbbreviationDomainModel>
        {
            // Intentionally out of order
            new() {
                Abbreviation = "A",
                UtcOffset = TimeSpan.FromHours(10),
                TimezoneId = "Australia/Sydney"
            },
            new() {
                Abbreviation = "B",
                UtcOffset = TimeSpan.FromHours(-5),
                TimezoneId = "America/New_York"
            },
            new() {
                Abbreviation = "C",
                UtcOffset = TimeSpan.Zero,
                TimezoneId = "Etc/UTC"
            }
        };

        // Act
        var response = items.ToResponse();
        var offsets = response.Abbreviations.Select(x => x.UtcOffset).ToList();

        // Assert
        Assert.That(offsets, Is.EqualTo(new List<TimeSpan>
        {
            TimeSpan.FromHours(-5),
            TimeSpan.Zero,
            TimeSpan.FromHours(10)
        }));
    }

    [Test]
    public void EmptyTimezoneAbbreviationDomainModelListToResponse()
    {
        // Arrange
        var items = new List<TimezoneAbbreviationDomainModel>();

        // Act
        var response = items.ToResponse();

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Abbreviations, Is.Not.Null);
        Assert.That(response.Abbreviations.Any(), Is.False);
    }
}
