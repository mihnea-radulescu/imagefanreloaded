using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageFanReloaded.Core.Collections.Implementation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;

namespace ImageFanReloaded.Core;

public class ImageViewPresenter
{
	public ImageViewPresenter(
		IDiscQueryEngine discQueryEngine,
		IInputPathHandler inputPathHandler,
		IGlobalParameters globalParameters,
		IImageView imageView)
	{
		_discQueryEngine = discQueryEngine;
		_inputPathHandler = inputPathHandler;

		_nameComparison = globalParameters.NameComparer.ToStringComparison();

		_imageView = imageView;
		_imageView.ImageChanged += OnImageChanged;
	}

	public async Task SetUpAccessToImages()
	{
		_imageFiles = await _discQueryEngine.GetImageFilesDefault(_inputPathHandler.FolderPath!);

		(_currentImageFile, _currentImageFileIndex) = _imageFiles
			.Select((anImageFile, index) => (anImageFile, index))
			.Single(anImageFileWithIndex => anImageFileWithIndex.anImageFile.ImageFileData.ImageFilePath.Equals(
				_inputPathHandler.FilePath, _nameComparison));

		await LoadCurrentImage();
	}

	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IInputPathHandler _inputPathHandler;

	private readonly StringComparison _nameComparison;

	private readonly IImageView _imageView;

	private IReadOnlyList<IImageFile>? _imageFiles;
	private IImageFile? _currentImageFile;
	private int _currentImageFileIndex;

	private async Task LoadCurrentImage() => await _imageView.SetImage(_currentImageFile!);

	private async void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var newImageFileIndex = _currentImageFileIndex + e.Increment;

		var canAdvanceToDesignatedImage = _imageFiles!.IsIndexWithinBounds(newImageFileIndex);
		_imageView.CanAdvanceToDesignatedImage = canAdvanceToDesignatedImage;

		if (canAdvanceToDesignatedImage)
		{
			_currentImageFileIndex = newImageFileIndex;
			_currentImageFile = _imageFiles![_currentImageFileIndex];

			await LoadCurrentImage();
		}
	}
}
