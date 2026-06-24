using System;
using Xunit;
using ImageFanReloaded.Core.DiscAccess.Implementation;

namespace ImageFanReloaded.Test.TestClasses;

public class PathExtensionsTest
{
	[Theory]
	[InlineData(@"/home", @"/media")]
	[InlineData(@"/media", @"/mount")]
	[InlineData(@"/home/mihnea", @"/media/mihnea")]
	[InlineData(@"/usr/lib/ImageFanReloaded", @"/lib/ImageFanReloaded")]
	[InlineData(@"/usr/lib/ImageFanReloaded", @"/usr/ImageFanReloaded")]
	[InlineData(@"/home/mihnea/Pictures", @"/media/mihnea/Pictures")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea/Pictures_Converted")]
	[InlineData(@"/home/mihnea/Pictures_Converted", @"/home/mihnea/Pictures")]
	public void StartsWithPath_UnixOs_LongerPathDoesNotStartWithShorterPath_ReturnsFalse(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison =
			StringComparison.InvariantCulture;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.False(startsWithPath);
	}

	[Theory]
	[InlineData(@"/", @"/")]
	[InlineData(@"/home", @"/home")]
	[InlineData(@"/home/mihnea", @"/home/mihnea")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea/Pictures")]
	public void StartsWithPath_UnixOs_LongerPathEqualsShorterPath_ReturnsTrue(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison =
			StringComparison.InvariantCulture;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(startsWithPath);
	}

	[Theory]
	[InlineData(@"/home/mihnea/Pictures", @"/")]
	[InlineData(@"/home/mihnea/Pictures", @"/home")]
	[InlineData(@"/home/mihnea/Pictures", @"/home/mihnea")]
	[InlineData(@"/usr/lib/ImageFanReloaded", @"/")]
	[InlineData(@"/usr/lib/ImageFanReloaded", @"/usr")]
	[InlineData(@"/usr/lib/ImageFanReloaded", @"/usr/lib")]
	public void StartsWithPath_UnixOs_LongerPathStartsWithShorterPath_ReturnsTrue(string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"/";
		const StringComparison pathComparison =
			StringComparison.InvariantCulture;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(startsWithPath);
	}

	[Theory]
	[InlineData(@"C:\", @"D:\")]
	[InlineData(@"C:\Media", @"C:\Mount")]
	[InlineData(@"C:\Users\mihnea", @"C:\Profiles\mihnea")]
	[InlineData(@"C:\Data\Programs\ImageFanReloaded", @"d:\")]
	[InlineData(@"C:\Data\Programs\ImageFanReloaded",
				@"C:\Data\ImageFanReloaded")]
	[InlineData(@"C:\Data\Programs\ImageFanReloaded",
				@"C:\Programs\ImageFanReloaded")]
	[InlineData(@"C:\Data\Programs\ImageFanReloaded", @"C:\ImageFanReloaded")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Profiles\mihnea\Pictures")]
	[InlineData(@"C:\Users\mihnea\Pictures",
				@"C:\Users\mihnea\Pictures_Converted")]
	[InlineData(@"C:\Users\mihnea\Pictures_Converted",
				@"C:\Users\mihnea\Pictures")]
	public void StartsWithPath_WindowsOs_LongerPathDoesNotStartWithShorterPath_ReturnsFalse(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison =
			StringComparison.InvariantCultureIgnoreCase;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.False(startsWithPath);
	}

	[Theory]
	[InlineData(@"C:\", @"c:\")]
	[InlineData(@"C:\Media", @"C:\media")]
	[InlineData(@"C:\Users\mihnea", @"C:\users\Mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users\Mihnea\pictures")]
	public void StartsWithPath_WindowsOs_LongerPathEqualsShorterPath_ReturnsTrue(string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison =
			StringComparison.InvariantCultureIgnoreCase;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(startsWithPath);
	}

	[Theory]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Users")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\Users\mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"c:\")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users")]
	[InlineData(@"C:\Users\mihnea\Pictures", @"C:\users\Mihnea")]
	public void StartsWithPath_WindowsOs_LongerPathStartsWithShorterPath_ReturnsTrue(
		string longerPath, string shorterPath)
	{
		// Arrange
		const string directorySeparator = @"\";
		const StringComparison pathComparison =
			StringComparison.InvariantCultureIgnoreCase;

		// Act
		var startsWithPath = longerPath.StartsWithPath(
			shorterPath, directorySeparator, pathComparison);

		// Assert
		Assert.True(startsWithPath);
	}

	[Theory]
	[InlineData(@"/media.jpg", @"/media")]
	[InlineData(@"/home/mihnea.jpg", @"/home/mihnea")]
	[InlineData(@"/home/mihnea/mihnea.jpg", @"/home")]
	[InlineData(@"/usr/lib/ImageFanReloaded.jpg", @"/usr")]
	[InlineData(@"/usr/lib/ImageFanReloaded.jpg", @"/usr/lib/ImageFanReloaded")]
	[InlineData(@"/usr/lib/ImageFanReloaded_image001.jpg", @"/usr/lib/ImageFanReloaded")]
	public void IsFileInFolder_UnixOs_FileIsNotInFolder_ReturnsFalse(
		string filePath, string folderPath)
	{
		// Arrange
		const char directorySeparatorChar = '/';
		const StringComparison pathComparison =
			StringComparison.InvariantCulture;

		// Act
		var isFileInFolder = PathExtensions.IsFileInFolder(
			filePath, folderPath, directorySeparatorChar, pathComparison);

		// Assert
		Assert.False(isFileInFolder);
	}

	[Theory]
	[InlineData(@"/media.jpg", @"/")]
	[InlineData(@"/home/mihnea.jpg", @"/home")]
	[InlineData(@"/home/mihnea/mihnea.jpg", @"/home/mihnea")]
	[InlineData(@"/usr/lib/ImageFanReloaded.jpg", @"/usr/lib")]
	[InlineData(@"/usr/lib/ImageFanReloaded_image001.jpg", @"/usr/lib")]
	[InlineData(@"/usr/lib/ImageFanReloaded/ImageFanReloaded_image001.jpg", @"/usr/lib/ImageFanReloaded")]
	public void IsFileInFolder_UnixOs_FileIsInFolder_ReturnsTrue(
		string filePath, string folderPath)
	{
		// Arrange
		const char directorySeparatorChar = '/';
		const StringComparison pathComparison =
			StringComparison.InvariantCulture;

		// Act
		var isFileInFolder = PathExtensions.IsFileInFolder(
			filePath, folderPath, directorySeparatorChar, pathComparison);

		// Assert
		Assert.True(isFileInFolder);
	}

	[Theory]
	[InlineData(@"C:\media.jpg", @"C:\media")]
	[InlineData(@"C:\Users\mihnea.jpg", @"C:\Users\mihnea")]
	[InlineData(@"C:\Users\mihnea.jpg", @"C:\")]
	[InlineData(@"C:\Users\mihnea\Pictures\mihnea.jpg", @"C:\Users\mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures\mihnea_image001.jpg", @"C:\Users\mihnea\Pictures\mihnea")]
	public void IsFileInFolder_WindowsOs_FileIsNotInFolder_ReturnsFalse(
		string filePath, string folderPath)
	{
		// Arrange
		const char directorySeparatorChar = '\\';
		const StringComparison pathComparison =
			StringComparison.InvariantCultureIgnoreCase;

		// Act
		var isFileInFolder = PathExtensions.IsFileInFolder(
			filePath, folderPath, directorySeparatorChar, pathComparison);

		// Assert
		Assert.False(isFileInFolder);
	}

	[Theory]
	[InlineData(@"C:\media.jpg", @"C:\")]
	[InlineData(@"C:\Users\mihnea.jpg", @"C:\Users")]
	[InlineData(@"C:\Users\mihnea\mihnea.jpg", @"C:\Users\mihnea")]
	[InlineData(@"C:\Users\mihnea\Pictures\mihnea.jpg", @"C:\Users\mihnea\Pictures")]
	[InlineData(@"C:\Users\mihnea\Pictures\mihnea_image001.jpg", @"C:\Users\mihnea\Pictures")]
	public void IsFileInFolder_WindowsOs_FileIsInFolder_ReturnsTrue(
		string filePath, string folderPath)
	{
		// Arrange
		const char directorySeparatorChar = '\\';
		const StringComparison pathComparison =
			StringComparison.InvariantCultureIgnoreCase;

		// Act
		var isFileInFolder = PathExtensions.IsFileInFolder(
			filePath, folderPath, directorySeparatorChar, pathComparison);

		// Assert
		Assert.True(isFileInFolder);
	}
}
