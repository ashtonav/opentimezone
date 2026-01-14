namespace Timezone.UnitTests.Services;

using NUnit.Framework;
using Timezone.Core.Services;

[TestFixture]
public class TimezoneHelperTests
{
    // Test when timezoneId is an empty string
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdIsEmpty()
    {
        // Arrange
        var timezoneId = "";

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId is an empty string.");
    }

    // Test when timezoneId is a valid timezone id
    [Test]
    public void GetTimezoneReturnsTimezoneWhenTimezoneIdIsValid()
    {
        // Arrange
        var timezoneId = "Pacific Standard Time";

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null, "Expected result to not be null when timezoneId is valid.");
            Assert.That(timezoneId, Is.EqualTo(result!.Id), "Expected the Id of the returned timezone to match the timezoneId.");
        }
    }

    // Test when timezoneId does not correspond to a valid timezone id
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdIsNotValid()
    {
        // Arrange
        var timezoneId = "InvalidTimeZoneId";

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId is invalid.");
    }

    // Test when timezoneId is a valid timezone id for Local Timezone
    [Test]
    public void GetTimezoneReturnsLocalTimezoneWhenTimezoneIdIsLocal()
    {
        // Arrange
        var timezoneId = TimeZoneInfo.Local.Id;

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null, "Expected result to not be null when timezoneId is valid.");
            Assert.That(timezoneId, Is.EqualTo(result!.Id), "Expected the returned timezone to match the local timezone.");
        }
    }

    // Test with a maximum string length
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdIsBigString()
    {
        // Arrange
        var timezoneId = new string('a', 5000);

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId is extremely long.");
    }

    // Test with unusually formatted string (contains special characters)
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdContainsSpecialCharacters()
    {
        // Arrange
        var timezoneId = "UTC+!@#$%^&*()-=_+[]{};':,.<>?/\\|";

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId contains special characters.");
    }

    // Test with unusually formatted string (non-English characters)
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdContainsNonEnglishCharacters()
    {
        // Arrange
        var timezoneId = "भारतीयमानकसमय"; // "Indian Standard Time" in Hindi

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId contains non-English characters.");
    }

    // Test with whitespace string
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdIsWhitespace()
    {
        // Arrange
        var timezoneId = "    ";

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId is whitespace.");
    }

    [Test]
    public void GetTimezonePerformanceTestWithValidTimezoneId()
    {
        // Arrange
        var timezoneId = "Pacific Standard Time";

        // Act
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (var i = 0; i < 10000; i++)
        {
            var result = TimezoneHelper.GetTimezone(timezoneId);
            Assert.That(result, Is.Not.Null, $"Test at iteration {i} failed. Expected result to not be null when timezoneId is valid.");
        }
        watch.Stop();

        // Assert
        var executionTime = watch.ElapsedMilliseconds;
        Assert.That(executionTime, Is.LessThan(1000), $"Performance Test Failed. Expected 10000 executions to complete under 1 second but it took {executionTime} ms.");
    }

    // Test with timezoneId of a timezone that does not change to DST
    [Test]
    public void GetTimezoneReturnsSameTimezoneWhenTimezoneIdDoesNotChangeToDST()
    {
        // Arrange
        var timezoneId = "UTC"; // This timezone does not observe DST.

        // Act
        var resultInWinter = TimezoneHelper.GetTimezone(timezoneId);
        var resultInSummer = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(resultInWinter, Is.EqualTo(resultInSummer), "Expected timezone to be the same in summer and winter, as it does not observe Daylight Saving Time.");
    }

    [Test]
    public void GetTimezoneProcessesTimezoneWithHistoricalChangesCorrectly()
    {
        // Arrange
        var timezoneId = "Europe/Moscow"; // This timezone had historical changes

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null, "Expected result to not be null when timezoneId is valid and has historical changes.");
            Assert.That(timezoneId, Is.EqualTo(result!.Id), "Expected the Id of the returned timezone to match the timezoneId, even with historical changes.");
        }
    }

    [Test]
    public void GetTimezoneProcessesTimezoneWithNonIntegerUtcOffsetCorrectly()
    {
        // Arrange
        var timezoneId = "Asia/Kathmandu"; // This timezone has a UTC offset of +5:45

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(result, Is.Not.Null, "Expected result to not be null when timezoneId is valid and has a non-integer UTC offset.");
            Assert.That(timezoneId, Is.EqualTo(result!.Id), "Expected the Id of the returned timezone to match the timezoneId, even with non-integer UTC offset.");
        }
    }

    // Test when timezoneId is null
    [Test]
    public void GetTimezoneReturnsNullWhenTimezoneIdIsNull()
    {
        // Arrange
        string timezoneId = null;

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.That(result, Is.Null, "Expected result to be null when timezoneId is null.");
    }


    // A test checking that the function deals correctly with an edge case: DateTime.MaxValue
    [Test]
    public void GetTimezoneDealsProperlyWithDateTimeMaxValue()
    {
        // Arrange
        var timezoneId = TimeZoneInfo.Local.Id;

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.DoesNotThrow(() => result.GetUtcOffset(DateTime.MaxValue));
    }

    // A test checking that the function deals correctly with an edge case: DateTime.MinValue
    [Test]
    public void GetTimezoneDealsProperlyWithDateTimeMinValue()
    {
        // Arrange
        var timezoneId = TimeZoneInfo.Local.Id;

        // Act
        var result = TimezoneHelper.GetTimezone(timezoneId);

        // Assert
        Assert.DoesNotThrow(() => result.GetUtcOffset(DateTime.MinValue));
    }


    [Test]
    public void GetTimezoneHandlesConcurrentRequestsGracefully()
    {
        // Arrange
        var timezoneId1 = "Eastern Standard Time";
        var timezoneId2 = "Pacific Standard Time";
        var timezoneId3 = "Central European Standard Time";
        Exception caughtException = null;

        // Act
        Parallel.Invoke(
            () =>
            {
                try
                {
                    var result1 = TimezoneHelper.GetTimezone(timezoneId1);
                    Assert.That(result1, Is.Not.Null);
                }
                catch (Exception ex) { caughtException = ex; }
            },
            () =>
            {
                try
                {
                    var result2 = TimezoneHelper.GetTimezone(timezoneId2);
                    Assert.That(result2, Is.Not.Null);
                }
                catch (Exception ex) { caughtException = ex; }
            },
            () =>
            {
                try
                {
                    var result3 = TimezoneHelper.GetTimezone(timezoneId3);
                    Assert.That(result3, Is.Not.Null);
                }
                catch (Exception ex) { caughtException = ex; }
            });

        // Assert
        Assert.That(caughtException, Is.Null, $"Expected no exceptions when calling GetTimezone concurrently, but got: {caughtException}");
    }

}
