using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;

namespace ImageFanReloaded.Controls.MessageBoxControl;

public static class MessageBoxManager
{
	public static async Task<MessageBoxResult> ShowAsync(
		string title,
		string text,
		MessageBoxType messageBoxType,
		Window owner)
	{
		var messageBox = new MessageBox
		{
			Title = title,
			MessageTextBlock =
			{
				Text = text
			}
		};

		switch (messageBoxType)
		{
			case MessageBoxType.Error:
				messageBox.IconPath.Data = StreamGeometry.Parse(
					ErrorIconPathData);
				messageBox.IconPath.Fill = Brushes.Red;

				messageBox.AddButton("OK", MessageBoxResult.Continue);
				break;

			case MessageBoxType.Warning:
				messageBox.IconPath.Data = StreamGeometry.Parse(
					WarningIconPathData);
				messageBox.IconPath.Fill = Brushes.Orange;

				messageBox.AddButton("Yes", MessageBoxResult.Continue);
				messageBox.AddButton("No", MessageBoxResult.Cancel);
				messageBox.AddButton("Cancel", MessageBoxResult.Cancel);
				break;

			case MessageBoxType.Info:
				messageBox.IconPath.Data = StreamGeometry.Parse(
					InfoIconPathData);
				messageBox.IconPath.Fill = Brushes.DodgerBlue;

				messageBox.AddButton("OK", MessageBoxResult.Continue);
				break;

			default:
				throw new NotSupportedException(
					$"Enum value {messageBoxType} not supported.");
		}

		await messageBox.ShowDialog(owner);
		return messageBox.Result;
	}

	private const string ErrorIconPathData =
		"M12,2C6.47,2 2,6.47 2,12C2,17.53 6.47,22 12,22C17.53,22 22,17.53 22,12C22,6.47 17.53,2 12,2M15.59,7L12,10.59L8.41,7L7,8.41L10.59,12L7,15.59L8.41,17L12,13.41L15.59,17L17,15.59L13.41,12L17,8.41L15.59,7Z";
	private const string WarningIconPathData =
		"M12,2L1,21H23L12,2M12,6L19.53,19H4.47L12,6M11,10V14H13V10H11M11,16V18H13V16H11Z";
	private const string InfoIconPathData =
		"M11,9H13V7H11V9M12,20C7.59,20 4,16.41 4,12C4,7.59 7.59,4 12,4C16.41,4 20,7.59 20,12C20,16.41 16.41,20 12,20M12,2A10,10 0 0,0 2,12A10,10 0 0,0 12,22A10,10 0 0,0 22,12A10,10 0 0,0 12,2M11,17H13V11H11V17Z";
}
