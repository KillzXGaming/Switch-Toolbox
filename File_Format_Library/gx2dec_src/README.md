# gx2dec: GX2 (Wii U Latte) shader to GLSL decompiler

`gx2dec.exe` is the standalone tool the toolbox shells out to when viewing an emitter's
GX2 vertex/pixel shaders as GLSL (see `../FileFormats/Effects/Gx2ShaderDecompiler.cs`).
It is not linked into the toolbox; it runs as a separate process.

## What's ours vs Cemu's

- `src/main.cpp`, `src/latteshader_impl.cpp`, `src/globals.cpp`: the CLI driver (this project).
  It reads raw shader microcode + context registers from files and calls the decompiler.
- `shim/`: stub `openssl`/`wx` headers and a no-op `Renderer` so the decompiler translation
  units compile without Cemu's full dependency set (this project; the GLSL generation code
  itself is unmodified Cemu source).
- The decompiler itself is **Cemu's legacy Latte shader decompiler**, licensed under the
  **Mozilla Public License 2.0**:
  - Source: https://github.com/cemu-project/Cemu, tag `v2.6`
  - License: https://www.mozilla.org/MPL/2.0/

The decompiler version matters: its uniform-remapping order must match the Cemu build that
produced any capture/dump it is compared against. `v2.6` matches the shader-research corpus
this feature was validated on.

## Building

The build compiles Cemu's MPL-2.0 sources directly, so it is a C++ (MSVC C++20) build, not
part of the C# `Toolbox.sln`. Fetch the dependencies listed at the top of `build.cmd`
(Cemu v2.6 source tree + header-only {fmt}, boost, glm, Vulkan-Headers) next to this
README, then run `build.cmd`. It finds Visual Studio with `vswhere`; set `VSDEVCMD` to pick a specific install.
The output replaces `../gx2dec.exe`.

A prebuilt `gx2dec.exe` is bundled at `../gx2dec.exe` for convenience; see
`../gx2dec.NOTICE.txt`.

## Usage

```
gx2dec <vs|ps> <program.bin> <regs.bin> [fetch.bin]
```

- `program.bin`: raw shader microcode bytes
- `regs.bin`: raw Latte context-register bytes (loaded into a zero-padded register array)
- `fetch.bin`: (vs only) raw fetch-shader microcode bytes

GLSL is written to stdout; the host binding/uniform map is written to stderr.
