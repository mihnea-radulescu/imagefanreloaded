using System;
using System.Threading.Tasks;
using Avalonia.Controls;
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
		
		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);

		_hasChangedTabOptions = false;
	}
	
	public IGlobalParameters? GlobalParameters { get; set; }
	
	public IContentTabItem? ContentTabItem { get; set; }
	
	public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	public int ThumbnailSize { get; set; }
	public bool RecursiveFolderBrowsing { get; set; }
	public bool ShowImageViewImageInfo { get; set; }
	
	public event EventHandler<TabOptionsChangedEventArgs>? TabOptionsChanged;

	public void PopulateTabOptions()
	{
		PopulateFolderOrdering();
		PopulateThumbnailSize();
		
		SetRecursiveFolderBrowsing();
		SetShowImageViewImageInfo();
	}
	
	public async Task ShowDialog(IMainView owner) => await ShowDialog((Window)owner);
	
	#region Private

	private bool _hasChangedTabOptions;

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
		if (_hasChangedTabOptions)
		{
			RaiseTabOptionsChanged();
		}
	}
	
	private void OnFolderOrderingSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var folderOrderingComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var folderOrdering = (FileSystemEntryInfoOrdering)folderOrderingComboBoxItem.Tag!;

		FileSystemEntryInfoOrdering = folderOrdering;
		_hasChangedTabOptions = true;
	}
	
	private void OnThumbnailSizeSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var thumbnailSizeComboBoxItem = (ComboBoxItem)e.AddedItems[0]!;
		var thumbnailSize = (int)thumbnailSizeComboBoxItem.Tag!;

		ThumbnailSize = thumbnailSize;
		_hasChangedTabOptions = true;
	}

	private void OnRecursiveFolderBrowsingIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var recursiveFolderBrowsing = _recursiveFolderBrowsingCheckBox.IsChecked!.Value;

		RecursiveFolderBrowsing = recursiveFolderBrowsing;
		_hasChangedTabOptions = true;
	}
	
	private void OnShowImageViewImageInfoIsCheckedChanged(object? sender, RoutedEventArgs e)
	{
		var showImageViewImageInfo = _showImageViewImageInfoCheckBox.IsChecked!.Value;

		ShowImageViewImageInfo = showImageViewImageInfo;
		_hasChangedTabOptions = true;
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
			
			if (aFileSystemEntryInfoOrderingValue == FileSystemEntryInfoOrdering)
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

			if (aValidThumbnailSize == ThumbnailSize)
			{
				_thumbnailSizeComboBox.SelectedItem = aValidThumbnailSizeItem;
			}
		}
	}

	private void SetRecursiveFolderBrowsing()
	{
		_recursiveFolderBrowsingCheckBox.IsChecked = RecursiveFolderBrowsing;
	}
	
	private void SetShowImageViewImageInfo()
	{
		_showImageViewImageInfoCheckBox.IsChecked = ShowImageViewImageInfo;
	}

	private void RaiseTabOptionsChanged()
	{
		TabOptionsChanged?.Invoke(this, new TabOptionsChangedEventArgs(
			ContentTabItem!,
			FileSystemEntryInfoOrdering,
			ThumbnailSize,
			RecursiveFolderBrowsing,
			ShowImageViewImageInfo));
	}

	#endregion
}
