using System;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class ImageFrame : IImageFrame
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

	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			_imageImplementationInstance.Dispose();

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	#region Private

	private readonly IDisposable _imageImplementationInstance;
	private readonly ImageSize _imageSize;
	private readonly TimeSpan _delayUntilNextFrame;

	private bool _hasBeenDisposed;

	private void ThrowObjectDisposedExceptionIfNecessary()
	{
		if (_hasBeenDisposed)
		{
			throw new ObjectDisposedException(nameof(ImageFrame));
		}
	}

	#endregion
}
