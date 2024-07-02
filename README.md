# imagefanreloaded
ImageFan Reloaded is a cross-platform, light-weight, tab-based image viewer, supporting multi-core processing.

It is written in C#, relies on the Avalonia UI framework, and targets .NET 8 on Linux, Windows and macOS.

Features:
* quick concurrent thumbnail generation, scaling to the number of processor cores present
* support for multiple image folder tabs
* keyboard and mouse user interaction
* toggleable recursive folder image browsing
* targeted zoom and mouse-drag zoomed image control
* fast and seamless full-screen navigation across images
* command-line direct access to the specified folder or image file

User interface:
* left mouse button for interacting with tabs and folders, and for selecting, opening, zooming in and out, and dragging images
* right mouse button for returning from the opened image to the main view
* mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through opened images
* key Tab for cycling through active tabs
* key R for toggling recursive folder access, and key combo Shift-R for toggling persistent recursive folder access
* keys W-A-S-D, Up-Down-Left-Right, Backspace-Space and PageUp-PageDown for back and forward navigation through thumbnails and opened images
* key Enter for entering image view and zoomed image view modes
* key I for toggling image info in image view and zoomed image view modes
* keys Esc and T for exiting image view and zoomed image view modes
* key Esc for quitting application
* key F1 for displaying About view

![Screenshot 1](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-1.jpg "ImageFan Reloaded - Screenshot 1")

![Screenshot 2](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-2.jpg "ImageFan Reloaded - Screenshot 2")

![Screenshot 3](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-3.jpg "ImageFan Reloaded - Screenshot 3")
