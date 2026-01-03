using System;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ImageViewFactory : IImageViewFactory
{
	public ImageViewFactory(
		IGlobalParameters globalParameters,
		IMouseCursorFactory mouseCursorFactory,
		IScreenInfo screenInfo)
	{
		_globalParameters = globalParameters;
		_mouseCursorFactory = mouseCursorFactory;
		_screenInfo = screenInfo;
	}

	public IImageView GetImageView(ITabOptions tabOptions)
	{
		var imageViewDisplayMode = tabOptions.ImageViewDisplayMode;

		IImageView imageView = tabOptions.ImageViewDisplayMode switch
		{
			ImageViewDisplayMode.FullScreen => new FullScreenImageWindow(),

			ImageViewDisplayMode.Windowed => new WindowedImageWindow(),
			ImageViewDisplayMode.WindowedMaximized => new WindowedImageWindow(),
			ImageViewDisplayMode.WindowedMaximizedBorderless => new WindowedImageWindow(),

			_ => throw new NotSupportedException($"Image view display mode {imageViewDisplayMode} not supported.")
		};

		imageView.GlobalParameters = _globalParameters;
		imageView.MouseCursorFactory = _mouseCursorFactory;
		imageView.ScreenInfo = _screenInfo;

		imageView.TabOptions = tabOptions;

		return imageView;
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IMouseCursorFactory _mouseCursorFactory;
	private readonly IScreenInfo _screenInfo;

	#endregion
}
