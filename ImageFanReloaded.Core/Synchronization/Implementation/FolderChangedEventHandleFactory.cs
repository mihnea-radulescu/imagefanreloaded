namespace ImageFanReloaded.Core.Synchronization.Implementation;

public class FolderChangedEventHandleFactory : IFolderChangedEventHandleFactory
{
	public IFolderChangedEventHandle GetFolderChangedEventHandle()
		=> new FolderChangedEventHandle();
}
