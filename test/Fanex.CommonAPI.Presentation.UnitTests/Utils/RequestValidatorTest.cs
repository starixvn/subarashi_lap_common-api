using System.ComponentModel.DataAnnotations;
using AutoFixture;
using AutoFixture.Xunit2;
using Fanex.CommonAPI.Presentation.Utils;
using FluentAssertions;
using Xunit;

namespace Fanex.CommonAPI.Presentation.UnitTests.Utils;

public class RequestValidatorTest
{
    private readonly IFixture _fixture;

    public RequestValidatorTest()
    {
        _fixture = new Fixture();
    }

    private class TestRequest
    {
        [Required]
        public string RequiredField { get; set; }

        [Range(1, 100)]
        public int RangeField { get; set; }

        [EmailAddress]
        public string EmailField { get; set; }
    }

    [Fact]
    public void IsValidRequest_ShouldReturnTrueForValidRequest()
    {
        // Arrange
        var request = _fixture.Build<TestRequest>()
            .With(x => x.RequiredField, "test")
            .With(x => x.RangeField, 50)
            .With(x => x.EmailField, "test@example.com")
            .Create();

        // Act
        var (isValid, results) = RequestValidator.IsValidRequest(request);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void IsValidRequest_ShouldReturnFalseForInvalidRequest()
    {
        // Arrange
        var request = _fixture.Build<TestRequest>()
            .With(x => x.RequiredField, (string)null)
            .With(x => x.RangeField, 101)
            .With(x => x.EmailField, "invalid-email")
            .Create();

        // Act
        var (isValid, results) = RequestValidator.IsValidRequest(request);

        // Assert
        isValid.Should().BeFalse();
        results.Should().HaveCount(3);
    }

    [Fact]
    public void GetInvalidParamList_ShouldReturnCommaSeparatedMemberNames()
    {
        // Arrange
        var validationResults = new List<ValidationResult>
        {
            new ValidationResult("Required field error", new[] { "RequiredField" }),
            new ValidationResult("Range field error", new[] { "RangeField" }),
            new ValidationResult("Email field error", new[] { "EmailField" })
        };

        // Act
        var result = RequestValidator.GetInvalidParamList(validationResults);

        // Assert
        result.Should().Be("RequiredField, RangeField, EmailField");
    }

    [Fact]
    public void GetInvalidParamList_ShouldHandleMultipleMemberNamesPerValidationResult()
    {
        // Arrange
        var validationResults = new List<ValidationResult>
        {
            new ValidationResult("Multiple fields error", new[] { "Field1", "Field2" }),
            new ValidationResult("Single field error", new[] { "Field3" })
        };

        // Act
        var result = RequestValidator.GetInvalidParamList(validationResults);

        // Assert
        result.Should().Be("Field1, Field2, Field3");
    }
}