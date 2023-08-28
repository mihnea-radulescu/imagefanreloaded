# imagefanreloaded
ImageFan Reloaded is a light-weight image viewer, supporting multi-core processing.

ImageFan Reloaded features two versions:
* __ImageFanReloaded-Avalonia__ - targets .NET 6 on Windows, Linux and macOS (requires .NET Runtime 6)
* __ImageFanReloaded-WPF__ - targets .NET Framework 4 Client Profile on Windows (requires .NET Framework 4 Client Profile, installed by default on Windows 8 and newer)

__ImageFan Reloaded-Avalonia__ features mouse-only interaction, using the left and the right mouse buttons, and the mouse wheel:
* the left mouse button for selecting, opening, zooming in, zooming out, and dragging images
* the right mouse button for returning to the main view
* the mouse wheel for moving back and forward through thumbnails and opened images

__ImageFanReloaded-WPF__, in addition to mouse interaction, also provides key-based interaction, using the keys W-A-S-D, Up-Down-Left-Right, Backspace-Space and PageUp-PageDown for back and forward navigation, and the keys Enter-Escape for moving into and out of display modes.

Detailed mouse-driven interactions:
* scroll the drives and folders section
* click on a drive or folder to browse through its images in the thumbnails section
* scroll the thumbnails section
* left-click on a thumbnail to view the image full-screen; the image is resized to screen size, if it does not fit on the screen
* left-click on the full-screen image to view it displayed at its actual size (only if the image did not fit on the screen)
* left-click on the actual-size image to view it full-screen, resized to screen size
* left-click-and-drag on the actual-size image to move it around
* scroll up and down to move back and forward through the images in the thumbnails section
* right-click on the full-screen image or the actual-size image to return to the main view

![Screenshot 1](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/master/Screenshot-Avalonia-Linux-Light.jpg "ImageFan Reloaded - Avalonia Linux Light Screenshot")

![Screenshot 2](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/master/Screenshot-Avalonia-Linux-Dark.jpg "ImageFan Reloaded - Avalonia Linux Dark Screenshot")

![Screenshot 3](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/master/Screenshot-WPF-Windows.jpg "ImageFan Reloaded - WPF Windows Screenshot")
