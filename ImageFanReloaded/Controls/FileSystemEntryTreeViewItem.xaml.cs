using System.Windows.Controls;

using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Controls
{
    public partial class FileSystemEntryTreeViewItem
        : TreeViewItem
    {
        public FileSystemEntryTreeViewItem()
        {
            InitializeComponent();
        }

        public FileSystemEntryInfo FileSystemEntryInfo
        { 
            get
            {
                return _fileSystemEntryInfo;
            }
            set
            {
                _fileSystemEntryInfo = value;

                _fileSystemEntryImage.Source = _fileSystemEntryInfo.Icon;
                _fileSystemEntryTextBlock.Text = _fileSystemEntryInfo.Name;
            }
        }

        #region Private

        private FileSystemEntryInfo _fileSystemEntryInfo;

        #endregion
    }
}
