using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Keyboard;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;
using ImageFanReloaded.Keyboard;

namespace ImageFanReloaded.Controls;

public partial class MainWindow : Window, IMainView
{
	public MainWindow()
    {
        InitializeComponent();

		_windowFontSize = FontSize;
		
		AddHandler(KeyDownEvent, OnKeyPressing, RoutingStrategies.Tunnel);
    }
	
	public IGlobalParameters? GlobalParameters { get; set; }
	public IFolderChangedMutexFactory? FolderChangedMutexFactory { get; set; }

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

	private const string DefaultTabItemTitle = "New tab";
	private const string FakeTabItemTitle = "➕";

	private readonly double _windowFontSize;

	private void OnKeyPressing(object? sender, Avalonia.Input.KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();
		
		var contentTabItem = GetActiveContentTabItem()!;

		if (ShouldWindowsOsCloseWindow(keyModifiers, keyPressing))
		{
			CloseWindow();
			e.Handled = true;
		}
		else if (ShouldHandleEscapeAction(keyModifiers, keyPressing))
		{
			if (contentTabItem.AreFolderInfoOrImageInfoFocused())
			{
				contentTabItem.FocusThumbnailScrollViewer();
			}
			else
			{
				CloseWindow();
			}
			e.Handled = true;
		}
		else if (ShouldAddNewTab(keyModifiers, keyPressing))
		{
			AddContentTabItem();
			e.Handled = true;
		}
		else if (ShouldCloseSelectedTab(keyModifiers, keyPressing))
		{
			CloseContentTabItem();
			e.Handled = true;
		}
		else if (ShouldNavigateToNextTab(keyModifiers, keyPressing))
		{
			NavigateToNextTab();
			e.Handled = true;
		}
		else if (contentTabItem.ShouldHandleControlKeyFunctions(keyModifiers, keyPressing))
		{
			contentTabItem.HandleControlKeyFunctions(keyModifiers, keyPressing);
			e.Handled = true;
		}
		else if (!ShouldAllowKeyPressingEventPropagation(keyModifiers, keyPressing))
		{
			e.Handled = true;
		}
	}

	private void OnTabChanged(object? sender, SelectionChangedEventArgs e)
	{
		var contentTabItem = GetActiveContentTabItem();
		var isFakeTabItem = contentTabItem is null;

		if (isFakeTabItem)
		{
			AddContentTabItem();
		}
		
		FocusSelectedContentTabItem();
	}

	private void AddContentTabItem()
	{
		var (contentTabItem, tabItem) = BuildTabItemData();

		var contentTabItemCount = GetContentTabItemCount();
		_tabControl.Items.Insert(contentTabItemCount, tabItem);

		ContentTabItemAdded?.Invoke(this, new ContentTabItemEventArgs(contentTabItem));

		var shouldAllowTabClose = ShouldAllowTabClose();
		TabCountChanged?.Invoke(this, new TabCountChangedEventArgs(shouldAllowTabClose));
	}

	private void CloseContentTabItem()
	{
		var contentTabItem = GetActiveContentTabItem();

		if (contentTabItem is not null)
		{
			CloseContentTabItem(this, new ContentTabItemEventArgs(contentTabItem));
		}
	}

	private (IContentTabItem contentTabItem, object tabItem) BuildTabItemData()
	{
		var contentTabItem = new ContentTabItem
		{
			MainView = this,
			GlobalParameters = GlobalParameters,
			FolderChangedMutex = FolderChangedMutexFactory!.GetFolderChangedMutex()
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
			Content = contentTabItem,
			IsSelected = true
		};
		
		tabItem.KeyDown += (_, e) => e.Handled = true;

		contentTabItem.WrapperTabItem = tabItem;
		
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
		
		FocusSelectedContentTabItem();
	}

	private void CloseContentTabItem(object? sender, ContentTabItemEventArgs e)
	{
		var contentTabItem = e.ContentTabItem;
		ContentTabItemClosed?.Invoke(this, new ContentTabItemEventArgs(contentTabItem));
		
		var tabItem = contentTabItem.WrapperTabItem;
		_tabControl.Items.Remove(tabItem);
		TabCountChanged?.Invoke(this, new TabCountChangedEventArgs(ShouldAllowTabClose()));
		
		contentTabItem.ContentTabItemHeader!.TabClosed -= CloseContentTabItem;
		contentTabItem.UnregisterMainViewEvents();

		SelectLastTabItem();
	}
	
	private void FocusSelectedContentTabItem()
	{
		var selectedContentTabItem = GetActiveContentTabItem()!;
		selectedContentTabItem.SetFocusOnSelectedFolderTreeViewItem();
	}

    private bool ShouldHandleEscapeAction(KeyModifiers keyModifiers, Key keyPressing)
    {
        if (keyModifiers == GlobalParameters!.NoneKeyModifier && keyPressing == GlobalParameters!.EscapeKey)
        {
            return true;
        }

		return false;
    }

    private bool ShouldWindowsOsCloseWindow(KeyModifiers keyModifiers, Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.AltKeyModifier && keyPressing == GlobalParameters!.F4Key)
	    {
		    return true;
	    }
	        
	    return false;
    }
    
    private bool ShouldAddNewTab(KeyModifiers keyModifiers, Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.CtrlKeyModifier && keyPressing == GlobalParameters!.PlusKey)
	    {
		    return true;
	    }

	    return false;
    }
    
    private bool ShouldCloseSelectedTab(KeyModifiers keyModifiers, Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.CtrlKeyModifier && keyPressing == GlobalParameters!.MinusKey)
	    {
		    return HasAtLeastOneContentTabItem();
	    }

	    return false;
    }

    private bool ShouldNavigateToNextTab(KeyModifiers keyModifiers, Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.ShiftKeyModifier && keyPressing == GlobalParameters!.TabKey)
	    {
		    return HasAtLeastOneContentTabItem();
	    }

	    return false;
    }

    private bool ShouldAllowKeyPressingEventPropagation(KeyModifiers keyModifiers, Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.NoneKeyModifier && GlobalParameters!.IsNavigationKey(keyPressing))
	    {
		    return true;
	    }

	    return false;
    }
    
    private bool HasAtLeastOneContentTabItem()
    {
	    var contentTabItemCount = GetContentTabItemCount();
	    
	    var hasAtLeastOneTabItem = contentTabItemCount > 1;
	    return hasAtLeastOneTabItem;
    }

	private void CloseWindow() => Close();

	private void NavigateToNextTab()
	{
		var contentTabItemCount = GetContentTabItemCount();
		
		var selectedTabItemIndex = _tabControl.SelectedIndex;
		var nextSelectedTabItemIndex = (selectedTabItemIndex + 1) % contentTabItemCount;
		_tabControl.SelectedIndex = nextSelectedTabItemIndex;
	}

	#endregion
}
