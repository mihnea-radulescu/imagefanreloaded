using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using MsBox.Avalonia;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;
using ImageFanReloaded.ImageHandling.Extensions;
using ImageFanReloaded.Keyboard;
using ImageFanReloaded.Mouse;

namespace ImageFanReloaded.Controls;

public partial class ImageEditWindow : Window, IImageEditView
{
	static ImageEditWindow()
	{
		TransformImageErrorMessage = string.Concat(
			"Could not perform image transformation.",
			Environment.NewLine,
			Environment.NewLine,
			"The selected image format does only support image reading.",
			Environment.NewLine,
			Environment.NewLine,
			"Consider saving the image in a different format, editing the saved image, and then retrying the image transformation."
		);

		SaveImageErrorMessage = string.Concat(
			@"Could not save image file ""{0}"".",
			Environment.NewLine,
			Environment.NewLine,
			"The selected file is not writable, or the selected image format does only support image reading.",
			Environment.NewLine,
			Environment.NewLine,
			"Consider saving the image as a different file, or in a different format."
		);
	}

	public ImageEditWindow()
	{
		InitializeComponent();

		_snapCropEdgesToolTip.Text =
			$"If the crop area is within {SnapCropEdgesThresholdInPixels} pixels of an image edge, it will automatically snap to the edge.";

		_downsizeComboBoxValueToComboBoxItemMapping = new Dictionary<string, ComboBoxItem>();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}

	public IGlobalParameters? GlobalParameters
	{
		get => _globalParameters;
		set
		{
			_globalParameters = value;

			_fileSystemStringComparison = _globalParameters!.NameComparer.ToStringComparison();
		}
	}

	public IMouseCursorFactory? MouseCursorFactory
	{
		get => _mouseCursorFactory;
		set
		{
			_mouseCursorFactory = value;

			_standardCursor = _mouseCursorFactory!.StandardCursor.GetCursor();
			_selectCursor = _mouseCursorFactory!.SelectCursor.GetCursor();
		}
	}

	public IEditableImageFactory? EditableImageFactory { get; set; }
	public ISaveFileImageFormatFactory? SaveFileImageFormatFactory { get; set; }
	public ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }

	public ImageFileData? ImageFileData { get; set; }

	public IContentTabItem? ContentTabItem { get; set; }

	public event EventHandler<ContentTabItemEventArgs>? ImageFileOverwritten;
	public event EventHandler<ContentTabItemEventArgs>? FolderContentChanged;

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private const int SnapCropEdgesThresholdInPixels = 5;

	private static readonly string TransformImageErrorMessage;
	private static readonly string SaveImageErrorMessage;

	private readonly IDictionary<string, ComboBoxItem> _downsizeComboBoxValueToComboBoxItemMapping;

	private IGlobalParameters? _globalParameters;
	private StringComparison? _fileSystemStringComparison;
	private IMouseCursorFactory? _mouseCursorFactory;

	private IEditableImage? _editableImage;

	private Cursor? _standardCursor;
	private Cursor? _selectCursor;

	private Point _mouseDownToImageCoordinates;
	private Point _mouseUpToImageCoordinates;

	private Point _topLeftPointToImage;
	private Point _bottomRightPointToImage;

	private bool _isLoading;
	private bool _hasInProgressUiUpdate;
	private bool _hasUnsavedChanges;

	private double DisplayImageWidth => _displayImage.Bounds.Width;
	private double DisplayImageHeight => _displayImage.Bounds.Height;

	private async void OnWindowLoaded(object? sender, RoutedEventArgs e) => await LoadImage();

	private async void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();

			e.Handled = true;
		}
		else if (ShouldUndo(keyModifiers, keyPressing))
		{
			await Undo();

			e.Handled = true;
		}
		else if (ShouldRedo(keyModifiers, keyPressing))
		{
			await Redo();

			e.Handled = true;
		}
		else if (ShouldRotate(keyModifiers, keyPressing))
		{
			Rotate();

			e.Handled = true;
		}
		else if (ShouldFlip(keyModifiers, keyPressing))
		{
			Flip();

			e.Handled = true;
		}
		else if (ShouldExecuteEffects(keyModifiers, keyPressing))
		{
			ExecuteEffects();

			e.Handled = true;
		}
		else if (ShouldSaveImageAs(keyModifiers, keyPressing))
		{
			SaveImageAs();

			e.Handled = true;
		}
		else if (ShouldCrop(keyModifiers, keyPressing))
		{
			await Crop();

			e.Handled = true;
		}
		else if (ShouldDownsizeImage(keyModifiers, keyPressing))
		{
			DownsizeImage();

			e.Handled = true;
		}
	}

	private void OnMouseDown(object? sender, PointerPressedEventArgs e)
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		ClearDrawnRectangle();

		_mouseDownToImageCoordinates = e.GetPosition(_displayImage);
	}

	private void OnMouseUp(object? sender, PointerReleasedEventArgs e)
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		if (e.InitialPressMouseButton == MouseButton.Left)
		{
			_mouseUpToImageCoordinates = e.GetPosition(_displayImage);

			ComputeTopLeftBottomRightPointsToImage();

			if (IsLineOrDotCropSelection())
			{
				ClearDrawnRectangle();
			}
			else
			{
				DrawCropToGridRectangle();
			}
		}
		else if (e.InitialPressMouseButton == MouseButton.Right)
		{
			ClearDrawnRectangle();
		}
	}

	private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		if (_isLoading || _hasInProgressUiUpdate)
		{
			e.Cancel = true;
		}
		else if (_hasUnsavedChanges)
		{
			e.Cancel = true;

			var closeWindowMessageBox = MessageBoxManager.GetMessageBoxStandard(
				"Unsaved image changes",
				"You have unsaved image changes. Are you sure you want to close the window?",
				MsBox.Avalonia.Enums.ButtonEnum.YesNoCancel,
				MsBox.Avalonia.Enums.Icon.Question,
				null,
				WindowStartupLocation.CenterOwner);

			var closeWindowButtonResult = await closeWindowMessageBox.ShowWindowDialogAsync(this);

			if (closeWindowButtonResult != MsBox.Avalonia.Enums.ButtonResult.Yes)
			{
				return;
			}
			else
			{
				_hasUnsavedChanges = false;

				Close();
			}
		}
		else
		{
			UnregisterEvents();

			_editableImage?.Dispose();
		}
	}

	private async void OnUndo(object? sender, RoutedEventArgs e) => await Undo();
	private async void OnRedo(object? sender, RoutedEventArgs e) => await Redo();

	private async void OnRotateLeft(object? sender, RoutedEventArgs e) => await RotateLeft();
	private async void OnRotateRight(object? sender, RoutedEventArgs e) => await RotateRight();

	private async void OnFlipHorizontally(object? sender, RoutedEventArgs e)
		=> await FlipHorizontally();
	private async void OnFlipVertically(object? sender, RoutedEventArgs e)
		=> await FlipVertically();

	private async void OnContrast(object? sender, RoutedEventArgs e)
		=> await Contrast();
	private async void OnGamma(object? sender, RoutedEventArgs e)
		=> await Gamma();
	private async void OnEnhance(object? sender, RoutedEventArgs e)
		=> await Enhance();
	private async void OnWhiteBalance(object? sender, RoutedEventArgs e)
		=> await WhiteBalance();
	private async void OnReduceNoise(object? sender, RoutedEventArgs e)
		=> await ReduceNoise();
	private async void OnSharpen(object? sender, RoutedEventArgs e)
		=> await Sharpen();
	private async void OnBlur(object? sender, RoutedEventArgs e)
		=> await Blur();

	private async void OnGrayscale(object? sender, RoutedEventArgs e)
		=> await Grayscale();
	private async void OnSepia(object? sender, RoutedEventArgs e)
		=> await Sepia();
	private async void OnNegative(object? sender, RoutedEventArgs e)
		=> await Negative();
	private async void OnOilPaint(object? sender, RoutedEventArgs e)
		=> await OilPaint();
	private async void OnEmboss(object? sender, RoutedEventArgs e)
		=> await Emboss();

	private async void OnSaveImageAsWithSameFormat(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(default);

	private async void OnSaveImageAsWithFormatJpeg(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatGif(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatPng(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatWebp(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatTiff(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
	private async void OnSaveImageAsWithFormatBmp(object? sender, RoutedEventArgs e)
		=> await SaveImageWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);

	private async void OnCrop(object? sender, RoutedEventArgs e) => await Crop();

	private void OnSnapCropEdgesCheckBoxIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var isCheckedSnapCropEdgesCheckBox = _snapCropEdgesCheckBox.IsChecked!.Value;

		if (isCheckedSnapCropEdgesCheckBox)
		{
			SnapCropRectangleToImageBounds();

			DrawCropToGridRectangle();

			LockSnapCropEdgesCheckBox();
		}
	}

	private void OnDownsizeToPercentageComboxBoxSelectionChanged(
		object? sender, SelectionChangedEventArgs e)
	{
		var selectedDownsizePercentage = GetSelectedDownsizeValue(
			_downsizeToPercentageComboBox);

		var computedDownsizedImageWidth =
			_editableImage!.ImageSize.Width * selectedDownsizePercentage / 100;
		var computedDownsizedImageHeight =
			_editableImage!.ImageSize.Height * selectedDownsizePercentage / 100;

		var canDownsize = computedDownsizedImageWidth >= 1 && computedDownsizedImageHeight >= 1;

		_downsizeToPercentageMenuItem.IsEnabled =
			canDownsize &&
			selectedDownsizePercentage != GetLastDownsizeValue(_downsizeToPercentageComboBox);

		SetDownsizeButtonEnabledStatus();
	}

	private void OnDownsizeToDimensionsComboBoxSelectionChanged(
		object? sender, SelectionChangedEventArgs e)
	{
		var selectedDownsizeDimensionsWidth = GetSelectedDownsizeValue(
			_downsizeToDimensionsWidthComboBox);
		var computedDownsizeDimensionsWidth = selectedDownsizeDimensionsWidth;

		var selectedDownsizeDimensionsHeight = GetSelectedDownsizeValue(
			_downsizeToDimensionsHeightComboBox);
		var computedDownsizeDimensionsHeight = selectedDownsizeDimensionsHeight;

		_downsizeToDimensionsWidthComboBox.SelectionChanged -=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
		_downsizeToDimensionsHeightComboBox.SelectionChanged -=
			OnDownsizeToDimensionsComboBoxSelectionChanged;

		var canDownsizeWidth = true;
		var canDownsizeHeight = true;

		if (sender == _downsizeToDimensionsWidthComboBox)
		{
			computedDownsizeDimensionsHeight = (int)
				((double)selectedDownsizeDimensionsWidth / _editableImage!.ImageSize.AspectRatio);

			canDownsizeHeight = computedDownsizeDimensionsHeight >= 1;

			if (canDownsizeHeight)
			{
				SetSelectedDownsizeValue(
					_downsizeToDimensionsHeightComboBox, computedDownsizeDimensionsHeight);
			}
		}
		else if (sender == _downsizeToDimensionsHeightComboBox)
		{
			computedDownsizeDimensionsWidth = (int)
				((double)selectedDownsizeDimensionsHeight * _editableImage!.ImageSize.AspectRatio);

			canDownsizeWidth = computedDownsizeDimensionsWidth >= 1;

			if (canDownsizeWidth)
			{
				SetSelectedDownsizeValue(
					_downsizeToDimensionsWidthComboBox, computedDownsizeDimensionsWidth);
			}
		}

		_downsizeToDimensionsWidthComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
		_downsizeToDimensionsHeightComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;

		var isDownsizeableDimensionWidth =
			canDownsizeWidth &&
			computedDownsizeDimensionsWidth != GetLastDownsizeValue(
				_downsizeToDimensionsWidthComboBox);

		var isDownsizeableDimensionHeight =
			canDownsizeHeight &&
			computedDownsizeDimensionsHeight != GetLastDownsizeValue(
				_downsizeToDimensionsHeightComboBox);

		_downsizeToDimensionsMenuItem.IsEnabled =
			isDownsizeableDimensionWidth && isDownsizeableDimensionHeight;

		SetDownsizeButtonEnabledStatus();
	}

	private async void OnDownsizeToPercentage(object? sender, RoutedEventArgs e)
		=> await DownsizeToPercentage();
	private async void OnDownsizeToDimensions(object? sender, RoutedEventArgs e)
		=> await DownsizeToDimensions();

	private async Task LoadImage()
	{
		_isLoading = true;

		SetLoadingImageTitle();
		SetControlsEnabledStatus(false);

		_editableImage = await EditableImageFactory!
			.CreateEditableImage(ImageFileData!.ImageFilePath);

		var isImageLoaded = _editableImage is not null;
		SetControlsEnabledStatus(isImageLoaded);

		if (isImageLoaded)
		{
			RefreshContent();
		}
		else
		{
			SetImageLoadErrorTitle();
		}

		_isLoading = false;
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldUndo(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.UKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldRedo(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.IKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldRotate(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.RKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldFlip(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.FKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldExecuteEffects(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAs(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldCrop(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.CKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldDownsizeImage(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers,
		ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.DKey)
		{
			return true;
		}

		return false;
	}

	private void Rotate()
	{
		if (!_rotateDropDownButton.IsEnabled)
		{
			return;
		}

		ExpandDropDownButton(_rotateDropDownButton);
	}

	private void Flip()
	{
		if (!_flipDropDownButton.IsEnabled)
		{
			return;
		}

		ExpandDropDownButton(_flipDropDownButton);
	}

	private void ExecuteEffects()
	{
		if (!_effectsDropDownButton.IsEnabled)
		{
			return;
		}

		ExpandDropDownButton(_effectsDropDownButton);
	}

	private void SaveImageAs()
	{
		if (!_saveAsDropDownButton.IsEnabled)
		{
			return;
		}

		ExpandDropDownButton(_saveAsDropDownButton);
	}

	private void DownsizeImage()
	{
		if (!_downsizeDropDownButton.IsEnabled)
		{
			return;
		}

		ExpandDropDownButton(_downsizeDropDownButton);
	}

	private async Task Undo()
	{
		if (!_undoButton.IsEnabled)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			_editableImage!.UndoLastEdit();
			await ApplyTransform(default);
		});
	}

	private async Task Redo()
	{
		if (!_redoButton.IsEnabled)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			_editableImage!.RedoLastEdit();
			await ApplyTransform(default);
		});
	}

	private async Task RotateLeft()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.RotateLeft());
		});
	}

	private async Task RotateRight()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.RotateRight());
		});
	}

	private async Task FlipHorizontally()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.FlipHorizontally());
		});
	}

	private async Task FlipVertically()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.FlipVertically());
		});
	}

	private async Task Contrast()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Contrast());
		});
	}

	private async Task Gamma()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Gamma());
		});
	}

	private async Task Enhance()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Enhance());
		});
	}

	private async Task WhiteBalance()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.WhiteBalance());
		});
	}

	private async Task ReduceNoise()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.ReduceNoise());
		});
	}

	private async Task Sharpen()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Sharpen());
		});
	}

	private async Task Blur()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Blur());
		});
	}

	private async Task Grayscale()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Grayscale());
		});
	}

	private async Task Sepia()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Sepia());
		});
	}

	private async Task Negative()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Negative());
		});
	}

	private async Task OilPaint()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.OilPaint());
		});
	}

	private async Task Emboss()
	{
		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.Emboss());
		});
	}

	private async Task SaveImageWithFormat(ISaveFileImageFormat? saveFileImageFormat)
	{
		await PerformUiUpdate(async () =>
		{
			var hasSameFormat = saveFileImageFormat is null;

			var imageFileName = hasSameFormat
				? ImageFileData!.ImageFileName
				: $"{ImageFileData!.ImageFileNameWithoutExtension}{saveFileImageFormat!.Extension}";
			var imageFilePath = ImageFileData!.ImageFilePath;
			var imageFolderPath = ImageFileData!.ImageFolderPath;

			var saveFileDialog = SaveFileDialogFactory!.GetSaveFileDialog();
			var saveFileDialogTitle = hasSameFormat
				? $"Select image file"
				: $"Select {saveFileImageFormat!.Name} image file";
			var imageToSaveFilePath = await saveFileDialog.ShowDialog(
				imageFileName, imageFolderPath, saveFileDialogTitle);

			if (imageToSaveFilePath is not null)
			{
				try
				{
					if (hasSameFormat)
					{
						await _editableImage!.SaveImageWithSameFormat(imageToSaveFilePath);
					}
					else
					{
						await _editableImage!.SaveImageWithFormat(
							imageToSaveFilePath, saveFileImageFormat!);
					}

					_hasUnsavedChanges = false;

					if (saveFileDialog.ShouldAlwaysRefreshSaveFolder)
					{
						FolderContentChanged?.Invoke(
							this, new ContentTabItemEventArgs(ContentTabItem!));
					}
					else if (HasOverwrittenCurrentImageFile(imageToSaveFilePath, imageFilePath))
					{
						ImageFileOverwritten?.Invoke(
							this, new ContentTabItemEventArgs(ContentTabItem!));
					}
					else if (HasSavedImageFileInCurrentFolder(imageToSaveFilePath, imageFolderPath))
					{
						FolderContentChanged?.Invoke(
							this, new ContentTabItemEventArgs(ContentTabItem!));
					}
				}
				catch
				{
					var imageToSaveFileName = GetFileNameFromPath(imageToSaveFilePath);

					var saveImageAsErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
						"Image save error",
						string.Format(SaveImageErrorMessage, imageToSaveFileName),
						MsBox.Avalonia.Enums.ButtonEnum.Ok,
						MsBox.Avalonia.Enums.Icon.Error,
						null,
						WindowStartupLocation.CenterOwner);

					await saveImageAsErrorMessageBox.ShowWindowDialogAsync(this);
				}
			}
		});
	}

	private async Task Crop()
	{
		if (!_cropButton.IsEnabled)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			var cropToEditableImageRectangle = GetCropToEditableImageRectangle();

			await ApplyTransform(() => _editableImage!.Crop(
				(int)cropToEditableImageRectangle.Left,
				(int)cropToEditableImageRectangle.Top,
				(int)cropToEditableImageRectangle.Width,
				(int)cropToEditableImageRectangle.Height));
		});
	}

	private async Task DownsizeToPercentage()
	{
		await PerformUiUpdate(async () =>
		{
			var downsizePercentage = GetSelectedDownsizeValue(_downsizeToPercentageComboBox);

			await ApplyTransform(() => _editableImage!.DownsizeToPercentage(downsizePercentage));
		});
	}

	private async Task DownsizeToDimensions()
	{
		await PerformUiUpdate(async () =>
		{
			var downsizeDimensionsWidth = GetSelectedDownsizeValue(
				_downsizeToDimensionsWidthComboBox);
			var downsizeDimensionsHeight = GetSelectedDownsizeValue(
				_downsizeToDimensionsHeightComboBox);

			await ApplyTransform(() => _editableImage!.DownsizeToDimensions(
				downsizeDimensionsWidth, downsizeDimensionsHeight));
		});
	}

	private async Task ApplyTransform(Action? transformImageAction)
	{
		try
		{
			if (transformImageAction is not null)
			{
				await Task.Run(transformImageAction);
			}

			_displayImage.Source = _editableImage!.ImageToDisplay.GetBitmap();
			_hasUnsavedChanges = true;
		}
		catch
		{
			var applyTransformErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
				"Image transformation error",
				TransformImageErrorMessage,
				MsBox.Avalonia.Enums.ButtonEnum.Ok,
				MsBox.Avalonia.Enums.Icon.Error,
				null,
				WindowStartupLocation.CenterOwner);

			await applyTransformErrorMessageBox.ShowWindowDialogAsync(this);
		}
	}

	private bool HasOverwrittenCurrentImageFile(string imageToSaveFilePath, string imageFilePath)
		=> imageToSaveFilePath.Equals(imageFilePath, _fileSystemStringComparison!.Value);

	private bool HasSavedImageFileInCurrentFolder(
		string imageToSaveFilePath, string imageFolderPath)
		=> imageToSaveFilePath.StartsWith(imageFolderPath, _fileSystemStringComparison!.Value);

	private void RefreshContent()
	{
		UnregisterEvents();

		ClearDownsizeComboBoxValueToComboBoxItemMapping();

		PopulateDownsizeComboBox(_downsizeToPercentageComboBox, 1, 100, "%");
		PopulateDownsizeComboBox(
			_downsizeToDimensionsWidthComboBox, 1, _editableImage!.ImageSize.Width, "px");
		PopulateDownsizeComboBox(
			_downsizeToDimensionsHeightComboBox, 1, _editableImage!.ImageSize.Height, "px");

		UpdateControls();

		RegisterEvents();
	}

	private void SetControlsEnabledStatus(bool areControlsEnabled)
	{
		_undoButton.IsEnabled = areControlsEnabled;
		_redoButton.IsEnabled = areControlsEnabled;

		_rotateDropDownButton.IsEnabled = areControlsEnabled;
		_flipDropDownButton.IsEnabled = areControlsEnabled;

		_effectsDropDownButton.IsEnabled = areControlsEnabled;

		_saveAsDropDownButton.IsEnabled = areControlsEnabled;

		_cropButton.IsEnabled = areControlsEnabled;
		_snapCropEdgesCheckBox.IsEnabled = areControlsEnabled;
		_displayImage.Cursor = areControlsEnabled
			? _selectCursor
			: _standardCursor;

		_downsizeDropDownButton.IsEnabled = areControlsEnabled;
		_downsizeToPercentageComboBox.IsEnabled = areControlsEnabled;
		_downsizeToDimensionsWidthComboBox.IsEnabled = areControlsEnabled;
		_downsizeToDimensionsHeightComboBox.IsEnabled = areControlsEnabled;
	}

	private void UpdateControls()
	{
		ClearDrawnRectangle();

		UpdateDisplayLayouts();

		UpdateControlsEnabledStatus();

		SetDownsizeButtonEnabledStatus();

		SetImageTitle();
	}

	private void UpdateControlsEnabledStatus()
	{
		_undoButton.IsEnabled = _editableImage!.CanUndoLastEdit;
		_redoButton.IsEnabled = _editableImage!.CanRedoLastEdit;

		_downsizeToPercentageMenuItem.IsEnabled = false;
		_downsizeToDimensionsMenuItem.IsEnabled = false;
	}

	private void UpdateDisplayLayouts()
	{
		SizeToContent = SizeToContent.Manual;

		_displayImage.MaxWidth = _editableImage!.ImageSize.Width;
		_displayImage.MaxHeight = _editableImage!.ImageSize.Height;
		_displayImage.Source = _editableImage!.ImageToDisplay.GetBitmap();
		_displayImage.InvalidateMeasure();
		_displayImage.InvalidateArrange();
		_displayImage.UpdateLayout();

		_displayGrid.InvalidateMeasure();
		_displayGrid.InvalidateArrange();
		_displayGrid.UpdateLayout();

		SizeToContent = SizeToContent.WidthAndHeight;
	}

	private void RegisterEvents()
	{
		_snapCropEdgesCheckBox.IsCheckedChanged +=
			OnSnapCropEdgesCheckBoxIsCheckedChanged;

		_downsizeToPercentageComboBox.SelectionChanged +=
			OnDownsizeToPercentageComboxBoxSelectionChanged;

		_downsizeToDimensionsWidthComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
		_downsizeToDimensionsHeightComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
	}

	private void UnregisterEvents()
	{
		_snapCropEdgesCheckBox.IsCheckedChanged -=
			OnSnapCropEdgesCheckBoxIsCheckedChanged;

		_downsizeToPercentageComboBox.SelectionChanged -=
			OnDownsizeToPercentageComboxBoxSelectionChanged;

		_downsizeToDimensionsWidthComboBox.SelectionChanged -=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
		_downsizeToDimensionsHeightComboBox.SelectionChanged -=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
	}

	private void PopulateDownsizeComboBox(
		ComboBox downsizeComboBox,
		int minimumDownsizeValue,
		int maximumDownsizeValue,
		string downsizeContentSuffix)
	{
		downsizeComboBox.Items.Clear();

		for (var aDownsizeValue = minimumDownsizeValue;
				 aDownsizeValue <= maximumDownsizeValue;
				 aDownsizeValue++)
		{
			var downsizeComboBoxItem = new ComboBoxItem
			{
				Tag = aDownsizeValue,
				Content = $"{aDownsizeValue}{downsizeContentSuffix}"
			};

			downsizeComboBox.Items.Add(downsizeComboBoxItem);

			var downsizeComboBoxKey = GetDownsizeComboBoxKey(downsizeComboBox, aDownsizeValue);
			_downsizeComboBoxValueToComboBoxItemMapping.Add(
				downsizeComboBoxKey, downsizeComboBoxItem);

			if (aDownsizeValue == maximumDownsizeValue)
			{
				downsizeComboBox.SelectedItem = downsizeComboBoxItem;
			}
		}
	}

	private static int GetSelectedDownsizeValue(ComboBox downsizeComboBox)
	{
		var selectedDownsizeComboBoxItem = (ComboBoxItem)downsizeComboBox.SelectedItem!;

		return GetDownsizeValue(selectedDownsizeComboBoxItem);
	}

	private static int GetLastDownsizeValue(ComboBox downsizeComboBox)
	{
		var downsizeComboBoxItemCount = downsizeComboBox.ItemCount;
		var lastDownsizeComboBoxItem =
			(ComboBoxItem)downsizeComboBox.Items[downsizeComboBoxItemCount - 1]!;

		return GetDownsizeValue(lastDownsizeComboBoxItem);
	}

	private static int GetDownsizeValue(ComboBoxItem downsizeComboBoxItem)
		=> (int)downsizeComboBoxItem.Tag!;

	private void SetSelectedDownsizeValue(
		ComboBox downsizeComboBox, int selectedDownsizeValue)
	{
		var selectedDownsizeComboBoxKey =
			GetDownsizeComboBoxKey(downsizeComboBox, selectedDownsizeValue);
		var selectedDownsizeComboBoxItem =
			_downsizeComboBoxValueToComboBoxItemMapping[selectedDownsizeComboBoxKey];

		downsizeComboBox.SelectedItem = selectedDownsizeComboBoxItem;
	}

	private void ClearDownsizeComboBoxValueToComboBoxItemMapping()
		=> _downsizeComboBoxValueToComboBoxItemMapping.Clear();

	private static string GetDownsizeComboBoxKey(
		ComboBox downsizeComboBox, int downsizeValue) => $"{downsizeComboBox.Name}_{downsizeValue}";

	private async Task PerformUiUpdate(Func<Task> uiUpdateFunc)
	{
		_hasInProgressUiUpdate = true;
		SetControlsEnabledStatus(false);

		await uiUpdateFunc();

		_hasInProgressUiUpdate = false;
		SetControlsEnabledStatus(true);

		RefreshContent();
	}

	private void SetDownsizeButtonEnabledStatus()
		=> _downsizeDropDownButton.IsEnabled =
			_downsizeToPercentageMenuItem.IsEnabled || _downsizeToDimensionsMenuItem.IsEnabled;

	private void SetLoadingImageTitle()
		=> Title = $"{ImageFileData!.ImageFileName} - loading image...";

	private void SetImageTitle()
		=> Title = $"{ImageFileData!.ImageFileName} - {_editableImage!.ImageSize}";

	private void SetImageLoadErrorTitle()
		=> Title = $"{ImageFileData!.ImageFileName} - image read error";

	private static void ExpandDropDownButton(DropDownButton dropDownButton)
	{
		var menuFlyout = (MenuFlyout)dropDownButton.Flyout!;
		menuFlyout.ShowAt(dropDownButton);

		var firstEnabledMenuItem = menuFlyout.Items
			.Cast<MenuItem>()
			.Where(aMenuItem => aMenuItem.IsEnabled)
			.First();

		firstEnabledMenuItem.IsSelected = true;
	}

	private void ClearDrawnRectangle()
	{
		_cropOverlayCanvas.Children.Clear();

		_cropButton.IsEnabled = false;

		_snapCropEdgesCheckBox.IsChecked = false;
		_snapCropEdgesCheckBox.IsEnabled = false;
	}

	private void DrawRectangle(Rectangle drawingCropRectangle)
	{
		_cropOverlayCanvas.Children.Clear();
		_cropOverlayCanvas.Children.Add(drawingCropRectangle);

		var shouldPerformCrop =
			_topLeftPointToImage.X > 0 ||
			_topLeftPointToImage.Y > 0 ||
			_bottomRightPointToImage.X < DisplayImageWidth ||
			_bottomRightPointToImage.Y < DisplayImageHeight;

		_cropButton.IsEnabled = shouldPerformCrop;

		_snapCropEdgesCheckBox.IsEnabled = ShouldEnableSnapCropEdgesCheckBox();
	}

	private bool ShouldEnableSnapCropEdgesCheckBox()
	{
		if (_topLeftPointToImage.X <= SnapCropEdgesThresholdInPixels)
		{
			return true;
		}

		if (_topLeftPointToImage.Y <= SnapCropEdgesThresholdInPixels)
		{
			return true;
		}

		if (DisplayImageWidth - _bottomRightPointToImage.X <= SnapCropEdgesThresholdInPixels)
		{
			return true;
		}

		if (DisplayImageHeight - _bottomRightPointToImage.Y <= SnapCropEdgesThresholdInPixels)
		{
			return true;
		}

		return false;
	}

	private void LockSnapCropEdgesCheckBox() => _snapCropEdgesCheckBox.IsEnabled = false;

	private void NormalizePointsToImage()
	{
		if (_topLeftPointToImage.X < 0)
		{
			_topLeftPointToImage = new Point(0, _topLeftPointToImage.Y);
		}

		if (_topLeftPointToImage.Y < 0)
		{
			_topLeftPointToImage = new Point(_topLeftPointToImage.X, 0);
		}

		if (DisplayImageWidth - _bottomRightPointToImage.X < 0)
		{
			_bottomRightPointToImage = new Point(DisplayImageWidth, _bottomRightPointToImage.Y);
		}

		if (DisplayImageHeight - _bottomRightPointToImage.Y < 0)
		{
			_bottomRightPointToImage = new Point(_bottomRightPointToImage.X, DisplayImageHeight);
		}
	}

	private void SnapCropRectangleToImageBounds()
	{
		if (_topLeftPointToImage.X <= SnapCropEdgesThresholdInPixels)
		{
			_topLeftPointToImage = new Point(0, _topLeftPointToImage.Y);
		}

		if (_topLeftPointToImage.Y <= SnapCropEdgesThresholdInPixels)
		{
			_topLeftPointToImage = new Point(_topLeftPointToImage.X, 0);
		}

		if (DisplayImageWidth - _bottomRightPointToImage.X <= SnapCropEdgesThresholdInPixels)
		{
			_bottomRightPointToImage = new Point(DisplayImageWidth, _bottomRightPointToImage.Y);
		}

		if (DisplayImageHeight - _bottomRightPointToImage.Y <= SnapCropEdgesThresholdInPixels)
		{
			_bottomRightPointToImage = new Point(_bottomRightPointToImage.X, DisplayImageHeight);
		}
	}

	private Point GetPointToGrid(Point pointToImage)
		=> _displayImage.TranslatePoint(pointToImage, _displayGrid)!.Value;

	private void DrawCropToGridRectangle()
	{
		NormalizePointsToImage();

		var topLeftPointToGrid = GetPointToGrid(_topLeftPointToImage);
		var bottomRightPointToGrid = GetPointToGrid(_bottomRightPointToImage);

		var cropToGridRectangle = new Rect(topLeftPointToGrid, bottomRightPointToGrid);

		var drawingCropRectangle = new Rectangle
		{
			Stroke = Brushes.DodgerBlue,
			StrokeThickness = 2,
			Width = cropToGridRectangle.Width,
			Height = cropToGridRectangle.Height
		};

		Canvas.SetLeft(drawingCropRectangle, cropToGridRectangle.Left);
		Canvas.SetTop(drawingCropRectangle, cropToGridRectangle.Top);

		DrawRectangle(drawingCropRectangle);
	}

	private Rect GetCropToEditableImageRectangle()
	{
		var editableImageToDisplayImageScale =
			(double)_editableImage!.ImageSize.Width / DisplayImageWidth;

		var topLeftPointToEditableImage =
			_topLeftPointToImage * editableImageToDisplayImageScale;
		var bottomRightPointToEditableImage =
			_bottomRightPointToImage * editableImageToDisplayImageScale;

		var cropToEditableImageRectangle = new Rect(
			topLeftPointToEditableImage, bottomRightPointToEditableImage);
		return cropToEditableImageRectangle;
	}

	private void ComputeTopLeftBottomRightPointsToImage()
	{
		_mouseDownToImageCoordinates.Deconstruct(out var x1, out var y1);
		_mouseUpToImageCoordinates.Deconstruct(out var x2, out var y2);

		_topLeftPointToImage = new Point(Math.Min(x1, x2), Math.Min(y1, y2));
		_bottomRightPointToImage = new Point(Math.Max(x1, x2), Math.Max(y1, y2));
	}

	private bool IsLineOrDotCropSelection()
		=> _topLeftPointToImage.X == _bottomRightPointToImage.X ||
		   _topLeftPointToImage.Y == _bottomRightPointToImage.Y;

	private static string GetFileNameFromPath(string filePath)
		=> System.IO.Path.GetFileName(filePath);

	#endregion
}
