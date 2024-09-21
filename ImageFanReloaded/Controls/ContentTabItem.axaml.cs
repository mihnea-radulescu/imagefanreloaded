using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItem : UserControl, IContentTabItem
{
	public ContentTabItem()
	{
		InitializeComponent();

		_thumbnailBoxCollection = new List<IThumbnailBox>();
	}
	
    public IMainView? MainView { get; set; }

    public IGlobalParameters? GlobalParameters
    {
	    get => _globalParameters;
	    set
	    {
		    _globalParameters = value!;

		    FileSystemEntryInfoOrdering = _globalParameters.DefaultFileSystemEntryInfoOrdering;
		    ThumbnailSize = _globalParameters.DefaultThumbnailSize;
	    }
    }
    
    public FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; private set; }
    public int ThumbnailSize { get; private set; }
    
    public IFolderChangedMutex? FolderChangedMutex { get; set; }
    
    public object? WrapperTabItem { get; set; }
    public IContentTabItemHeader? ContentTabItemHeader { get; set; }

	public IImageViewFactory? ImageViewFactory { get; set; }

	public IFolderVisualState? FolderVisualState { get; set; }

	public event EventHandler<FolderChangedEventArgs>? FolderChanged;
	public event EventHandler<FolderChangedEventArgs>? FolderOrderingChanged;

	public void EnableFolderTreeViewSelectedItemChanged()
	{
		_folderTreeView.SelectionChanged += OnFolderTreeViewSelectedItemChanged;
	}
	
	public void DisableFolderTreeViewSelectedItemChanged()
	{
		_folderTreeView.SelectionChanged -= OnFolderTreeViewSelectedItemChanged;
	}
	
	public bool ShouldHandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing)
	{
		var shouldHandleKeyPressing = ShouldChangeFolderOrdering(keyModifiers, keyPressing)
		                              || ShouldChangeThumbnailSize(keyModifiers, keyPressing)
		                              || ShouldSwitchControlFocus(keyModifiers, keyPressing)
		                              || ShouldToggleRecursiveFolderAccess(keyModifiers, keyPressing)
		                              || ShouldHandleThumbnailSelection(keyModifiers, keyPressing)
		                              || ShouldHandleThumbnailScrolling(keyModifiers, keyPressing)
		                              || ShouldHandleThumbnailNavigation(keyModifiers, keyPressing);

		return shouldHandleKeyPressing;
	}

	public void HandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (ShouldChangeFolderOrdering(keyModifiers, keyPressing))
		{
			ChangeFolderOrdering(keyPressing);
		}
		else if (ShouldChangeThumbnailSize(keyModifiers, keyPressing))
		{
			ChangeThumbnailSize(keyPressing);
		}
		else if (ShouldSwitchControlFocus(keyModifiers, keyPressing))
		{
			SwitchControlFocus();
		}
		else if (ShouldToggleRecursiveFolderAccess(keyModifiers, keyPressing))
		{
			ToggleRecursiveFolderAccess(keyModifiers);
		}
		else if (ShouldHandleThumbnailSelection(keyModifiers, keyPressing))
		{
			FocusThumbnailScrollViewer();
			BringThumbnailIntoView();
			DisplayImage();
		}
		else if (ShouldHandleThumbnailScrolling(keyModifiers, keyPressing))
		{
			HandleThumbnailScrolling(keyPressing);
		}
		else if (ShouldHandleThumbnailNavigation(keyModifiers, keyPressing))
		{
			HandleThumbnailNavigation(keyPressing);
		}
	}

	public string GetFolderTreeViewSelectedItemFolderPath()
	{
		var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem()!;
		var fileSystemTreeViewItem = (IFileSystemTreeViewItem)folderTreeViewSelectedItem.Header!;
		var selectedItemFolderPath = fileSystemTreeViewItem.FileSystemEntryInfo!.Path;

		return selectedItemFolderPath;
	}

	public void SetFolderTreeViewSelectedItem()
	{
		if (_folderTreeView.SelectedItem is null)
		{
			_folderTreeView.SelectedItem = _folderTreeView.Items.FirstOrDefault();
		}
		
		if (_folderTreeView.SelectedItem is not null)
		{
			var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem()!;
			folderTreeViewSelectedItem.Focus();
		}
	}

	public bool? GetFolderTreeViewSelectedItemExpandedState()
	{
		var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem();
		var isExpanded = folderTreeViewSelectedItem?.IsExpanded;

		return isExpanded;
	}
	
	public void SetFolderTreeViewSelectedItemExpandedState(bool isExpanded)
	{
		var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem();

		if (folderTreeViewSelectedItem is not null)
		{
			folderTreeViewSelectedItem.IsExpanded = isExpanded;
		}
	}

	public void SetTitle(string title) => ContentTabItemHeader!.SetTabTitle(title);

	public void RegisterMainViewEvents() => MainView!.TabCountChanged += OnTabCountChanged;
	public void UnregisterMainViewEvents() => MainView!.TabCountChanged -= OnTabCountChanged;

	public void PopulateRootNodesSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> rootFolders)
	{
		var itemCollection = _folderTreeView.Items;
		
		ClearItemCollection(itemCollection);
		AddSubFoldersToTreeView(itemCollection, rootFolders);
	}

	public void PopulateSubFoldersTree(IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		if (_folderTreeView.SelectedItem is not null)
		{
			var selectedItem = GetFolderTreeViewSelectedItem()!;
			var itemCollection = selectedItem.Items;
			
			ClearItemCollection(itemCollection);
			AddSubFoldersToTreeView(itemCollection, subFolders);
		}
	}
	
	public void PopulateSubFoldersTreeOfParentTreeViewItem(IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		if (_selectedFolderTreeViewItem is not null)
		{
			var selectedItem = _selectedFolderTreeViewItem;
			var itemCollection = selectedItem.Items;
			
			ClearItemCollection(itemCollection);
			AddSubFoldersToTreeView(itemCollection, subFolders);

			selectedItem.IsExpanded = true;
			selectedItem.IsSelected = true;
			_folderTreeView.SelectedItem = selectedItem;
		}
	}
	
	public void PopulateThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection)
	{
		var thumbnailCount = thumbnailInfoCollection.Count;
		
		for (var i = 0; i < thumbnailCount; i++)
		{
			var thumbnailInfo = thumbnailInfoCollection[i];
			
			var aThumbnailBox = new ThumbnailBox();
			aThumbnailBox.Index = _maxThumbnailIndex + i;
			aThumbnailBox.ThumbnailInfo = thumbnailInfo;
			aThumbnailBox.SetControlProperties(ThumbnailSize, GlobalParameters!);

			thumbnailInfo.ThumbnailBox = aThumbnailBox;
			aThumbnailBox.ThumbnailBoxSelected += OnThumbnailBoxSelected;
			aThumbnailBox.ThumbnailBoxClicked += OnThumbnailBoxClicked;

			_thumbnailBoxCollection.Add(aThumbnailBox);

			if (IsFirstThumbnail())
			{
				SelectThumbnailBox(aThumbnailBox);
			}

			var aSurroundingStackPanel = new StackPanel();
			aSurroundingStackPanel.Children.Add(aThumbnailBox);
			_thumbnailWrapPanel.Children.Add(aSurroundingStackPanel);
		}

		_maxThumbnailIndex += thumbnailCount;
	}

	public void ClearThumbnailBoxes(bool resetContent)
	{
		_thumbnailWrapPanel.Children.Clear();

		_maxThumbnailIndex = 0;
		_selectedThumbnailBox = null;

		if (_thumbnailBoxCollection.Any())
		{
			foreach (var aThumbnailBox in _thumbnailBoxCollection)
			{
				aThumbnailBox.DisposeThumbnail();
				aThumbnailBox.ThumbnailBoxSelected -= OnThumbnailBoxSelected;
				aThumbnailBox.ThumbnailBoxClicked -= OnThumbnailBoxClicked;
			}

			_thumbnailBoxCollection.Clear();
		}
		
		if (resetContent)
		{
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
		_textBoxFolderInfo.Text = folderStatusBarText;
	}
	
	public void SetImageStatusBarText(string imageStatusBarText)
	{
		_textBoxImageInfo.Text = imageStatusBarText;
	}
	
	public void SaveMatchingTreeViewItem(FileSystemEntryInfo selectedFileSystemEntryInfo, bool startAtRootFolders)
	{
		var subItems = _selectedFolderTreeViewItem is null || startAtRootFolders
			? _folderTreeView.Items
			: _selectedFolderTreeViewItem.Items;

		foreach (TreeViewItem? aSubItem in subItems)
		{
			if (aSubItem!.Header is IFileSystemTreeViewItem fileSystemTreeViewItem)
			{
				if (fileSystemTreeViewItem.FileSystemEntryInfo == selectedFileSystemEntryInfo)
				{
					_selectedFolderTreeViewItem = aSubItem;
					break;
				}
			}
		}
	}

	public bool AreFolderInfoOrImageInfoFocused() => _textBoxFolderInfo.IsFocused || _textBoxImageInfo.IsFocused;
	
	public void FocusThumbnailScrollViewer() => _thumbnailScrollViewer.Focus();
	
    #region Private

    private const string FakeTreeViewItemText = "Loading...";
    private const int ThumbnailScrollAdvanceCount = 25;
    
    private readonly IList<IThumbnailBox> _thumbnailBoxCollection;

    private IGlobalParameters? _globalParameters;
    
    private int _maxThumbnailIndex;
	private int _selectedThumbnailIndex;
	private IThumbnailBox? _selectedThumbnailBox;
	
	private TreeViewItem? _selectedFolderTreeViewItem;
	private FolderAccessType _folderAccessType;
	
	private void OnThumbnailBoxSelected(object? sender, ThumbnailBoxEventArgs e)
	{
		var imageFile = e.ThumbnailBox.ImageFile!;
		var imageInfo = imageFile.GetImageInfo(_folderAccessType.IsRecursive());
		
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

    private void OnFolderTreeViewSelectedItemChanged(object? sender, SelectionChangedEventArgs e)
    {
	    _selectedFolderTreeViewItem = (TreeViewItem)e.AddedItems[0]!;

	    UpdateRecursiveFolderAccess();
	    RaiseFolderChangedEvent();
    }

    private void UpdateRecursiveFolderAccess()
    {
	    if (_folderAccessType != FolderAccessType.PersistentRecursive)
	    {
		    _folderAccessType = FolderAccessType.Normal;
	    }
    }

    private void RaiseFolderChangedEvent()
    {
	    if (_selectedFolderTreeViewItem?.Header is IFileSystemTreeViewItem fileSystemEntryItem)
	    {
		    var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo!;
		    var folderChangedEventArgs = GetFolderChangedEventArgs(fileSystemEntryInfo);

		    FolderChanged?.Invoke(this, folderChangedEventArgs);
	    }
    }
    
    private void RaiseFolderOrderingChangedEvent()
    {
	    if (_selectedFolderTreeViewItem?.Header is IFileSystemTreeViewItem fileSystemEntryItem)
		{
		    var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo!;
		    var folderChangedEventArgs = GetFolderChangedEventArgs(fileSystemEntryInfo);

		    FolderOrderingChanged?.Invoke(this, folderChangedEventArgs);
		}
    }
    
    private void OnTreeViewItemPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
	    OnTreeViewItemIsExpanded(sender, e);
    }

    private void OnTreeViewItemIsExpanded(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
	    if (e.Property.Name == "IsExpanded" && (bool)e.OldValue! == false && (bool)e.NewValue! == true)
	    {
		    var selectedItem = (TreeViewItem)sender!;
			
		    selectedItem.IsSelected = true;
		    _folderTreeView.SelectedItem = selectedItem;
	    }
    }

    private void OnTabCountChanged(object? sender, TabCountChangedEventArgs e) => ShowCloseButton(e.ShowTabCloseButton);

	private void OnImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var imageView = e.ImageView;
		var increment = e.Increment;
		
		if (AdvanceFromSelectedThumbnail(increment))
		{
			imageView.SetImage(_selectedThumbnailBox!.ImageFile!, _folderAccessType.IsRecursive());
		}
	}

    private void SelectThumbnailBox(IThumbnailBox thumbnailBox)
	{
		if (_selectedThumbnailBox != thumbnailBox)
		{
			UnselectThumbnail();
			
			_selectedThumbnailBox = thumbnailBox;
			_selectedThumbnailIndex = thumbnailBox.Index;

			SelectThumbnail();
		}
	}

	private bool AdvanceFromSelectedThumbnail(int increment)
	{
		var canAdvanceToNewSelectedThumbnailIndex = false;
		
		if (_selectedThumbnailBox is not null)
		{
			var newSelectedThumbnailIndex = _selectedThumbnailIndex + increment;

			if (newSelectedThumbnailIndex < 0)
			{
				newSelectedThumbnailIndex = 0;
			}
			else if (newSelectedThumbnailIndex >= _thumbnailBoxCollection.Count)
			{
				newSelectedThumbnailIndex = _thumbnailBoxCollection.Count - 1;
			}

			if (_selectedThumbnailIndex != newSelectedThumbnailIndex)
			{
				canAdvanceToNewSelectedThumbnailIndex = true;
				
				UnselectThumbnail();

				_selectedThumbnailIndex = newSelectedThumbnailIndex;
				_selectedThumbnailBox = _thumbnailBoxCollection[_selectedThumbnailIndex];

				SelectThumbnail();
			}
		}

		return canAdvanceToNewSelectedThumbnailIndex;
	}

	private void BringThumbnailIntoView() => _selectedThumbnailBox?.BringThumbnailIntoView();
	private void SelectThumbnail() => _selectedThumbnailBox?.SelectThumbnail();
	private void UnselectThumbnail() => _selectedThumbnailBox?.UnselectThumbnail();

	private async void DisplayImage()
	{
		var imageView = ImageViewFactory!.GetImageView();
		imageView.SetImage(_selectedThumbnailBox!.ImageFile!, _folderAccessType.IsRecursive());

		imageView.ImageChanged += OnImageChanged;
		await imageView.ShowDialog(MainView!);
		imageView.ImageChanged -= OnImageChanged;
	}
	
	private void AddSubFoldersToTreeView(ItemCollection itemCollection, IReadOnlyList<FileSystemEntryInfo> subFolders)
	{
		foreach (var aSubFolder in subFolders)
		{
			var treeViewItem = GetTreeViewItem(aSubFolder);
			treeViewItem.PropertyChanged += OnTreeViewItemPropertyChanged;
			
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

	private void ClearItemCollection(ItemCollection itemCollection)
	{
		var treeViewItems = itemCollection.Cast<TreeViewItem>().ToList();
		
		foreach (var aTreeViewItem in treeViewItems)
		{
			aTreeViewItem.PropertyChanged -= OnTreeViewItemPropertyChanged;
		}
		
		itemCollection.Clear();
	}
	
	private void ShowCloseButton(bool showTabCloseButton)
		=> ContentTabItemHeader!.ShowTabCloseButton(showTabCloseButton);
	
	private bool IsFirstThumbnail() => _thumbnailBoxCollection.Count == 1;
	
	private bool ShouldChangeFolderOrdering(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    (keyPressing == GlobalParameters!.NKey ||
		     keyPressing == GlobalParameters!.CKey ||
		     keyPressing == GlobalParameters!.MKey))
		{
			return true;
		}

		return false;
	}
	
	private bool ShouldChangeThumbnailSize(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    (keyPressing == GlobalParameters!.PlusKey || keyPressing == GlobalParameters!.MinusKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldSwitchControlFocus(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier && keyPressing == GlobalParameters!.TabKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldToggleRecursiveFolderAccess(KeyModifiers keyModifiers, Key keyPressing)
	{
		if ((keyModifiers == GlobalParameters!.NoneKeyModifier || keyModifiers == GlobalParameters!.ShiftKeyModifier) &&
			keyPressing == GlobalParameters!.RKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleThumbnailSelection(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (_selectedThumbnailBox is not null &&
		    keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    keyPressing == GlobalParameters!.EnterKey)
		{
			return true;
		}

		return false;
	}
	
	private bool ShouldHandleThumbnailScrolling(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (_selectedThumbnailBox is not null &&
		    keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    (keyPressing == GlobalParameters!.PageUpKey || keyPressing == GlobalParameters!.PageDownKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldHandleThumbnailNavigation(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (_folderTreeView.SelectedItem is null)
		{
			return false;
		}

		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
		    GlobalParameters!.IsNavigationKey(keyPressing))
		{
			var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem()!;

			if (!folderTreeViewSelectedItem.IsFocused && !_gridSplitter.IsFocused)
			{
				return true;
			}
		}

		return false;
	}
	
	private void ChangeFolderOrdering(Key keyPressing)
	{
		var newFileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering;
		
		if (keyPressing == GlobalParameters!.NKey)
		{
			newFileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering.NameAscending;
		}
		else if (keyPressing == GlobalParameters!.CKey)
		{
			newFileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering.CreationTimeDescending;
		}
		else if (keyPressing == GlobalParameters!.MKey)
		{
			newFileSystemEntryInfoOrdering = FileSystemEntryInfoOrdering.LastModificationTimeDescending;
		}

		if (newFileSystemEntryInfoOrdering != FileSystemEntryInfoOrdering)
		{
			FileSystemEntryInfoOrdering = newFileSystemEntryInfoOrdering;
			
			RaiseFolderOrderingChangedEvent();
		}
	}

	private void ChangeThumbnailSize(Key keyPressing)
	{
		var increment = keyPressing == GlobalParameters!.PlusKey
			? GlobalParameters!.ThumbnailSizeIncrement
			: -GlobalParameters!.ThumbnailSizeIncrement;
		
		var newThumbnailSize = ThumbnailSize + increment;

		if (GlobalParameters!.IsValidThumbnailSize(newThumbnailSize))
		{
			ThumbnailSize = newThumbnailSize;
			
			RaiseFolderChangedEvent();
		}
	}
	
	private void SwitchControlFocus()
	{
		if (_folderTreeView.SelectedItem is null)
		{
			return;
		}
		
		var folderTreeViewSelectedItem = GetFolderTreeViewSelectedItem()!;
		
		if (folderTreeViewSelectedItem.IsFocused)
		{
			FocusGridSplitter();
		}
		else if (_gridSplitter.IsFocused)
		{
			FocusThumbnailScrollViewer();
		}
		else
		{
			FocusTreeViewItem(folderTreeViewSelectedItem);
		}
	}
	
	private void FocusGridSplitter() => _gridSplitter.Focus();
	private static void FocusTreeViewItem(TreeViewItem treeViewItem) => treeViewItem.Focus();

	private void ToggleRecursiveFolderAccess(KeyModifiers keyModifiers)
	{
		var isPersistentRecursive = keyModifiers == GlobalParameters!.ShiftKeyModifier;
		
		if (_folderAccessType == FolderAccessType.Normal)
		{
			_folderAccessType = isPersistentRecursive
				? FolderAccessType.PersistentRecursive
				: FolderAccessType.Recursive;
		}
		else
		{
			_folderAccessType = FolderAccessType.Normal;
		}
		
		RaiseFolderChangedEvent();
	}
	
	private void HandleThumbnailScrolling(Key keyPressing)
	{
		if (_selectedThumbnailBox is not null)
		{
			if (keyPressing == GlobalParameters!.PageUpKey)
			{
				AdvanceFromSelectedThumbnail(-ThumbnailScrollAdvanceCount);
			}
			else if (keyPressing == GlobalParameters!.PageDownKey)
			{
				AdvanceFromSelectedThumbnail(ThumbnailScrollAdvanceCount);
			}
		}
	}
	
	private void HandleThumbnailNavigation(Key keyPressing)
	{
		if (GlobalParameters!.IsBackwardNavigationKey(keyPressing))
		{
			AdvanceFromSelectedThumbnail(-1);
		}
		else if (GlobalParameters!.IsForwardNavigationKey(keyPressing))
		{
			AdvanceFromSelectedThumbnail(1);
		}
	}
	
	private FolderChangedEventArgs GetFolderChangedEventArgs(FileSystemEntryInfo fileSystemEntryInfo)
	{
		var selectedFolderName = fileSystemEntryInfo.Name;
		var selectedFolderPath = fileSystemEntryInfo.Path;

		var folderChangedEventArgs = new FolderChangedEventArgs(
			selectedFolderName,
			selectedFolderPath,
			FileSystemEntryInfoOrdering,
			ThumbnailSize,
			_folderAccessType.IsRecursive());
		
		return folderChangedEventArgs;
	}
	
	private TreeViewItem? GetFolderTreeViewSelectedItem() => (TreeViewItem?)_folderTreeView.SelectedItem;

	#endregion
}
