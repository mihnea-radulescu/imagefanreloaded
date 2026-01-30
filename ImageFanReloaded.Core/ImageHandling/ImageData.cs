using System.IO;
using ImageFanReloaded.Core.BaseTypes;

namespace ImageFanReloaded.Core.ImageHandling;

public class ImageData : DisposableBase
{
	public ImageData(Stream? imageDataStream)
		: this(imageDataStream, true)
	{
	}

	public ImageData(Stream? imageDataStream, bool shouldUpdateThumbnail)
	{
		ImageDataStream = imageDataStream;
		ShouldUpdateThumbnail = shouldUpdateThumbnail;
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

	public bool ShouldUpdateThumbnail
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
