namespace Timezone.UnitTests.Models;

using System;
using NUnit.Framework;
using Timezone.Core.Models;

[TestFixture]
public class ErrorModelTests
{
    [TestCase(typeof(InvalidOperationException), "Unsupported operation")]
    [TestCase(typeof(ArgumentNullException), "Argument cannot be null")]
    [TestCase(typeof(FormatException), "Invalid format")]
    public void ErrorModelConstructedWithExceptionExceptionPropertiesMatchModelProperties(Type exceptionType, string message)
    {
        // Arrange
        var comparedException = (Exception)Activator.CreateInstance(exceptionType, message);

        // Act
        var errorModel = new ErrorModel(comparedException);
        using (Assert.EnterMultipleScope())
        {

            // Assert
            Assert.That(errorModel.Type, Is.EqualTo(comparedException.GetType().Name));
            Assert.That(errorModel.Message, Is.EqualTo(comparedException.Message));
            Assert.That(errorModel.StackTrace, Is.EqualTo(comparedException.ToString()));
        }
    }
}
