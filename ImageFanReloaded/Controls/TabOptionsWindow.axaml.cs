using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.MarkupExtensions;
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
		PopulateFolderOrdering();
		PopulateThumbnailSize();

		SetRecursiveFolderBrowsing();
		SetShowImageViewImageInfo();
	}

	public void RegisterTabOptionEvents()
	{
		_folderOrderingComboBox.SelectionChanged += OnFolderOrderingSelectionChanged;
		_thumbnailSizeComboBox.SelectionChanged += OnThumbnailSizeSelectionChanged;
		_recursiveFolderBrowsingCheckBox.IsCheckedChanged += OnRecursiveFolderBrowsingIsCheckedChanged;
		_showImageViewImageInfoCheckBox.IsCheckedChanged += OnShowImageViewImageInfoIsCheckedChanged;
		_saveAsDefaultCheckBox.IsCheckedChanged += OnSaveAsDefaultIsCheckedChanged;
	}

	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);

	#region Private

	private ITabOptionChanges _tabOptionChanges;

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
	}

	private void OnFolderOrderingSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrdering = (FileSystemEntryInfoOrdering)folderOrderingComboBoxItem.Tag!;

		TabOptions!.FileSystemEntryInfoOrdering = folderOrdering;
		_tabOptionChanges.HasChangedFolderOrdering = true;
	}

	private void OnThumbnailSizeSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var thumbnailSizeComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var thumbnailSize = (int)thumbnailSizeComboBoxItem.Tag!;

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

	private void OnSaveAsDefaultIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var saveAsDefault = _saveAsDefaultCheckBox.IsChecked!.Value;

		_tabOptionChanges.ShouldSaveAsDefault = saveAsDefault;
	}

	private bool ShouldCloseWindow(
		ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.EscapeKey ||
			 keyPressing == GlobalParameters!.EnterKey ||
			 keyPressing == GlobalParameters!.OKey))
		{
			return true;
		}

		return false;
	}

	private void PopulateFolderOrdering()
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

	private void PopulateThumbnailSize()
	{
		var validThumbnailSizes = GlobalParameters!.GetValidThumbnailSizes();

		foreach (var aValidThumbnailSize in validThumbnailSizes)
		{
			var aValidThumbnailSizeItem = new ComboBoxItem
			{
				Tag = aValidThumbnailSize,
				Content = $"{aValidThumbnailSize}px"
			};

			_thumbnailSizeComboBox.Items.Add(aValidThumbnailSizeItem);

			if (aValidThumbnailSize == TabOptions!.ThumbnailSize)
			{
				_thumbnailSizeComboBox.SelectedItem = aValidThumbnailSizeItem;
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
	
	#endregion
}
