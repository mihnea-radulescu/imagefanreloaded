using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.CustomEventArgs;
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
	public event EventHandler? HelpMenuRequested;

	public void AddFakeTabItem()
	{
		var fakeTabItem = new TabItem
		{
			Header = FakeTabItemTitle,
			FontSize = _windowFontSize
		};

		_tabControl.Items.Add(fakeTabItem);
	}
	
	public async Task ShowInfoMessage(string title, string text)
	{
		var infoMessageBox = MessageBoxManager.GetMessageBoxStandard(
			title,
			text,
			MsBox.Avalonia.Enums.ButtonEnum.Ok,
			MsBox.Avalonia.Enums.Icon.Info,
			WindowStartupLocation.CenterOwner);

		await infoMessageBox.ShowWindowDialogAsync(this);
	}

	#region Private

	private const string DefaultTabItemTitle = "New Tab";
	private const string FakeTabItemTitle = "➕";

	private readonly double _windowFontSize;

	private void OnKeyPressing(object? sender, KeyEventArgs e)
	{
		var keyModifiers = e.KeyModifiers.ToCoreKeyModifiers();
		var keyPressing = e.Key.ToCoreKey();
		
		var contentTabItem = GetActiveContentTabItem();

		if (ShouldCloseWindow(keyModifiers, keyPressing))
		{
			CloseWindow();
			e.Handled = true;
		}
		else if (ShouldNavigateToNextTab(keyModifiers, keyPressing))
		{
			NavigateToNextTab();
			e.Handled = true;
		}
		else if (ShouldDisplayHelp(keyPressing))
		{
			DisplayHelp();
			e.Handled = true;
		}
		else if (contentTabItem!.ShouldHandleKeyPressing(keyModifiers, keyPressing))
		{
			contentTabItem!.HandleKeyPressing(keyModifiers, keyPressing);
			e.Handled = true;
		}
		else if (contentTabItem!.IsFolderTreeViewFocused() && !GlobalParameters!.IsNavigationKey(keyPressing))
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
			Content = contentTabItem
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

    private bool ShouldCloseWindow(
	    ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
    {
        if (keyPressing == GlobalParameters!.EscapeKey)
        {
            return true;
        }

        if (GlobalParameters!.IsWindows)
        {
	        return ShouldWindowsOsCloseWindow(keyModifiers, keyPressing);
        }

		return false;
    }

    private bool ShouldWindowsOsCloseWindow(
	    ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.AltKeyModifier && keyPressing == GlobalParameters!.F4Key)
	    {
		    return true;
	    }
	        
	    return false;
    }

    private bool ShouldNavigateToNextTab(
	    ImageFanReloaded.Core.Keyboard.KeyModifiers keyModifiers, ImageFanReloaded.Core.Keyboard.Key keyPressing)
    {
	    if (keyModifiers == GlobalParameters!.ShiftKeyModifier && keyPressing == GlobalParameters!.TabKey)
	    {
		    return HasAtLeastOneTabItem();
	    }

	    return false;
    }
    
    private bool ShouldDisplayHelp(ImageFanReloaded.Core.Keyboard.Key keyPressing)
	    => keyPressing == GlobalParameters!.F1Key;
    
    private bool HasAtLeastOneTabItem()
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

	private void DisplayHelp()
	{
		HelpMenuRequested?.Invoke(this, EventArgs.Empty);
	}

	#endregion
}
