using FluentAssertions;
using Xunit;

namespace TodoListApp.Tests;

/// <summary>
/// Basic sanity tests to verify test runner and infrastructure
/// </summary>
public class SanityTests
{
    [Fact]
    public void TestRunner_ShouldWork()
    {
        // Arrange & Act & Assert
        Assert.True(true);
    }

    [Fact]
    public void FluentAssertions_ShouldWork()
    {
        // Arrange
        var value = 42;

        // Act & Assert
        value.Should().Be(42);
        value.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(-1, 1, 0)]
    public void TheoryTests_ShouldWork(int a, int b, int expected)
    {
        // Act
        var result = a + b;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void DateTime_ShouldWorkInTests()
    {
        // Arrange
        var now = DateTime.UtcNow;

        // Act
        var later = now.AddHours(1);

        // Assert
        later.Should().BeAfter(now);
    }

    [Fact]
    public void Collections_ShouldWorkInTests()
    {
        // Arrange
        var list = new List<string> { "one", "two", "three" };

        // Assert
        list.Should().HaveCount(3);
        list.Should().Contain("two");
        list.Should().NotContain("four");
    }

    [Fact]
    public void Strings_ShouldWorkInTests()
    {
        // Arrange
        var text = "Hello World";

        // Assert
        text.Should().StartWith("Hello");
        text.Should().EndWith("World");
        text.Should().Contain("lo Wo");
    }

    [Fact]
    public void Exceptions_ShouldBeTestable()
    {
        // Act
        Action act = () => throw new InvalidOperationException("Test exception");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Test exception");
    }
}