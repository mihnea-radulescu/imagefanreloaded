using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Infrastructure.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Views
{
    public partial class MainView
        : Window, IMainView
    {
        public MainView(IImageViewFactory imageViewFactory, IVisualActionDispatcher dispatcher)
        {
            _imageViewFactory = imageViewFactory ?? throw new ArgumentNullException(nameof(imageViewFactory));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

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
                    selectedItem.Items.Clear();

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

        public void ClearThumbnailBoxes()
        {
            _thumbnailWrapPanel.Children.Clear();
            _thumbnailScrollViewer.ScrollToVerticalOffset(0);

            _thumbnailBoxList = new List<ThumbnailBox>();
            _selectedThumbnailIndex = -1;
            _selectedThumbnailBox = null;
        }

        public void PopulateThumbnailBoxes(ICollection<ThumbnailInfo> thumbnails)
        {
            foreach (var aThumbnail in thumbnails)
            {
                var aThumbnailBox = new ThumbnailBox(_dispatcher, aThumbnail);
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

        #region Private

        private readonly IImageViewFactory _imageViewFactory;
        private readonly IVisualActionDispatcher _dispatcher;

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
