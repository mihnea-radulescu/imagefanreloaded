using ImageFanReloaded.Core.AboutInformation;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

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
		@"Cross-platform, feature-rich, tab-based image viewer, supporting multi-core processing

Version {0}
Copyright © Mihnea Rădulescu 2017 - {1}

https://github.com/mihnea-radulescu/imagefanreloaded

User interface:

• left mouse button for interacting with tabs and folders, and for selecting, opening, zooming in and out, and
	dragging images
• right mouse button for displaying image info, and for returning from the opened image to the main view
• mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through
	opened images
• key combos Ctrl+Plus for adding a new tab, and Ctrl+Minus for closing an existing tab
• key combo Shift+Tab for cycling through tabs
• key Tab for cycling through controls in the active tab
• keys N and M for changing folder ordering between name and last modification time
• keys A and D for changing folder ordering direction between ascending and descending
• keys + and - for changing thumbnail size by an increment of 50 pixels
• key U to toggle showing file names under thumbnail images
• key S for slideshow navigation
• key F for displaying Image info view
• key T for displaying Image edit view, and for switching from command-line image file access mode to
	thumbnail navigation mode
• keys in Image edit view: U for undo, I for redo, R for rotate, F for flip, E for effects, S for save as, C for crop
	and D for downsize
• key R to toggle recursive folder browsing
• key E for applying EXIF image orientation
• key O for displaying Tab options view
• keys H and F1 for displaying About view
• keys Up, Down, Left and Right for back and forward navigation through the folders tree, thumbnails and
	opened images
• keys PageUp and PageDown for scrolling through thumbnails
• key Enter for entering image view and zoomed image view modes
• key combos Ctrl+Up, Ctrl+Down, Ctrl+Left and Ctrl+Right for dragging zoomed images
• key I to toggle showing image info in image view and zoomed image view modes
• key Esc for exiting image view and zoomed image view modes, and for quitting application
";

	private readonly IAboutInformationProvider _aboutInformationProvider;
	private readonly IGlobalParameters _globalParameters;

	#endregion
}
