using Xunit;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.RuntimeEnvironment;
using ImageFanReloaded.Core.RuntimeEnvironment.Implementation;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Settings;

namespace ImageFanReloaded.Test.TestClasses;

public class GlobalParametersTest : TestBase
{
	public GlobalParametersTest()
	{
		IRuntimeEnvironmentSettings runtimeEnvironmentSettings =
			new RuntimeEnvironmentSettings();

		IImageResizeCalculator imageResizeCalculator =
			new ImageResizeCalculator();
		IImageResizer imageResizer = new ImageResizer(imageResizeCalculator);

		_globalParameters = new GlobalParameters(
			runtimeEnvironmentSettings, imageResizer);
	}

	[Fact]
	public void GlobalParameters_IsCorrectlyInitialized()
	{
		// Arrange

		// Act

		// Assert
		Assert.False(string.IsNullOrEmpty(_globalParameters.ApplicationName));

		Assert.NotEqual(0, _globalParameters.ProcessorCount);

		Assert.NotEqual(
			RuntimeEnvironmentType.None,
			_globalParameters.RuntimeEnvironmentType);

		Assert.Equal(6, _globalParameters.MaxRecursionDepth);

		Assert.NotEqual(KeyModifiers.Other, _globalParameters.NoneKeyModifier);
		Assert.NotEqual(KeyModifiers.Other, _globalParameters.CtrlKeyModifier);
		Assert.NotEqual(KeyModifiers.Other, _globalParameters.AltKeyModifier);
		Assert.NotEqual(KeyModifiers.Other, _globalParameters.ShiftKeyModifier);

		Assert.NotEqual(Key.Other, _globalParameters.TabKey);
		Assert.NotEqual(Key.Other, _globalParameters.EscapeKey);
		Assert.NotEqual(Key.Other, _globalParameters.EnterKey);

		Assert.NotEqual(Key.Other, _globalParameters.SKey);
		Assert.NotEqual(Key.Other, _globalParameters.OKey);

		Assert.NotEqual(Key.Other, _globalParameters.HKey);
		Assert.NotEqual(Key.Other, _globalParameters.F1Key);

		Assert.NotEqual(Key.Other, _globalParameters.FKey);

		Assert.NotEqual(Key.Other, _globalParameters.F4Key);

		Assert.NotEqual(Key.Other, _globalParameters.NKey);
		Assert.NotEqual(Key.Other, _globalParameters.MKey);
		Assert.NotEqual(Key.Other, _globalParameters.BKey);

		Assert.NotEqual(Key.Other, _globalParameters.AKey);
		Assert.NotEqual(Key.Other, _globalParameters.DKey);

		Assert.NotEqual(Key.Other, _globalParameters.RKey);
		Assert.NotEqual(Key.Other, _globalParameters.GKey);
		Assert.NotEqual(Key.Other, _globalParameters.EKey);

		Assert.NotEqual(Key.Other, _globalParameters.TKey);
		Assert.NotEqual(Key.Other, _globalParameters.IKey);
		Assert.NotEqual(Key.Other, _globalParameters.UKey);
		Assert.NotEqual(Key.Other, _globalParameters.CKey);

		Assert.NotEqual(Key.Other, _globalParameters.Digit1Key);
		Assert.NotEqual(Key.Other, _globalParameters.Digit2Key);
		Assert.NotEqual(Key.Other, _globalParameters.Digit3Key);
		Assert.NotEqual(Key.Other, _globalParameters.Digit4Key);

		Assert.NotEqual(Key.Other, _globalParameters.UpKey);
		Assert.NotEqual(Key.Other, _globalParameters.DownKey);
		Assert.NotEqual(Key.Other, _globalParameters.LeftKey);
		Assert.NotEqual(Key.Other, _globalParameters.RightKey);
		Assert.NotEqual(Key.Other, _globalParameters.BackspaceKey);
		Assert.NotEqual(Key.Other, _globalParameters.SpaceKey);

		Assert.NotEqual(Key.Other, _globalParameters.PlusKey);
		Assert.NotEqual(Key.Other, _globalParameters.MinusKey);

		Assert.NotEqual(Key.Other, _globalParameters.PageUpKey);
		Assert.NotEqual(Key.Other, _globalParameters.PageDownKey);

		Assert.NotNull(_globalParameters.NameComparer);

		Assert.NotEmpty(_globalParameters.DirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters
			.IndirectlySupportedImageFileExtensions);
		Assert.NotEmpty(_globalParameters.AnimationEnabledImageFileExtensions);
		Assert.NotEmpty(_globalParameters.ImageFileExtensions);

		Assert.NotEqual(0U, _globalParameters.ImageQualityLevel);
		Assert.NotEqual(0, _globalParameters.DecimalDigitCountForDisplay);

		Assert.False(string.IsNullOrEmpty(_globalParameters.UserHomePath));
		Assert.False(string.IsNullOrEmpty(_globalParameters.UserConfigPath));

		Assert.NotEmpty(_globalParameters.SpecialFolders);

		Assert.NotNull(_globalParameters.InvalidImage.Bitmap);

		var expectedPersistentImagesCount = 1 + 2 * ThumbnailSizes.Values.Count;
		Assert.Equal(
			expectedPersistentImagesCount,
			_globalParameters.PersistentImages.Count);

		Assert.NotNull(_globalParameters.DriveIcon.Bitmap);
		Assert.NotNull(_globalParameters.FolderIcon.Bitmap);

		SaveImageToDisc(
			_globalParameters.InvalidImage.Bitmap,
			$"{nameof(_globalParameters.InvalidImage)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.GetInvalidImageThumbnail(ThumbnailSize).Bitmap,
			$"InvalidImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.GetLoadingImageThumbnail(ThumbnailSize).Bitmap,
			$"LoadingImageThumbnail_{OutputFileExtension}");

		SaveImageToDisc(
			_globalParameters.DriveIcon.Bitmap,
			$"{nameof(_globalParameters.DriveIcon)}{OutputFileExtension}");
		SaveImageToDisc(
			_globalParameters.FolderIcon.Bitmap,
			$"{nameof(_globalParameters.FolderIcon)}{OutputFileExtension}");
	}

	private const int ThumbnailSize = 250;

	private readonly GlobalParameters _globalParameters;
}
