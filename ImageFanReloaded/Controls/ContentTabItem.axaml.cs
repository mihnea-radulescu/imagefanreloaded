using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.Mouse;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Controls;

public partial class ContentTabItem : UserControl, IContentTabItem
{
	static ContentTabItem()
	{
		StartSlideshowDelay = TimeSpan.FromMilliseconds(50);
	}

	public ContentTabItem()
	{
		InitializeComponent();

		AddMainGridColumnDefinitions();

		_thumbnailBoxCollection = new List<IThumbnailBox>();
	}

	public IMainView? MainView { get; set; }

	public IGlobalParameters? GlobalParameters { get; set; }
	public IMouseCursorFactory? MouseCursorFactory { get; set; }

	public ITabOptions? TabOptions { get; set; }

	public IAsyncMutex? FolderChangedMutex { get; set; }
	public void DisposeFolderChangedMutex() => FolderChangedMutex?.Dispose();

	public object? WrapperTabItem { get; set; }
	public IContentTabItemHeader? ContentTabItemHeader { get; set; }

	public IImageViewFactory? ImageViewFactory { get; set; }

	public IFolderVisualState? FolderVisualState { get; set; }

	public event EventHandler<FolderChangedEventArgs>? FolderChanged;
	public event EventHandler<FolderOrderingChangedEventArgs>? FolderOrderingChanged;

	public event EventHandler<ImageSelectedEventArgs>? ImageInfoRequested;
	public event EventHandler<ImageSelectedEventArgs>? ImageEditRequested;
	public event EventHandler<ContentTabItemEventArgs>? TabOptionsRequested;
	public event EventHandler<ContentTabItemEventArgs>? AboutInfoRequested;

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
		var shouldHandleKeyPressing = ShouldStartSlideshow(keyModifiers, keyPressing) ||
			ShouldDisplayImageInfo(keyModifiers, keyPressing) ||
			ShouldDisplayImageEdit(keyModifiers, keyPressing) ||
			ShouldDisplayTabOptions(keyModifiers, keyPressing) ||
			ShouldDisplayAboutInfo(keyModifiers, keyPressing) ||
			ShouldChangeFolderOrdering(keyModifiers, keyPressing) ||
			ShouldChangeFolderOrderingDirection(keyModifiers, keyPressing) ||
			ShouldChangeImageFileOrdering(keyModifiers, keyPressing) ||
			ShouldChangeImageFileOrderingDirection(keyModifiers, keyPressing) ||
			ShouldChangeThumbnailSize(keyModifiers, keyPressing) ||
			ShouldToggleRecursiveFolderAccess(keyModifiers, keyPressing) ||
			ShouldChangeApplyImageOrientation(keyModifiers, keyPressing) ||
			ShouldChangeShowThumbnailImageFileName(keyModifiers, keyPressing) ||
			ShouldChangeImageViewImageInfoVisibility(keyModifiers, keyPressing) ||
			ShouldSwitchControlFocus(keyModifiers, keyPressing) ||
			ShouldHandleThumbnailSelection(keyModifiers, keyPressing) ||
			ShouldHandleThumbnailScrolling(keyModifiers, keyPressing) ||
			ShouldHandleThumbnailNavigation(keyModifiers, keyPressing);

		return shouldHandleKeyPressing;
	}

	public void HandleControlKeyFunctions(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (ShouldStartSlideshow(keyModifiers, keyPressing))
		{
			RaiseSlideshowRequested();
		}
		else if (ShouldDisplayImageInfo(keyModifiers, keyPressing))
		{
			RaiseImageInfoRequested();
		}
		else if (ShouldDisplayImageEdit(keyModifiers, keyPressing))
		{
			RaiseImageEditRequested();
		}
		else if (ShouldDisplayTabOptions(keyModifiers, keyPressing))
		{
			RaiseTabOptionsRequested();
		}
		else if (ShouldDisplayAboutInfo(keyModifiers, keyPressing))
		{
			RaiseAboutInfoRequested();
		}
		else if (ShouldChangeFolderOrdering(keyModifiers, keyPressing))
		{
			ChangeFolderOrdering(keyPressing);
		}
		else if (ShouldChangeFolderOrderingDirection(keyModifiers, keyPressing))
		{
			ChangeFolderOrderingDirection(keyPressing);
		}
		else if (ShouldChangeImageFileOrdering(keyModifiers, keyPressing))
		{
			ChangeImageFileOrdering(keyPressing);
		}
		else if (ShouldChangeImageFileOrderingDirection(keyModifiers, keyPressing))
		{
			ChangeImageFileOrderingDirection(keyPressing);
		}
		else if (ShouldChangeThumbnailSize(keyModifiers, keyPressing))
		{
			ChangeThumbnailSize(keyPressing);
		}
		else if (ShouldToggleRecursiveFolderAccess(keyModifiers, keyPressing))
		{
			ToggleRecursiveFolderAccess();
		}
		else if (ShouldChangeApplyImageOrientation(keyModifiers, keyPressing))
		{
			ChangeApplyImageOrientation();
		}
		else if (ShouldChangeShowThumbnailImageFileName(keyModifiers, keyPressing))
		{
			ChangeShowThumbnailImageFileName();
		}
		else if (ShouldChangeImageViewImageInfoVisibility(keyModifiers, keyPressing))
		{
			ChangeImageViewImageInfoVisibility();
		}
		else if (ShouldSwitchControlFocus(keyModifiers, keyPressing))
		{
			SwitchControlFocus();
		}
		else if (ShouldHandleThumbnailSelection(keyModifiers, keyPressing))
		{
			FocusThumbnailScrollViewer();
			BringThumbnailIntoView();

			DisplayImage(false);
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

	public void SetFocusOnSelectedFolderTreeViewItem()
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

	public void SetTabInfo(string folderName, string folderPath)
		=> ContentTabItemHeader!.SetTabHeader(folderName, folderPath);

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
		if (_activeFolderTreeViewItem is not null)
		{
			var itemCollection = _activeFolderTreeViewItem.Items;

			ClearItemCollection(itemCollection);
			AddSubFoldersToTreeView(itemCollection, subFolders);

			_activeFolderTreeViewItem.IsExpanded = true;
			_activeFolderTreeViewItem.IsSelected = true;
			_folderTreeView.SelectedItem = _activeFolderTreeViewItem;
		}
	}

	public void PopulateThumbnailBoxes(IReadOnlyList<IThumbnailInfo> thumbnailInfoCollection)
	{
		var thumbnailCount = thumbnailInfoCollection.Count;

		for (var i = 0; i < thumbnailCount; i++)
		{
			var thumbnailInfo = thumbnailInfoCollection[i];

			var aThumbnailBox = new ThumbnailBox();
			aThumbnailBox.TabOptions = TabOptions;
			aThumbnailBox.MouseCursorFactory = MouseCursorFactory;

			aThumbnailBox.Index = _maxThumbnailIndex + i;
			aThumbnailBox.ThumbnailInfo = thumbnailInfo;

			aThumbnailBox.SetControlProperties(TabOptions!.ThumbnailSize.ToInt(), GlobalParameters!);

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

	public async Task ClearThumbnailBoxes(bool resetContent)
	{
		_thumbnailWrapPanel.Children.Clear();

		_selectedThumbnailIndex = -1;
		_maxThumbnailIndex = 0;

		_selectedThumbnailBox = default;

		_slideshowButton.IsEnabled = false;
		_imageInfoButton.IsEnabled = false;
		_imageEditButton.IsEnabled = false;

		var thumbnailBoxCollectionToClear = _thumbnailBoxCollection.ToList();
		_thumbnailBoxCollection.Clear();

		if (thumbnailBoxCollectionToClear.Any())
		{
			await StopThumbnailAnimation(thumbnailBoxCollectionToClear);

			foreach (var aThumbnailBox in thumbnailBoxCollectionToClear)
			{
				await aThumbnailBox.DisposeThumbnail();

				aThumbnailBox.ThumbnailBoxSelected -= OnThumbnailBoxSelected;
				aThumbnailBox.ThumbnailBoxClicked -= OnThumbnailBoxClicked;
			}
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

	public void SetFolderInfoText(string folderInfoText)
		=> _folderInfoTextBox.Text = folderInfoText;

	public void SetImageInfoText(string imageInfoText)
		=> _imageInfoTextBox.Text = imageInfoText;

	public void SaveMatchingTreeViewItem(
		FileSystemEntryInfo selectedFileSystemEntryInfo, bool startAtRootFolders)
	{
		var subItems = _activeFolderTreeViewItem is null || startAtRootFolders
			? _folderTreeView.Items
			: _activeFolderTreeViewItem.Items;

		var subTreeViewItems = subItems.Cast<TreeViewItem>().ToList();

		foreach (var aSubTreeViewItem in subTreeViewItems)
		{
			if (aSubTreeViewItem.Header is IFileSystemTreeViewItem fileSystemTreeViewItem)
			{
				if (fileSystemTreeViewItem.FileSystemEntryInfo == selectedFileSystemEntryInfo)
				{
					_activeFolderTreeViewItem = aSubTreeViewItem;
					break;
				}
			}
		}
	}

	public bool AreFolderInfoOrImageInfoFocused()
		=> _folderInfoTextBox.IsFocused || _imageInfoTextBox.IsFocused;

	public void FocusThumbnailScrollViewer() => _thumbnailScrollViewer.Focus();

	public void RaiseFolderChangedEvent()
	{
		if (_activeFolderTreeViewItem?.Header is IFileSystemTreeViewItem fileSystemEntryItem)
		{
			var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo!;
			var selectedFolderName = fileSystemEntryInfo.Name;
			var selectedFolderPath = fileSystemEntryInfo.Path;

			var folderChangedEventArgs = new FolderChangedEventArgs(
				this,
				selectedFolderName,
				selectedFolderPath,
				TabOptions!.RecursiveFolderBrowsing);

			FolderChanged?.Invoke(this, folderChangedEventArgs);
		}
	}

	public void RaiseFolderOrderingChangedEvent()
	{
		if (_activeFolderTreeViewItem?.Header is IFileSystemTreeViewItem fileSystemEntryItem)
		{
			var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo!;
			var selectedFolderPath = fileSystemEntryInfo.Path;

			var folderOrderingChangedEventArgs = new FolderOrderingChangedEventArgs(this, selectedFolderPath);

			FolderOrderingChanged?.Invoke(this, folderOrderingChangedEventArgs);
		}
	}

	public void RaisePanelsSplittingRatioChangedEvent()
	{
		_folderTreeViewColumn!.Width = new GridLength(
			TabOptions!.PanelsSplittingRatio, GridUnitType.Star);

		_thumbnailsScrollViewerColumn!.Width = new GridLength(
			100 - TabOptions!.PanelsSplittingRatio, GridUnitType.Star);
	}

	public async Task RefreshSelectedImage()
	{
		var selectedImageFile = GetSelectedImageFile();
		await Task.Run(() => selectedImageFile.RefreshTransientImageFileData());

		UpdateSelectedImageStatus();
	}

	public async Task UpdateSelectedImageAfterImageFileChange()
	{
		try
		{
			await FolderChangedMutex!.Wait();

			var previousSelectedImageSizeOnDiscInKilobytes =
				GetSelectedImageSizeOnDiscInKilobytes();

			await RefreshSelectedImage();
			await _selectedThumbnailBox!.UpdateThumbnailAfterImageFileChange();

			var currentSelectedImageSizeOnDiscInKilobytes =
				GetSelectedImageSizeOnDiscInKilobytes();

			FolderVisualState!.UpdateFolderInfoText(
				TabOptions!,
				previousSelectedImageSizeOnDiscInKilobytes,
				currentSelectedImageSizeOnDiscInKilobytes);
		}
		finally
		{
			FolderChangedMutex!.Signal();
		}
	}

	public async Task ShowImageEdit(IImageEditView imageEditView)
		=> await imageEditView.ShowDialog(MainView!);
	public async Task ShowTabOptions(ITabOptionsView tabOptionsView)
		=> await tabOptionsView.ShowDialog(MainView!);
	public async Task ShowAboutInfo(IAboutView aboutView)
		=> await aboutView.ShowDialog(MainView!);
	public async Task ShowImageInfo(IImageInfoView imageInfoView)
		=> await imageInfoView.ShowDialog(MainView!);

	#region Private

	private const string FakeTreeViewItemText = "Loading...";

	private const int OneImageForward = 1;
	private const int OneImageBackward = -1;
	private const int ThumbnailScrollForwardCount = 12;
	private const int ThumbnailScrollBackwardCount = -12;

	private static readonly TimeSpan StartSlideshowDelay;

	private ColumnDefinition? _folderTreeViewColumn;
	private ColumnDefinition? _gridSplitterColumn;
	private ColumnDefinition? _thumbnailsScrollViewerColumn;

	private readonly IList<IThumbnailBox> _thumbnailBoxCollection;

	private int _maxThumbnailIndex;
	private int _selectedThumbnailIndex;
	private IThumbnailBox? _selectedThumbnailBox;

	private TreeViewItem? _activeFolderTreeViewItem;

	private void OnThumbnailBoxSelected(object? sender, ThumbnailBoxSelectedEventArgs e)
		=> UpdateSelectedImageStatus();

	private void OnThumbnailBoxClicked(object? sender, ThumbnailBoxClickedEventArgs e)
	{
		var thumbnailBox = e.ThumbnailBox;

		if (e.MouseClickType == MouseClickType.Left)
		{
			if (thumbnailBox.IsSelected)
			{
				DisplayImage(false);
			}
			else
			{
				SelectThumbnailBox(thumbnailBox);
			}
		}
		else if (e.MouseClickType == MouseClickType.Right)
		{
			if (!thumbnailBox.IsSelected)
			{
				SelectThumbnailBox(thumbnailBox);
			}

			RaiseImageInfoRequested();
		}
	}

	private void OnFolderTreeViewSelectedItemChanged(object? sender, SelectionChangedEventArgs e)
	{
		_activeFolderTreeViewItem = (TreeViewItem)e.AddedItems[0]!;

		RaiseFolderChangedEvent();
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

	private async void OnImageViewImageChanged(object? sender, ImageChangedEventArgs e)
	{
		var imageView = e.ImageView;
		var increment = e.Increment;

		var canAdvanceToDesignatedImage = AdvanceFromSelectedThumbnail(increment);
		imageView.CanAdvanceToDesignatedImage = canAdvanceToDesignatedImage;

		if (canAdvanceToDesignatedImage)
		{
			var selectedImageFile = GetSelectedImageFile();
			await imageView.SetImage(selectedImageFile);
		}
	}

	private void OnImageViewClosing(object? sender, ImageViewClosingEventArgs e)
		=> UpdateSelectedImageStatus();

	private void OnSlideshowButtonClicked(object? sender, RoutedEventArgs e)
		=> RaiseSlideshowRequested();
	private void OnImageInfoButtonClicked(object? sender, RoutedEventArgs e)
		=> RaiseImageInfoRequested();
	private void OnImageEditButtonClicked(object? sender, RoutedEventArgs e)
		=> RaiseImageEditRequested();

	private void OnTabOptionsButtonClicked(object? sender, RoutedEventArgs e)
		=> RaiseTabOptionsRequested();
	private void OnAboutButtonClicked(object? sender, RoutedEventArgs e)
		=> RaiseAboutInfoRequested();

	private void AddMainGridColumnDefinitions()
	{
		_folderTreeViewColumn = new ColumnDefinition();

		_gridSplitterColumn = new ColumnDefinition
		{
			Width = GridLength.Auto
		};

		_thumbnailsScrollViewerColumn = new ColumnDefinition();

		_contentGrid.ColumnDefinitions.Add(_folderTreeViewColumn);
		_contentGrid.ColumnDefinitions.Add(_gridSplitterColumn);
		_contentGrid.ColumnDefinitions.Add(_thumbnailsScrollViewerColumn);
	}

	private void SelectThumbnailBox(IThumbnailBox thumbnailBox)
	{
		if (_selectedThumbnailBox != thumbnailBox)
		{
			UnselectThumbnail();

			_selectedThumbnailIndex = thumbnailBox.Index;

			_selectedThumbnailBox = thumbnailBox;

			_slideshowButton.IsEnabled = true;
			_imageInfoButton.IsEnabled = true;
			_imageEditButton.IsEnabled = !_selectedThumbnailBox.HasImageReadError;

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

	private async void DisplayImage(bool startSlideshow)
	{
		var imageView = ImageViewFactory!.GetImageView();

		imageView.TabOptions = TabOptions;

		var selectedImageFile = GetSelectedImageFile();
		await imageView.SetImage(selectedImageFile);

		imageView.ImageChanged += OnImageViewImageChanged;
		imageView.ViewClosing += OnImageViewClosing;

		if (startSlideshow)
		{
#pragma warning disable CS4014
			Task.Run(async () =>
			{
				do
				{
					await Task.Delay(StartSlideshowDelay);
				} while (!await imageView.CanStartSlideshowFromContentTabItem());

				await imageView.StartSlideshowFromContentTabItem();
			});
#pragma warning restore CS4014
		}

		await imageView.ShowDialog(MainView!);

		imageView.ImageChanged -= OnImageViewImageChanged;
		imageView.ViewClosing -= OnImageViewClosing;
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

	private bool ShouldStartSlideshow(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.SKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldDisplayImageInfo(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.FKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldDisplayImageEdit(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.TKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldDisplayTabOptions(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.OKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldDisplayAboutInfo(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.F1Key || keyPressing == GlobalParameters!.HKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeFolderOrdering(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.NKey || keyPressing == GlobalParameters!.MKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeFolderOrderingDirection(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			(keyPressing == GlobalParameters!.AKey || keyPressing == GlobalParameters!.DKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeImageFileOrdering(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.CtrlKeyModifier &&
			(keyPressing == GlobalParameters!.NKey || keyPressing == GlobalParameters!.MKey))
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeImageFileOrderingDirection(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.CtrlKeyModifier &&
			(keyPressing == GlobalParameters!.AKey || keyPressing == GlobalParameters!.DKey))
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

	private bool ShouldToggleRecursiveFolderAccess(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.RKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeApplyImageOrientation(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.EKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeShowThumbnailImageFileName(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.UKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldChangeImageViewImageInfoVisibility(
		KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.IKey)
		{
			return true;
		}

		return false;
	}

	private bool ShouldSwitchControlFocus(KeyModifiers keyModifiers, Key keyPressing)
	{
		if (keyModifiers == GlobalParameters!.NoneKeyModifier &&
			keyPressing == GlobalParameters!.TabKey)
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
			(keyPressing == GlobalParameters!.PageUpKey ||
			 keyPressing == GlobalParameters!.PageDownKey))
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

			if (!folderTreeViewSelectedItem.IsFocused)
			{
				return true;
			}
		}

		return false;
	}

	private void RaiseSlideshowRequested()
	{
		if (_selectedThumbnailBox is null)
		{
			return;
		}

		FocusThumbnailScrollViewer();
		BringThumbnailIntoView();

		DisplayImage(true);
	}

	private void RaiseImageInfoRequested()
	{
		if (_selectedThumbnailBox is null)
		{
			return;
		}

		FocusThumbnailScrollViewer();
		BringThumbnailIntoView();

		var selectedImageFile = GetSelectedImageFile();
		ImageInfoRequested?.Invoke(this, new ImageSelectedEventArgs(this, selectedImageFile));
	}

	private void RaiseImageEditRequested()
	{
		if (_selectedThumbnailBox is null || _selectedThumbnailBox.HasImageReadError)
		{
			return;
		}

		FocusThumbnailScrollViewer();
		BringThumbnailIntoView();

		var selectedImageFile = GetSelectedImageFile();
		ImageEditRequested?.Invoke(this, new ImageSelectedEventArgs(this, selectedImageFile));
	}

	private void RaiseTabOptionsRequested()
	{
		TabOptionsRequested?.Invoke(this, new ContentTabItemEventArgs(this));
	}

	private void RaiseAboutInfoRequested()
	{
		AboutInfoRequested?.Invoke(this, new ContentTabItemEventArgs(this));
	}

	private void ChangeFolderOrdering(Key keyPressing)
	{
		var newFolderOrdering = TabOptions!.FolderOrdering;

		if (keyPressing == GlobalParameters!.NKey)
		{
			newFolderOrdering = FileSystemEntryInfoOrdering.Name;
		}
		else if (keyPressing == GlobalParameters!.MKey)
		{
			newFolderOrdering = FileSystemEntryInfoOrdering.LastModificationTime;
		}

		if (newFolderOrdering != TabOptions!.FolderOrdering)
		{
			TabOptions!.FolderOrdering = newFolderOrdering;

			RaiseFolderOrderingChangedEvent();
		}
	}

	private void ChangeFolderOrderingDirection(Key keyPressing)
	{
		var newFolderOrderingDirection = TabOptions!.FolderOrderingDirection;

		if (keyPressing == GlobalParameters!.AKey)
		{
			newFolderOrderingDirection = FileSystemEntryInfoOrderingDirection.Ascending;
		}
		else if (keyPressing == GlobalParameters!.DKey)
		{
			newFolderOrderingDirection = FileSystemEntryInfoOrderingDirection.Descending;
		}

		if (newFolderOrderingDirection != TabOptions!.FolderOrderingDirection)
		{
			TabOptions!.FolderOrderingDirection = newFolderOrderingDirection;

			RaiseFolderOrderingChangedEvent();
		}
	}

	private void ChangeImageFileOrdering(Key keyPressing)
	{
		var newImageFileOrdering = TabOptions!.ImageFileOrdering;

		if (keyPressing == GlobalParameters!.NKey)
		{
			newImageFileOrdering = FileSystemEntryInfoOrdering.Name;
		}
		else if (keyPressing == GlobalParameters!.MKey)
		{
			newImageFileOrdering = FileSystemEntryInfoOrdering.LastModificationTime;
		}

		if (newImageFileOrdering != TabOptions!.ImageFileOrdering)
		{
			TabOptions!.ImageFileOrdering = newImageFileOrdering;

			RaiseFolderChangedEvent();
		}
	}

	private void ChangeImageFileOrderingDirection(Key keyPressing)
	{
		var newImageFileOrderingDirection = TabOptions!.ImageFileOrderingDirection;

		if (keyPressing == GlobalParameters!.AKey)
		{
			newImageFileOrderingDirection = FileSystemEntryInfoOrderingDirection.Ascending;
		}
		else if (keyPressing == GlobalParameters!.DKey)
		{
			newImageFileOrderingDirection = FileSystemEntryInfoOrderingDirection.Descending;
		}

		if (newImageFileOrderingDirection != TabOptions!.ImageFileOrderingDirection)
		{
			TabOptions!.ImageFileOrderingDirection = newImageFileOrderingDirection;

			RaiseFolderChangedEvent();
		}
	}

	private void ChangeThumbnailSize(Key keyPressing)
	{
		var increment = keyPressing == GlobalParameters!.PlusKey
			? ThumbnailSizeExtensions.ThumbnailSizeIncrement
			: -ThumbnailSizeExtensions.ThumbnailSizeIncrement;

		var newThumbnailSize = TabOptions!.ThumbnailSize.ToInt() + increment;

		if (newThumbnailSize.IsValidThumbnailSize())
		{
			TabOptions!.ThumbnailSize = newThumbnailSize.ToThumbnailSize();

			RaiseFolderChangedEvent();
		}
	}

	private void ChangeImageViewImageInfoVisibility()
	{
		TabOptions!.ShowImageViewImageInfo = !TabOptions!.ShowImageViewImageInfo;
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
			FocusThumbnailScrollViewer();
		}
		else
		{
			FocusTreeViewItem(folderTreeViewSelectedItem);
		}
	}

	private static void FocusTreeViewItem(TreeViewItem treeViewItem) => treeViewItem.Focus();

	private void ToggleRecursiveFolderAccess()
	{
		TabOptions!.RecursiveFolderBrowsing = !TabOptions!.RecursiveFolderBrowsing;

		RaiseFolderChangedEvent();
	}

	private void ChangeApplyImageOrientation()
	{
		TabOptions!.ApplyImageOrientation = !TabOptions!.ApplyImageOrientation;

		RaiseFolderChangedEvent();
	}

	private void ChangeShowThumbnailImageFileName()
	{
		TabOptions!.ShowThumbnailImageFileName = !TabOptions!.ShowThumbnailImageFileName;

		RaiseFolderChangedEvent();
	}

	private void HandleThumbnailScrolling(Key keyPressing)
	{
		if (_selectedThumbnailBox is not null)
		{
			if (keyPressing == GlobalParameters!.PageUpKey)
			{
				AdvanceFromSelectedThumbnail(ThumbnailScrollBackwardCount);
			}
			else if (keyPressing == GlobalParameters!.PageDownKey)
			{
				AdvanceFromSelectedThumbnail(ThumbnailScrollForwardCount);
			}
		}
	}

	private void HandleThumbnailNavigation(Key keyPressing)
	{
		if (GlobalParameters!.IsBackwardNavigationKey(keyPressing))
		{
			AdvanceFromSelectedThumbnail(OneImageBackward);
		}
		else if (GlobalParameters!.IsForwardNavigationKey(keyPressing))
		{
			AdvanceFromSelectedThumbnail(OneImageForward);
		}
	}

	private TreeViewItem? GetFolderTreeViewSelectedItem()
		=> (TreeViewItem?)_folderTreeView.SelectedItem;

	private IImageFile GetSelectedImageFile() => _selectedThumbnailBox!.ImageFile!;

	private static async Task StopThumbnailAnimation(
		IReadOnlyList<IThumbnailBox> thumbnailBoxCollectionToClear)
	{
		var animatedThumbnailBoxes = thumbnailBoxCollectionToClear
			.Where(aThumbnailBox => aThumbnailBox.IsAnimated)
			.ToList();

		foreach (var aThumbnailBox in animatedThumbnailBoxes)
		{
			aThumbnailBox.NotifyStopAnimation();
		}

		var runningAnimationTasks = animatedThumbnailBoxes
			.Select(aThumbnailBox => aThumbnailBox.AnimationTask)
			.ToList();

		try
		{
			await Task.WhenAll(runningAnimationTasks);
		}
		catch
		{
		}
	}

	private void UpdateSelectedImageStatus()
	{
		var selectedImageFile = GetSelectedImageFile();
		var basicImageInfo = selectedImageFile.GetBasicImageInfo(
			TabOptions!.RecursiveFolderBrowsing);
		SetImageInfoText(basicImageInfo);

		var hasImageReadError = _selectedThumbnailBox!.HasImageReadError;
		_imageEditButton.IsEnabled = !hasImageReadError;
	}

	private decimal GetSelectedImageSizeOnDiscInKilobytes()
	{
		var selectedImageFile = GetSelectedImageFile();
		var selectedImageSizeOnDiscInKilobytes =
			selectedImageFile.TransientImageFileData.SizeOnDiscInKilobytes.GetValueOrDefault();

		return selectedImageSizeOnDiscInKilobytes;
	}

	#endregion
}
