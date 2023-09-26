using System;
using Avalonia.Controls;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.Controls.Implementation;

namespace ImageFanReloaded.Views.Implementation;

public partial class MainWindow
    : Window, IMainView
{
    public MainWindow()
    {
        InitializeComponent();
	}

	public event EventHandler<TabItemEventArgs> ContentTabItemAdded;

	public void AddFakeTabItem()
	{
		var tabItem = new TabItem
		{
			Header = "+",
			FontSize = TabItemFontSize
		};

		_tabControl.Items.Add(tabItem);
	}

	#region Private

	private const int TabItemFontSize = 12;

	private int _imagesTabCounter;

	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var tabControl = (TabControl)sender;
		var tabItem = (TabItem)tabControl.SelectedItem;

		if (tabItem.Content == null)
		{
			AddContentTabItem();
		}
	}

	private void AddContentTabItem()
	{
		var contentTabItem = new ContentTabItem();

		var tabItem = new TabItem
		{
			Content = contentTabItem,
			FontSize = TabItemFontSize
		};

		contentTabItem.TabItem = tabItem;
		contentTabItem.Window = this;

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

	#endregion
}
