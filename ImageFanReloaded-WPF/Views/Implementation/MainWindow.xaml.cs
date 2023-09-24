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
			var contentTabItem = new ContentTabItem();

			var tabItem = new TabItem
			{
				Content = contentTabItem
			};

			contentTabItem.TabItem = tabItem;

			_imagesTabCounter++;
			contentTabItem.Title = $"Images Tab {_imagesTabCounter}";

			var tabItemsCount = _tabControl.Items.Count;

			if (tabItemsCount == 0)
			{
				_tabControl.Items.Add(tabItem);
			}
			else
			{
				_tabControl.Items.Insert(tabItemsCount - 1, tabItem);
			}

			ContentTabItemAdded?.Invoke(this, new TabItemEventArgs(contentTabItem));
		}

		public void AddFakeTabItem()
		{
			var tabItem = new TabItem
			{
				Header = "+"
			};

			_tabControl.Items.Add(tabItem);
		}

		#region Private

		private int _imagesTabCounter;

		private void OnKeyPressed(object sender, KeyEventArgs e)
		{
			var keyPressed = e.Key;

			if (keyPressed == GlobalData.TabSwitchKey)
			{
				var tabItemsCount = _tabControl.Items.Count;
				var selectedTabItemIndex = _tabControl.SelectedIndex;

				if (tabItemsCount > 1)
				{
					var nextSelectedTabItemIndex = (selectedTabItemIndex + 1) % (tabItemsCount - 1);
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

			if (tabItem.Content == null)
			{
				AddContentTabItem();

				if (tabControl.SelectedIndex > 0)
				{
					tabControl.SelectedIndex--;
				}
			}
		}

		#endregion
	}
}
