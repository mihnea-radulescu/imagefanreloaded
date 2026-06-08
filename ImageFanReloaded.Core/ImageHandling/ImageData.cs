using System.IO;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling;

public class ImageData : DisposableBase
{
	public ImageData(Stream? imageDataStream, bool isKnownImage = false)
	{
		ImageDataStream = imageDataStream;
		IsKnownImage = isKnownImage;
	}

	public Stream? ImageDataStream
	{ 
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return field;
		}

		private set;
	}

	public bool IsKnownImage
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return field;
		}
	}

	protected override void DisposeSpecific()
	{
		ImageDataStream?.Dispose();
		ImageDataStream = null;
	}
}
