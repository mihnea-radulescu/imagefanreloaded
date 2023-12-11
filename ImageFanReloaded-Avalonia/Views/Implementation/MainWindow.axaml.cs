﻿using System;
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

		_imagesTabCounter = 0;
	}

	public event EventHandler<TabItemEventArgs> ContentTabItemAdded;

	public void AddFakeTabItem()
	{
		SetTabItem(FakeTabItemTitle, out ContentTabItem contentTabItem, out TabItem tabItem);

		_tabControl.Items.Add(tabItem);

		_fakeTabItem = tabItem;
	}

	#region Private

	private const int MaxContentTabs = 10;
	private const string FakeTabItemTitle = "+";
	private const int TabItemFontSize = 12;

	private TabItem _fakeTabItem;
	private int _imagesTabCounter;

	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var tabControl = (TabControl)sender;
		var tabItem = (TabItem)tabControl.SelectedItem;

		var tabItemContent = (ContentTabItem)tabItem.Content;
		var isFakeTabItem = tabItemContent.Title == FakeTabItemTitle;

		if (isFakeTabItem)
		{
			AddContentTabItem();
		}
	}

	private void AddContentTabItem()
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

	private void SetTabItem(string title, out ContentTabItem contentTabItem, out TabItem tabItem)
	{
		contentTabItem = new ContentTabItem();

		tabItem = new TabItem
		{
			Content = contentTabItem,
			FontSize = TabItemFontSize
		};

		contentTabItem.TabItem = tabItem;
		contentTabItem.Title = title;
		contentTabItem.Window = this;
	}

	#endregion
}
