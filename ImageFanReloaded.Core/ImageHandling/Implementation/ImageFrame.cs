using System;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFrame : DisposableBase, IImageFrame
{
	public ImageFrame(
		IDisposable imageImplementationInstance,
		ImageSize imageSize,
		TimeSpan delayUntilNextFrame)
	{
		_imageImplementationInstance = imageImplementationInstance;
		_imageSize = imageSize;
		_delayUntilNextFrame = delayUntilNextFrame;
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

		return (TImageImplementation)_imageImplementationInstance;
	}

	#region Protected

	protected override void DisposeSpecific()
	{
		_imageImplementationInstance.Dispose();
	}

	#endregion

	#region Private

	private readonly IDisposable _imageImplementationInstance;
	private readonly ImageSize _imageSize;
	private readonly TimeSpan _delayUntilNextFrame;

	#endregion
}
