using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Controls.Implementation;

namespace ImageFanReloaded.Views.Implementation;

public partial class MainWindow
    : Window, IMainView
{
	public MainWindow()
    {
        InitializeComponent();

		_windowFontSize = FontSize;

		_imagesTabCounter = 0;

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
		AddHandler(KeyUpEvent, OnKeyPressed, RoutingStrategies.Tunnel);

		_tabControl.AddHandler(KeyDownEvent, OnTabControlKeyPressing, RoutingStrategies.Tunnel);
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

	private readonly double _windowFontSize;

	private TabItem _fakeTabItem;
	private int _imagesTabCounter;

	private void OnKeyPressing(object sender, KeyEventArgs e)
	{
		var keyPressing = e.Key;

		if (keyPressing == GlobalData.TabSwitchKey)
		{
			var canNavigateAcrossTabs = CanNavigateAcrossTabs();

			if (!canNavigateAcrossTabs)
			{
				e.Handled = true;
			}
		}
	}

	private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        var keyPressed = e.Key;

        if (keyPressed == GlobalData.TabSwitchKey)
        {
			var selectedTabItemIndex = _tabControl.SelectedIndex;

			var contentTabItemCount = GetContentTabItemCount();
			var nextSelectedTabItemIndex = (selectedTabItemIndex + 1) % contentTabItemCount;

			_tabControl.SelectedIndex = nextSelectedTabItemIndex;
		}
        else
        {
	        var contentTabItem = GetActiveContentTabItem();
	        contentTabItem.OnKeyPressed(sender, e);
        }
        
        e.Handled = true;
    }

	private void OnTabControlKeyPressing(object sender, KeyEventArgs e)
	{
		e.Handled = true;
	}

	private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var contentTabItem = GetActiveContentTabItem();
		var isFakeTabItem = contentTabItem.Title == FakeTabItemTitle;

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
			_tabControl.Focus();
		}
	}

	private void SetTabItem(string title, out ContentTabItem contentTabItem, out TabItem tabItem)
	{
		contentTabItem = new ContentTabItem();

		tabItem = new TabItem
		{
			Content = contentTabItem,
			FontSize = _windowFontSize
		};

        contentTabItem.TabItem = tabItem;
		contentTabItem.Title = title;
		contentTabItem.Window = this;
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

	private bool CanNavigateAcrossTabs() => GetContentTabItemCount() > 1;

	private IContentTabItem GetActiveContentTabItem()
    {
	    var tabItem = (TabItem)_tabControl.SelectedItem;
	    var contentTabItem = (IContentTabItem)tabItem.Content;

	    return contentTabItem;
    }

    #endregion
}
