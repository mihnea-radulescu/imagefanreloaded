using System;

namespace ImageFanReloaded.Core.ImageHandling.Implementation;

public class Image : IImage
{
	public Image(IDisposable imageImplementationInstance, ImageSize imageSize)
	{
		_imageImplementationInstance = imageImplementationInstance;
		_imageSize = imageSize;

		_hasBeenDisposed = false;
	}

	public ImageSize Size
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageSize;
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
