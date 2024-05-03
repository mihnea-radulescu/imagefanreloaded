using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using ImageFanReloaded.Core.Collections.Implementation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItem : UserControl, IContentTabItem
{
	public ContentTabItem()
	{
		InitializeComponent();
	}
	
    public IMainView? MainView { get; set; }
    public IGlobalParameters? GlobalParameters { get; set; }
    public IFolderChangedEventHandle? FolderChangedEventHandle { get; set; }
    
    public object? WrapperTabItem { get; set; }
    public IContentTabItemHeader? ContentTabItemHeader { get; set; }

	public IImageViewFactory? ImageViewFactory { get; set; }

	public IFolderVisualState? FolderVisualState { get; set; }

	public event EventHandler<FolderChangedEventArgs>? FolderChanged;

	public void EnableFolderTreeViewSelectedItemChanged()
		=> _folderTreeView.SelectionChanged += OnFolderTreeViewSelectedItemChanged;
	
	public void OnKeyPressed(object? sender, KeyboardKeyEventArgs e)
	{
		if (_selectedThumbnailBox is not null)
		{
			var keyPressed = e.Key;
	        
			if (GlobalParameters!.BackwardNavigationKeys.Contains(keyPressed))
			{
				AdvanceToThumbnailIndex(-1);
			}
			else if (GlobalParameters!.ForwardNavigationKeys.Contains(keyPressed))
			{
				AdvanceToThumbnailIndex(1);
			}
			else if (keyPressed == GlobalParameters!.EnterKey)
			{
				BringThumbnailIntoView();
				DisplayImage();
			}
		}
	}

	public void SetTitle(string title) => ContentTabItemHeader!.SetTabTitle(title);

	public void RegisterMainViewEvents() => MainView!.TabCountChanged += OnTabCountChanged;
	public void UnregisterMainViewEvents() => MainView!.TabCountChanged -= OnTabCountChanged;

	public void PopulateRootNodesSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> rootFolders)
	{
		var itemCollection = _folderTreeView.Items;
		
		AddSubFoldersToTreeView(itemCollection, rootFolders);
	}

	public void PopulateSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		if (_folderTreeView.SelectedItem is not null)
		{
			var selectedItem = (TreeViewItem)_folderTreeView.SelectedItem;
			var itemCollection = selectedItem.Items;
			
			itemCollection.Clear();

			AddSubFoldersToTreeView(itemCollection, subFolders);
		}
	}
	
	public void PopulateSubFoldersTreeOfParentTreeViewItem(IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		if (_inputFolderTreeViewItem is not null)
		{
			var selectedItem = _inputFolderTreeViewItem;
			var itemCollection = selectedItem.Items;
			
			itemCollection.Clear();

			AddSubFoldersToTreeView(itemCollection, subFolders);

			selectedItem.IsExpanded = true;
			selectedItem.IsSelected = true;

			_folderTreeView.SelectedItem = selectedItem;
		}
	}
	
	public void PopulateThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection)
	{
		foreach (var thumbnailInfo in thumbnailInfoCollection)
		{
			var aThumbnailBox = new ThumbnailBox();
			aThumbnailBox.ThumbnailInfo = thumbnailInfo;
			aThumbnailBox.SetControlProperties(GlobalParameters!);

			thumbnailInfo.ThumbnailBox = aThumbnailBox;
			aThumbnailBox.ThumbnailBoxSelected += OnThumbnailBoxSelected;
			aThumbnailBox.ThumbnailBoxClicked += OnThumbnailBoxClicked;

			_thumbnailBoxCollection!.Add(aThumbnailBox);

			if (IsFirstThumbnail())
			{
				SelectThumbnailBox(aThumbnailBox, 0);
			}

			var aSurroundingStackPanel = new StackPanel();
			aSurroundingStackPanel.Children.Add(aThumbnailBox);
			_thumbnailWrapPanel.Children.Add(aSurroundingStackPanel);
		}
	}

	public void ClearThumbnailBoxes(bool resetContent)
	{
		_thumbnailWrapPanel.Children.Clear();
		_selectedThumbnailBox = null;

		if (_thumbnailBoxCollection is not null)
		{
			foreach (var aThumbnailBox in _thumbnailBoxCollection)
			{
				aThumbnailBox.DisposeThumbnail();
				aThumbnailBox.ThumbnailBoxSelected -= OnThumbnailBoxSelected;
				aThumbnailBox.ThumbnailBoxClicked -= OnThumbnailBoxClicked;
			}

			_thumbnailBoxCollection = null;
		}
		
		if (resetContent)
		{
			_thumbnailBoxCollection = new List<IThumbnailBox>();
			_selectedThumbnailIndex = -1;
			
			_thumbnailScrollViewer.Offset = new Vector(_thumbnailScrollViewer.Offset.X, 0);
		}
	}

	public void RefreshThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection)
	{
		foreach (var thumbnailInfo in thumbnailInfoCollection)
		{
			thumbnailInfo.RefreshThumbnail();
		}
	}

	public void SetFolderStatusBarText(string folderStatusBarText)
	{
		_textBlockFolderInfo.Text = folderStatusBarText;
	}
	
	public void SetImageStatusBarText(string imageStatusBarText)
	{
		_textBlockImageInfo.Text = imageStatusBarText;
	}
	
	public void SaveMatchingTreeViewItem(FileSystemEntryInfo selectedFileSystemEntryInfo)
	{
		var subItems = _inputFolderTreeViewItem is null
			? _folderTreeView.Items
			: _inputFolderTreeViewItem.Items;

		foreach (TreeViewItem? aSubItem in subItems)
		{
			if (aSubItem!.Header is IFileSystemTreeViewItem fileSystemTreeViewItem)
			{
				if (fileSystemTreeViewItem.FileSystemEntryInfo == selectedFileSystemEntryInfo)
				{
					_inputFolderTreeViewItem = aSubItem;
				}
			}
		}
	}
	
    #region Private

    private const string FakeTreeViewItemText = "Loading...";

    private IList<IThumbnailBox>? _thumbnailBoxCollection;
	private int _selectedThumbnailIndex;
	private IThumbnailBox? _selectedThumbnailBox;

	private TreeViewItem? _inputFolderTreeViewItem;
	
	private void OnThumbnailBoxSelected(object? sender, ThumbnailBoxEventArgs e)
	{
		var imageFile = e.ThumbnailBox.ImageFile!;
		var imageInfo = imageFile.GetImageInfo();
		
		SetImageStatusBarText(imageInfo);
	}

	private void OnThumbnailBoxClicked(object? sender, ThumbnailBoxEventArgs e)
	{
		var thumbnailBox = e.ThumbnailBox;

		if (thumbnailBox.IsSelected)
		{
			DisplayImage();
		}
		else
		{
			SelectThumbnailBox(thumbnailBox, null);
		}
	}

    private void OnFolderTreeViewSelectedItemChanged(object? sender, SelectionChangedEventArgs e)
    {
	    var selectedFolderTreeViewItem = (TreeViewItem)e.AddedItems[0]!;

		if (selectedFolderTreeViewItem.Header is IFileSystemTreeViewItem fileSystemEntryItem)
		{
			var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo!;
			var selectedFolderName = fileSystemEntryInfo.Name;
			var selectedFolderPath = fileSystemEntryInfo.Path;

			FolderChanged?.Invoke(
				this, new FolderChangedEventArgs(selectedFolderName, selectedFolderPath));
		}
	}
    
	private void OnTabCountChanged(object? sender, TabCountChangedEventArgs e)
		=> ShowCloseButton(e.ShowTabCloseButton);

	private void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var imageView = e.ImageView;
		var increment = e.Increment;
		
		if (AdvanceToThumbnailIndex(increment))
		{
			imageView.SetImage(_selectedThumbnailBox!.ImageFile!);
		}
	}

    private void SelectThumbnailBox(IThumbnailBox thumbnailBox, int? indexToSelect)
	{
		if (_selectedThumbnailBox != thumbnailBox)
		{
			UnselectThumbnail();
			
			_selectedThumbnailBox = thumbnailBox;

			if (indexToSelect is null)
			{
				_selectedThumbnailIndex = _thumbnailBoxCollection!
					.Select((aThumbnailBox, index) => (aThumbnailBox, index))
					.Single(aThumbnailBoxWithIndex => aThumbnailBoxWithIndex.aThumbnailBox == _selectedThumbnailBox)
					.index;
			}
			else
			{
				_selectedThumbnailIndex = indexToSelect.Value;
			}

			SelectThumbnail();
		}
	}

    private bool AdvanceToThumbnailIndex(int increment)
	{
		if (_selectedThumbnailBox is not null)
		{
			var newSelectedThumbnailIndex = _selectedThumbnailIndex + increment;

			if (_thumbnailBoxCollection!.IsIndexWithinBounds(newSelectedThumbnailIndex))
			{
				UnselectThumbnail();

				_selectedThumbnailIndex = newSelectedThumbnailIndex;
				_selectedThumbnailBox = _thumbnailBoxCollection![_selectedThumbnailIndex];

				SelectThumbnail();

				return true;
			}
		}

		return false;
	}

	private void BringThumbnailIntoView() => _selectedThumbnailBox?.BringThumbnailIntoView();
	private void SelectThumbnail() => _selectedThumbnailBox?.SelectThumbnail();
	private void UnselectThumbnail() => _selectedThumbnailBox?.UnselectThumbnail();

	private async void DisplayImage()
	{
		var imageView = ImageViewFactory!.GetImageView();
		imageView.SetImage(_selectedThumbnailBox!.ImageFile!);

		imageView.ImageChanged += OnImageChanged;
		await imageView.ShowDialog(MainView!);
		imageView.ImageChanged -= OnImageChanged;
	}
	
	private static void AddSubFoldersToTreeView(
		ItemCollection itemCollection, IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		foreach (var aSubFolder in subFolders)
		{
			var treeViewItem = GetTreeViewItem(aSubFolder);
			itemCollection.Add(treeViewItem);
		}
	}

	private static TreeViewItem GetTreeViewItem(FileSystemEntryInfo fileSystemEntryInfo)
	{
		IFileSystemTreeViewItem fileSystemTreeViewItem = new FileSystemTreeViewItem
		{
			FileSystemEntryInfo = fileSystemEntryInfo
		};

		var treeViewItem = new TreeViewItem
		{
			Header = fileSystemTreeViewItem
		};

		if (fileSystemEntryInfo.HasSubFolders)
		{
			var fakeTreeViewItem = new TreeViewItem
			{
				Header = FakeTreeViewItemText
			};
			
			treeViewItem.Items.Add(fakeTreeViewItem);
		}

		return treeViewItem;
	}
	
	private void ShowCloseButton(bool showTabCloseButton)
		=> ContentTabItemHeader!.ShowTabCloseButton(showTabCloseButton);
	
	private bool IsFirstThumbnail() => _thumbnailBoxCollection!.Count == 1;

	#endregion
}
