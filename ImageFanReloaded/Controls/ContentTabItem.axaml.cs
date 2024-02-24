using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItem : UserControl, IContentTabItem
{
	public ContentTabItem()
	{
		InitializeComponent();
    }
	
    public IMainView? MainView { get; set; }
    public IGlobalParameters? GlobalParameters { get; set; }
    
    public object? WrapperTabItem { get; set; }
    public IContentTabItemHeader? ContentTabItemHeader { get; set; }

	public IImageViewFactory? ImageViewFactory { get; set; }
	public object? GenerateThumbnailsLockObject { get; set; }

	public IFolderVisualState? FolderVisualState { get; set; }

	public event EventHandler<FolderChangedEventArgs>? FolderChanged;
	
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

	public void PopulateSubFoldersTree(IReadOnlyCollection<FileSystemEntryInfo> subFolders,
									   bool rootNodes)
	{
		if (rootNodes)
		{
			foreach (var aSubFolder in subFolders)
			{
				var treeViewItem = GetTreeViewItem(aSubFolder);

				_folderTreeView.Items.Add(treeViewItem);
			}
		}
		else if (_folderTreeView.SelectedItem is not null)
		{
			var selectedItem = (TreeViewItem)_folderTreeView.SelectedItem;

			selectedItem.Items.Clear();

			foreach (var aSubFolder in subFolders)
			{
				var treeViewItem = GetTreeViewItem(aSubFolder);

				selectedItem.Items.Add(treeViewItem);
			}
		}
	}
	
	public void PopulateThumbnailBoxes(
		IReadOnlyCollection<IThumbnailInfo> thumbnailInfoCollection)
	{
		foreach (var thumbnailInfo in thumbnailInfoCollection)
		{
			var aThumbnailBox = new ThumbnailBox();
			aThumbnailBox.ThumbnailInfo = thumbnailInfo;
			aThumbnailBox.SetControlProperties(GlobalParameters!);

			thumbnailInfo.ThumbnailBox = aThumbnailBox;
			aThumbnailBox.ThumbnailBoxSelected += OnThumbnailBoxSelected;
			aThumbnailBox.ThumbnailBoxClicked += OnThumbnailBoxClicked;

			_thumbnailBoxList!.Add(aThumbnailBox);

			if (IsFirstThumbnail())
			{
				SelectThumbnailBox(aThumbnailBox);
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

		if (_thumbnailBoxList is not null)
		{
			foreach (var aThumbnailBox in _thumbnailBoxList)
			{
				aThumbnailBox.DisposeThumbnail();
				aThumbnailBox.ThumbnailBoxSelected -= OnThumbnailBoxSelected;
				aThumbnailBox.ThumbnailBoxClicked -= OnThumbnailBoxClicked;
			}

			_thumbnailBoxList = null;
		}
		
		if (resetContent)
		{
			_thumbnailBoxList = new List<IThumbnailBox>();
			_selectedThumbnailIndex = -1;
			
			_thumbnailScrollViewer.Offset = new Vector(_thumbnailScrollViewer.Offset.X, 0);
		}
	}

	public void RefreshThumbnailBoxes(
		IReadOnlyCollection<IThumbnailInfo> thumbnailInfoCollection)
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
	
    #region Private

    private const string FakeTreeViewItemText = "Loading...";

    private List<IThumbnailBox>? _thumbnailBoxList;
	private int _selectedThumbnailIndex;
	private IThumbnailBox? _selectedThumbnailBox;
	
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
			SelectThumbnailBox(thumbnailBox);
		}
	}

    private void OnFolderTreeViewSelectedItemChanged(object? sender,
													 SelectionChangedEventArgs e)
	{
		var selectedFolderTreeViewItem = (TreeViewItem)e.AddedItems[0]!;

		if (selectedFolderTreeViewItem.Header is IFileSystemTreeViewItem fileSystemEntryItem)
		{
			var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo;
			var selectedFolderName = fileSystemEntryInfo.Name;
			var selectedFolderPath = fileSystemEntryInfo.Path;

			FolderChanged?.Invoke(
				this, new FolderChangedEventArgs(selectedFolderName, selectedFolderPath));
		}
	}
    
	private void OnTabCountChanged(object? sender, TabCountChangedEventArgs e)
		=> ShowCloseButton(e.ShowTabCloseButton);

	private void OnThumbnailChanged(object? sender, ThumbnailChangedEventArgs e)
	{
		var imageView = e.ImageView;
		var increment = e.Increment;
		
		if (AdvanceToThumbnailIndex(increment))
		{
			imageView.SetImage(_selectedThumbnailBox!.ImageFile!);
		}
	}

    private void SelectThumbnailBox(IThumbnailBox thumbnailBox)
	{
		if (_selectedThumbnailBox != thumbnailBox)
		{
			UnselectThumbnail();
			
			_selectedThumbnailBox = thumbnailBox;
			_selectedThumbnailIndex = _thumbnailBoxList
				!.FindIndex(aThumbnailBox => aThumbnailBox == _selectedThumbnailBox);

			SelectThumbnail();
		}
	}

    private bool AdvanceToThumbnailIndex(int increment)
	{
		if (_selectedThumbnailBox is not null)
		{
			var newSelectedThumbnailIndex = _selectedThumbnailIndex + increment;

			if (newSelectedThumbnailIndex >= 0 &&
				newSelectedThumbnailIndex < _thumbnailBoxList!.Count)
			{
				UnselectThumbnail();

				_selectedThumbnailIndex = newSelectedThumbnailIndex;
				_selectedThumbnailBox = _thumbnailBoxList[_selectedThumbnailIndex];

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

		imageView.ThumbnailChanged += OnThumbnailChanged;
		await imageView.ShowDialog(MainView!);
		imageView.ThumbnailChanged -= OnThumbnailChanged;
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
	
	private bool IsFirstThumbnail() => _thumbnailBoxList!.Count == 1;

	#endregion
}
