using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.TextHandling.Implementation;
using ImageFanReloaded.Keyboard;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.Controls;

public partial class ImageEditWindow : Window, IImageEditView
{
	public ImageEditWindow()
	{
		InitializeComponent();

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

	public IEditableImageFactory? EditableImageFactory { get; set; }
	public ISaveFileImageFormatFactory? SaveFileImageFormatFactory { get; set; }
	public ISaveFileDialogFactory? SaveFileDialogFactory { get; set; }

	public ImageFileData? ImageFileData { get; set; }

	public IContentTabItem? ContentTabItem { get; set; }

	public async Task LoadImage()
	{
		_editableImage = await EditableImageFactory!
			.CreateEditableImage(ImageFileData!.ImageFilePath);

		if (!IsImageLoaded)
		{
			Title = $"{ImageFileData!.ImageFileName} - error loading image";
		}

		SetControlsEnabledStatus(IsImageLoaded);

		RefreshContent();
	}

	public event EventHandler<ContentTabItemEventArgs>? FolderContentChanged;

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private readonly IDictionary<string, ComboBoxItem> _downsizeComboBoxValueToComboBoxItemMapping;

	private IGlobalParameters? _globalParameters;
	private StringComparison? _fileSystemStringComparison;

	private IEditableImage? _editableImage;
	private bool _hasUnsavedChanges;

	private bool _hasInProgressUiUpdate;

	private async void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();

			e.Handled = true;
		}
		else if (ShouldRevertChanges(keyModifiers, keyPressing))
		{
			await RevertChanges(keyPressing);

			e.Handled = true;
		}
		else if (ShouldSaveImageAs(keyModifiers, keyPressing))
		{
			await SaveImageAs(keyPressing);

			e.Handled = true;
		}
		else if (ShouldEditImage(keyModifiers, keyPressing))
		{
			await EditImage(keyPressing);

			e.Handled = true;
		}
		else if (ShouldDownsizeImage(keyModifiers, keyPressing))
		{
			await DownsizeImage(keyPressing);

			e.Handled = true;
		}
	}

	private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		if (_hasInProgressUiUpdate)
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

	private async void OnRotateLeft(object? sender, RoutedEventArgs e) => await RotateLeft();
	private async void OnRotateRight(object? sender, RoutedEventArgs e) => await RotateRight();

	private async void OnFlipHorizontally(object? sender, RoutedEventArgs e)
		=> await FlipHorizontally();
	private async void OnFlipVertically(object? sender, RoutedEventArgs e)
		=> await FlipVertically();

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

	private bool IsImageLoaded => _editableImage is not null;

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.EscapeKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldRevertChanges(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.UKey ||
			 keyPressing == GlobalParameters!.IKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldSaveImageAs(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.SKey ||
			 keyPressing == GlobalParameters!.JKey ||
			 keyPressing == GlobalParameters!.GKey ||
			 keyPressing == GlobalParameters!.PKey ||
			 keyPressing == GlobalParameters!.WKey ||
			 keyPressing == GlobalParameters!.TKey ||
			 keyPressing == GlobalParameters!.BKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldEditImage(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.LKey ||
			 keyPressing == GlobalParameters!.RKey ||
			 keyPressing == GlobalParameters!.HKey ||
			 keyPressing == GlobalParameters!.VKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldDownsizeImage(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.EKey ||
			 keyPressing == GlobalParameters!.DKey))
		{
			return true;
		}

		return false;
	}

	private async Task RevertChanges(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.UKey)
		{
			await Undo();
		}
		else if (keyPressing == GlobalParameters!.IKey)
		{
			await Redo();
		}
	}

	private async Task SaveImageAs(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.SKey)
		{
			await SaveImageWithFormat(default);
		}
		else if (keyPressing == GlobalParameters!.JKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.JpegSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.GKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.GifSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.PKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.PngSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.WKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.WebpSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.TKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.TiffSaveFileImageFormat);
		}
		else if (keyPressing == GlobalParameters!.BKey)
		{
			await SaveImageWithFormat(SaveFileImageFormatFactory!.BmpSaveFileImageFormat);
		}
	}

	private async Task EditImage(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.LKey)
		{
			await RotateLeft();
		}
		else if (keyPressing == GlobalParameters!.RKey)
		{
			await RotateRight();
		}
		else if (keyPressing == GlobalParameters!.HKey)
		{
			await FlipHorizontally();
		}
		else if (keyPressing == GlobalParameters!.VKey)
		{
			await FlipVertically();
		}
	}

	private async Task DownsizeImage(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (!IsImageLoaded)
		{
			return;
		}

		if (keyPressing == GlobalParameters!.EKey)
		{
			await DownsizeToPercentage();
		}
		else if (keyPressing == GlobalParameters!.DKey)
		{
			await DownsizeToDimensions();
		}
	}

	private async Task Undo()
	{
		if (!_undoButton.IsEnabled || _hasInProgressUiUpdate)
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
		if (!_redoButton.IsEnabled || _hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			_editableImage!.RedoLastEdit();
			await ApplyTransform(default);
		});
	}

	private async Task SaveImageWithFormat(ISaveFileImageFormat? saveFileImageFormat)
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

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

					if (HasSavedImageFileInCurrentFolder(imageToSaveFilePath, imageFolderPath))
					{
						FolderContentChanged?.Invoke(this, new ContentTabItemEventArgs(ContentTabItem!));
					}
				}
				catch
				{
					var saveImageAsErrorMessageBox = MessageBoxManager.GetMessageBoxStandard(
						"Image file save error",
						$"Could not save image file '{imageToSaveFilePath}'.",
						MsBox.Avalonia.Enums.ButtonEnum.Ok,
						MsBox.Avalonia.Enums.Icon.Error,
						WindowStartupLocation.CenterOwner);

					await saveImageAsErrorMessageBox.ShowWindowDialogAsync(this);
				}
			}
		});
	}

	private async Task RotateLeft()
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.RotateLeft());
		});
	}

	private async Task RotateRight()
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.RotateRight());
		});
	}

	private async Task FlipHorizontally()
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.FlipHorizontally());
		});
	}

	private async Task FlipVertically()
	{
		if (_hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			await ApplyTransform(() => _editableImage!.FlipVertically());
		});
	}

	private async Task DownsizeToPercentage()
	{
		if (!_downsizeToPercentageMenuItem.IsEnabled || _hasInProgressUiUpdate)
		{
			return;
		}

		await PerformUiUpdate(async () =>
		{
			var downsizePercentage = GetSelectedDownsizeValue(_downsizeToPercentageComboBox);

			await ApplyTransform(() => _editableImage!.DownsizeToPercentage(downsizePercentage));
		});
	}

	private async Task DownsizeToDimensions()
	{
		if (!_downsizeToDimensionsMenuItem.IsEnabled || _hasInProgressUiUpdate)
		{
			return;
		}

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
				$"Could not perform image transformation.",
				MsBox.Avalonia.Enums.ButtonEnum.Ok,
				MsBox.Avalonia.Enums.Icon.Error,
				WindowStartupLocation.CenterOwner);

			await applyTransformErrorMessageBox.ShowWindowDialogAsync(this);
		}
	}

	private bool HasSavedImageFileInCurrentFolder(
		string imageToSaveFilePath, string imageFolderPath)
		=> imageToSaveFilePath.StartsWith(imageFolderPath, _fileSystemStringComparison!.Value);

	private void RefreshContent()
	{
		if (!IsImageLoaded)
		{
			return;
		}

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

		_downsizeButton.IsEnabled = areControlsEnabled;

		_downsizeToPercentageMenuItem.IsEnabled = areControlsEnabled;
		_downsizeToPercentageComboBox.IsEnabled = areControlsEnabled;

		_downsizeToDimensionsMenuItem.IsEnabled = areControlsEnabled;
		_downsizeToDimensionsWidthComboBox.IsEnabled = areControlsEnabled;
		_downsizeToDimensionsHeightComboBox.IsEnabled = areControlsEnabled;

		_saveAsDropDownButton.IsEnabled = areControlsEnabled;
	}

	private void UpdateControls()
	{
		_displayImage.MaxWidth = _editableImage!.ImageSize.Width;
		_displayImage.MaxHeight = _editableImage!.ImageSize.Height;
		_displayImage.Source = _editableImage!.ImageToDisplay.GetBitmap();

		_undoButton.IsEnabled = _editableImage!.CanUndoLastEdit;
		_redoButton.IsEnabled = _editableImage!.CanRedoLastEdit;

		_downsizeToPercentageMenuItem.IsEnabled = false;
		_downsizeToDimensionsMenuItem.IsEnabled = false;

		SetDownsizeButtonEnabledStatus();

		Title = $"{ImageFileData!.ImageFileName} - {_editableImage!.ImageSize}";

		SizeToContent = SizeToContent.WidthAndHeight;
	}

	private void RegisterEvents()
	{
		_downsizeToPercentageComboBox.SelectionChanged +=
			OnDownsizeToPercentageComboxBoxSelectionChanged;

		_downsizeToDimensionsWidthComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
		_downsizeToDimensionsHeightComboBox.SelectionChanged +=
			OnDownsizeToDimensionsComboBoxSelectionChanged;
	}

	private void UnregisterEvents()
	{
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
		try
		{
			_hasInProgressUiUpdate = true;
			SetControlsEnabledStatus(false);

			await uiUpdateFunc();
		}
		finally
		{
			_hasInProgressUiUpdate = false;
			SetControlsEnabledStatus(true);

			RefreshContent();
		}
	}

	private void SetDownsizeButtonEnabledStatus()
		=> _downsizeButton.IsEnabled =
			_downsizeToPercentageMenuItem.IsEnabled || _downsizeToDimensionsMenuItem.IsEnabled;

	#endregion
}
