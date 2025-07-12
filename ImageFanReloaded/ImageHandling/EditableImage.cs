using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		MagickImageCollection? imageFramesToEdit = default;
		Bitmap? imageToDisplay = default;

		try
		{
			imageFramesToEdit = new MagickImageCollection(imageFilePath);

			if (imageFramesToEdit.Count > 1)
			{
				imageFramesToEdit.Coalesce();
			}
			else
			{
				imageFramesToEdit[0].AutoOrient();
			}

			imageFramesToEdit.ForEach(anImageFrameToEdit
				=> anImageFrameToEdit.Quality = imageQualityLevel);

			using var imageToDisplayStream = new MemoryStream();
			imageFramesToEdit[0].Write(imageToDisplayStream, MagickFormat.Jpg);
			imageToDisplayStream.Reset();

			imageToDisplay = new Bitmap(imageToDisplayStream);

			var imageSize = new ImageSize(imageToDisplay.Size.Width, imageToDisplay.Size.Height);

			_editableImageData = new EditableImageData(
				imageFramesToEdit, imageToDisplay, imageSize);

			_previousOperationsStack = new Stack<EditableImageData>();
			_revertedOperationsStack = new Stack<EditableImageData>();
		}
		catch
		{
			imageFramesToEdit?.Dispose();
			imageToDisplay?.Dispose();

			throw;
		}
	}

	public Bitmap ImageToDisplay
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _editableImageData.ImageToDisplay;
		}
	}

	public ImageSize ImageSize
	{
		get
		{
			ThrowObjectDisposedExceptionIfNecessary();

			return _editableImageData.ImageSize;
		}
	}

	public bool CanUndoLastEdit => _previousOperationsStack.Any();
	public bool CanRedoLastEdit => _revertedOperationsStack.Any();

	public void UndoLastEdit()
	{
		if (!CanUndoLastEdit)
		{
			return;
		}

		_revertedOperationsStack.Push(_editableImageData);
		_editableImageData = _previousOperationsStack.Pop();
	}

	public void RedoLastEdit()
	{
		if (!CanRedoLastEdit)
		{
			return;
		}

		_previousOperationsStack.Push(_editableImageData);
		_editableImageData = _revertedOperationsStack.Pop();
	}

	public void RotateLeft()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Rotate(270)));
	}

	public void RotateRight()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Rotate(90)));
	}

	public void FlipHorizontally()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Flop()));
	}

	public void FlipVertically()
	{
		ThrowObjectDisposedExceptionIfNecessary();

		CreateTransformedImage(
			imageColection => imageColection.ForEach(anImageFrame => anImageFrame.Flip()));
	}

	public async Task SaveImageWithSameFormat(string imageFilePath)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		await _editableImageData.ImageFramesToEdit.WriteAsync(imageFilePath);
	}

	public async Task SaveImageWithFormat(
		string imageFilePath, ISaveFileImageFormat saveFileImageFormat)
	{
		ThrowObjectDisposedExceptionIfNecessary();

		var magickFormat = saveFileImageFormat.GetMagickFormat();

		if (saveFileImageFormat.IsAnimationEnabled)
		{
			await _editableImageData.ImageFramesToEdit.WriteAsync(imageFilePath, magickFormat);
		}
		else
		{
			await _editableImageData.ImageFramesToEdit[0].WriteAsync(imageFilePath, magickFormat);
		}
	}

	#region Protected

	protected override void DisposeSpecific()
	{
		_editableImageData.Dispose();

		foreach (var editableImageData in _previousOperationsStack)
		{
			editableImageData.Dispose();
		}

		foreach (var editableImageData in _revertedOperationsStack)
		{
			editableImageData.Dispose();
		}
	}

	#endregion

	#region Private

	private EditableImageData _editableImageData;

	private readonly Stack<EditableImageData> _previousOperationsStack;
	private readonly Stack<EditableImageData> _revertedOperationsStack;

	private void CreateTransformedImage(Action<MagickImageCollection> transformAction)
	{
		var transformedImageFramesToEdit = CopyMagickImage(_editableImageData.ImageFramesToEdit);

		try
		{
			transformAction(transformedImageFramesToEdit);
		}
		catch
		{
			transformedImageFramesToEdit.Dispose();

			throw;
		}

		using var imageToDisplayStream = new MemoryStream();
		transformedImageFramesToEdit[0].Write(imageToDisplayStream, MagickFormat.Jpg);
		imageToDisplayStream.Reset();

		var transformedImageToDisplay = new Bitmap(imageToDisplayStream);

		var transformedImageSize = new ImageSize(
			transformedImageToDisplay.Size.Width, transformedImageToDisplay.Size.Height);

		var transformedEditableImageData = new EditableImageData(
			transformedImageFramesToEdit, transformedImageToDisplay, transformedImageSize);

		_previousOperationsStack.Push(_editableImageData);
		_revertedOperationsStack.Clear();
		_editableImageData = transformedEditableImageData;
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
