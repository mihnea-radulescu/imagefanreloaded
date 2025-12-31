using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class TabOptionsWindow : Window, ITabOptionsView
{
	public TabOptionsWindow()
	{
		InitializeComponent();

		_tabOptionChanges = new TabOptionChanges();

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
	}

	public IGlobalParameters? GlobalParameters { get; set; }
	public ITabOptions? TabOptions { get; set; }

	public IContentTabItem? ContentTabItem { get; set; }

	public event EventHandler<TabOptionsChangedEventArgs>? TabOptionsChanged;

	public void PopulateTabOptions()
	{
		PopulateFolderOrderings();
		PopulateFolderOrderingDirections();

		PopulateImageFileOrderings();
		PopulateImageFileOrderingDirections();

		PopulateImageViewDisplayModes();

		PopulateThumbnailSizes();

		SetRecursiveFolderBrowsing();
		SetGlobalOrderingForRecursiveFolderBrowsing();

		SetShowImageViewImageInfo();

		SetPanelsSplittingRatioSlider();

		PopulateSlideshowIntervals();

		SetApplyImageOrientation();
		SetShowThumbnailImageFileName();

		PopulateKeyboardScrollThumbnailIncrements();
		PopulateUpsizeFullScreenImagesUpToScreenSizes();

		RegisterTabOptionEvents();
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private readonly TabOptionChanges _tabOptionChanges;

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			Close();

			e.Handled = true;
		}
	}

	private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
	{
		TabOptionsChanged?.Invoke(
			this, new TabOptionsChangedEventArgs(ContentTabItem!, TabOptions!, _tabOptionChanges));

		UnregisterTabOptionEvents();
	}

	private void OnFolderOrderingSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrdering = (FileSystemEntryInfoOrdering)folderOrderingComboBoxItem.Tag!;

		TabOptions!.FolderOrdering = folderOrdering;
		_tabOptionChanges.HasChangedFolderOrdering = true;
	}

	private void OnFolderOrderingDirectionSelectionChanged(
		object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingDirectionComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrderingDirection =
			(FileSystemEntryInfoOrderingDirection)folderOrderingDirectionComboBoxItem.Tag!;

		TabOptions!.FolderOrderingDirection = folderOrderingDirection;
		_tabOptionChanges.HasChangedFolderOrderingDirection = true;
	}

	private void OnImageFileOrderingSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var imageFileOrderingComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var imageFileOrdering = (FileSystemEntryInfoOrdering)imageFileOrderingComboBoxItem.Tag!;

		TabOptions!.ImageFileOrdering = imageFileOrdering;
		_tabOptionChanges.HasChangedImageFileOrdering = true;
	}

	private void OnImageFileOrderingDirectionSelectionChanged(
		object? sender, SelectionChangedEventArgs e)
	{
		var imageFileOrderingDirectionComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var imageFileOrderingDirection =
			(FileSystemEntryInfoOrderingDirection)imageFileOrderingDirectionComboBoxItem.Tag!;

		TabOptions!.ImageFileOrderingDirection = imageFileOrderingDirection;
		_tabOptionChanges.HasChangedImageFileOrderingDirection = true;
	}

	private void OnImageViewDisplayModeSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var imageViewDisplayModeComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var imageViewDisplayMode =
			(ImageViewDisplayMode)imageViewDisplayModeComboBoxItem.Tag!;

		TabOptions!.ImageViewDisplayMode = imageViewDisplayMode;
		_tabOptionChanges.HasChangedImageViewDisplayMode = true;
	}

	private void OnThumbnailSizeSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var thumbnailSizeComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var thumbnailSize = (ThumbnailSize)thumbnailSizeComboBoxItem.Tag!;

		TabOptions!.ThumbnailSize = thumbnailSize;
		_tabOptionChanges.HasChangedThumbnailSize = true;
	}

	private void OnRecursiveFolderBrowsingIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var recursiveFolderBrowsing = _recursiveFolderBrowsingCheckBox.IsChecked!.Value;

		TabOptions!.RecursiveFolderBrowsing = recursiveFolderBrowsing;
		_tabOptionChanges.HasChangedRecursiveFolderBrowsing = true;
	}

	private void OnGlobalOrderingForRecursiveFolderBrowsingIsCheckedChanged(
		object? sender, RoutedEventArgs e)
	{
		var globalOrderingForRecursiveFolderBrowsing =
			_globalOrderingForRecursiveFolderBrowsingCheckBox.IsChecked!.Value;

		TabOptions!.GlobalOrderingForRecursiveFolderBrowsing =
			globalOrderingForRecursiveFolderBrowsing;
		_tabOptionChanges.HasChangedGlobalOrderingForRecursiveFolderBrowsing = true;
	}

	private void OnShowImageViewImageInfoIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var showImageViewImageInfo = _showImageViewImageInfoCheckBox.IsChecked!.Value;

		TabOptions!.ShowImageViewImageInfo = showImageViewImageInfo;
		_tabOptionChanges.HasChangedShowImageViewImageInfo = true;
	}

	private void OnPanelsSplittingRatioChanged(object? sender, RangeBaseValueChangedEventArgs e)
	{
		var panelsSplittingRatio = (int)_panelsSplittingRatioSlider.Value;

		TabOptions!.PanelsSplittingRatio = panelsSplittingRatio;
		_tabOptionChanges.HasChangedPanelsSplittingRatio = true;
	}

	private void OnSlideshowIntervalSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var slideshowIntervalComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var slideshowInterval = (SlideshowInterval)slideshowIntervalComboBoxItem.Tag!;

		TabOptions!.SlideshowInterval = slideshowInterval;
		_tabOptionChanges.HasChangedSlideshowInterval = true;
	}

	private void OnApplyImageOrientationIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var applyImageOrientation = _applyImageOrientationCheckBox.IsChecked!.Value;

		TabOptions!.ApplyImageOrientation = applyImageOrientation;
		_tabOptionChanges.HasChangedApplyImageOrientation = true;
	}

	private void OnShowThumbnailImageFileNameIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var showThumbnailImageFileName = _showThumbnailImageFileNameCheckBox.IsChecked!.Value;

		TabOptions!.ShowThumbnailImageFileName = showThumbnailImageFileName;
		_tabOptionChanges.HasChangedShowThumbnailImageFileName = true;
	}

	private void OnKeyboardScrollThumbnailIncrementSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var keyboardScrollThumbnailIncrementComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var keyboardScrollThumbnailIncrement =
			(KeyboardScrollThumbnailIncrement)keyboardScrollThumbnailIncrementComboBoxItem.Tag!;

		TabOptions!.KeyboardScrollThumbnailIncrement = keyboardScrollThumbnailIncrement;
		_tabOptionChanges.HasChangedKeyboardScrollThumbnailIncrement = true;
	}

	private void OnUpsizeFullScreenImagesUpToScreenSizeSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var upsizeFullScreenImagesUpToScreenSizeComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var upsizeFullScreenImagesUpToScreenSize =
			(UpsizeFullScreenImagesUpToScreenSize)upsizeFullScreenImagesUpToScreenSizeComboBoxItem.Tag!;

		TabOptions!.UpsizeFullScreenImagesUpToScreenSize = upsizeFullScreenImagesUpToScreenSize;
		_tabOptionChanges.HasChangedUpsizeFullScreenImagesUpToScreenSize = true;
	}

	private void OnSaveAsDefaultIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var saveAsDefault = _saveAsDefaultCheckBox.IsChecked!.Value;

		_tabOptionChanges.ShouldSaveAsDefault = saveAsDefault;
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EscapeKey)
		{
			return true;
		}

		return false;
	}

	private void PopulateFolderOrderings()
	{
		var folderOrderingValues = Enum.GetValues<FileSystemEntryInfoOrdering>();

		foreach (var aFolderOrderingValue in folderOrderingValues)
		{
			var aFolderOrderingItem = new ComboBoxItem
			{
				Tag = aFolderOrderingValue,
				Content = aFolderOrderingValue.Description
			};

			_folderOrderingComboBox.Items.Add(aFolderOrderingItem);

			if (aFolderOrderingValue == TabOptions!.FolderOrdering)
			{
				_folderOrderingComboBox.SelectedItem = aFolderOrderingItem;
			}
		}
	}

	private void PopulateFolderOrderingDirections()
	{
		var folderOrderingDirectionValues = Enum.GetValues<FileSystemEntryInfoOrderingDirection>();

		foreach (var aFolderOrderingDirectionValue in folderOrderingDirectionValues)
		{
			var aFolderOrderingDirectionItem = new ComboBoxItem
			{
				Tag = aFolderOrderingDirectionValue,
				Content = aFolderOrderingDirectionValue.ToString()
			};

			_folderOrderingDirectionComboBox.Items.Add(aFolderOrderingDirectionItem);

			if (aFolderOrderingDirectionValue == TabOptions!.FolderOrderingDirection)
			{
				_folderOrderingDirectionComboBox.SelectedItem = aFolderOrderingDirectionItem;
			}
		}
	}

	private void PopulateImageFileOrderings()
	{
		var imageFileOrderingValues = Enum.GetValues<FileSystemEntryInfoOrdering>();

		foreach (var anImageFileOrderingValue in imageFileOrderingValues)
		{
			var anImageFileOrderingItem = new ComboBoxItem
			{
				Tag = anImageFileOrderingValue,
				Content = anImageFileOrderingValue.Description
			};

			_imageFileOrderingComboBox.Items.Add(anImageFileOrderingItem);

			if (anImageFileOrderingValue == TabOptions!.ImageFileOrdering)
			{
				_imageFileOrderingComboBox.SelectedItem = anImageFileOrderingItem;
			}
		}
	}

	private void PopulateImageFileOrderingDirections()
	{
		var imageFileOrderingDirectionValues = Enum
			.GetValues<FileSystemEntryInfoOrderingDirection>();

		foreach (var anImageFileOrderingDirectionValue in imageFileOrderingDirectionValues)
		{
			var anImageFileOrderingDirectionItem = new ComboBoxItem
			{
				Tag = anImageFileOrderingDirectionValue,
				Content = anImageFileOrderingDirectionValue.ToString()
			};

			_imageFileOrderingDirectionComboBox.Items.Add(anImageFileOrderingDirectionItem);

			if (anImageFileOrderingDirectionValue == TabOptions!.ImageFileOrderingDirection)
			{
				_imageFileOrderingDirectionComboBox.SelectedItem = anImageFileOrderingDirectionItem;
			}
		}
	}

	private void PopulateImageViewDisplayModes()
	{
		var imageViewDisplayModeValues = Enum.GetValues<ImageViewDisplayMode>();

		foreach (var anImageViewDisplayModeValue in imageViewDisplayModeValues)
		{
			var anImageViewDisplayModeItem = new ComboBoxItem
			{
				Tag = anImageViewDisplayModeValue,
				Content = anImageViewDisplayModeValue.Description
			};

			_imageViewDisplayModeComboBox.Items.Add(anImageViewDisplayModeItem);

			if (anImageViewDisplayModeValue == TabOptions!.ImageViewDisplayMode)
			{
				_imageViewDisplayModeComboBox.SelectedItem = anImageViewDisplayModeItem;
			}
		}
	}

	private void PopulateThumbnailSizes()
	{
		foreach (var aThumbnailSize in ThumbnailSizeExtensions.ThumbnailSizes)
		{
			var aThumbnailSizeItem = new ComboBoxItem
			{
				Tag = aThumbnailSize,
				Content = $"{aThumbnailSize.ToInt()}px"
			};

			_thumbnailSizeComboBox.Items.Add(aThumbnailSizeItem);

			if (aThumbnailSize == TabOptions!.ThumbnailSize)
			{
				_thumbnailSizeComboBox.SelectedItem = aThumbnailSizeItem;
			}
		}
	}

	private void SetRecursiveFolderBrowsing()
	{
		_recursiveFolderBrowsingCheckBox.IsChecked = TabOptions!.RecursiveFolderBrowsing;
	}

	private void SetGlobalOrderingForRecursiveFolderBrowsing()
	{
		_globalOrderingForRecursiveFolderBrowsingCheckBox.IsChecked =
			TabOptions!.GlobalOrderingForRecursiveFolderBrowsing;
	}

	private void SetShowImageViewImageInfo()
	{
		_showImageViewImageInfoCheckBox.IsChecked = TabOptions!.ShowImageViewImageInfo;
	}

	private void SetPanelsSplittingRatioSlider()
	{
		_panelsSplittingRatioSlider.Value = TabOptions!.PanelsSplittingRatio;
	}

	private void PopulateSlideshowIntervals()
	{
		foreach (var aSlideshowInterval in SlideshowIntervalExtensions.SlideshowIntervals)
		{
			var aSlideshowIntervalText = aSlideshowInterval == SlideshowInterval.OneSecond
				? $"{aSlideshowInterval.ToInt()} second"
				: $"{aSlideshowInterval.ToInt()} seconds";

			var aSlideshowIntervalItem = new ComboBoxItem
			{
				Tag = aSlideshowInterval,
				Content = aSlideshowIntervalText
			};

			_slideshowIntervalComboBox.Items.Add(aSlideshowIntervalItem);

			if (aSlideshowInterval == TabOptions!.SlideshowInterval)
			{
				_slideshowIntervalComboBox.SelectedItem = aSlideshowIntervalItem;
			}
		}
	}

	private void SetApplyImageOrientation()
	{
		_applyImageOrientationCheckBox.IsChecked = TabOptions!.ApplyImageOrientation;
	}

	private void SetShowThumbnailImageFileName()
	{
		_showThumbnailImageFileNameCheckBox.IsChecked = TabOptions!.ShowThumbnailImageFileName;
	}

	private void PopulateKeyboardScrollThumbnailIncrements()
	{
		foreach (var aKeyboardScrollThumbnailIncrement in
			KeyboardScrollThumbnailIncrementExtensions.KeyboardScrollThumbnailIncrements)
		{
			var aKeyboardScrollThumbnailIncrementText =
				$"{aKeyboardScrollThumbnailIncrement.ToInt()} thumbnails";

			var aKeyboardScrollThumbnailIncrementItem = new ComboBoxItem
			{
				Tag = aKeyboardScrollThumbnailIncrement,
				Content = aKeyboardScrollThumbnailIncrementText
			};

			_keyboardScrollThumbnailIncrementComboBox.Items.Add(aKeyboardScrollThumbnailIncrementItem);

			if (aKeyboardScrollThumbnailIncrement == TabOptions!.KeyboardScrollThumbnailIncrement)
			{
				_keyboardScrollThumbnailIncrementComboBox.SelectedItem =
					aKeyboardScrollThumbnailIncrementItem;
			}
		}
	}

	private void PopulateUpsizeFullScreenImagesUpToScreenSizes()
	{
		foreach (var anUpsizeFullScreenImagesUpToScreenSize in
		         UpsizeFullScreenImagesUpToScreenSizeExtensions.UpsizeFullScreenImagesUpToScreenSizes)
		{
			var anUpsizeFullScreenImagesUpToScreenSizeItem = new ComboBoxItem
			{
				Tag = anUpsizeFullScreenImagesUpToScreenSize,
				Content = anUpsizeFullScreenImagesUpToScreenSize.Description
			};

			_upsizeFullScreenImagesUpToScreenSizeComboBox.Items.Add(anUpsizeFullScreenImagesUpToScreenSizeItem);

			if (anUpsizeFullScreenImagesUpToScreenSize == TabOptions!.UpsizeFullScreenImagesUpToScreenSize)
			{
				_upsizeFullScreenImagesUpToScreenSizeComboBox.SelectedItem =
					anUpsizeFullScreenImagesUpToScreenSizeItem;
			}
		}
	}

	private void RegisterTabOptionEvents()
	{
		_folderOrderingComboBox.SelectionChanged += OnFolderOrderingSelectionChanged;
		_folderOrderingDirectionComboBox.SelectionChanged +=
			OnFolderOrderingDirectionSelectionChanged;

		_imageFileOrderingComboBox.SelectionChanged += OnImageFileOrderingSelectionChanged;
		_imageFileOrderingDirectionComboBox.SelectionChanged +=
			OnImageFileOrderingDirectionSelectionChanged;

		_imageViewDisplayModeComboBox.SelectionChanged += OnImageViewDisplayModeSelectionChanged;

		_thumbnailSizeComboBox.SelectionChanged += OnThumbnailSizeSelectionChanged;

		_recursiveFolderBrowsingCheckBox.IsCheckedChanged +=
			OnRecursiveFolderBrowsingIsCheckedChanged;
		_globalOrderingForRecursiveFolderBrowsingCheckBox.IsCheckedChanged +=
			OnGlobalOrderingForRecursiveFolderBrowsingIsCheckedChanged;

		_showImageViewImageInfoCheckBox.IsCheckedChanged += OnShowImageViewImageInfoIsCheckedChanged;
		_panelsSplittingRatioSlider.ValueChanged += OnPanelsSplittingRatioChanged;
		_slideshowIntervalComboBox.SelectionChanged += OnSlideshowIntervalSelectionChanged;
		_applyImageOrientationCheckBox.IsCheckedChanged += OnApplyImageOrientationIsCheckedChanged;
		_showThumbnailImageFileNameCheckBox.IsCheckedChanged +=
			OnShowThumbnailImageFileNameIsCheckedChanged;
		_keyboardScrollThumbnailIncrementComboBox.SelectionChanged +=
			OnKeyboardScrollThumbnailIncrementSelectionChanged;
		_upsizeFullScreenImagesUpToScreenSizeComboBox.SelectionChanged +=
			OnUpsizeFullScreenImagesUpToScreenSizeSelectionChanged;

		_saveAsDefaultCheckBox.IsCheckedChanged += OnSaveAsDefaultIsCheckedChanged;
	}

	private void UnregisterTabOptionEvents()
	{
		_folderOrderingComboBox.SelectionChanged -= OnFolderOrderingSelectionChanged;
		_folderOrderingDirectionComboBox.SelectionChanged -=
			OnFolderOrderingDirectionSelectionChanged;

		_imageFileOrderingComboBox.SelectionChanged -= OnImageFileOrderingSelectionChanged;
		_imageFileOrderingDirectionComboBox.SelectionChanged -=
			OnImageFileOrderingDirectionSelectionChanged;

		_imageViewDisplayModeComboBox.SelectionChanged -= OnImageViewDisplayModeSelectionChanged;

		_thumbnailSizeComboBox.SelectionChanged -= OnThumbnailSizeSelectionChanged;

		_recursiveFolderBrowsingCheckBox.IsCheckedChanged -=
			OnRecursiveFolderBrowsingIsCheckedChanged;
		_globalOrderingForRecursiveFolderBrowsingCheckBox.IsCheckedChanged -=
			OnGlobalOrderingForRecursiveFolderBrowsingIsCheckedChanged;

		_showImageViewImageInfoCheckBox.IsCheckedChanged -= OnShowImageViewImageInfoIsCheckedChanged;
		_panelsSplittingRatioSlider.ValueChanged -= OnPanelsSplittingRatioChanged;
		_slideshowIntervalComboBox.SelectionChanged -= OnSlideshowIntervalSelectionChanged;
		_applyImageOrientationCheckBox.IsCheckedChanged -= OnApplyImageOrientationIsCheckedChanged;
		_showThumbnailImageFileNameCheckBox.IsCheckedChanged -=
			OnShowThumbnailImageFileNameIsCheckedChanged;
		_keyboardScrollThumbnailIncrementComboBox.SelectionChanged -=
			OnKeyboardScrollThumbnailIncrementSelectionChanged;
		_upsizeFullScreenImagesUpToScreenSizeComboBox.SelectionChanged -=
			OnUpsizeFullScreenImagesUpToScreenSizeSelectionChanged;

		_saveAsDefaultCheckBox.IsCheckedChanged -= OnSaveAsDefaultIsCheckedChanged;
	}

	#endregion
}
