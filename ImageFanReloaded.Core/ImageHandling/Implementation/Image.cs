using System;
using System.Collections.Generic;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class Image : IImage
{
	public Image(IDisposable imageImplementationInstance, ImageSize imageSize)
	{
		IImageFrame singleImageFrame = new ImageFrame(
			imageImplementationInstance, imageSize, TimeSpan.Zero);

		_imageFrames = new List<IImageFrame> { singleImageFrame };
	}

	public Image(IReadOnlyList<IImageFrame> imageFrames)
	{
		_imageFrames = imageFrames;
	}

	public ImageSize Size
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageFrames[0].Size;
		}
	}

	public TimeSpan DelayUntilNextFrame
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return TimeSpan.Zero;
		}
	}

	public TImageImplementation GetInstance<TImageImplementation>()
		where TImageImplementation : class, IDisposable
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return _imageFrames[0].GetInstance<TImageImplementation>();
	}

	public IReadOnlyList<IImageFrame> GetImageFrames() => _imageFrames;

	public void Dispose()
	{
		if (!_hasBeenDisposed)
		{
			foreach (var anImageFrame in _imageFrames)
			{
				anImageFrame.Dispose();
			}

			_hasBeenDisposed = true;
			GC.SuppressFinalize(this);
		}
	}

	#region Private

	private readonly IReadOnlyList<IImageFrame> _imageFrames;

	private bool _hasBeenDisposed;

	private void ThrowObjectDisposedExceptionIfNecessary()
	{
		if (_hasBeenDisposed)
		{
			throw new ObjectDisposedException(nameof(Image));
		}
	}

	#endregion
}
