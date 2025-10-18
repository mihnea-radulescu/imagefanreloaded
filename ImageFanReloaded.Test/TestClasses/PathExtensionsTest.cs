using System;
using Xunit;
using ImageFanReloaded.Core.DiscAccess.Implementation;

namespace ImageFanReloaded.Test.TestClasses;

public class PathExtensionsTest
{
	#region UnixOs

	[Theory]
	[InlineData(@"/home", @"/media")]
	[InlineData(@"/media", @"/mount")]
	[InlineData(@"/home/mihnea", @"/media/mihnea")]
	[InlineData(@"/home/mihnea/Pictures", @"/media/mihnea/Pictures")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea/Pictures_Converted")]
	[InlineData(@"/home/mihnea/Pictures_Converted", @"/home/mihnea/Pictures")]
	public void ContainsPath_UnixOs_LongerPathDoesNotContainShorterPath_ReturnsFalse(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison = StringComparison.InvariantCulture;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.False(containsPath);
	}

	[Theory]
	[InlineData(@"/", @"/")]
	[InlineData(@"/home", @"/home")]
	[InlineData(@"/home/mihnea", @"/home/mihnea")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea/Pictures")]
	public void ContainsPath_UnixOs_EqualPaths_ReturnsTrue(string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison = StringComparison.InvariantCulture;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(containsPath);
	}

	[Theory]
	[InlineData(@"/home/mihnea/Pictures", @"/")]
	[InlineData(@"/home/mihnea/Pictures", @"/home")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea")]
	public void ContainsPath_UnixOs_LongerPathContainsShorterPath_ReturnsTrue(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison = StringComparison.InvariantCulture;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(containsPath);
	}

	#endregion

	#region WindowsOs

	[Theory]
	[InlineData(@"C:\", @"D:\")]
	[InlineData(@"C:\Media", @"C:\Mount")]
	[InlineData(@"C:\Users\mihnea", @"C:\Profiles\mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Profiles\mihnea\Pictures")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Users\mihnea\Pictures_Converted")]
	[InlineData(@"C:\Users\mihnea\Pictures_Converted", @"C:\Users\mihnea\Pictures")]
	public void ContainsPath_WindowsOs_LongerPathDoesNotContainShorterPath_ReturnsFalse(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison = StringComparison.InvariantCultureIgnoreCase;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.False(containsPath);
	}

	[Theory]
	[InlineData(@"C:\", @"c:\")]
	[InlineData(@"C:\Media", @"C:\media")]
	[InlineData(@"C:\Users\mihnea", @"C:\users\Mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users\Mihnea\pictures")]
	public void ContainsPath_WindowsOs_EqualPaths_ReturnsTrue(string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison = StringComparison.InvariantCultureIgnoreCase;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(containsPath);
	}

	[Theory]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Users")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Users\mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"c:\")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users\Mihnea")]
	public void ContainsPath_WindowsOs_LongerPathContainsShorterPath_ReturnsTrue(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison = StringComparison.InvariantCultureIgnoreCase;

		// Act
		var containsPath = longerPath.ContainsPath(shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(containsPath);
	}

	#endregion
}
