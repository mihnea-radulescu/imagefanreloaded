# imagefanreloaded
ImageFan Reloaded is a cross-platform, light-weight, tab-based image viewer, supporting multi-core processing.

It is written in C#, and targets .NET 8 on Linux, Windows and macOS. It relies on [Avalonia](https://github.com/AvaloniaUI/Avalonia), as its UI framework, and on [Magick.NET](https://github.com/dlemstra/Magick.NET), as its image manipulation library.

Features:
* quick concurrent thumbnail generation, scaling to the number of processor cores present
* support for multiple folder tabs
* keyboard and mouse user interaction
* 44 supported image formats: bmp, cr2, cur, dds, dng, exr, fts, gif, hdr, heic, heif, ico, jfif, jp2, jpe/jpeg/jpg, jps, mng, nef, nrw, orf, pam, pbm, pcd, pcx, pef, pes, pfm, pgm, picon, pict, png, ppm, psd, qoi, raf, rw2, sgi, svg, tga, tif/tiff, wbmp, webp, xbm, xpm
* image editing capabilities, with undo support: rotate, flip, effects, save in various formats, crop and downsize
* image animation support for the formats gif, mng and webp
* folder ordering by name and last modification time, ascending and descending
* configurable thumbnail size, between 100 and 400 pixels
* slideshow navigation across images
* image info containing file, image, color, EXIF, IPTC and XMP profiles
* automatic image orientation according to the EXIF Orientation tag
* toggle-able recursive folder browsing
* targeted zooming in, and moving over the zoomed image
* fast and seamless full-screen navigation across images
* command-line direct access to the specified folder or image file

User interface:
* left mouse button for interacting with tabs and folders, and for selecting, opening, zooming in and out, and dragging images
* right mouse button for displaying image info, and for returning from the opened image to the main view
* mouse wheel for scrolling through folders and thumbnails, and for navigating back and forward through opened images
* key combos Ctrl+Plus for adding a new tab, and Ctrl+Minus for closing an existing tab
* key combo Shift+Tab for cycling through tabs
* key Tab for cycling through controls in the active tab
* keys N and M for changing folder ordering between name and last modification time
* keys A and D for changing folder ordering direction between ascending and descending
* keys + and - for changing thumbnail size by an increment of 50 pixels
* key S for slideshow navigation
* key T for displaying Image edit view, and for switching from command-line image file access mode to thumbnail navigation mode
* keys in Image edit view: U for undo, I for redo, R for rotate, F for flip, E for effects, S for save as, C for crop and D for downsize
* key F for displaying Image info view
* key R for toggling recursive folder browsing
* key E for applying EXIF image orientation
* key O for displaying Tab options view
* keys H and F1 for displaying About view
* keys Up, Down, Left and Right for back and forward navigation through the folders tree, thumbnails and opened images
* keys PageUp and PageDown for scrolling through thumbnails
* key Enter for entering image view and zoomed image view modes
* key combos Ctrl+Up, Ctrl+Down, Ctrl+Left and Ctrl+Right for dragging zoomed images
* key I for toggling image info in image view and zoomed image view modes
* key Esc for exiting image view and zoomed image view modes, and for quitting application

![Screenshot 1](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-1.jpg "ImageFan Reloaded - Screenshot 1")

![Screenshot 2](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-2.jpg "ImageFan Reloaded - Screenshot 2")

![Screenshot 3](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-3.jpg "ImageFan Reloaded - Screenshot 3")

![Screenshot 4](https://raw.githubusercontent.com/mihnea-radulescu/imagefanreloaded/main/Screenshot-4.jpg "ImageFan Reloaded - Screenshot 4")
