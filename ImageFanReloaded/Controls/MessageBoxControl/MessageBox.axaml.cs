using Avalonia.Controls;
using Avalonia.Layout;

namespace ImageFanReloaded.Controls.MessageBoxControl;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        InitializeComponent();
    }

    public MessageBoxResult Result { get; private set; }

    public void AddButton(string buttonText, MessageBoxResult buttonClickResult)
    {
        var button = new Button
        {
            Content = buttonText,
            MinWidth = ButtonMinWidth,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            IsDefault = buttonClickResult == MessageBoxResult.Continue,
            IsCancel = buttonClickResult == MessageBoxResult.Cancel
        };

        button.Click += (_, _) =>
        {
            Result = buttonClickResult;
            Close();
        };

        ButtonsStackPanel.Children.Add(button);
    }

    private const int ButtonMinWidth = 80;
}
