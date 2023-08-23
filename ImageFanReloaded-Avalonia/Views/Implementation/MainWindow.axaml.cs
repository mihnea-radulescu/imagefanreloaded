﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Controls.Implementation;
using ImageFanReloaded.Factories;

namespace ImageFanReloaded.Views.Implementation;

public partial class MainWindow
    : Window, IMainView
{
    public MainWindow()
    {
        InitializeComponent();

		var screenBounds = Screens.ScreenFromWindow(this).Bounds;
		_screenSize = new ImageSize(screenBounds.Width, screenBounds.Height);
	}

	public IImageViewFactory ImageViewFactory { get; set; }

	public event EventHandler<FolderChangedEventArgs> FolderChanged;

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
		else if (_folderTreeView.SelectedItem != null)
		{
			var selectedItem = (TreeViewItem)_folderTreeView.SelectedItem;

			if (selectedItem.Items.Count == 0)
			{
				foreach (var aSubFolder in subFolders)
				{
					var treeViewItem = GetTreeViewItem(aSubFolder);

					selectedItem.Items.Add(treeViewItem);
				}
			}
		}
	}

	public void ClearThumbnailBoxes()
	{
		if (_thumbnailBoxList != null)
		{
			foreach (var aThumbnailBox in _thumbnailBoxList)
			{
				aThumbnailBox.DisposeThumbnail();
			}
		}

		_thumbnailBoxList = new List<ThumbnailBox>();
		_selectedThumbnailIndex = -1;
		_selectedThumbnailBox = null;

		_thumbnailWrapPanel.Children.Clear();
		_thumbnailScrollViewer.Offset = new Vector(
			_thumbnailScrollViewer.Offset.X, 0);
	}

	public void PopulateThumbnailBoxes(
		IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection)
	{
		foreach (var thumbnailInfo in thumbnailInfoCollection)
		{
			var aThumbnailBox = new ThumbnailBox
			{
				ThumbnailInfo = thumbnailInfo
			};

			thumbnailInfo.ThumbnailBox = aThumbnailBox;

			aThumbnailBox.ThumbnailBoxClicked +=
				(sender, e) =>
				{
					var thumbnailBox = (ThumbnailBox)sender;

					if (thumbnailBox.IsSelected)
					{
						DisplayImage();
					}
					else
					{
						SelectThumbnailBox(thumbnailBox);
					}
				};

			_thumbnailBoxList.Add(aThumbnailBox);

			if (_thumbnailBoxList.Count == 1)
			{
				SelectThumbnailBox(aThumbnailBox);
				_thumbnailScrollViewer.Focus();
			}

			var aSurroundingStackPanel = new StackPanel();
			aSurroundingStackPanel.Children.Add(aThumbnailBox);
			_thumbnailWrapPanel.Children.Add(aSurroundingStackPanel);
		}
	}

	public void RefreshThumbnailBoxes(
		IReadOnlyCollection<ThumbnailInfo> thumbnailInfoCollection)
	{
		foreach (var thumbnailInfo in thumbnailInfoCollection)
		{
			thumbnailInfo.RefreshThumbnail();
		}
	}

	#region Private

	private readonly ImageSize _screenSize;

	private List<ThumbnailBox> _thumbnailBoxList;
	private int _selectedThumbnailIndex;
	private ThumbnailBox _selectedThumbnailBox;

	private void OnFolderTreeViewSelectedItemChanged(object sender,
													 SelectionChangedEventArgs e)
	{
		var folderChangedHandler = FolderChanged;

		if (folderChangedHandler != null)
		{
			var selectedFolderTreeViewItem = (TreeViewItem)e.AddedItems[0];
			var fileSystemEntryItem = (FileSystemEntryItem)
				selectedFolderTreeViewItem.Header;

			var fileSystemEntryInfo = fileSystemEntryItem.FileSystemEntryInfo;
			var selectedFolderPath = fileSystemEntryInfo.Path;

			folderChangedHandler(this,
								 new FolderChangedEventArgs(selectedFolderPath));
		}
	}

	private void OnKeyPressed(object sender, KeyEventArgs e)
	{
		if (_selectedThumbnailBox != null)
		{
			var keyPressed = e.Key;

			if (GlobalData.BackwardNavigationKeys.Contains(keyPressed))
			{
				AdvanceToThumbnailIndex(-1);
			}
			else if (GlobalData.ForwardNavigationKeys.Contains(keyPressed))
			{
				AdvanceToThumbnailIndex(1);
			}
			else if (keyPressed == Key.Enter)
			{
				DisplayImage();
			}
		}
	}

	private void SelectThumbnailBox(ThumbnailBox thumbnailBox)
	{
		if (_selectedThumbnailBox != thumbnailBox)
		{
			if (_selectedThumbnailBox != null)
			{
				_selectedThumbnailBox.UnselectThumbnail();
			}

			_selectedThumbnailBox = thumbnailBox;
			_selectedThumbnailIndex = _thumbnailBoxList
										.FindIndex(aThumbnailBox =>
												   aThumbnailBox == _selectedThumbnailBox);

			_selectedThumbnailBox.SelectThumbnail();
		}
	}

	private void AdvanceToThumbnailIndex(int increment)
	{
		if (_selectedThumbnailBox != null)
		{
			var newSelectedThumbnailIndex = _selectedThumbnailIndex + increment;

			if ((newSelectedThumbnailIndex >= 0) &&
				(newSelectedThumbnailIndex < _thumbnailBoxList.Count))
			{
				_selectedThumbnailBox.UnselectThumbnail();

				_selectedThumbnailIndex = newSelectedThumbnailIndex;
				_selectedThumbnailBox = _thumbnailBoxList[_selectedThumbnailIndex];

				_selectedThumbnailBox.SelectThumbnail();
			}
		}
	}

	private void DisplayImage()
	{
		var imageView = ImageViewFactory.ImageView;
		imageView.SetImage(_screenSize, _selectedThumbnailBox.ImageFile);

		imageView.ThumbnailChanged +=
			(sender, e) =>
			{
				AdvanceToThumbnailIndex(e.Increment);
				imageView.SetImage(_screenSize, _selectedThumbnailBox.ImageFile);
			};

		imageView.ShowDialog(this);
	}

	private static TreeViewItem GetTreeViewItem(
		FileSystemEntryInfo fileSystemEntryInfo)
	{
		var fileSystemEntryItem = new FileSystemEntryItem
		{
			FileSystemEntryInfo = fileSystemEntryInfo
		};

		var treeViewItem = new TreeViewItem
		{
			Header = fileSystemEntryItem
		};

		return treeViewItem;
	}

	#endregion
}
