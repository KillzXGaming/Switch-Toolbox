# Switch-Toolbox
A tool to edit many formats of Nintendo Switch and Wii U. 

## Changelog 1.0 Experimental / BETA
https://docs.google.com/spreadsheets/d/16JLhGBJL5U5hpKWspL-pzYIaRL23X1YKEmia6pbsGbc/edit#gid=1386834576

## Releases
https://github.com/KillzXGaming/Switch-Toolbox/releases

## Features

This tool currently features:
- BFRES
   - Fully supports Wii U and Switch.
   - Model importing (dae, fbx, obj, and csv)
   - Material editing (Render info, texture mapping, parameters, etc)
   - Material copying
   - Animation and model sub section can be exported/imported.
   - Can delete, add, replace individual objects from an fmdl.
   - Can create new sub sections and data
   - Can preview skeletal, SRT, param, texture pattern, and bone visual animations. (Param ones will vary)
   - Can export and import fully rigged models with bone support.
   - Can convert gif files to texture pattern animations. Very WIP atm.
- BNTX
   - Can add/remove textures.
   - Can import textures as DDS. (Thanks to AboodXD! png/jpeg, etc planned later)
   - Can export as binary, dds, png, tga, etc.
   - Can preview mipmap and surface(array) levels.
- SARC
   - Supported editing/saving data opened. (Automatically saves data opened if supported)
   - Supports padding (Thanks to Exelix and AboodXD)
   - Can save sarcs in sarcs in sarcs.
- BARS
   - Can extract and replace audio files.. (rebuilds the file)
- KCL
   - Preview collision models.
   - Replace/Export as obj (Thanks to Exelix)
- BFFNT
   - Can extract and replace images.
- BFFNT
   - Can extract font images (BNTX)
- GFPAK
   - Supported editing/saving data opened. (Automatically saves data opened if supported)
- BEA
   - Supported editing/saving data opened. (Automatically saves data opened if supported)
- XTX
   - Can preview textures.
- NARC
   - Can extract files. (Thanks to Gericom/EFE)
- BFSHA
   - Can view options, samplers, attributes, and uniform blocks.
- BNSH
   - Can extract shader vertex and fragment shaders from variations/programs
- SHARCFB and SHARC
   - Basic preview of some shader program data.
- AAMP
   - Can edit v1 AAMP (Wii U) and preview v2 AAMP
- NUTEXB
   - Can extract and replace textures.
- PTCL & EFFN
   - Can edit colors (Wii U) and edit textures (Wii U). Switch ptcl can only preview both atm.
- EFC
   - Can preview effect tables and link PTCL.
- NUT
   - Can preview NTWU, NTP3, and NTWD variants. Editng will be soon
- MSBT
   - Very basic previewing.
- TMPK
  - Can extract files and decompress
- MP3, OGG, IDSP, HPS, WAV, BFWAV, BFSTM, BCWAV, BCWAV
  - Can listen to audio and convert between certain formats. Thanks to VGAudio and CSCore
	

   
## Building
To build make sure you have Visual Studio installed (I use 2017, older versions may not work) and open the .sln. Then build the solution as release. It should compile properly on the latest.

In the event that the tool cannot compile, check references. All the libraries are stored in Switch-Toolbox/Lib folder. 

- Smash Forge Devs (SMG, Ploaj,  jam1garner, smb123w64gb, etc) for some code ported over. Specifically animation stuff, GTX c# implementation, and some rendering.
- Assimp devs for their massive asset library!
- Wexos (helped figure out a few things, ie format list to assign each attribute)
- JuPaHe64 for the base 3D renderer.
- Every File Explorer devs (Gericom) for Yaz0 stuff
- Exelix for Byaml, Sarc and KCL library
- Syroot for helpful IO extensions and libraries
- GDK Chan for some DDS decode methods
- AboodXD for some foundation stuff with exelix's SARC library, Wii U (GPU7) and Switch (Tegra X1) textures swizzling, and documentation for GTX and BNTX
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
- [CSCore](https://github.com/filoe/cscore)
- [Assimp](https://bitbucket.org/Starnick/assimpnet/src/master/)
- [OpenTK](https://github.com/opentk/opentk)
- [BezelEngineArchive Library](https://github.com/KillzXGaming/BEA-Library-Editor)
- [Syroot BinaryData](https://gitlab.com/Syroot/BinaryData)
- [Syroot Maths](https://gitlab.com/Syroot/Maths)
- [Syroot Bfres Library (Wii U)](https://gitlab.com/Syroot/NintenTools.Bfres)

License
 in Switch_Toolbox\Lib\Licenses
 
 Please note if you do not want your library used or if i'm missing credits! 
