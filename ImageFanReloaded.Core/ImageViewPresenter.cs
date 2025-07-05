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
		ITabOptions tabOptions,
		IImageView imageView)
	{
		_discQueryEngine = discQueryEngine;
		_inputPathHandler = inputPathHandler;
		_tabOptions = tabOptions;

		_nameComparison = globalParameters.NameComparer.ToStringComparison();

		_imageView = imageView;
		_imageView.ImageChanged += OnImageChanged;

		_imageFiles = new List<IImageFile>();
		_currentImageFileIndex = 0;
	}
	
	public async Task SetUpAccessToImages()
	{
		_imageFiles = await _discQueryEngine.GetImageFiles(_inputPathHandler.FolderPath!, false);

		(_currentImageFile, _currentImageFileIndex) = _imageFiles
			.Select((anImageFile, index) => (anImageFile, index))
			.Single(anImageFileWithIndex =>
				anImageFileWithIndex.anImageFile.ImageFileData.ImageFilePath.Equals(
					_inputPathHandler.FilePath,
					_nameComparison));
		
		await LoadCurrentImage();
	}
	
	#region Private
	
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IInputPathHandler _inputPathHandler;
	private readonly ITabOptions _tabOptions;

	private readonly StringComparison _nameComparison;

	private readonly IImageView _imageView;

	private IReadOnlyList<IImageFile> _imageFiles;
	private IImageFile? _currentImageFile;
	private int _currentImageFileIndex;

	private async Task LoadCurrentImage()
	{
		_currentImageFile!.ReadImageDataFromDisc(_tabOptions.ApplyImageOrientation);
		
		await _imageView.SetImage(_currentImageFile);
	}

	private async void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var newImageFileIndex = _currentImageFileIndex + e.Increment;

		var canAdvanceToDesignatedImage = _imageFiles.IsIndexWithinBounds(newImageFileIndex);
		_imageView.CanAdvanceToDesignatedImage = canAdvanceToDesignatedImage;

		if (canAdvanceToDesignatedImage)
		{
			_currentImageFileIndex = newImageFileIndex;
			_currentImageFile = _imageFiles[_currentImageFileIndex];

			await LoadCurrentImage();
		}
	}

	#endregion
}
