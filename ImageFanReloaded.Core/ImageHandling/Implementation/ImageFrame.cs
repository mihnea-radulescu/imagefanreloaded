using System;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFrame : DisposableBase, IImageFrame
{
	public ImageFrame(
		IDisposable imageFrameImplementationInstance,
		ImageSize imageSize,
		TimeSpan delayUntilNextFrame)
	{
		_imageFrameImplementationInstance = imageFrameImplementationInstance;
		_imageSize = imageSize;
		_delayUntilNextFrame = delayUntilNextFrame >= MinimumDelayUntilNextFrame
			? delayUntilNextFrame
			: MinimumDelayUntilNextFrame;
	}

	public ImageSize Size
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageSize;
		}
	}

	public TimeSpan DelayUntilNextFrame
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _delayUntilNextFrame;
		}
	}

	public TImageImplementation GetInstance<TImageImplementation>()
		where TImageImplementation : class, IDisposable
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return (TImageImplementation)_imageFrameImplementationInstance;
	}

	#region Protected

	protected override void DisposeSpecific()
	{
		_imageFrameImplementationInstance.Dispose();
	}

	#endregion

	#region Private

	private static TimeSpan MinimumDelayUntilNextFrame => TimeSpan.FromMilliseconds(50);

	private readonly IDisposable _imageFrameImplementationInstance;
	private readonly ImageSize _imageSize;
	private readonly TimeSpan _delayUntilNextFrame;

	#endregion
}
