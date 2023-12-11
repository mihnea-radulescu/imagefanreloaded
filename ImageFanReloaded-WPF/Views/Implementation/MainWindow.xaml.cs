using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.Controls.Implementation;

namespace ImageFanReloaded.Views.Implementation
{
	public partial class MainWindow
		: Window, IMainView
	{
		public MainWindow()
		{
			InitializeComponent();

			_imagesTabCounter = 0;
		}

		public event EventHandler<TabItemEventArgs> ContentTabItemAdded;

		public void AddContentTabItem()
		{
			_imagesTabCounter++;
			var title = $"Images Tab {_imagesTabCounter}";

			SetTabItem(title, out ContentTabItem contentTabItem, out TabItem tabItem);

			var tabItemsCount = _tabControl.Items.Count;
			_tabControl.Items.Insert(tabItemsCount - 1, tabItem);

			ContentTabItemAdded?.Invoke(this, new TabItemEventArgs(contentTabItem));

			if (_imagesTabCounter == MaxContentTabs)
			{
				_tabControl.Items.Remove(_fakeTabItem);
			}
		}

		public void AddFakeTabItem()
		{
			SetTabItem(FakeTabItemTitle, out ContentTabItem contentTabItem, out TabItem tabItem);

			_tabControl.Items.Add(tabItem);

			_fakeTabItem = tabItem;
		}

		#region Private

		private const int MaxContentTabs = 10;
		private const string FakeTabItemTitle = "+";

		private TabItem _fakeTabItem;
		private int _imagesTabCounter;

		private void OnKeyPressed(object sender, KeyEventArgs e)
		{
			var keyPressed = e.Key;

			if (keyPressed == GlobalData.TabSwitchKey)
			{
				var contentTabItemCount = GetContentTabItemCount();
				var canNavigateAcrossTabs = contentTabItemCount > 1;

				if (canNavigateAcrossTabs)
				{
					var selectedTabItemIndex = _tabControl.SelectedIndex;

					var nextSelectedTabItemIndex = (selectedTabItemIndex + 1) % contentTabItemCount;
					_tabControl.SelectedIndex = nextSelectedTabItemIndex;
				}
			}
		}

		private void OnKeyPressing(object sender, KeyEventArgs e)
		{
			e.Handled = true;
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var tabControl = (TabControl)sender;
			var tabItem = (TabItem)tabControl.SelectedItem;

			if (tabItem == null)
			{
				return;
			}

			var tabItemContent = (ContentTabItem)tabItem.Content;
			var isFakeTabItem = tabItemContent.Title == FakeTabItemTitle;

			if (isFakeTabItem)
			{
				AddContentTabItem();

				if (tabControl.SelectedItem == _fakeTabItem)
				{
					tabControl.SelectedIndex--;
				}
			}
		}

		private void SetTabItem(string title, out ContentTabItem contentTabItem, out TabItem tabItem)
		{
			contentTabItem = new ContentTabItem();

			tabItem = new TabItem
			{
				Content = contentTabItem
			};

			contentTabItem.TabItem = tabItem;
			contentTabItem.Title = title;
		}

		private int GetContentTabItemCount()
		{
			var contentTabItemCount = _tabControl.Items.Count;

			var isFakeTabPresent = _tabControl.Items.Contains(_fakeTabItem);
			if (isFakeTabPresent)
			{
				contentTabItemCount--;
			}

			return contentTabItemCount;
		}

		#endregion
	}
}
