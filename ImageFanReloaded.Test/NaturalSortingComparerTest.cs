using System;
using Xunit;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Test;

public class NaturalSortingComparerTest
{
    public NaturalSortingComparerTest()
    {
        _naturalSortingComparer = new NaturalSortingComparer(StringComparer.InvariantCulture);
    }

    [InlineData("a", "b", -1)]
	[InlineData("aaa", "aaa", 0)]
	[InlineData("cba", "abc", 1)]
	[Theory]
    public void Compare_NoDigitsPresentInEitherString_ReturnsExpectedResult(
        string x, string y, int expectedComparisonResult)
    {
        // Arrange

        // Act
        var actualComparisonResult = _naturalSortingComparer.Compare(x, y);

        // Assert
        Assert.Equal(expectedComparisonResult, actualComparisonResult);
    }

	[InlineData("1", "b", -1)]
	[InlineData("cba", "2", 1)]
	[Theory]
	public void Compare_DigitsPresentInOnlyOneString_ReturnsExpectedResult(
		string x, string y, int expectedComparisonResult)
	{
		// Arrange

		// Act
		var actualComparisonResult = _naturalSortingComparer.Compare(x, y);

		// Assert
		Assert.Equal(expectedComparisonResult, actualComparisonResult);
	}

	[InlineData("collection0", "collection1", -1)]
	[InlineData("collection2", "collection10", -1)]
	[InlineData("collection1", "collection1", 0)]
	[InlineData("collection1", "collection0", 1)]
	[InlineData("collection10", "collection2", 1)]
	[InlineData("collection0files", "collection1files", -1)]
	[InlineData("collection2files", "collection10files", -1)]
	[InlineData("collection1files", "collection1files", 0)]
	[InlineData("collection1files", "collection0files", 1)]
	[InlineData("collection10files", "collection2files", 1)]
	[Theory]
	public void Compare_DigitsPresentInBothStrings_ReturnsExpectedResult(
		string x, string y, int expectedComparisonResult)
	{
		// Arrange

		// Act
		var actualComparisonResult = _naturalSortingComparer.Compare(x, y);

		// Assert
		Assert.Equal(expectedComparisonResult, actualComparisonResult);
	}

	#region Private

	private readonly NaturalSortingComparer _naturalSortingComparer;

    #endregion
}
