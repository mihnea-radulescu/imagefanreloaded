using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ImageFanReloaded.CommonTypes.CustomEventArgs;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Controls.Implementation;

namespace ImageFanReloaded.Views.Implementation;

public partial class MainWindow : Window, IMainView
{
	public MainWindow()
    {
        InitializeComponent();

		_windowFontSize = FontSize;

		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
		AddHandler(KeyUpEvent, OnKeyPressed, RoutingStrategies.Tunnel);

		_tabControl.AddHandler(KeyDownEvent, OnTabControlKeyPressing, RoutingStrategies.Tunnel);
    }

	public event EventHandler<ContentTabItemEventArgs>? ContentTabItemAdded;
	public event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	
	public event EventHandler<TabCountChangedEventArgs>? TabCountChanged;

	public void AddFakeTabItem()
	{
		var fakeTabItem = new TabItem
		{
			Header = FakeTabItemTitle,
			FontSize = _windowFontSize
		};

		_tabControl.Items.Add(fakeTabItem);
	}
	
	#region Private

	private const string DefaultTabItemTitle = "New Tab";
	private const string FakeTabItemTitle = "+";

	private readonly double _windowFontSize;

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyPressing = e.Key;

		if (keyPressing == GlobalData.TabSwitchKey)
		{
			var canNavigateAcrossTabs = GetContentTabItemCount() > 1;

			if (!canNavigateAcrossTabs)
			{
				e.Handled = true;
			}
		}
	}

	private void OnKeyPressed(object? sender, KeyEventArgs e)
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
	        contentTabItem!.OnKeyPressed(sender, e);
        }
        
        e.Handled = true;
    }

	private void OnTabControlKeyPressing(object? sender, KeyEventArgs e)
	{
		_tabControl.Focus();

		e.Handled = true;
	}

	private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		var contentTabItem = GetActiveContentTabItem();
		var isFakeTabItem = contentTabItem is null;

		if (isFakeTabItem)
		{
			AddContentTabItem();
		}
	}

	private void AddContentTabItem()
	{
		var (contentTabItem, tabItem) = BuildTabItemData();

		var contentTabItemCount = GetContentTabItemCount();
		_tabControl.Items.Insert(contentTabItemCount, tabItem);

		ContentTabItemAdded?.Invoke(this, new ContentTabItemEventArgs(contentTabItem));
		TabCountChanged?.Invoke(this, new TabCountChangedEventArgs(ShouldAllowTabClose()));
	}

	private (ContentTabItem contentTabItem, TabItem tabItem) BuildTabItemData()
	{
		var contentTabItem = new ContentTabItem
		{
			Window = this,
			MainView = this
		};

		var contentTabItemHeader = new ContentTabItemHeader
		{
			ContentTabItem = contentTabItem
		};

		contentTabItem.ContentTabItemHeader = contentTabItemHeader;
		contentTabItem.ContentTabItemHeader.TabClosed += CloseContentTabItem;
		contentTabItem.RegisterMainViewEvents();
		
		contentTabItem.SetTitle(DefaultTabItemTitle);
		
		var tabItem = new TabItem
		{
			FontSize = _windowFontSize,
			Header = contentTabItem.ContentTabItemHeader,
			Content = contentTabItem
		};

		contentTabItem.TabItem = tabItem;

		return (contentTabItem, tabItem);
	}

	private int GetContentTabItemCount() => _tabControl.Items.Count - 1;
	private bool ShouldAllowTabClose() => GetContentTabItemCount() > 1;

	private IContentTabItem? GetActiveContentTabItem()
    {
	    var tabItem = (TabItem)_tabControl.SelectedItem!;
	    var contentTabItem = tabItem.Content as IContentTabItem;

	    return contentTabItem;
    }

	private void SelectLastTabItem()
	{
		var lastTabItemIndex = GetContentTabItemCount() - 1;
		var lastTabItem = (TabItem)_tabControl.Items[lastTabItemIndex]!;
		lastTabItem.IsSelected = true;
	}

	private void CloseContentTabItem(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		ContentTabItemClosed?.Invoke(this, new ContentTabItemEventArgs(contentTabItem));
		
		var tabItem = contentTabItem.TabItem;
		_tabControl.Items.Remove(tabItem);
		TabCountChanged?.Invoke(this, new TabCountChangedEventArgs(ShouldAllowTabClose()));
		
		contentTabItem.ContentTabItemHeader!.TabClosed -= CloseContentTabItem;
		contentTabItem.UnregisterMainViewEvents();

		SelectLastTabItem();
	}

    #endregion
}
