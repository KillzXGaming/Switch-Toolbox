# Switch-Toolbox
A tool to edit many formats of Nintendo Switch and some Wii U. 

## Changelog

v0.73 Changelog
- Param default values can be set on import for models. (Colors reset to white, bake coords are reset)
- Fixed some bugs.

v0.72 Changelog
- Fixed crashes from RGBA DDS.
- Added an option to disable viewport. (May be quicker, and for those who use old GL versions).

v0.71 Changelog
- Stability improvements. 
- Add byml editor back in.
- Some bug fixes and exception issues

## Features

This tool currently features:
- BFRES
   - Model importing (dae, fbx, obj, and csv)
   - Material editing (Render info, texture mapping, parameters, etc)
   - Material  copying
   - Animation and model sub section can be exported/imported.
   - Can delete, add, replace individual objects from an fmdl.
- BNTX
   - Can add/remove textures.
   - Can import textures as DDS. (Thanks to AboodXD! png/jpeg, etc planned later)
   - Can export as binary, dds, png, tga, etc.
   - Can preview mipmap and surface(array) levels.
- SARC
   - Supported editing/saving data opened. (Automatically saves data opened in objectlist if supported)
   - Supports padding (Thanks to Exelix and AboodXD)
   - Can save sarcs in sarcs in sarcs.
- BARS
   - Can extract and replace audio files.. (rebuilds the file)
- KCL
   - Preview collision models.
   - Replace/Export as obj (Thanks to Exelix)
- BFFNT
   - Can extract font images (BNTX)
- GFPAK
   - Can extract files.

## Buidling
To build make sure you have Visual Studio installed (I use 2017, older versions may not work) and open the .sln. Then build the solution as release. It should compile properly on the latest.

In the event that the tool cannot compile, check references. All the libraries are stored in Switch-Toolbox/Lib folder. 

## This tool is in BETA and not final! Code also needs some major clean up!
## Credits

- Smash Forge Devs (SMG, Ploaj,  jam1garner, smb123w64gb, etc) for some code ported over. Specifically animation stuff, GTX c# implementation, and some rendering.
- Assimp devs for their massive asset library!
- Wexos (helped figure out a few things, ie format list to assign each attribute)
- JuPaHe64 for the base 3D renderer.
- Every File Explorer devs (Gericom) for Yaz0 stuff
- Exelix for Byaml, Sarc and KCL library
- Syroot for helpful IO extensions and libraries
- GDK Chan for some DDS decode methods
- AboodXD for some foundation stuff with exelix's SARC library, GTX and BNTX texture swizzling and documentation
- MelonSpeedruns for logo.

Resources
- [Treeview Icons by icons8](https://icons8.com/)
- Smash Forge (Currently placeholders)

Libraries
- [Exelix (Sarc, kcl, and byml libraries)](https://github.com/exelix11/EditorCore/tree/master/FileFormatPlugins)
- [ZstdNet (Compression)](https://github.com/skbkontur/ZstdNet)
- [Be.HexEditor by Bernhard Elbl](https://sourceforge.net/projects/hexbox/)
- GL EditorFramwork by jupahe64
- [WeifenLuo for docking suite](http://dockpanelsuite.com/)
- [SF Graphics by SMG (Experimental](https://github.com/ScanMountGoat/SFGraphics) (currently just a placeholder for shader workflow and some useful things)
- [Audio & MIDI library](https://github.com/naudio/NAudio)
- [VGAudio](https://github.com/Thealexbarney/VGAudio)
- [Assimp](https://bitbucket.org/Starnick/assimpnet/src/master/)
- [OpenTK](https://github.com/opentk/opentk)
- [BezelEngineArchive Library](https://github.com/KillzXGaming/BEA-Library-Editor)
- [Syroot BinaryData](https://gitlab.com/Syroot/BinaryData)
- [Syroot Maths](https://gitlab.com/Syroot/Maths)
- [Syroot Bfres Library (Wii U)](https://gitlab.com/Syroot/NintenTools.Bfres)
- [Costura for embedding data for plugins](https://github.com/Fody/Costura) 
- [CsvHelper (unused atm but planned to be used](https://joshclose.github.io/CsvHelper/)

License
 in Switch_Toolbox\Lib\Licenses
 
 Please note if you do not want your library used or if i'm missing credits! 
