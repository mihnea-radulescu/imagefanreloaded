﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Controls.Implementation
{
	public partial class ContentTabItem
		: UserControl, IContentTabItem
	{
		public ContentTabItem()
		{
			InitializeComponent();
		}

		public TabItem TabItem
		{
			get => _tabItem;
			set
			{
				_tabItem = value;

				_tabItem.KeyUp += OnKeyPressed;
			}
		}

		public IImageViewFactory ImageViewFactory { get; set; }
		public object GenerateThumbnailsLockObject { get; set; }

		public string Title
		{
			get => (string)TabItem.Header;
			set => TabItem.Header = value;
		}

		public event EventHandler<FolderChangedEventArgs> FolderChanged;

		public IFolderVisualState FolderVisualState { get; set; }

		public void PopulateSubFoldersTree(ICollection<FileSystemEntryInfo> subFolders,
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
			_thumbnailScrollViewer.ScrollToVerticalOffset(0);
		}

		public void PopulateThumbnailBoxes(
			ICollection<ThumbnailInfo> thumbnailInfoCollection)
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
			ICollection<ThumbnailInfo> thumbnailInfoCollection)
		{
			foreach (var thumbnailInfo in thumbnailInfoCollection)
			{
				thumbnailInfo.RefreshThumbnail();
			}
		}

		#region Private

		private TabItem _tabItem;

		private List<ThumbnailBox> _thumbnailBoxList;
		private int _selectedThumbnailIndex;
		private ThumbnailBox _selectedThumbnailBox;

		private void OnFolderTreeViewSelectedItemChanged(object sender,
														 RoutedPropertyChangedEventArgs<object> e)
		{
			var folderChangedHandler = FolderChanged;

			if (folderChangedHandler != null)
			{
				var selectedFolderTreeViewItem = (TreeViewItem)e.NewValue;
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
				else if (keyPressed == GlobalData.EnterKey)
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

		private bool AdvanceToThumbnailIndex(int increment)
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

					return true;
				}
			}

			return false;
		}

		private void DisplayImage()
		{
			var imageView = ImageViewFactory.GetImageView();
			imageView.SetImage(_selectedThumbnailBox.ImageFile);

			imageView.ThumbnailChanged +=
				(sender, e) =>
				{
					if (AdvanceToThumbnailIndex(e.Increment))
					{
						imageView.SetImage(_selectedThumbnailBox.ImageFile);
					}
				};

			imageView.ShowDialog();
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
}