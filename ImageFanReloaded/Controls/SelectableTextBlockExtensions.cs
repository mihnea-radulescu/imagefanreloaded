using Avalonia.Controls;

namespace ImageFanReloaded.Controls;

public static class SelectableTextBlockExtensions
{
	extension(SelectableTextBlock selectableTextBlock)
	{
		public bool HasSelectedText
			=> selectableTextBlock.SelectedText != string.Empty;

		public void SetText(string text)
		{
			selectableTextBlock.ClearSelection();
			selectableTextBlock.Text = text;
		}
	}
}
