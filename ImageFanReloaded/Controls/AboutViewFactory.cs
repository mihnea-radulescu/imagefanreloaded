using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls;

public class AboutViewFactory : IAboutViewFactory
{
    public AboutViewFactory(
        IAboutInformationProvider aboutInformationProvider,
        IGlobalParameters globalParameters)
    {
        _aboutInformationProvider = aboutInformationProvider;
        _globalParameters = globalParameters;
    }
    
    public IAboutView GetAboutView()
    {
        var aboutText = string.Format(
            AboutTextTemplate, _aboutInformationProvider.VersionString, _aboutInformationProvider.Year);

        IAboutView aboutView = new AboutWindow();
        aboutView.GlobalParameters = _globalParameters;
        aboutView.SetAboutText(aboutText);
        
        return aboutView;
    }
    
    #region Private
    
    private const string AboutTextTemplate =
        @"Cross-platform, light-weight, tab-based image viewer, supporting multi-core processing

Version {0}
Copyright © Mihnea Rădulescu 2017 - {1}

https://github.com/mihnea-radulescu/imagefanreloaded

User interface:

• left mouse button for interacting with tabs and folders, and for selecting, opening, zooming in and out, and
   dragging images
• right mouse button for returning from the opened image to the main view
• mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through
   opened images
• key combos Ctrl+Plus for adding a new tab, and Ctrl+Minus for closing an existing tab
• key combo Shift+Tab for cycling through tabs
• key Tab for cycling through controls in the active tab
• keys N and M for changing folder ordering between name ascending and last modification time descending
• keys + and - for changing thumbnail size by an increment of 50 pixels
• key R for toggling recursive folder browsing
• key O for displaying Tab options view
• keys F1 and H for displaying About view
• keys Up, Down, Left and Right for back and forward navigation through the folders tree, thumbnails and
   opened images
• keys PageUp and PageDown for scrolling through thumbnails
• key Enter for entering image view and zoomed image view modes
• key combos Ctrl+Up, Ctrl+Down, Ctrl+Left and Ctrl+Right for dragging zoomed images
• key I for toggling image info in image view and zoomed image view modes
• keys Esc and T for exiting image view and zoomed image view modes
• key Esc for quitting application";
    
    private readonly IAboutInformationProvider _aboutInformationProvider;
    private readonly IGlobalParameters _globalParameters;

    #endregion
}
