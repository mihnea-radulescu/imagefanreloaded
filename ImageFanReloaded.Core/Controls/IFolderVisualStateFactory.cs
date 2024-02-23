namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualStateFactory
{
	IFolderVisualState GetFolderVisualState(
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath);
}
