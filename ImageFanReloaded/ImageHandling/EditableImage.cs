using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ImageMagick;
using ImageFanReloaded.Core.BaseTypes;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.ImageHandling;

public class EditableImage : DisposableBase
{
	public EditableImage(string imageFilePath, uint imageQualityLevel)
	{
		try
		{
			_imageFramesToEdit = new MagickImageCollection(imageFilePath);

			if (_imageFramesToEdit.Count > 1)
			{
				_imageFramesToEdit.Coalesce();
			}
			else
			{
				_imageFramesToEdit[0].AutoOrient();
			}

			_imageFramesToEdit.ForEach(anImageFrameToEdit
				=> anImageFrameToEdit.Quality = imageQualityLevel);

			using var imageToDisplayStream = new MemoryStream();
			_imageFramesToEdit[0].Write(imageToDisplayStream, MagickFormat.Jpg);
			imageToDisplayStream.Reset();

			_imageToDisplay = new Bitmap(imageToDisplayStream);
			_imageSize = new ImageSize(_imageToDisplay.Size.Width, _imageToDisplay.Size.Height);
		}
		catch
		{
			_imageFramesToEdit?.Dispose();
			_imageToDisplay?.Dispose();

			throw;
		}
	}

	public Bitmap ImageToDisplay
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

	public async Task<EditableImage> RotateLeft()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return await CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Rotate(270)));
	}

	public async Task<EditableImage> RotateRight()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return await CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Rotate(90)));
	}

	public async Task<EditableImage> FlipHorizontally()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return await CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Flop()));
	}

	public async Task<EditableImage> FlipVertically()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		return await CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Flip()));
	}

	public async Task SaveImageWithSameFormat(string imageFilePath)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		await _imageFramesToEdit.WriteAsync(imageFilePath);
	}

	public async Task SaveImageWithFormat(
		string imageFilePath, ISaveFileImageFormat saveFileImageFormat)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		var magickFormat = saveFileImageFormat.GetMagickFormat();

		if (saveFileImageFormat.IsAnimationEnabled)
		{
			await _imageFramesToEdit.WriteAsync(imageFilePath, magickFormat);
		}
		else
		{
			await _imageFramesToEdit[0].WriteAsync(imageFilePath, magickFormat);
		}
	}

	#region Protected

	protected override void DisposeSpecific()
	{
		_imageFramesToEdit.Dispose();
		_imageToDisplay.Dispose();
	}

	#endregion

	#region Private

	private readonly MagickImageCollection _imageFramesToEdit;
	private readonly Bitmap _imageToDisplay;

	private readonly ImageSize _imageSize;

	private EditableImage(MagickImageCollection imageFramesToEdit, Bitmap imageToDisplay)
	{
		_imageFramesToEdit = imageFramesToEdit;
		_imageToDisplay = imageToDisplay;
		_imageSize = new ImageSize(_imageToDisplay.Size.Width, _imageToDisplay.Size.Height);
	}

	private async Task<EditableImage> CreateTransformedImage(
		Action<MagickImageCollection> transformAction)
	{
		return await Task.Run(() =>
		{
			var transformedImageFramesToEdit = CopyMagickImage(_imageFramesToEdit);

			transformAction(transformedImageFramesToEdit);

			using var imageToDisplayStream = new MemoryStream();
			transformedImageFramesToEdit[0].Write(imageToDisplayStream, MagickFormat.Jpg);
			imageToDisplayStream.Reset();

			var transformedImageToDisplay = new Bitmap(imageToDisplayStream);

			var transformedEditableImage = new EditableImage(
				transformedImageFramesToEdit, transformedImageToDisplay);
			return transformedEditableImage;
		});
	}

	private static MagickImageCollection CopyMagickImage(MagickImageCollection sourceImageFrames)
	{
		using var copyImageFramesStream = new MemoryStream();
		sourceImageFrames.Write(copyImageFramesStream);
		copyImageFramesStream.Reset();

		MagickImageCollection destinationImageFrames = new MagickImageCollection(
			copyImageFramesStream);
		return destinationImageFrames;
	}

	#endregion
}
