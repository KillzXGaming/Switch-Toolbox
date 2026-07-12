# BotW EFT Renderer Changelog

## V3.0.0: Emitter preview with the game's own shaders (2026-07-12)

Added an experimental but promising preview that uses the decoded GX2 vertex/fragment shaders.

### Added
- **The emitter editor previews through the shaders the game itself renders with.** Its *Game shaders (GPU)* mode,
  on by default, decompiles the emitter's own GX2 vertex/fragment pair to GLSL and draws with it. The uniform banks
  are rebuilt from the emitter's file data, the per-particle streams come from the existing simulation, and the
  inputs the file does not carry (the environment and scene banks, the HDR environment cube, the vertex-shader
  lookup tables and the scene depth) are shipped beside the plugin. Billboards, primitive meshes, connection and
  trail stripes, and the rain and snow drop classes all draw. The software approximation is now the fallback, taking
  over with a note when an emitter's shader pair will not decompile; the mode is offered only for files that carry
  shaders at all.
- **High dynamic range, resolved with the game's own tonemap.** The preview accumulates into a floating-point
  target and resolves through BotW's operator, so an additive effect such as fire keeps its colour instead of
  clipping to a flat yellow.
- **Sampler state read from the file**, so art textures honour their per-slot GX2 wrap modes and their own flipbook
  grid rather than the first slot's.
- **Three more emitter fields in the Parameters tab**, decoded while wiring the shaders up. The scale a particle is
  born at, and the percentage it is randomly shrunk by: this, not the emission radius the tab already showed, is the
  size the game's vertex shader reads. And the waveform the blink intensity and duration pairs drive the alpha with,
  which is a sine, a random or a blink in every emitter the library ships.

### Changed
- **The preview no longer starves the paint of the controls around it.** It asked for its next animation frame from
  inside its own paint, which kept a repaint permanently pending and left the controls beside it (the shader toggle,
  the viewport's menu bar and model row) unpainted until the mouse passed over them. The request now goes through the
  idle pump, which runs only once everything else has been painted.
- **Preview playback runs on 60fps** instead of one step per repaint, so an effect plays at its real
  speed whatever the monitor does; on a 144Hz display it previously ran at 2.4x speed. This applies to the
  software preview too.
- **The GX2 to GLSL decompiler is rebuilt from Cemu v2.6**, which is what lets the emitter shaders decompile well
  enough to execute. Its source, shims and build script are vendored under `gx2dec_src`; the script now locates
  Visual Studio with `vswhere` instead of a fixed path.
- A texture that fails to decode no longer takes the emitter texture panel down with it.

## V2.0.2: ELink2 actor names recovered from the file itself (2026-06-29)

### Changed
- **Actor names now resolve from the file, with no sidecar.** A user record stores only a CRC32 of its name, but
  the same name strings live in the file's global name table (referenced there as asset keys and property names),
  so the viewer recovers each name by hashing every name-table string and matching it against the stored user
  hashes. This labels nearly every stock actor on load (1345 of 1348 in the retail file) with no external file;
  the few with no string anywhere in the file stay as their raw hash.
- **Added and renamed actor names persist inside the file.** Creating, duplicating or renaming an actor appends
  its name to the name table (deduped), so it round-trips through save and reload by that same recovery path. The
  old `<file>.names.txt` sidecar (read on load, written on save) is removed.

## v10: ELink2 curve-driven override editing (2026-06-27)  [= V2.0.1]

### Added
- **Edit curve-driven overrides.** A Float override whose value follows a curve (Scale, Alpha, EmissionRate,
  LifeScale and so on) opens a curve editor from the "..." button on its row: the driving property, the curve
  type (Standard or Constant), and a points table (X = property, Y = value) with Add / Insert above / Insert
  below / Remove so points go anywhere. Editing gives the asset a private copy, reusing an identical curve when
  present and otherwise appending the points and the record at their table ends, so shared curves and the rest
  of the file stay untouched. A local-property curve's driving property is fixed, since the game keys it by index.
- **English names beside the built-in Japanese ones.** Property and enum-value dropdowns gloss the game's
  built-in names, for example "速度  (Speed)" and "天候  (Weather)". The stored value is always the original; a
  typed name is kept as-is.

### Changed
- *Edit triggers* moved from the actor's menu to its **Triggers** folder (right-click it); the folder now always
  shows, so an actor with no triggers can still add one.

## v9: ELink2 trigger and condition editing, and Switch/Random/Sequence groups (2026-06-27)

### Added
- **Edit the conditions that gate an effect.** Right-click a group child for *Edit condition*: a Switch child
  takes a property branch (value type, comparison and value, or a default case), a Random child takes a weight,
  and a Sequence child takes force-continue. The dialog shows the group's watched property and adapts its fields
  to the group type.
- **Edit a Switch group's watched property** (*Edit watch property*), as a global game property or, for an
  existing local watch, keeping its local property. The property name and a global/local explanation are shown.
- **Edit triggers** (*Edit triggers* on an actor): list, add, edit and remove the triggers that fire its effects.
  A trigger is an Action (animation slot and action, with a frame range and type), a Property (a watched property
  with an optional gating condition) or an Always (unconditional) trigger, and Edit can change one kind into another.
- **Create Switch, Random, Random (no repeat) and Sequence groups**, not just Blend. The create dialog picks the
  group type; Random children get a default weight, and a Switch's watch property and child conditions are set
  afterward.
- Conditions live in a shared table, so editing one child's condition gives it a private record (reusing an
  identical one when present, otherwise appended at the table end), leaving every other user of the original
  untouched. The same applies to a property trigger's gating condition.
- The watch property, condition value and trigger property fields are editable dropdowns listing the names and
  values the file already uses, with a Switch branch's value list filtered to its watched property. Common
  values are one selection away, and a new name or value can still be typed.

### Changed
- The effect-group create menu item is now *Add effect group* with a type picker and per-type help text, replacing
  the Blend-only *Add effect group (Blend)*.

## v8: ELink2 actor editing: create, delete, duplicate and rename user entries (2026-06-27)

### Added
- **Create, delete, duplicate and rename whole actors** (the user entries that own an effect setup).
  - Right-click the file: *Add actor* makes a new empty actor (give it effects with the node tools).
  - Right-click an actor: *Duplicate actor* clones its entire effect setup under a new actor name (the clone plays
    the same effects, sharing the global blocks, and later edits stay isolated copy-on-write); *Rename actor*
    recomputes the name hash and re-sorts the lookup table in place (the block does not move, so the file size is
    unchanged); *Delete actor* removes the actor and all its effects.
  - An actor name is hashed (CRC32) into the sorted lookup table; a name that collides with an existing actor is
    rejected, and the last actor cannot be deleted. The new actor is selected and labelled by the typed name.
- **Actor names persist across save and reload.** The file stores only a CRC32 of each name, so added or renamed
  actor names are written to a `<file>.names.txt` sidecar on save (merged non-destructively with any existing one),
  which the viewer reads back on load. Without this, a custom actor reopened as its raw hash.
- The UserDataTable (sorted name-hashes plus parallel exRegion offsets) and the actor's block are inserted or removed
  together, and the ParamDefineTable (a derived position) plus every region after the header shift accordingly. The
  parser now derives that position from the actor count instead of assuming a fixed offset. Each mechanism was
  prototyped and verified in Python against the real file, then the C# was checked headlessly to match byte-for-byte,
  with every edit re-parsing clean across all actors.

### Changed
- `.sbelnk` (the Yaz0-compressed form) now appears in the Open dialog's supported-files filter.

### Upcoming features
- **Automatic actor-name recovery.** Because the `.belnk` stores only a CRC32 hash of each actor name, a planned
  addition will recover the names directly from the game's actor packs (each actor's ActorLink records its ELink user)
  and label every actor automatically. In the meantime, names are supplied by a `<file>.names.txt` sidecar (a plain
  list matched by hash, read on load and appended to on save); any actor not covered by it shows as its raw hash.

## v7: ELink2 node editing: create, delete and duplicate call nodes (2026-06-27)

### Added
- **Create, delete and duplicate ELink call nodes** from the asset-call tree (right-click).
  - *Delete* removes a leaf or a whole container subtree. It cascades: a container left with no children is removed,
    and any trigger that fires a removed node is removed along with its now-empty action and slot.
  - *Duplicate* clones a node or subtree as a sibling with a `_copy` name. Duplicating a top-level effect also clones
    the trigger that fires it, so the copy is an independent, playable effect.
  - *Add* inserts a new asset leaf, or a Blend container with one child, under a container, or a new top-level effect.
    A top-level effect also gets an alwaysTrigger, since a node that no trigger points at is never reached by the game.
    The emitter set a new node plays is chosen from a dropdown of the sets in any open `.sesetlist`. The new or
    duplicated node is selected in the tree.
- The edited user's call list is rebuilt and spliced back in place: the call table is re-laid-out (top-level calls
  first, then a depth-first pass that gives each node's children a contiguous index block, matching the game's own
  layout), the name-sorted call-index table is regenerated, the trigger range tables are kept consistent, and the
  regions after the edited user shift by the size delta. Each mechanism was prototyped and verified in Python against
  the real file, then the C# was exercised headlessly against the same file, with every edit re-parsing clean across
  all 1345 users.

## v6: ELink2 effect-link viewer and editor (2026-06-26)  [= V2.0.0]

A viewer and editor for BotW's `Bootup.pack/ELink2/ELink2DB.belnk` (the effect-link database, xlink2
big-endian version 0x1E). The game plays emitter sets through ELink, which can override their params and gate
them by runtime state, so this exposes and edits that second data source alongside the `.sesetlist` editor.

### Viewer
- **ELink2DB tree**: one folder per effect "user" (the actor or system that triggers effects), its
  AssetCallTable tree (Switch / Random / Blend / Sequence containers show their watch property and branch
  condition inline), and an Actions / Triggers list (event or property to asset, with frame ranges). User
  blocks build lazily on expand, since the file holds 1345 users.
- **User names**: the file stores only a CRC32 of each user name, so users show as hashes unless an optional
  sidecar `<file>.belnk.names.txt` (one user name per line) is present; matching names are labelled, the rest
  stay hashes.
- **Open original emitter set**: the asset panel links to the emitter set the asset plays; clicking selects
  and reveals that ESET node in any open `.sesetlist` (including one in another window), or names the set if
  none is open. The ELink set name is byte-identical to the `.sesetlist` set name (verified), so the match is exact.
- **In-panel preview with overrides applied**: the asset panel renders the referenced emitter set beside its
  data, with the ELink overrides applied as multipliers (Scale, LifeScale, DirectionalVel, EmissionRate /
  Interval, EmissionScale, and colour / alpha) so the preview matches how the game plays the effect, not the raw
  set. It reuses the emitter renderer in a single shared GL host; the override multipliers default to identity,
  so the existing emitter editor is unchanged. Needs the matching `.sesetlist` open (a hint is shown otherwise).
  `Duration` bounds the preview's emission window (a timed effect plays one puff then replays, instead of
  emitting endlessly). Curve / random overrides use a representative constant (curve first-point / random
  midpoint). Not modelled (the renderer draws at origin, has no character skeleton): Bone / Position / Rotation /
  Matrix and the engine-internal `val*` params; those show in the data panel but not the viewport.

### Editing
- **Asset panel is a PropertyGrid** (matching the emitter Parameters tab). It lists every asset param; a blank
  value means "not overridden". Type a value to add or set the override, clear it (or right-click and Reset) to
  remove it, so unset params are never written as defaults. Set overrides show bold, strings show bare (the
  quoting is handled on save), and the row description states the real param type and override kind.
- **Add, edit and remove overrides, isolated per asset.** Override blocks are de-duplicated (one block can back
  1000+ assets), so a naive in-place edit would change every asset sharing a block. The edited asset is given a
  private (copy-on-write) copy of its block, appended at the end of the ResParam region, and only that asset is
  repointed, so an edit touches exactly one asset and its block-mates are untouched (verified: one of 1002
  sharers changed). Within the private block, repeated value edits reuse a spare (unreferenced) directValueTable
  slot so shared values are never disturbed.
- **Float params are expandable** to a Mode (NotSet / Constant / Random): Constant shows one Value, Random shows
  Min and Max, and the parent row summarises ("0.5", "-0.025 .. 0.025", or "(not set)"). Setting Mode to Random
  or editing Min/Max writes a RandomLinear override; a random entry is reused when an identical min/max exists,
  edited in place when this param owns it, or appended to the randomTable. The randomTable lives mid-file, so
  when it grows the curve, point, exRegion, condition and name regions after it shift and all their offsets are
  fixed up. Only Float params can be random (verified from the data), so other types stay single fields.
- **String overrides are editable** (e.g. the emitter set an asset plays): the new name is reused from the name
  pool or appended to it (the pool is the last region, so appending only grows the file end).
- Appending blocks at the end of the ResParam region means existing block offsets never move, so every region
  not rebuilt (TriggerOverwrite table, sorted tables, conditions, value tables, name pool) stays byte-verbatim,
  sidestepping the file's hidden cross-references. Save writes the bytes and the toolbox re-applies Yaz0.
- Every byte mechanism was prototyped and verified in Python against the real file (each re-parses clean across
  all 1345 users) before porting: value edit, add, remove, copy-on-write isolation, string edit, and growing the
  randomTable (17k+ random / curve refs intact; a random-range edit on a block shared 1227 ways changed exactly
  one asset).

## v5: Editing polish and preview fixes (2026-06-25)

Usability fixes from hands-on testing.

### Editing
- **Delete warnings name the references**: deleting a texture or primitive now lists up to 5 "Set / Emitter"
  names of the emitters that use it (plus the total), so they are easy to find, instead of only a count.
- **Import an .obj into an empty primitive table**: when a file's PRMA has no primitive to use as a header
  template, the importer borrows one from any other open file. An .obj carries only geometry, so a PRIM
  header template is still required; this removes the dead end as long as another effect file is open.
- **Delete the last texture**: now allowed. An empty texture table is valid, and Add Texture can create a
  new texture from scratch, so blocking the final delete served no purpose.
- **Primitive context menu**: dropped the inherited generic STGenericWrapper items (Export / Replace /
  Rename / Delete), which do nothing useful on a raw PRIM mesh block: Export opened a save dialog but
  wrote no file (the base Export() is an empty stub that was never overridden), Replace and Delete were
  greyed out (CanReplace / CanDelete are false), and Rename only relabeled the tree node. The
  purpose-built "Export mesh (.obj)" / "Replace mesh (.obj)" / "Delete Primitive" items remain.

## v4: Review hardening (2026-06-25)

### Fixes
- **Cross-file resource pool leak**: the per-file texture/primitive lists were appended on every reparse
  and never cleared, growing unbounded and holding stale objects after each structural edit. Now cleared
  before each republish.
- **Malformed PRIM no longer aborts the file open**: the mesh decoder clamps its vertex count to what the
  block can hold, so a truncated/corrupt primitive yields a partial mesh instead of throwing.

### Cleanup
- Moved the renderer to `GL/` alongside the other GL renderers; split the EFTB section/resource types
  (section-tree editing node, textures, PRIM mesh) into a `PCTL.Eftb.cs` partial class so the shared
  parser file drops from ~4900 to ~3500 lines.

### Cross-format safety / licensing
- The Parameters tab + live preview + mesh column are now built lazily on the first EFTB emitter, so the
  3DS / Switch PTCL editor keeps its stock layout instead of gaining an inert tab and preview pane.
- Published `gx2dec_src/` (the CLI driver + build script + README) and an MPL-2.0 NOTICE next to the
  bundled `gx2dec.exe`; it links Cemu's MPL-2.0 Latte decompiler (commit 079a4af), so the source is
  available per the license. The NOTICE is copied alongside the binary on build.

## v3: Shader integration: visibility + CRUD + cross-file (2026-06-25)

The toolbox decompiles GX2 -> GLSL for viewing but has no compiler, so shaders can be viewed, re-pointed,
deleted, copied and moved between files, but never authored from scratch (every "create" is a copy of an
existing compiled shader).

### New features
- **Shader usage view**: the "GTX Shader" node lists each shader program in use and which emitters bind
  it, with the fragment sampler count and a "[shared with <other open file>]" tag (byte-identical GX2
  program shared with another loaded `.sesetlist`), making the shader<->emitter<->file links explicit.
- **Re-point an emitter's shader**: the emitter Parameters tab ("1c. Shader") picks the vertex / fragment
  shader index (dropdown of the file's pool) the emitter binds; bakes on save.
- **Prune Unused Shaders / Clear All Shaders**: right-click the GTX Shader node. Prune drops every shader
  no emitter references (re-indexing the survivors); Clear blanks the whole bundle (guarded, irreversible).
  Useful because deleting emitter sets does not by itself remove the shaders they used.
- **Shaders travel with export/import**: EFTX bundles (v2) now carry the referenced GX2 shader groups;
  importing an emitter/set merges them into the target bundle (dedup by content) and re-points the imported
  emitter onto them, so a moved emitter actually renders in the target file.

## v2: Editing, import/export, shader view & stability (2026-06-25)

Builds on the v1 renderer below. Adds file-editing and shader-inspection tooling and hardens the
emitter-editing pipeline.

### New features

- **Clear folder contents**: right-click "Clear All" on the Emitter Sets, Textures, or PRMA roots to
  empty them while keeping the (now-empty) root. Clearing textures/primitives warns how many emitters
  still reference them, listing up to 5 example emitter names.
- **Export / import emitters and sets**: right-click to export an emitter (`.eftemitter`) or a whole
  set (`.eftset`) to a toolbox-only bundle, and import it into another `.sesetlist`. The bundle copies
  the referenced textures/primitives that exist in any open file (so it's self-contained); a reference
  to a resource in an unopened file is exported as a plain reference and resolves against the target.
  Imported resources dedupe by texture id / primitive hash (no duplicates, no remap).
- **View an emitter's GX2 shader as GLSL**: right-click an emitter -> "View Shader (GLSL)" decompiles
  the vertex + fragment shaders it binds out of the SHDA bundle (via a CEMU-derived GX2 / Wii U Latte ->
  GLSL decompiler) and reports which shader indices it uses plus the texture/uniform binding map.

### Fixes & stability

- **Preview leak crash**: fixed a GPU-resource leak that crashed the toolbox after previewing several
  emitters; shaders/VBO/scene texture are now shared per GL control and per-emitter mesh buffers are
  freed when a render is swapped out.
- **Hidden save dialogs**: the "Compress with Yaz0?" prompt and the "saved" notice are now owned by the
  main window so they no longer render behind it; the live preview pauses while the app isn't foreground.
- **File Explorer blanking on delete**: deleting an emitter no longer blanks/freezes the tree (the
  texture-id mapping was O(n^2)); after an add/delete the tree keeps the root folders expanded and
  selects the neighbor (previous entry, else the parent folder).

---

## v1: Renderer (initial features)

- **Live 3D preview of BotW particle effects** inside Switch-Toolbox, select any emitter in a
  `.sesetlist` and it renders, driven entirely from the file's own bytes.
- **Billboard particles**: camera-facing textured quads, world-space sizing, particle spin.
- **Mesh / primitive particles**: PRIM geometry for ring / shockwave / ripple emitters.
- **Stripe & ribbon types**: connection stripes (threaded through live particles) and trail stripes
  (per-particle position history), rendered as tapered camera-facing ribbons.
- **CPU motion simulation** from decoded fields: lifespan, emit rate / interval, two-stage emission
  velocity, dispersion cone, air resistance, volume shapes, momentum randomization.
- **Color / alpha / scale over life**: driven by the 8-key animation curves.
- **3-texture combiner**: each emitter composites up to 3 texture slots with its own per-slot blend
  ops (multiply / add / subtract / max).
- **Faithful render state**: additive vs alpha blend, depth test, face culling, and a universal alpha
  test
- **Emission classes**: continuous / one-shot burst / windowed, with a one-shot replay pause so
  explosions fire-and-pause instead of streaming forever; ambient effects (rain, torches) keep flowing.
- **Automatic camera framing**: to the effect's extent, with relative particle sizes preserved.
- **Distortion / refraction pass**: `_ind` / Haze / ripple emitters warp the rendered scene buffer.
- **Per-axis texture wrap + quadrant mirror-tiling**: e.g. the Guardian targeting reticle ring is
  stored as one quarter and mirror-tiled into the full ring.
- **Per-slot flipbook atlases**: each texture slot samples its own grid cell, on both the mesh and
  billboard paths.
- **Static flipbook cell selection**: variant-picker emitters (e.g. the 12 field flowers) each hold
  their own assigned atlas cell instead of cycling.
- **BC5 normal/flow-map detection**: keeps data textures out of the alpha coverage mask so they don't
  draw as opaque rectangles.
- **Cursor-follow emitter**: moving the mouse over the preview drives stripe/trail emitters so ribbons
  sweep (supplying the emitter motion the `.sesetlist` itself doesn't carry).
- **Emitter authoring tools**: a verified Parameters editor with live preview, scale-curve editing,
  texture-sampler params (wrap / UV-expand / max-aniso), and per-slot flipbook grid fields.
- **Diagnostics**: texture-slot role tooltips + console shader-link reporting that catches silent
  GLSL failures.

---

## Reverse-engineering findings (offset + meaning)

Offsets reverse-engineered using open-source projects and RenderDoc.

### Color / alpha / scale
- Key counts: Color0 `0x10`, Alpha0 `0x14`, Color1 `0x18`, Alpha1 `0x1C`, Scale `0x20`.
- 8-key curves (x,y,z,Time): Color0 `0x370`, Alpha0 `0x3F0`, Color1 `0x470`, Alpha1 `0x4F0`,
  Scale `0x5B0`; constant colors ConstColor0 `0x958`, ConstColor1 `0x968`.
- Base particle radius `0x360`.

### Render state (verified vs captured GPU pipeline)
- `blendType` `0x88D`, 0 = alpha, 1 = additive (refuted the earlier `0x8DC` guess).
- `zBufATestType` `0x88E`, 0 = depth-test / no-write, 1 = ignore-Z.
- `displaySide` `0x84F`, 0 = both, 1 = front (cull back), 2 = back (cull front).

### Motion / emission
- `lifespan` `0x6F0`, `emitRate` `0x6F4`, `emitInterval` `0x710`, `emitFunction/volumeType` `0x714`,
  `endFrame` `0x780` (-1 = infinite), `allDirVel` `0x7B0`, `airResist` `0x6DC`,
  `dispersionAngle` `0x7F4`, `arcLength` `0x7F0`, `volumeScale` VEC3 `0x808/0x80C/0x814`,
  emission `dir` VEC3 `0x7C8`, `dirVel` `0x7D4`.
- Decoded the nw::eft **two-stage emission velocity** model (omnidirectional shape-burst + directional
  cone) from the NW4F-Eft decomp.
- Established that **gravity is effectively flag-off in BotW**: no isolable downward-Y accel VEC3
  exists across 9,572 emitters; falling vs rising is carried by the emission direction.
- Velocity is a plain big-endian float (no fixed-point /10), capture-confirmed.

### Rotation
- Z-rotation init `0x6C8` (2*pi sentinel = random orientation), `angularVelocity` `0x6D8`,
  `momentumRandom` `0x7C4`.

### Particle / render type
- `vertexTransformMode` `0x8F4`, 0 = billboard, 2 = trail stripe, 3 = connection stripe;
  VS-verified as CPU-built strip geometry (not GPU velocity-stretch).

### Textures & flipbook
- 3 real texture sampler slots @ `0x9A8` (3 x 0x20; low 32 bits = texture hash; empty = 0xFFFFFFFF).
- **Per-slot flipbook grid** (cols/rows): slot0 `0x2B8/0x2BC`, slot1 `0x308/0x30C`, slot2 `0x358/0x35C`
  (stride 0x50). Fixed emitters that drew their whole atlas at once.
- **Static flipbook cell index** `0xD0`, the per-emitter cell pick; proven by the 12 `Flower` emitters
  each holding cell 0..11 of one 4x4 atlas (static variants, never an animation).
- Per-axis texture wrap: `wrapU` @ sampler+`0x08`, `wrapV` @ sampler+`0x09` (0 = mirror, 1 = repeat,
  2 = clamp); separate UV-expand flag @ sampler+`0x17`; `maxAniso` @ sampler+`0x0C`.
- Decoded the nw::eft texture-pattern-animation mechanism (enable / period / used-size / clamp /
  random-start + a cell indirection table) from the decomp; BotW relocated the block, so the static
  cell is the verified faithful default.

### Combiner / fragment shading
- 3-texture FragmentComposite ops (0=Mul 1=Add 2=Sub 3=Max): `textureColorBlend` `0x8AD`,
  `primitiveColorBlend` `0x8AE`, `textureAlphaBlend` `0x8B1`, `primitiveAlphaBlend` `0x8B2`.
- `fragmentColorMode` `0x8A8`, `fragmentAlphaMode` `0x8A9`, value 3 = subtract/erosion alpha
  (`clamp(texAlpha - particleAlpha)`), gated to the erosion (low-birth-alpha) signature.
- slot2 is a coverage mask multiplied **within** the slot0/slot1 shape, never added on top
  (capture-grounded; fixed a flooded-alpha square).
- Distortion/refraction discriminator: `0x8B8 == 1` OR `0x701 == 3`.

### Textures shading model
- Textures are alpha / intensity **masks**; particle RGB comes from the color curves. BC4/BC5 data
  maps contribute coverage or are skipped (normal/flow), never tinting the albedo.
