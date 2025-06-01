using System.Collections.Generic;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Test;

public class GlobalParametersTest : TestBase
{
	public GlobalParametersTest()
	{
		IOperatingSystemSettings operatingSystemSettings = new OperatingSystemSettings();
		
		IImageResizeCalculator imageResizeCalculator = new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(operatingSystemSettings, imageResizer);
	}
	
	[Fact]
	public void GlobalParameters_IsCorrectlyInitialized()
	{
		// Arrange

		// Act

		// Assert
		Assert.NotEqual(0, _globalParameters.ProcessorCount);

		List<bool> isOperatingSystemCollection =
		[
			_globalParameters.IsLinux,
			_globalParameters.IsWindows,
			_globalParameters.IsMacOS
		];
		Assert.Single(isOperatingSystemCollection, isOperatingSystem => isOperatingSystem == true);
		
		Assert.Equal(KeyModifiers.None, _globalParameters.NoneKeyModifier);
		
		Assert.NotEqual(KeyModifiers.None, _globalParameters.CtrlKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.AltKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.ShiftKeyModifier);

		Assert.NotEqual(Key.None, _globalParameters.TabKey);
		Assert.NotEqual(Key.None, _globalParameters.EscapeKey);
		Assert.NotEqual(Key.None, _globalParameters.EnterKey);

		Assert.NotEqual(Key.None, _globalParameters.F1Key);
		Assert.NotEqual(Key.None, _globalParameters.HKey);
		Assert.NotEqual(Key.None, _globalParameters.OKey);

		Assert.NotEqual(Key.None, _globalParameters.F4Key);
		
		Assert.NotEqual(Key.None, _globalParameters.NKey);
		Assert.NotEqual(Key.None, _globalParameters.MKey);
		
		Assert.NotEqual(Key.None, _globalParameters.RKey);
		
		Assert.NotEqual(Key.None, _globalParameters.TKey);
		Assert.NotEqual(Key.None, _globalParameters.IKey);
		
		Assert.NotEqual(Key.None, _globalParameters.UpKey);
		Assert.NotEqual(Key.None, _globalParameters.DownKey);
		Assert.NotEqual(Key.None, _globalParameters.LeftKey);
		Assert.NotEqual(Key.None, _globalParameters.RightKey);
		
		Assert.NotEqual(Key.None, _globalParameters.PlusKey);
		Assert.NotEqual(Key.None, _globalParameters.MinusKey);
		
		Assert.NotEqual(Key.None, _globalParameters.PageUpKey);
		Assert.NotEqual(Key.None, _globalParameters.PageDownKey);

		Assert.NotNull(_globalParameters.NameComparer);

		Assert.NotEmpty(_globalParameters.ImageFileExtensions);

		Assert.NotNull(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.SpecialFolders);
		Assert.Equal(
			FileSystemEntryInfoOrdering.NameAscending,
			_globalParameters.DefaultFileSystemEntryInfoOrdering);

		Assert.Equal(400, _globalParameters.DefaultThumbnailSize);
		Assert.Equal(50, _globalParameters.ThumbnailSizeIncrement);
		Assert.True(_globalParameters.IsValidThumbnailSize(_globalParameters.DefaultThumbnailSize));

		Assert.NotNull(_globalParameters.InvalidImage.GetBitmap());

		Assert.NotNull(
			_globalParameters.GetInvalidImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap());
		Assert.NotNull(
			_globalParameters.GetLoadingImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap()
		);

		Assert.Equal(17, _globalParameters.PersistentImages.Count);

		Assert.NotNull(_globalParameters.DriveIcon.GetBitmap());
		Assert.NotNull(_globalParameters.FolderIcon.GetBitmap());

		SaveImageToDisc(
			_globalParameters.InvalidImage.GetBitmap(),
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.GetInvalidImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap(),
			$"InvalidImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.GetLoadingImageThumbnail(_globalParameters.DefaultThumbnailSize).GetBitmap(),
			$"LoadingImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.DriveIcon.GetBitmap(),
			$"{nameof(_globalParameters.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.FolderIcon.GetBitmap(), 
			$"{nameof(_globalParameters.FolderIcon)}{OutputFileExtension}");
	}
	
	#region Private

	private readonly GlobalParameters _globalParameters;

	#endregion
}
