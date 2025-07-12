using System.Collections.Generic;
using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.OperatingSystem;
using ImageFanReloaded.Core.OperatingSystem.Implementation;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;
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

		Assert.Equal(6, _globalParameters.MaxRecursionDepth);

		Assert.Equal(KeyModifiers.None, _globalParameters.NoneKeyModifier);

		Assert.NotEqual(KeyModifiers.None, _globalParameters.CtrlKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.AltKeyModifier);
		Assert.NotEqual(KeyModifiers.None, _globalParameters.ShiftKeyModifier);

		Assert.NotEqual(Key.None, _globalParameters.TabKey);
		Assert.NotEqual(Key.None, _globalParameters.EscapeKey);
		Assert.NotEqual(Key.None, _globalParameters.EnterKey);

		Assert.NotEqual(Key.None, _globalParameters.SKey);
		Assert.NotEqual(Key.None, _globalParameters.OKey);
		Assert.NotEqual(Key.None, _globalParameters.DKey);

		Assert.NotEqual(Key.None, _globalParameters.HKey);
		Assert.NotEqual(Key.None, _globalParameters.F1Key);

		Assert.NotEqual(Key.None, _globalParameters.FKey);

		Assert.NotEqual(Key.None, _globalParameters.F4Key);

		Assert.NotEqual(Key.None, _globalParameters.NKey);
		Assert.NotEqual(Key.None, _globalParameters.MKey);

		Assert.NotEqual(Key.None, _globalParameters.RKey);
		Assert.NotEqual(Key.None, _globalParameters.EKey);

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

		Assert.NotEqual(Key.None, _globalParameters.LKey);
		Assert.NotEqual(Key.None, _globalParameters.VKey);

		Assert.NotEqual(Key.None, _globalParameters.JKey);
		Assert.NotEqual(Key.None, _globalParameters.GKey);
		Assert.NotEqual(Key.None, _globalParameters.PKey);
		Assert.NotEqual(Key.None, _globalParameters.WKey);
		Assert.NotEqual(Key.None, _globalParameters.BKey);

		Assert.NotEqual(Key.None, _globalParameters.UKey);

		Assert.NotNull(_globalParameters.NameComparer);

		Assert.NotEmpty(_globalParameters.DirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters.IndirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters.AnimationEnabledImageFileExtensions);
		Assert.NotEmpty(_globalParameters.ImageFileExtensions);

		Assert.NotEqual(0U, _globalParameters.ImageQualityLevel);

		Assert.NotNull(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.UserProfilePath);
		Assert.NotEmpty(_globalParameters.SpecialFolders);

		Assert.NotNull(_globalParameters.InvalidImage.GetBitmap());

		Assert.Equal(15, _globalParameters.PersistentImages.Count);

		Assert.NotNull(_globalParameters.DriveIcon.GetBitmap());
		Assert.NotNull(_globalParameters.FolderIcon.GetBitmap());

		SaveImageToDisc(
			_globalParameters.InvalidImage.GetBitmap(),
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.GetInvalidImageThumbnail(ThumbnailSize).GetBitmap(),
			$"InvalidImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.GetLoadingImageThumbnail(ThumbnailSize).GetBitmap(),
			$"LoadingImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.DriveIcon.GetBitmap(),
			$"{nameof(_globalParameters.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.FolderIcon.GetBitmap(),
			$"{nameof(_globalParameters.FolderIcon)}{OutputFileExtension}");
	}

	#region Private

	private const int ThumbnailSize = 250;

	private readonly GlobalParameters _globalParameters;

	#endregion
}
