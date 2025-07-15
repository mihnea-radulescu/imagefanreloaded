using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Settings.Implementation;
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

		PopulateThumbnailSizes();

		SetRecursiveFolderBrowsing();
		SetShowImageViewImageInfo();

		SetPanelsSplittingRatioSlider();

		PopulateSlideshowIntervals();

		SetApplyImageOrientation();

		RegisterTabOptionEvents();
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private TabOptionChanges _tabOptionChanges;

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
			this, new TabOptionsChangedEventArgs(ContentTabItem!, _tabOptionChanges));

		UnregisterTabOptionEvents();
	}

	private void OnFolderOrderingSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrdering = (FileSystemEntryInfoOrdering)folderOrderingComboBoxItem.Tag!;

		TabOptions!.FileSystemEntryInfoOrdering = folderOrdering;
		_tabOptionChanges.HasChangedFolderOrdering = true;
	}

	private void OnFolderOrderingDirectionSelectionChanged(
		object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingDirectionComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrderingDirection =
			(FileSystemEntryInfoOrderingDirection)folderOrderingDirectionComboBoxItem.Tag!;

		TabOptions!.FileSystemEntryInfoOrderingDirection = folderOrderingDirection;
		_tabOptionChanges.HasChangedFolderOrderingDirection = true;
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
		var fileSystemEntryInfoOrderingValues = Enum.GetValues<FileSystemEntryInfoOrdering>();

		foreach (var aFileSystemEntryInfoOrderingValue in fileSystemEntryInfoOrderingValues)
		{
			var aFileSystemEntryInfoOrderingItem = new ComboBoxItem
			{
				Tag = aFileSystemEntryInfoOrderingValue,
				Content = aFileSystemEntryInfoOrderingValue.GetDescription()
			};

			_folderOrderingComboBox.Items.Add(aFileSystemEntryInfoOrderingItem);

			if (aFileSystemEntryInfoOrderingValue == TabOptions!.FileSystemEntryInfoOrdering)
			{
				_folderOrderingComboBox.SelectedItem = aFileSystemEntryInfoOrderingItem;
			}
		}
	}

	private void PopulateFolderOrderingDirections()
	{
		var fileSystemEntryInfoOrderingDirectionValues = Enum
			.GetValues<FileSystemEntryInfoOrderingDirection>();

		foreach (var aFileSystemEntryInfoOrderingDirectionValue in
			fileSystemEntryInfoOrderingDirectionValues)
		{
			var aFileSystemEntryInfoOrderingDirectionItem = new ComboBoxItem
			{
				Tag = aFileSystemEntryInfoOrderingDirectionValue,
				Content = aFileSystemEntryInfoOrderingDirectionValue.ToString()
			};

			_folderOrderingDirectionComboBox.Items.Add(aFileSystemEntryInfoOrderingDirectionItem);

			if (aFileSystemEntryInfoOrderingDirectionValue ==
				TabOptions!.FileSystemEntryInfoOrderingDirection)
			{
				_folderOrderingDirectionComboBox.SelectedItem =
					aFileSystemEntryInfoOrderingDirectionItem;
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

	private void RegisterTabOptionEvents()
	{
		_folderOrderingComboBox.SelectionChanged += OnFolderOrderingSelectionChanged;
		_folderOrderingDirectionComboBox.SelectionChanged +=
			OnFolderOrderingDirectionSelectionChanged;
		_thumbnailSizeComboBox.SelectionChanged += OnThumbnailSizeSelectionChanged;
		_recursiveFolderBrowsingCheckBox.IsCheckedChanged += OnRecursiveFolderBrowsingIsCheckedChanged;
		_showImageViewImageInfoCheckBox.IsCheckedChanged += OnShowImageViewImageInfoIsCheckedChanged;
		_panelsSplittingRatioSlider.ValueChanged += OnPanelsSplittingRatioChanged;
		_slideshowIntervalComboBox.SelectionChanged += OnSlideshowIntervalSelectionChanged;
		_applyImageOrientationCheckBox.IsCheckedChanged += OnApplyImageOrientationIsCheckedChanged;

		_saveAsDefaultCheckBox.IsCheckedChanged += OnSaveAsDefaultIsCheckedChanged;
	}

	private void UnregisterTabOptionEvents()
	{
		_folderOrderingComboBox.SelectionChanged -= OnFolderOrderingSelectionChanged;
		_folderOrderingDirectionComboBox.SelectionChanged -=
			OnFolderOrderingDirectionSelectionChanged;
		_thumbnailSizeComboBox.SelectionChanged -= OnThumbnailSizeSelectionChanged;
		_recursiveFolderBrowsingCheckBox.IsCheckedChanged -= OnRecursiveFolderBrowsingIsCheckedChanged;
		_showImageViewImageInfoCheckBox.IsCheckedChanged -= OnShowImageViewImageInfoIsCheckedChanged;
		_panelsSplittingRatioSlider.ValueChanged -= OnPanelsSplittingRatioChanged;
		_slideshowIntervalComboBox.SelectionChanged -= OnSlideshowIntervalSelectionChanged;
		_applyImageOrientationCheckBox.IsCheckedChanged -= OnApplyImageOrientationIsCheckedChanged;

		_saveAsDefaultCheckBox.IsCheckedChanged -= OnSaveAsDefaultIsCheckedChanged;
	}

	#endregion
}
