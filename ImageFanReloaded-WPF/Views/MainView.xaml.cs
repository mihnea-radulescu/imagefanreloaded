using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ImageFanReloadedWPF.CommonTypes.CommonEventArgs;
using ImageFanReloadedWPF.CommonTypes.Info;
using ImageFanReloadedWPF.Controls;
using ImageFanReloadedWPF.Factories.Interface;
using ImageFanReloadedWPF.Views.Interface;

namespace ImageFanReloadedWPF.Views
{
    public partial class MainView
        : Window, IMainView
    {
        public MainView(IImageViewFactory imageViewFactory)
        {
            _imageViewFactory = imageViewFactory ?? throw new ArgumentNullException(nameof(imageViewFactory));

            InitializeComponent();
        }

        public event EventHandler<FolderChangedEventArgs> FolderChanged;

        public void PopulateSubFoldersTree(ICollection<FileSystemEntryInfo> subFolders,
                                           bool rootNodes)
        {
            if (rootNodes)
            {
                foreach (var aSubFolder in subFolders)
                {
                    var aTreeViewItem = new FileSystemEntryTreeViewItem
                    {
                        FileSystemEntryInfo = aSubFolder
                    };

                    _folderTreeView.Items.Add(aTreeViewItem);
                }
            }
            else if (_folderTreeView.SelectedItem != null)
            {
                var selectedItem = _folderTreeView.SelectedItem as TreeViewItem;

                if (selectedItem != null)
                {
                    if (selectedItem.Items.Count == 0)
                    {
                        foreach (var aSubFolder in subFolders)
                        {
                            var aTreeViewItem = new FileSystemEntryTreeViewItem
                            {
                                FileSystemEntryInfo = aSubFolder
                            };

                            selectedItem.Items.Add(aTreeViewItem);
                        }
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

        public void PopulateThumbnailBoxes(ICollection<ThumbnailInfo> thumbnails)
        {
            foreach (var aThumbnail in thumbnails)
            {
                var aThumbnailBox = new ThumbnailBox(aThumbnail);
                aThumbnail.ThumbnailBox = aThumbnailBox;

                aThumbnailBox.ThumbnailBoxClicked +=
                    (sender, e) =>
                    {
                        var thumbnailBox = sender as ThumbnailBox;

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

        public void RefreshThumbnailBoxes(ICollection<ThumbnailInfo> thumbnails)
        {
            foreach (var aThumbnail in thumbnails)
            {
                aThumbnail.RefreshThumbnail();
            }
        }

        #region Private

        private readonly IImageViewFactory _imageViewFactory;

        private List<ThumbnailBox> _thumbnailBoxList;
        private int _selectedThumbnailIndex;
        private ThumbnailBox _selectedThumbnailBox;

        private void OnFolderTreeViewSelectedItemChanged(object sender,
                                                         RoutedPropertyChangedEventArgs<object> e)
        {
            var folderChangedHandler = FolderChanged;

            if (folderChangedHandler != null)
            {
                var selectedFolderTreeViewItem = e.NewValue as FileSystemEntryTreeViewItem;

                if (selectedFolderTreeViewItem != null)
                {
                    var selectedFolderPath = selectedFolderTreeViewItem.FileSystemEntryInfo.Path;
                    folderChangedHandler(this,
                                         new FolderChangedEventArgs(selectedFolderPath));
                }
            }
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource == _thumbnailScrollViewer &&
                _selectedThumbnailBox != null)
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

        private void OnScrollKeyPressed(object sender, KeyEventArgs e)
        {
            e.Handled = true;
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
            var imageView = _imageViewFactory.ImageView;
            imageView.SetImage(_selectedThumbnailBox.ImageFile);

            imageView.ThumbnailChanged +=
                (sender, e) =>
                {
                    AdvanceToThumbnailIndex(e.Increment);
                    imageView.SetImage(_selectedThumbnailBox.ImageFile);
                };
            
            imageView.ShowDialog();
        }

        #endregion
    }
}
