namespace ImageFanReloaded.Core.Synchronization;

public interface IFolderChangedEventHandleFactory
{
	IFolderChangedEventHandle GetFolderChangedEventHandle();
}
