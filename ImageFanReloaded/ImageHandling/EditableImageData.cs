using ImageMagick;
using ImageFanReloaded.Core.BaseTypes;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.ImageHandling;

public class EditableImageData : DisposableBase
{
	public EditableImageData(MagickImageCollection imageFramesToEdit, IImage imageToDisplay, ImageSize imageSize)
	{
		_imageFramesToEdit = imageFramesToEdit;
		_imageToDisplay = imageToDisplay;
		_imageSize = imageSize;
	}

	public MagickImageCollection ImageFramesToEdit
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageFramesToEdit;
		}
	}

	public IImage ImageToDisplay
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageToDisplay;
		}
	}

	public ImageSize ImageSize
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _imageSize;
		}
	}

	protected override void DisposeSpecific()
	{
		_imageFramesToEdit.Dispose();
		_imageToDisplay.Dispose();
	}

	private readonly MagickImageCollection _imageFramesToEdit;
	private readonly IImage _imageToDisplay;
	private readonly ImageSize _imageSize;
}
