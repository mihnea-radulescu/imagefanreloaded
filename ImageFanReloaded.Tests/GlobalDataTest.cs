using FluentAssertions;
using Xunit;

namespace ImageFanReloaded.Tests;

public class GlobalDataTest : TestBase
{
	[Fact]
	public void GlobalData_IsCorrectlyInitialized()
	{
		// Arrange

		// Act

		// Assert
		GlobalData.InvalidImage.Should().NotBeNull();
		GlobalData.InvalidImageThumbnail.Should().NotBeNull();

		GlobalData.LoadingImageThumbnail.Should().NotBeNull();

		GlobalData.DriveIcon.Should().NotBeNull();
		GlobalData.FolderIcon.Should().NotBeNull();

		GlobalData.ProcessorCount.Should().NotBe(0);

		SaveImageToDisc(
			GlobalData.InvalidImage,
			$"{nameof(GlobalData.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			GlobalData.InvalidImageThumbnail,
			$"{nameof(GlobalData.InvalidImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			GlobalData.LoadingImageThumbnail,
			$"{nameof(GlobalData.LoadingImageThumbnail)}{OutputFileExtension}");

		SaveImageToDisc(
			GlobalData.DriveIcon,
			$"{nameof(GlobalData.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			GlobalData.FolderIcon,
			$"{nameof(GlobalData.FolderIcon)}{OutputFileExtension}");
	}
}
