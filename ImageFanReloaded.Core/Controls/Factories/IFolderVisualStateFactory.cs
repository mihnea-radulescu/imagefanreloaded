namespace ImageFanReloaded.Core.Controls.Factories;

public interface IFolderVisualStateFactory
{
	IFolderVisualState GetFolderVisualState(
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath);
}
