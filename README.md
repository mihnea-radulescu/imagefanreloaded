# imagefanreloaded
ImageFan Reloaded is a cross-platform, light-weight, tab-based image viewer, supporting multi-core processing.

It is written in C#, relies on the Avalonia UI framework, and targets .NET 8 on Linux, Windows and macOS.

Features:
* quick concurrent thumbnail generation, scaling to the number of processor cores present
* support for multiple image folder tabs
* keyboard and mouse user interaction
* targeted zoom and mouse-drag zoomed image control
* fast and seamless full-screen navigation across images

User interface:
* left mouse button for selecting tabs, folders and images, and for opening, zooming in and out, and dragging images
* right mouse button for returning from the opened image to the main view
* mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through opened images
* key Tab for cycling though active tabs
* keys W-A-S-D, Up-Down-Left-Right, Backspace-Space and PageUp-PageDown for back and forward navigation through thumbnails and opened images
* keys Enter-Escape for entering into and exiting from image display modes

![Screenshot 1](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-Dark.jpg "ImageFan Reloaded - Dark Screenshot")

![Screenshot 2](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-Light.jpg "ImageFan Reloaded - Light Screenshot")
