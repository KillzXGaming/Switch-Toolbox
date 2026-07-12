using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using OpenTK;
using System.IO;
using Syroot.BinaryData;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class PTCL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Particle" };
        public string[] Extension { get; set; } = new string[] { "*.ptcl", "*.sesetlist" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                if (reader.CheckSignature(4, "VFXB") ||
                    reader.CheckSignature(4, "SPBD") ||
                    reader.CheckSignature(4, "EFTF") ||
                    reader.CheckSignature(4, "EFTB"))
                    return true;
                else
                    return false;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public Header header;
        public PTCL_WiiU.Header headerU;
        public PTCL_3DS.Header header3DS;

        public byte[] data;

        // --- cross-file external resource pool ---
        // A .sesetlist emitter binds textures/meshes by a 32-bit HASH, and the referenced resource often lives in ANOTHER
        // loaded file (a shared GameResident texture, or a mesh in a sibling effect, e.g. AfterImage's "Biribiri" -> a mesh
        // in SiteBoss_ShieldDamage). Each loaded PTCL publishes its own TEXRs + Primitives here, so an emitter whose hash is
        // absent in its OWN file still resolves it from any sibling that is ALSO open. Registered in Load(), removed in
        // Unload(). When a hash misses every open file, GameResident is loaded ONCE from beside an open file; the game
        // keeps that set resident at all times, it IS the shared pool (e.g. the sword-fire flame sheets live there).
        internal static readonly List<PTCL> LoadedFiles = new List<PTCL>();
        internal readonly List<TEXR> FileTextures = new List<TEXR>();
        internal readonly List<Primitive> FilePrimitives = new List<Primitive>();
        internal static STGenericTexture FindGlobalTexture(uint id)
        {
            if (id == 0 || id == 0xFFFFFFFF) return null;
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (var f in LoadedFiles) foreach (var t in f.FileTextures) if (t.TextureID == id) return t;
                if (!LoadResidentPool()) break;
            }
            return null;
        }
        internal static Primitive FindGlobalPrimitive(uint hash)
        {
            if (hash == 0 || hash == 0xFFFFFFFF) return null;
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (var f in LoadedFiles) foreach (var p in f.FilePrimitives) if (p.Hash == hash) return p;
                if (!LoadResidentPool()) break;
            }
            return null;
        }

        static readonly HashSet<string> residentProbedDirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Probe for the pool beside each open file (GameResident*.sesetlist also accepts renamed copies) and at the
        // dump layout's ..\Pack\Bootup\Effect, relative to an actor Effect dir. Each directory is probed once.
        static bool LoadResidentPool()
        {
            var candidates = new List<string>();
            foreach (var f in LoadedFiles.ToArray())
            {
                string dir = null;
                try { dir = string.IsNullOrEmpty(f.FilePath) ? null : System.IO.Path.GetDirectoryName(f.FilePath); } catch { }
                if (string.IsNullOrEmpty(dir) || !residentProbedDirs.Add(dir)) continue;
                try
                {
                    string exact = System.IO.Path.Combine(dir, "GameResident.sesetlist");
                    if (System.IO.File.Exists(exact)) candidates.Add(exact);
                    else candidates.AddRange(System.IO.Directory.GetFiles(dir, "GameResident*.sesetlist"));
                    candidates.Add(System.IO.Path.Combine(dir, @"..\Pack\Bootup\Effect\GameResident.sesetlist"));
                }
                catch { }
            }
            foreach (var cand in candidates)
            {
                try
                {
                    string full = System.IO.Path.GetFullPath(cand);
                    if (!System.IO.File.Exists(full)) continue;
                    if (LoadedFiles.Exists(p => string.Equals(p.FilePath, full, StringComparison.OrdinalIgnoreCase)))
                        continue;   // already open (or already pooled)
                    byte[] raw = System.IO.File.ReadAllBytes(full);
                    if (raw.Length > 4 && raw[0] == 'Y' && raw[1] == 'a' && raw[2] == 'z' && raw[3] == '0')
                    {
                        var dec = new Toolbox.Library.Yaz0().Decompress(new System.IO.MemoryStream(raw));
                        var ms = new System.IO.MemoryStream(); dec.CopyTo(ms); raw = ms.ToArray();
                    }
                    var pool = new PTCL();
                    pool.FileName = System.IO.Path.GetFileName(full);
                    pool.FilePath = full;
                    pool.Load(new System.IO.MemoryStream(raw));   // registers itself in LoadedFiles
                    return true;
                }
                catch { }
            }
            return false;
        }

        //Reveal the emitter SET named setName in any loaded .sesetlist, for the ELink viewer's "open original
        //emitter" link. An ELink RuntimeAssetName is the emitter-set name, byte-identical to the ESET node's Text.
        //Returns false when no open file holds it, so the caller can prompt to open the right file.
        public static bool TrySelectEmitterSet(string setName)
        {
            if (string.IsNullOrEmpty(setName)) return false;
            var node = FindEsetNode(setName, StringComparison.Ordinal)
                    ?? FindEsetNode(setName, StringComparison.OrdinalIgnoreCase);
            var tv = node?.TreeView;
            if (tv == null) return false;
            tv.FindForm()?.Activate();   //bring the owning ObjectEditor window forward
            tv.SelectedNode = node;
            node.EnsureVisible();
            return true;
        }
        private static SectionBase FindEsetNode(string setName, StringComparison cmp)
        {
            foreach (var ptcl in LoadedFiles)
            {
                var hit = FindSection(ptcl.Nodes, s => s.Signature == "ESET" && string.Equals(s.Text, setName, cmp));
                if (hit != null) return hit;
            }
            return null;
        }
        private static SectionBase FindSection(TreeNodeCollection nodes, Func<SectionBase, bool> match)
        {
            foreach (TreeNode n in nodes)
            {
                if (n is SectionBase sb && match(sb)) return sb;
                var deep = FindSection(n.Nodes, match);
                if (deep != null) return deep;
            }
            return null;
        }

        //All emitters of the loaded emitter SET named setName, for the ELink viewer's in-panel preview. Empty if no
        //open .sesetlist holds the set. Same name match as TrySelectEmitterSet (RuntimeAssetName == ESET node Text).
        public static List<Emitter> GetEmitterSetEmitters(string setName)
        {
            var list = new List<Emitter>();
            if (string.IsNullOrEmpty(setName)) return list;
            var eset = FindEsetNode(setName, StringComparison.Ordinal) ?? FindEsetNode(setName, StringComparison.OrdinalIgnoreCase);
            if (eset != null) CollectEmitters(eset.Nodes, list);
            return list;
        }
        private static void CollectEmitters(TreeNodeCollection nodes, List<Emitter> outl)
        {
            foreach (TreeNode n in nodes)
            {
                if (n is SectionBase sb && sb.Signature == "EMTR" && sb.BinaryData is Emitter em) outl.Add(em);
                CollectEmitters(n.Nodes, outl);
            }
        }

        //Distinct emitter-set names across every open .sesetlist, sorted, for the ELink create dialog's set dropdown.
        public static List<string> GetEmitterSetNames()
        {
            var seen = new HashSet<string>(); var names = new List<string>();
            foreach (var ptcl in LoadedFiles) CollectSetNames(ptcl.Nodes, seen, names);
            names.Sort(StringComparer.OrdinalIgnoreCase);
            return names;
        }
        private static void CollectSetNames(TreeNodeCollection nodes, HashSet<string> seen, List<string> outl)
        {
            foreach (TreeNode n in nodes)
            {
                if (n is SectionBase sb && sb.Signature == "ESET" && seen.Add(sb.Text)) outl.Add(sb.Text);
                CollectSetNames(n.Nodes, seen, outl);
            }
        }

        bool IsWiiU = false;
        bool Is3DS = false;

        public void Load(Stream stream)
        {
            data = stream.ToArray();

            Text = FileName;
            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                string Signature = reader.ReadString(4, Encoding.ASCII);

                byte VersionNum = reader.ReadByte();
                if (VersionNum != 0 && Signature == "SPBD")
                    Is3DS = true;

                reader.Position = 0;
                if (Is3DS)
                {
                    reader.ByteOrder = ByteOrder.LittleEndian;
                    header3DS = new PTCL_3DS.Header();
                    header3DS.Read(reader, this);
                }
                else if (Signature == "EFTF" || Signature == "SPBD")
                {
                    IsWiiU = true;
                    headerU = new PTCL_WiiU.Header();
                    headerU.Read(reader, this);
                }
                else
                {
                    header = new Header();
                    header.Read(reader, this);
                }
            }

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S));

            if (!LoadedFiles.Contains(this)) LoadedFiles.Add(this);   // publish this file's resources for cross-file resolution
        }

        public void Unload()
        {
            LoadedFiles.Remove(this);
        }
        public void Save(System.IO.Stream stream)
        {
            if (Is3DS)
                header3DS.Write(new FileWriter(stream), this);
            else if (IsWiiU)
                headerU.Write(new FileWriter(stream), this);
            else if (header != null && header.Signature == "EFTB")
            {
                //The generic Header.Write() is VFXB-only and corrupts EFTB, so build the image from the loaded
                //bytes plus the tool's edits instead.
                byte[] outBytes = BuildEftbBytes();
                stream.Write(outBytes, 0, outBytes.Length);
            }
            else
                header.Write(new FileWriter(stream));
        }

        //Walk the parsed section MODEL (Header.Sections + each section's ChildSections), independent of what is
        //shown in the tree. Save uses this so it still finds sections we hide from the tree (e.g. the redundant
        //per-texture "Texture Info" nodes under TEXA).
        private IEnumerable<SectionBase> CollectSections()
        {
            if (header == null || header.Sections == null) yield break;
            var stack = new Stack<SectionBase>();
            for (int i = header.Sections.Count - 1; i >= 0; i--) stack.Push(header.Sections[i]);
            while (stack.Count > 0)
            {
                var s = stack.Pop();
                yield return s;
                if (s.ChildSections != null)
                    for (int i = s.ChildSections.Count - 1; i >= 0; i--) stack.Push(s.ChildSections[i]);
            }
        }

        //Full EFTB image for the current state: loaded bytes + flushed emitter edits + spliced texture/PRIM
        //replacements. Save writes this, and structural ops reuse it so unsaved edits survive a splice.
        private byte[] BuildEftbBytes()
        {
            var mem = new MemoryStream();
            mem.Write(data, 0, data.Length);
            using (var writer = new FileWriter(mem, true))
            {
                writer.SetByteOrder(true); //EFTB is big endian
                foreach (var node in TreeViewExtensions.Collect(Nodes))
                {
                    if (node is SectionBase && ((SectionBase)node).BinaryData is Emitter)
                    {
                        var emitter = (Emitter)((SectionBase)node).BinaryData;
                        writer.Seek(emitter.DataPosition, SeekOrigin.Begin);
                        emitter.Write(writer, header);
                    }
                }
            }
            byte[] outBytes = mem.ToArray();

            //Gather any replaced textures or PRIM meshes; if present, re-lay-out the file around their new sizes.
            var reps = new List<SectionRep>();
            foreach (var node in CollectSections())   //section MODEL walk (not the tree), so save still finds the hidden TEXR/PRIM sections
            {
                var sec = node as SectionBase;
                if (sec == null) continue;

                if (sec.Signature == "TEXR" && sec.BinaryData is TEXR && ((TEXR)sec.BinaryData).IsReplaced)
                {
                    var gx2b = sec.ChildSections.FirstOrDefault(c => c.Signature == "GX2B");
                    if (gx2b != null && ((TEXR)sec.BinaryData).data != null)
                    {
                        reps.Add(new SectionRep()
                        {
                            Tex = (TEXR)sec.BinaryData,
                            TexrHdr = sec,
                            SizeSection = gx2b,
                            DataPos = (int)(gx2b.Position + gx2b.BinaryDataOffset),
                            OldLen = (int)gx2b.SectionSize,
                            NewBytes = ((TEXR)sec.BinaryData).data,
                        });
                    }
                }
                else if (sec.Signature == "PRIM" && sec.BinaryData is Primitive && ((Primitive)sec.BinaryData).IsReplaced)
                {
                    var prim = (Primitive)sec.BinaryData;
                    if (prim.NewBlock != null)
                    {
                        reps.Add(new SectionRep()
                        {
                            SizeSection = sec,
                            DataPos = (int)(sec.Position + sec.BinaryDataOffset),
                            OldLen = (int)sec.SectionSize,
                            NewBytes = prim.NewBlock,
                        });
                    }
                }
            }
            if (reps.Count > 0)
                outBytes = RebuildSections(outBytes, reps);

            return outBytes;
        }
        //--- Emitter set structural edits (duplicate / add / delete emitters) ----------------------------
        //An EMTR is a self-contained contiguous section (its SectionSize covers its whole subtree) and every
        //offset in the file is relative, so an emitter can be spliced in/out by copying bytes and patching only
        //the counts/sizes/chain links that span the splice point. Algorithm validated on real files.

        //Re-parse the section tree from a freshly built byte image (used after a structural edit). Preserves the
        //tree's expanded folders + selection across the rebuild and batches it under BeginUpdate, so the explorer
        //never flashes empty while a large file (e.g. GameResident.sesetlist) re-parses.
        public void ReloadFromData(byte[] newData)
        {
            var tv = this.TreeView;
            HashSet<string> expanded = null;
            if (tv != null)
            {
                try { expanded = new HashSet<string>(); CaptureExpanded(this, "", expanded); } catch { expanded = null; }
                tv.BeginUpdate();
            }
            try
            {
                data = newData;
                Nodes.Clear();
                header = new Header();
                using (var reader = new FileReader(new MemoryStream(newData)))
                {
                    reader.ByteOrder = ByteOrder.BigEndian;
                    header.Read(reader, this);
                }
                //NOTE: selection is intentionally NOT set here. Setting SelectedNode mid-rebuild fires AfterSelect ->
                //opens the emitter editor + pumps the message loop while we are still nested inside the edit/reparse,
                //which blanked the tree and could crash. Callers set the final selection via SelectSectionPath, which
                //defers it to after this call fully unwinds.
                if (tv != null && expanded != null) { try { RestoreExpanded(expanded); } catch { } }
            }
            finally { if (tv != null) tv.EndUpdate(); }
        }

        //Select a node by section signature + child-index path (each index clamped to range; a short path or an
        //out-of-range index falls back to the nearest valid ancestor). Called after a structural edit to land
        //selection on a sensible node: the new item on add/duplicate, the previous sibling (or parent folder when
        //there is none) on delete, instead of collapsing the view to the root.
        public void SelectSectionPath(string topSig, params int[] childIdx)
        {
            var tv = this.TreeView;
            if (tv == null) return;
            Action doSelect = () =>
            {
                try
                {
                    TreeNode n = null;
                    foreach (TreeNode t in Nodes)
                        if (t is SectionBase sb && sb.Signature == topSig) { n = t; break; }
                    if (n == null) return;
                    foreach (int idx in childIdx)
                    {
                        if (n.Nodes.Count == 0) break;
                        int c = Math.Max(0, Math.Min(idx, n.Nodes.Count - 1));
                        n = n.Nodes[c];
                    }
                    n.EnsureVisible();
                    tv.SelectedNode = n;
                }
                catch { }
            };
            //Defer: run the selection (and the editor it opens) as a fresh top-level event after the current edit
            //fully returns, so it is never nested inside ReloadFromData's rebuild. Mirrors a normal user click.
            try { tv.BeginInvoke(doSelect); } catch { }
        }

        //Record the index-path ("a/b/c") of every EXPANDED node. Only descends into expanded nodes, so the walk is
        //proportional to what is open, not to the whole file.
        private static void CaptureExpanded(TreeNode parent, string prefix, HashSet<string> set)
        {
            for (int i = 0; i < parent.Nodes.Count; i++)
            {
                var c = parent.Nodes[i];
                if (!c.IsExpanded) continue;
                string p = (prefix.Length == 0) ? i.ToString() : prefix + "/" + i;
                set.Add(p);
                CaptureExpanded(c, p, set);
            }
        }
        //Re-expand the recorded paths (shallowest first, so a parent is expanded before its children).
        private void RestoreExpanded(HashSet<string> set)
        {
            if (set == null || set.Count == 0) return;
            var paths = new List<string>(set);
            paths.Sort((a, b) => a.Length - b.Length);
            foreach (var p in paths) { var n = NodeAtPath(this, p); if (n != null) n.Expand(); }
        }
        private static TreeNode NodeAtPath(TreeNode root, string path)
        {
            if (string.IsNullOrEmpty(path)) return root;
            TreeNode n = root;
            foreach (var part in path.Split('/'))
            {
                if (part.Length == 0) continue;
                int i; if (!int.TryParse(part, out i)) return null;
                if (i < 0 || i >= n.Nodes.Count) return null;
                n = n.Nodes[i];
            }
            return n;
        }

        //First top-level section with the given magic (sections chain by NextSectionOffset from 0x30). -1 if none.
        private static int FindTopLevel(byte[] d, string magic)
        {
            int p = 0x30, guard = 0;
            while (guard++ < 1024 && p >= 0 && p + 0x20 <= d.Length)
            {
                if (Encoding.ASCII.GetString(d, p, 4) == magic) return p;
                uint nx = ReadU32BE(d, p + 12);
                if (nx == NullOffset) break;
                p += (int)nx;
            }
            return -1;
        }

        //Locate emitter set #setIndex (child of ESTA) plus the positions/sizes of its emitter children.
        private bool LocateSet(byte[] d, int setIndex, out int estaPos, out int esetPos, out List<int> ePos, out List<int> eSize)
        {
            estaPos = FindTopLevel(d, "ESTA");
            esetPos = -1; ePos = new List<int>(); eSize = new List<int>();
            if (estaPos < 0) return false;

            int c = estaPos + (int)ReadU32BE(d, estaPos + 8);   //first ESET
            for (int si = 0; ; si++)
            {
                if (si == setIndex) { esetPos = c; break; }
                uint nx = ReadU32BE(d, c + 12);
                if (nx == NullOffset) return false;
                c += (int)nx;
            }

            //Emitter SIZE = its FOOTPRINT (the gap to the next sibling = NextSectionOffset), NOT the SectionSize
            //field. An emitter's SectionSize can be smaller than its real span when it owns child sections / trailing
            //padding (8 such emitters exist in GameResident), so splicing by SectionSize removed/cloned too few bytes
            //and corrupted the chain. The last emitter's footprint runs to the end of the ESET's emitter region.
            uint esetNx = ReadU32BE(d, esetPos + 12);
            int esetEnd = (esetNx != NullOffset) ? esetPos + (int)esetNx : esetPos + (int)ReadU32BE(d, esetPos + 4);
            int e = esetPos + (int)ReadU32BE(d, esetPos + 8);   //first EMTR
            int guard = 0;
            while (guard++ < 4096)
            {
                ePos.Add(e);
                uint nx = ReadU32BE(d, e + 12);
                eSize.Add((nx == NullOffset) ? (esetEnd - e) : (int)nx);   //footprint to next sibling (or ESET end)
                if (nx == NullOffset) break;
                e += (int)nx;
            }
            return true;
        }

        //Locate every emitter set (ESET child of ESTA): the ESTA position plus each set's position and size.
        //setPos is empty when the container has no sets (SubSectionOffset == NULL).
        private bool LocateAllSets(byte[] d, out int estaPos, out List<int> setPos, out List<int> setSize)
        {
            estaPos = FindTopLevel(d, "ESTA");
            setPos = new List<int>(); setSize = new List<int>();
            if (estaPos < 0) return false;

            uint sub = ReadU32BE(d, estaPos + 8);
            if (sub == NullOffset) return true;             //empty container (no sets)
            int c = estaPos + (int)sub, guard = 0;
            while (guard++ < 4096)
            {
                setPos.Add(c); setSize.Add((int)ReadU32BE(d, c + 4));
                uint nx = ReadU32BE(d, c + 12);
                if (nx == NullOffset) break;
                c += (int)nx;
            }
            return true;
        }

        private static byte[] SpliceInsert(byte[] s, int at, byte[] block)
        {
            byte[] o = new byte[s.Length + block.Length];
            Array.Copy(s, 0, o, 0, at);
            Array.Copy(block, 0, o, at, block.Length);
            Array.Copy(s, at, o, at + block.Length, s.Length - at);
            return o;
        }
        private static byte[] SpliceRemove(byte[] s, int at, int len)
        {
            byte[] o = new byte[s.Length - len];
            Array.Copy(s, 0, o, 0, at);
            Array.Copy(s, at + len, o, at, s.Length - at - len);
            return o;
        }

        //Wii U GX2 texture data must sit on an aligned file offset; an emitter splice shifts it, so re-pad the gap
        //between ESTA and the first GPU section to realign the texture data. Sampled BotW effect textures sit on a
        //0x1000 boundary; GpuAlign is the single grid value used everywhere (BuildTexaBlock + RebuildSections too)
        //and is kept at 0x2000 as a safe superset of the observed 0x1000 (PRMA/mesh is not alignment-critical).
        private const int GpuAlign = 0x2000;
        private static int IndexOfMagic(byte[] d, string magic, int start)
        {
            byte[] m = Encoding.ASCII.GetBytes(magic);
            for (int i = Math.Max(0, start); i + 4 <= d.Length; i++)
                if (d[i] == m[0] && d[i + 1] == m[1] && d[i + 2] == m[2] && d[i + 3] == m[3]) return i;
            return -1;
        }
        private byte[] AlignTextureGap(byte[] o)
        {
            int esta = FindTopLevel(o, "ESTA");
            if (esta < 0) return o;
            uint estaNext = ReadU32BE(o, esta + 12);
            if (estaNext == NullOffset) return o;
            int estaEnd = esta + (int)ReadU32BE(o, esta + 4);
            int texaPos = esta + (int)estaNext;

            int gx2b = IndexOfMagic(o, "GX2B", texaPos);
            if (gx2b < 0) return o;                       //no texture -> nothing alignment-critical follows
            int texOff = gx2b + (int)ReadU32BE(o, gx2b + 0x14);
            int texRel = texOff - texaPos;                //texture-data offset relative to TEXA (invariant)

            int currentGap = texaPos - estaEnd;
            int desiredGap = (GpuAlign - ((estaEnd + texRel) % GpuAlign)) % GpuAlign;
            int gapDelta = desiredGap - currentGap;
            if (gapDelta == 0) return o;

            byte[] r = gapDelta > 0
                ? SpliceInsert(o, texaPos, new byte[gapDelta])      //grow the gap
                : SpliceRemove(o, texaPos + gapDelta, -gapDelta);   //trim padding from the gap's tail
            WriteU32BE(r, esta + 12, ReadU32BE(r, esta + 12) + (uint)gapDelta); //ESTA -> TEXA moved by gapDelta
            return r;
        }

        private static string ReadEmitterName(byte[] d, int emtrPos)
        {
            int binOff = (int)ReadU32BE(d, emtrPos + 0x14);
            int o = emtrPos + binOff + 0x10;
            var sb = new StringBuilder();
            while (o < d.Length && d[o] != 0 && sb.Length < 63) { sb.Append((char)d[o]); o++; }
            return sb.ToString();
        }
        //The emitter name occupies the 0x40 bytes at EMTR + BinaryDataOffset + 0x10 (the struct follows at +0x50).
        private static void WriteEmitterName(byte[] d, int emtrPos, string name)
        {
            int binOff = (int)ReadU32BE(d, emtrPos + 0x14);
            int o = emtrPos + binOff + 0x10;
            for (int i = 0; i < 0x40; i++) d[o + i] = 0;
            byte[] nm = Encoding.ASCII.GetBytes(name);
            Array.Copy(nm, 0, d, o, Math.Min(nm.Length, 0x3F));
        }
        private static string UniqueName(string want, List<string> existing)
        {
            if (!existing.Contains(want)) return want;
            for (int i = 2; i < 1000; i++) { string c = want + i; if (!existing.Contains(c)) return c; }
            return want;
        }

        //The emitter-set name is a fixed 0x40-byte field at ESET +0x30; nw::eft numEmitter sits right after it
        //at +0x70, so only the first 0x40 bytes are touched.
        private static string ReadSetName(byte[] d, int esetPos)
        {
            int o = esetPos + 0x30;
            var sb = new StringBuilder();
            while (o < d.Length && d[o] != 0 && sb.Length < 63) { sb.Append((char)d[o]); o++; }
            return sb.ToString();
        }
        private static void WriteSetName(byte[] d, int esetPos, string name)
        {
            int o = esetPos + 0x30;
            for (int i = 0; i < 0x40; i++) d[o + i] = 0;
            byte[] nm = Encoding.ASCII.GetBytes(name);
            Array.Copy(nm, 0, d, o, Math.Min(nm.Length, 0x3F));
        }

        //Duplicate emitter #emtrIndex of set #setIndex, inserting the copy right after it.
        public void DuplicateEmitterAt(int setIndex, int emtrIndex)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (emtrIndex < 0 || emtrIndex >= ePos.Count) return;

            int anchor = ePos[emtrIndex], L = eSize[emtrIndex], P = anchor + L;
            byte[] clone = new byte[L];
            Array.Copy(src, anchor, clone, 0, L);
            byte[] o = SpliceInsert(src, P, clone);

            WriteU16BE(o, esetPos + 0x1C, (ushort)(ReadU16BE(o, esetPos + 0x1C) + 1)); //ESET child count
            WriteU32BE(o, esetPos + EsetNumEmitterOff, ReadU32BE(o, esetPos + EsetNumEmitterOff) + 1);            //nw::eft numEmitter (game-side count)
            WriteU32BE(o, esetPos + 4, ReadU32BE(o, esetPos + 4) + (uint)L);            //ESET size
            uint esetNext = ReadU32BE(o, esetPos + 12);   //ESET -> next ESET link grows by L (the next set shifted down)
            if (esetNext != NullOffset && esetPos + esetNext >= P) WriteU32BE(o, esetPos + 12, esetNext + (uint)L);
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)L);            //ESTA size
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)L);

            uint anchorNext = ReadU32BE(src, anchor + 12);  //what the original pointed to (END or next sibling)
            WriteU32BE(o, anchor + 12, (uint)L);            //anchor -> clone
            WriteU32BE(o, P + 12, anchorNext);              //clone -> anchor's old successor

            var names = new List<string>();
            for (int i = 0; i < ePos.Count; i++) names.Add(ReadEmitterName(src, ePos[i]));
            WriteEmitterName(o, P, UniqueName(ReadEmitterName(src, anchor) + "Copy", names));

            ReloadFromData(AlignTextureGap(o));
        }

        //Remove emitter #emtrIndex of set #setIndex (a set must keep at least one emitter).
        public void DeleteEmitterAt(int setIndex, int emtrIndex)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (emtrIndex < 0 || emtrIndex >= ePos.Count) return;
            if (ePos.Count <= 1) { MessageBox.Show("An emitter set must keep at least one emitter."); return; }

            int P = ePos[emtrIndex], L = eSize[emtrIndex];
            byte[] o = SpliceRemove(src, P, L);

            WriteU16BE(o, esetPos + 0x1C, (ushort)(ReadU16BE(o, esetPos + 0x1C) - 1));
            WriteU32BE(o, esetPos + EsetNumEmitterOff, ReadU32BE(o, esetPos + EsetNumEmitterOff) - 1); //nw::eft numEmitter (game-side count)
            WriteU32BE(o, esetPos + 4, ReadU32BE(o, esetPos + 4) - (uint)L);
            uint esetNext = ReadU32BE(o, esetPos + 12);   //ESET -> next ESET link: the next set shifted up by L, so shrink it too
            if (esetNext != NullOffset && esetPos + esetNext >= P + L) WriteU32BE(o, esetPos + 12, esetNext - (uint)L);
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) - (uint)L);
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P + L) WriteU32BE(o, estaPos + 12, estaNext - (uint)L);

            if (emtrIndex == ePos.Count - 1 && emtrIndex > 0)
                WriteU32BE(o, ePos[emtrIndex - 1] + 12, NullOffset); //previous emitter becomes the last

            ReloadFromData(AlignTextureGap(o));
        }

        //Emitter-struct layout. EmitterData begins at EMTR + BinaryDataOffset + EmtStructOff; the primitive-mesh
        //descriptor (enable flag / paired field / hash) lives at these offsets within it.
        private const int EmtStructOff      = 0x50;
        private const int EmtPrimEnableOff  = 0x874; //"uses primitive" flag
        private const int EmtPrimUnkOff     = 0x878; //paired field (0 when a primitive is linked, else 0xFFFFFFFF)
        private const int EmtPrimHashOff    = 0x87C; //primitive hash (matches a PRIM block's +0x04 id)
        private const int EmtShaderVtxOff   = 0x8C4; //emitter's vertex-shader index into the SHDA GFD bundle
        private const int EmtShaderFrgOff   = 0x8C8; //emitter's fragment-shader index into the SHDA GFD bundle
        private const uint EmtPrimEnableVal = 0x13;  //value written to EmtPrimEnableOff when a primitive is linked
        private const int EsetNumEmitterOff = 0x70;  //nw::eft numEmitter (game-side count), ESET-relative

        //Texture samplers: nw::eft gives every emitter a FIXED array of 3 texture slots (Texture0/1/2), each a
        //0x20-byte struct, sampled together on whatever geometry the emitter draws (billboard or primitive) and
        //blended by the shader. Slot 0 is the base offset for EFTB (VFXVersion 0 -> the 2472/0x9A8 seek in
        //Emitter.Read); each emitter caches the exact base it parsed in SamplerBaseOffset.
        private const int EmtSamplerStride  = 0x20;  //bytes per sampler
        private const int EmtSamplerCount   = 3;     //Texture0, Texture1, Texture2

        //Read / clear an emitter's primitive-mesh reference.
        private static uint EmitterPrimHash(byte[] d, int emtrPos)
        {
            int o = emtrPos + (int)ReadU32BE(d, emtrPos + 0x14) + EmtStructOff + EmtPrimHashOff;
            return (o + 4 <= d.Length) ? ReadU32BE(d, o) : 0xFFFFFFFF;
        }
        private static void ClearEmitterPrim(byte[] d, int emtrPos)
        {
            int e = emtrPos + (int)ReadU32BE(d, emtrPos + 0x14) + EmtStructOff;
            if (e + EmtPrimHashOff + 4 > d.Length) return;
            WriteU32BE(d, e + EmtPrimEnableOff, 0);          //"uses primitive" flag off
            WriteU32BE(d, e + EmtPrimUnkOff, 0xFFFFFFFF);
            WriteU32BE(d, e + EmtPrimHashOff, 0xFFFFFFFF);   //no primitive hash
        }

        //Add a new emitter to set #setIndex with the given name. Seeded from a primitive-free emitter (so the
        //new one draws no mesh by default), and its primitive descriptor is cleared as a safeguard.
        public void AddEmitterTo(int setIndex, string name)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (ePos.Count == 0) return;

            //prefer a template that already draws no primitive; fall back to the first emitter.
            int tIdx = 0;
            for (int i = 0; i < ePos.Count; i++)
            {
                uint h = EmitterPrimHash(src, ePos[i]);
                if (h == 0 || h == 0xFFFFFFFF) { tIdx = i; break; }
            }
            int template = ePos[tIdx], Lt = eSize[tIdx];
            int last = ePos[ePos.Count - 1], Ls = eSize[ePos.Count - 1], P = last + Ls;
            byte[] clone = new byte[Lt];
            Array.Copy(src, template, clone, 0, Lt);
            byte[] o = SpliceInsert(src, P, clone);

            WriteU16BE(o, esetPos + 0x1C, (ushort)(ReadU16BE(o, esetPos + 0x1C) + 1));
            WriteU32BE(o, esetPos + EsetNumEmitterOff, ReadU32BE(o, esetPos + EsetNumEmitterOff) + 1); //nw::eft numEmitter (game-side count)
            WriteU32BE(o, esetPos + 4, ReadU32BE(o, esetPos + 4) + (uint)Lt);
            uint esetNext = ReadU32BE(o, esetPos + 12);   //ESET -> next ESET link grows by Lt (the next set shifted down)
            if (esetNext != NullOffset && esetPos + esetNext >= P) WriteU32BE(o, esetPos + 12, esetNext + (uint)Lt);
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)Lt);
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)Lt);

            WriteU32BE(o, last + 12, (uint)Ls); //old last emitter -> new emitter
            WriteU32BE(o, P + 12, NullOffset);  //new emitter is the last

            ClearEmitterPrim(o, P); //no primitive link regardless of the template

            var names = new List<string>();
            for (int i = 0; i < ePos.Count; i++) names.Add(ReadEmitterName(src, ePos[i]));
            string want = string.IsNullOrEmpty(name) ? "NewEmitter" : name;
            WriteEmitterName(o, P, UniqueName(want, names));

            ReloadFromData(AlignTextureGap(o));
        }

        //Rename emitter #emtrIndex of set #setIndex. The name field is fixed-size, so no splice is needed.
        public void RenameEmitterAt(int setIndex, int emtrIndex, string name)
        {
            if (header == null || header.Signature != "EFTB" || string.IsNullOrEmpty(name)) return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (emtrIndex < 0 || emtrIndex >= ePos.Count) return;
            WriteEmitterName(src, ePos[emtrIndex], name);
            ReloadFromData(src);
        }

        //Rename emitter set #setIndex. The set name is a fixed 64-byte field at ESET +0x30; the emitter count
        //(nw::eft numEmitter) sits right after it at +0x70, so clearing only 0x40 bytes leaves it intact.
        public void RenameSetAt(int setIndex, string name)
        {
            if (header == null || header.Signature != "EFTB" || string.IsNullOrEmpty(name)) return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            WriteSetName(src, esetPos, name);
            ReloadFromData(src);
        }

        //--- Whole emitter-SET structural edits (duplicate / add / delete an ESET under ESTA) -------------
        //An ESET is contiguous (its SectionSize covers its entire emitter subtree) and ESTA is its only
        //ancestor, so a set splices in/out exactly like an emitter but with a shorter patch list: ESTA's set
        //count is the only count (no ESET+0x70-style redundant field), and ESTA is the only size to bubble.
        //All resources are referenced into the shared PRMA/TEXA/SHDA tables, so those never change.

        //Duplicate emitter set #setIndex, inserting the copy right after it.
        public void DuplicateEmitterSetAt(int setIndex)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos; List<int> setPos, setSize;
            if (!LocateAllSets(src, out estaPos, out setPos, out setSize)) return;
            if (setIndex < 0 || setIndex >= setPos.Count) return;

            int anchor = setPos[setIndex], L = setSize[setIndex], P = anchor + L;
            byte[] clone = new byte[L];
            Array.Copy(src, anchor, clone, 0, L);
            byte[] o = SpliceInsert(src, P, clone);

            WriteU16BE(o, estaPos + 0x1C, (ushort)(ReadU16BE(o, estaPos + 0x1C) + 1)); //ESTA set count
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)L);            //ESTA size
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)L);

            uint anchorNext = ReadU32BE(src, anchor + 12);  //END or next set
            WriteU32BE(o, anchor + 12, (uint)L);            //anchor -> clone
            WriteU32BE(o, P + 12, anchorNext);              //clone -> anchor's old successor

            var names = new List<string>();
            for (int i = 0; i < setPos.Count; i++) names.Add(ReadSetName(src, setPos[i]));
            WriteSetName(o, P, UniqueName(ReadSetName(src, anchor) + "Copy", names));

            ReloadFromData(AlignTextureGap(o));
        }

        //Build a one-emitter ESET template from set 0 of src: its header+params plus only its first emitter,
        //counts forced to 1, sibling/child chains terminated, primitive link cleared. Returns null if no sets.
        private byte[] BuildTemplateFromSet0(byte[] src, List<int> setPos)
        {
            if (setPos.Count == 0) return null;
            int tset = setPos[0];
            int tsub = (int)ReadU32BE(src, tset + 8);   //ESET header+params length (offset to first emitter)
            int e0 = tset + tsub;                       //first emitter
            int Le0 = (int)ReadU32BE(src, e0 + 4);      //first emitter size

            byte[] t = new byte[tsub + Le0];
            Array.Copy(src, tset, t, 0, tsub);          //set header + params + name field + numEmitter
            Array.Copy(src, e0, t, tsub, Le0);          //its first emitter only
            WriteU32BE(t, 4, (uint)(tsub + Le0));       //ESET size
            WriteU16BE(t, 0x1C, 1);                     //SubSectionCount (parser-side) = 1 emitter
            WriteU32BE(t, EsetNumEmitterOff, 1);        //numEmitter (game-side) = 1
            WriteU32BE(t, 0x0C, NullOffset);            //no next set (set when appended)
            WriteU32BE(t, tsub + 0x0C, NullOffset);     //the lone emitter is the last emitter
            ClearEmitterPrim(t, tsub);                  //starter emitter draws no mesh
            return t;
        }

        //Add a new emitter set, cloned from set 0 trimmed to one emitter so its texture/shader references stay valid
        //for this file. Refuses (like AddEmitterTo) when there is no set to clone from.
        public void AddEmitterSet(string name)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos; List<int> setPos, setSize;
            if (!LocateAllSets(src, out estaPos, out setPos, out setSize)) return;

            byte[] newset = BuildTemplateFromSet0(src, setPos);
            if (newset == null)   //no set to clone from (e.g. after deleting them all)
            {
                MessageBox.Show(Runtime.MainForm, "This file has no emitter set to copy from. Open a .sesetlist " +
                    "that already has an emitter set and duplicate it (or add the set there).",
                    "Add Emitter Set", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int L = newset.Length;

            int last = setPos[setPos.Count - 1], Ls = setSize[setPos.Count - 1];
            int P = last + Ls;                                 //append after the current last set
            byte[] o = SpliceInsert(src, P, newset);
            WriteU16BE(o, estaPos + 0x1C, (ushort)(ReadU16BE(o, estaPos + 0x1C) + 1));
            WriteU32BE(o, last + 12, (uint)Ls);                //old last set -> new set
            WriteU32BE(o, P + 12, NullOffset);                 //new set is the last

            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)L); //ESTA size
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)L);

            var names = new List<string>();
            for (int i = 0; i < setPos.Count; i++) names.Add(ReadSetName(src, setPos[i]));
            string want = string.IsNullOrEmpty(name) ? "NewEmitterSet" : name;
            WriteSetName(o, P, UniqueName(want, names));

            //Give the lone emitter a fixed name instead of inheriting set 0's first emitter name.
            WriteEmitterName(o, P + (int)ReadU32BE(o, P + 8), "NewEmitter");

            ReloadFromData(AlignTextureGap(o));
        }

        //Remove emitter set #setIndex. No "keep at least one set" guard by request: deleting the last set leaves
        //an empty ESTA (useless in-game but valid to the tool). When the count hits 0 the container's child
        //pointer MUST be nulled, or the parser's count==0 quirk reads a phantom self-referential section and
        //stack-overflows; see ReadSectionData's tempCount handling.
        public void DeleteEmitterSetAt(int setIndex)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos; List<int> setPos, setSize;
            if (!LocateAllSets(src, out estaPos, out setPos, out setSize)) return;
            if (setIndex < 0 || setIndex >= setPos.Count) return;

            int P = setPos[setIndex], L = setSize[setIndex];
            byte[] o = SpliceRemove(src, P, L);

            WriteU16BE(o, estaPos + 0x1C, (ushort)(ReadU16BE(o, estaPos + 0x1C) - 1)); //ESTA set count
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) - (uint)L);           //ESTA size
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P + L) WriteU32BE(o, estaPos + 12, estaNext - (uint)L);

            if (setPos.Count == 1)                                       //deleted the only set -> empty ESTA
                WriteU32BE(o, estaPos + 8, NullOffset);                  //null child pointer (parser-crash fix)
            else if (setIndex == setPos.Count - 1)                       //deleted the last of several
                WriteU32BE(o, setPos[setIndex - 1] + 12, NullOffset);    //previous set becomes the last

            ReloadFromData(AlignTextureGap(o));
        }

        //--- Shared texture-table edits (create / delete a texture in TEXA) ------------------------------
        //TEXA holds, in order: a 0x20 section header, then one 0x50 TEXR descriptor per texture (contiguous),
        //then one GX2B data block per texture. Each TEXR points (SubSectionOffset) at its own GX2B; the GX2B
        //blocks form one shared chain. Because adding/removing a descriptor (0x50) and a data block both shift
        //the GX2-aligned payloads, the whole table is re-laid-out from scratch rather than surgically spliced.
        //Emitters bind textures by the TextureID hash in their samplers (NOT by table index), so create mints a
        //unique id (orphan until wired to an emitter) and delete warns if the id is still referenced.
        private const int TexrFootprint = 0x50;  //section header (0x20) + descriptor struct (0x30)
        private const int SecHeader = 0x20;
        private const int DescSize = 0x30;        //TEXR descriptor: W,H @0; Mip @0x0C; Tile @0x14; ImageSize @0x1C; TexID @0x24; Fmt @0x28

        private class TexEntry { public byte[] Desc; public byte[] Data; } //Desc = 0x30 descriptor struct, Data = raw GX2B payload

        //Parse the shared texture table from a raw file image. Returns false if there is no TEXA section.
        //texaPos/oldFootprint locate the block to replace; entries lists the textures in file order.
        private static bool ParseTexa(byte[] d, out int texaPos, out int oldFootprint, out List<TexEntry> entries)
        {
            entries = new List<TexEntry>(); oldFootprint = 0;
            texaPos = FindTopLevel(d, "TEXA");
            if (texaPos < 0) return false;

            uint tnext = ReadU32BE(d, texaPos + 12);
            oldFootprint = (tnext == NullOffset) ? d.Length - texaPos : (int)tnext;

            int count = ReadU16BE(d, texaPos + 0x1C);
            uint sub = ReadU32BE(d, texaPos + 8);
            if (sub == NullOffset || count == 0) return true;          //valid but empty table

            int t = texaPos + (int)sub;
            for (int i = 0; i < count; i++)
            {
                var e = new TexEntry();
                e.Desc = new byte[DescSize];
                Array.Copy(d, t + SecHeader, e.Desc, 0, DescSize);     //the 0x30 descriptor struct
                int gx = t + (int)ReadU32BE(d, t + 8);                 //SubSectionOffset -> this texture's GX2B
                int gsize = (int)ReadU32BE(d, gx + 4);                 //GX2B section size == payload length
                int gbin = (int)ReadU32BE(d, gx + 0x14);               //GX2B BinaryDataOffset (alignment pad)
                e.Data = new byte[gsize];
                Array.Copy(d, gx + gbin, e.Data, 0, gsize);
                entries.Add(e);

                uint nx = ReadU32BE(d, t + 12);
                if (nx == NullOffset) break;
                t += (int)nx;
            }
            return true;
        }

        private static int AlignUp(int v, int a) { int r = v % a; return r == 0 ? v : v + (a - r); }
        private static void WriteSig(byte[] d, int o, string s) { for (int i = 0; i < 4; i++) d[o + i] = (byte)s[i]; }

        //Build a complete, self-consistent TEXA section for the given textures. Data payloads are placed on a
        //TEXA-relative 0x2000 grid so AlignTextureGap (which only aligns the first block) keeps them all aligned.
        private static byte[] BuildTexaBlock(List<TexEntry> entries, bool texaWasLast)
        {
            int n = entries.Count;
            int descBase = SecHeader;
            var gxPos = new int[n];
            var binOff = new int[n];

            int pos = descBase + n * TexrFootprint;        //first GX2B header sits right after the descriptors
            for (int i = 0; i < n; i++)
            {
                gxPos[i] = pos;
                int dataPos = AlignUp(pos + SecHeader, GpuAlign);
                binOff[i] = dataPos - pos;
                pos = dataPos + entries[i].Data.Length;
            }
            int footprint = pos;
            byte[] o = new byte[footprint];

            //TEXA header
            WriteSig(o, 0, "TEXA");
            WriteU32BE(o, 4, (uint)(footprint - SecHeader));
            WriteU32BE(o, 8, n > 0 ? (uint)SecHeader : NullOffset);             //-> first TEXR (NULL when empty)
            WriteU32BE(o, 12, texaWasLast ? NullOffset : (uint)footprint);      //-> following top-level section (PRMA)
            WriteU32BE(o, 0x10, NullOffset);
            WriteU32BE(o, 0x14, n > 0 ? (uint)SecHeader : NullOffset);          //empty table uses a NULL BDO
            WriteU32BE(o, 0x18, 0);
            WriteU16BE(o, 0x1C, (ushort)n);                                     //texture count
            WriteU16BE(o, 0x1E, 1);

            for (int i = 0; i < n; i++)
            {
                int tp = descBase + i * TexrFootprint;
                WriteSig(o, tp, "TEXR");
                WriteU32BE(o, tp + 4, (uint)DescSize);
                WriteU32BE(o, tp + 8, (uint)(gxPos[i] - tp));                   //-> own GX2B
                WriteU32BE(o, tp + 12, i < n - 1 ? (uint)TexrFootprint : NullOffset);
                WriteU32BE(o, tp + 0x10, NullOffset);
                WriteU32BE(o, tp + 0x14, (uint)SecHeader);                      //-> descriptor struct
                WriteU32BE(o, tp + 0x18, 0);
                WriteU16BE(o, tp + 0x1C, (ushort)(n - i));                      //blocks remaining in the chain (n,n-1,...,1)
                WriteU16BE(o, tp + 0x1E, 1);
                Array.Copy(entries[i].Desc, 0, o, tp + SecHeader, DescSize);

                int gp = gxPos[i];
                WriteSig(o, gp, "GX2B");
                WriteU32BE(o, gp + 4, (uint)entries[i].Data.Length);
                WriteU32BE(o, gp + 8, NullOffset);                             //GX2B has no children
                WriteU32BE(o, gp + 12, i < n - 1 ? (uint)(gxPos[i + 1] - gp) : NullOffset); //chain -> next block
                WriteU32BE(o, gp + 0x10, NullOffset);
                WriteU32BE(o, gp + 0x14, (uint)binOff[i]);                     //-> aligned payload
                WriteU32BE(o, gp + 0x18, 0);
                WriteU16BE(o, gp + 0x1C, 0);
                WriteU16BE(o, gp + 0x1E, 1);
                Array.Copy(entries[i].Data, 0, o, gp + binOff[i], entries[i].Data.Length);
            }
            return o;
        }

        //Replace the whole TEXA region in src with newTexa and re-parse. ESTA->TEXA is unchanged (TEXA start is
        //fixed); TEXA->PRMA and everything after are carried by relative offsets, so only the splice + realign run.
        private void SpliceTexa(byte[] src, int texaPos, int oldFootprint, byte[] newTexa)
        {
            byte[] o = new byte[src.Length - oldFootprint + newTexa.Length];
            Array.Copy(src, 0, o, 0, texaPos);
            Array.Copy(newTexa, 0, o, texaPos, newTexa.Length);
            Array.Copy(src, texaPos + oldFootprint, o, texaPos + newTexa.Length,
                       src.Length - texaPos - oldFootprint);
            ReloadFromData(AlignTextureGap(o));
        }

        //Deterministic unique 32-bit id (FNV-1a of the source name), avoiding 0/0xFFFFFFFF and existing ids.
        private static uint NewUniqueId(string seedName, List<uint> existing)
        {
            uint h = 2166136261u;
            foreach (char c in (seedName ?? "tex")) { h ^= c; h *= 16777619u; }
            while (h == 0 || h == NullOffset || existing.Contains(h)) h++;
            return h;
        }

        //Add a new texture, importing the image with the same GX2 pipeline as Replace. Seeded from texture 0's
        //descriptor so the component selector / unknown fields stay valid; only size/format/id are overwritten.
        //The new texture is an orphan (no emitter references it) until a sampler's TextureID is pointed at it.
        public void AddTextureFromImage(string imageFile)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int texaPos, oldFootprint; List<TexEntry> entries;
            if (!ParseTexa(src, out texaPos, out oldFootprint, out entries) || texaPos < 0)
            { MessageBox.Show("This file has no TEXA texture table at all; adding a texture isn't supported."); return; }

            try
            {
                bool fromScratch = entries.Count == 0;          //empty table -> no donor descriptor to clone

                var t = new TEXR();
                t.TileMode = fromScratch ? 4u : ReadU32BE(entries[0].Desc, 0x14); //4 = the tiling every BotW effect texture uses
                t.Replace(imageFile);                          //fills data, Width, Height, ImageSize, MipCount, TileMode, SurfFormat
                if (t.data == null || t.data.Length == 0) { MessageBox.Show("Image import produced no texture data."); return; }

                var ids = new List<uint>();
                foreach (var e in entries) ids.Add(ReadU32BE(e.Desc, 0x24));
                uint newId = NewUniqueId(imageFile, ids);

                byte[] desc = fromScratch ? SynthesizeDescriptor(t, newId) : CloneDescriptor(entries[0].Desc, t, newId);

                entries.Add(new TexEntry() { Desc = desc, Data = t.data });
                SpliceTexa(src, texaPos, oldFootprint,
                           BuildTexaBlock(entries, ReadU32BE(src, texaPos + 12) == NullOffset));
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show("Create texture failed: " + ex.Message); }
        }

        //Descriptor fields that come straight from the imported surface (shared by the clone and synthesize paths).
        //Offsets are within the 0x30 TEXR descriptor struct.
        private static void SetDescriptorImportFields(byte[] desc, TEXR t, uint id)
        {
            WriteU16BE(desc, 0x00, (ushort)t.Width);
            WriteU16BE(desc, 0x02, (ushort)t.Height);
            WriteU32BE(desc, 0x08, CompSelForFormat(t.SurfFormat)); //channel selector (format-dependent)
            WriteU32BE(desc, 0x0C, t.MipCount);
            WriteU32BE(desc, 0x10, Gx2FormatCode(t.SurfFormat));    //GX2 surface-format code the game decodes from
            WriteU32BE(desc, 0x14, t.TileMode);
            WriteU32BE(desc, 0x1C, t.ImageSize);
            WriteU32BE(desc, 0x24, id);
            desc[0x28] = (byte)t.SurfFormat;
        }

        //Clone a donor descriptor and retarget it. Keeps the donor's unknown fields, but size, GX2 format code
        //(@0x10) and CompSel (@0x08) are all set from the imported surface (via SetDescriptorImportFields), so a
        //clone whose format differs from the donor stays internally consistent instead of misdecoding in-game.
        private static byte[] CloneDescriptor(byte[] template, TEXR t, uint id)
        {
            byte[] desc = (byte[])template.Clone();
            SetDescriptorImportFields(desc, t, id);
            return desc;
        }

        //Build a descriptor with no donor (first texture into an empty table). SetDescriptorImportFields already
        //writes the format-dependent CompSel (@0x08) and GX2 format code (@0x10); the only extra here is the
        //constant unk@0x04 = 1 seen across every sampled BotW texture (the rest stay 0).
        private static byte[] SynthesizeDescriptor(TEXR t, uint id)
        {
            byte[] desc = new byte[DescSize];
            SetDescriptorImportFields(desc, t, id);
            WriteU32BE(desc, 0x04, 1);
            return desc;
        }

        //GX2 surface-format code stored at descriptor +0x10 (BC4_UNORM=0x34, BC5_UNORM=0x35, BC5_SNORM=0x235, ...).
        private static uint Gx2FormatCode(TEXR.SurfaceFormat f)
        {
            switch (f)
            {
                case TEXR.SurfaceFormat.T_BC1_UNORM: return (uint)GX2.GX2SurfaceFormat.T_BC1_UNORM;
                case TEXR.SurfaceFormat.T_BC1_SRGB:  return (uint)GX2.GX2SurfaceFormat.T_BC1_SRGB;
                case TEXR.SurfaceFormat.T_BC2_UNORM: return (uint)GX2.GX2SurfaceFormat.T_BC2_UNORM;
                case TEXR.SurfaceFormat.T_BC2_SRGB:  return (uint)GX2.GX2SurfaceFormat.T_BC2_SRGB;
                case TEXR.SurfaceFormat.T_BC3_UNORM: return (uint)GX2.GX2SurfaceFormat.T_BC3_UNORM;
                case TEXR.SurfaceFormat.T_BC3_SRGB:  return (uint)GX2.GX2SurfaceFormat.T_BC3_SRGB;
                case TEXR.SurfaceFormat.T_BC4_UNORM: return (uint)GX2.GX2SurfaceFormat.T_BC4_UNORM;
                case TEXR.SurfaceFormat.T_BC4_SNORM: return (uint)GX2.GX2SurfaceFormat.T_BC4_SNORM;
                case TEXR.SurfaceFormat.T_BC5_UNORM: return (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;
                case TEXR.SurfaceFormat.T_BC5_SNORM: return (uint)GX2.GX2SurfaceFormat.T_BC5_SNORM;
                case TEXR.SurfaceFormat.TC_R8_UNORM: return (uint)GX2.GX2SurfaceFormat.TC_R8_UNORM;
                case TEXR.SurfaceFormat.TC_R8_G8_UNORM: return (uint)GX2.GX2SurfaceFormat.TC_R8_G8_UNORM;
                case TEXR.SurfaceFormat.TCS_R8_G8_B8_A8_UNORM:
                case TEXR.SurfaceFormat.TCS_R8_G8_B8_A8: return (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                case TEXR.SurfaceFormat.TCS_R5_G6_B5_UNORM: return (uint)GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNORM;
                default: return (uint)GX2.GX2SurfaceFormat.T_BC1_UNORM;
            }
        }

        //Packed RGBA component selector stored at descriptor +0x08. Values match those BotW uses for the common
        //single/dual-channel effect formats; full-colour formats fall back to identity R,G,B,A.
        private static uint CompSelForFormat(TEXR.SurfaceFormat f)
        {
            switch (f)
            {
                case TEXR.SurfaceFormat.T_BC4_UNORM:
                case TEXR.SurfaceFormat.T_BC4_SNORM: return 0x00000000; //R,R,R,R
                case TEXR.SurfaceFormat.T_BC5_UNORM: return 0x00000001; //R,R,R,G
                case TEXR.SurfaceFormat.T_BC5_SNORM: return 0x00010405; //R,G,0,1
                default: return 0x00010203;                             //R,G,B,A
            }
        }

        //Remove the texture whose descriptor carries this id. Matched by id (not list position) so it is robust
        //to display ordering. Warns first when emitters still reference the id.
        public void DeleteTextureById(uint textureId)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int texaPos, oldFootprint; List<TexEntry> entries;
            if (!ParseTexa(src, out texaPos, out oldFootprint, out entries) || texaPos < 0) return;

            int k = entries.FindIndex(e => ReadU32BE(e.Desc, 0x24) == textureId);
            if (k < 0) { MessageBox.Show("Texture not found in the table."); return; }
            //Deleting the last texture is allowed (empty TEXA is valid, and Add Texture seeds a fresh descriptor).

            int refs; var refNames = EmittersReferencing(true, 5, out refs, textureId);
            if (refs > 0)
            {
                string list = "\n  " + string.Join("\n  ", refNames) + (refs > refNames.Count ? "\n  ..." : "");
                if (MessageBox.Show(Runtime.MainForm,
                        $"This texture is referenced by {refs} emitter(s); they will lose their texture:{list}\n\nDelete anyway?",
                        "Delete Texture", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            try
            {
                entries.RemoveAt(k);
                SpliceTexa(src, texaPos, oldFootprint,
                           BuildTexaBlock(entries, ReadU32BE(src, texaPos + 12) == NullOffset));
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show("Delete texture failed: " + ex.Message); }
        }

        //--- Shared primitive-table edits (create / delete a primitive in PRMA) --------------------------
        //PRMA holds one PRIM section per mesh: each PRIM is a single contiguous section (0x20 header + the mesh
        //block at BinaryDataOffset 0x20), chained by NextSectionOffset, with no sub-blocks or GX2-style alignment.
        //PRMA sits after TEXA, so editing it never moves the (alignment-critical) texture data. Emitters bind a
        //primitive by the 32-bit hash at mesh-block +0x04 (EmitterData +0x87C), so create mints a unique hash
        //(assign it to an emitter via the editor's Primitive Mesh dropdown) and delete warns if it's referenced.

        private static bool ParsePrma(byte[] d, out int prmaPos, out int oldFootprint, out List<byte[]> blocks)
        {
            blocks = new List<byte[]>(); oldFootprint = 0;
            prmaPos = FindTopLevel(d, "PRMA");
            if (prmaPos < 0) return false;

            uint pnext = ReadU32BE(d, prmaPos + 12);
            oldFootprint = (pnext == NullOffset) ? d.Length - prmaPos : (int)pnext;

            int count = ReadU16BE(d, prmaPos + 0x1C);
            uint sub = ReadU32BE(d, prmaPos + 8);
            if (sub == NullOffset || count == 0) return true;          //valid but empty table

            int p = prmaPos + (int)sub;
            for (int i = 0; i < count; i++)
            {
                int b = p + (int)ReadU32BE(d, p + 0x14);               //BinaryDataOffset -> mesh block
                int sz = (int)ReadU32BE(d, p + 4);                     //SectionSize == mesh-block length
                byte[] blk = new byte[sz];
                Array.Copy(d, b, blk, 0, sz);
                blocks.Add(blk);

                uint nx = ReadU32BE(d, p + 12);
                if (nx == NullOffset) break;
                p += (int)nx;
            }
            return true;
        }

        //Lay out a complete PRMA section: 0x20 header, then one PRIM (0x20 header + mesh block) per primitive.
        private static byte[] BuildPrmaBlock(List<byte[]> blocks, bool prmaWasLast)
        {
            int n = blocks.Count;
            var primPos = new int[n];
            int pos = SecHeader;
            for (int i = 0; i < n; i++) { primPos[i] = pos; pos += SecHeader + blocks[i].Length; }
            int footprint = pos;
            byte[] o = new byte[footprint];

            WriteSig(o, 0, "PRMA");
            WriteU32BE(o, 4, (uint)(footprint - SecHeader));
            WriteU32BE(o, 8, n > 0 ? (uint)SecHeader : NullOffset);
            WriteU32BE(o, 12, prmaWasLast ? NullOffset : (uint)footprint);
            WriteU32BE(o, 0x10, NullOffset);
            WriteU32BE(o, 0x14, n > 0 ? (uint)SecHeader : NullOffset);   //empty PRMA uses a NULL BDO (matches the wild)
            WriteU32BE(o, 0x18, 0);
            WriteU16BE(o, 0x1C, (ushort)n);
            WriteU16BE(o, 0x1E, 1);

            for (int i = 0; i < n; i++)
            {
                int pp = primPos[i];
                WriteSig(o, pp, "PRIM");
                WriteU32BE(o, pp + 4, (uint)blocks[i].Length);
                WriteU32BE(o, pp + 8, NullOffset);                     //PRIM has no children
                WriteU32BE(o, pp + 12, i < n - 1 ? (uint)(SecHeader + blocks[i].Length) : NullOffset);
                WriteU32BE(o, pp + 0x10, NullOffset);
                WriteU32BE(o, pp + 0x14, (uint)SecHeader);             //-> mesh block
                WriteU32BE(o, pp + 0x18, 0);
                WriteU16BE(o, pp + 0x1C, 0);
                WriteU16BE(o, pp + 0x1E, 1);
                Array.Copy(blocks[i], 0, o, pp + SecHeader, blocks[i].Length);
            }
            return o;
        }

        //Swap the whole PRMA region. PRMA is after TEXA, so the only thing downstream is SHDA (not alignment-
        //critical); AlignTextureGap is a no-op here but kept for uniformity with the other structural ops.
        private void SplicePrma(byte[] src, int prmaPos, int oldFootprint, byte[] newPrma)
        {
            byte[] o = new byte[src.Length - oldFootprint + newPrma.Length];
            Array.Copy(src, 0, o, 0, prmaPos);
            Array.Copy(newPrma, 0, o, prmaPos, newPrma.Length);
            Array.Copy(src, prmaPos + oldFootprint, o, prmaPos + newPrma.Length,
                       src.Length - prmaPos - oldFootprint);
            ReloadFromData(AlignTextureGap(o));
        }

        //Import an .obj as a new primitive. Seeded from primitive 0's mesh block (BuildBlock keeps the donor's
        //header verbatim, as Replace does), then given a unique hash. Orphan until assigned to an emitter.
        public void AddPrimitiveFromObj(string objFile)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int prmaPos, oldFootprint; List<byte[]> blocks;
            if (!ParsePrma(src, out prmaPos, out oldFootprint, out blocks) || prmaPos < 0)
            { MessageBox.Show("This file has no PRMA primitive table at all; adding a primitive isn't supported."); return; }
            //BuildBlock keeps a donor PRIM header verbatim (only the geometry-dependent counts/offsets are patched),
            //so an .obj (which carries no header) needs a template block. Use this file's first primitive, or
            //borrow one from any other open file when this PRMA is empty.
            byte[] donor = blocks.Count > 0 ? blocks[0] : DonorPrimBlockFromOpenFiles();
            if (donor == null)
            { MessageBox.Show(Runtime.MainForm, "This file has no primitive to use as a header template, and no other open file has one. Open a .sesetlist that has a primitive, then import.", "Add Primitive"); return; }

            try
            {
                var p = new Primitive();
                p.LoadMesh((byte[])donor.Clone());              //donor header for BuildBlock
                byte[] nb = p.ImportObjAsBlock(objFile);
                if (nb == null) { MessageBox.Show("Could not read any usable triangles from that .obj."); return; }

                var hashes = new List<uint>();
                foreach (var b in blocks) if (b.Length >= 8) hashes.Add(ReadU32BE(b, 4));
                WriteU32BE(nb, 4, NewUniqueId(objFile, hashes));  //unique mesh hash at block +0x04

                blocks.Add(nb);
                SplicePrma(src, prmaPos, oldFootprint,
                           BuildPrmaBlock(blocks, ReadU32BE(src, prmaPos + 12) == NullOffset));
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show("Add primitive failed: " + ex.Message); }
        }

        //A PRIM mesh block from any open file, used as a header template when this file's PRMA is empty.
        private static byte[] DonorPrimBlockFromOpenFiles()
        {
            foreach (var f in LoadedFiles)
                foreach (var p in f.FilePrimitives)
                    if (p.BlockData != null && p.BlockData.Length >= 0x54) return p.BlockData;
            return null;
        }

        //Remove the primitive whose mesh-block hash matches. Empty PRMA is valid (e.g. Item_Ore_G), so deleting
        //the last primitive is allowed. Warns when emitters still reference the hash.
        public void DeletePrimitiveByHash(uint hash)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int prmaPos, oldFootprint; List<byte[]> blocks;
            if (!ParsePrma(src, out prmaPos, out oldFootprint, out blocks) || prmaPos < 0) return;

            int k = blocks.FindIndex(b => b.Length >= 8 && ReadU32BE(b, 4) == hash);
            if (k < 0) { MessageBox.Show("Primitive not found in the table."); return; }

            int refs; var refNames = EmittersReferencing(false, 5, out refs, hash);
            if (refs > 0)
            {
                string list = "\n  " + string.Join("\n  ", refNames) + (refs > refNames.Count ? "\n  ..." : "");
                if (MessageBox.Show(Runtime.MainForm,
                        $"This primitive is referenced by {refs} emitter(s); they will draw no mesh:{list}\n\nDelete anyway?",
                        "Delete Primitive", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            try
            {
                blocks.RemoveAt(k);
                SplicePrma(src, prmaPos, oldFootprint,
                           BuildPrmaBlock(blocks, ReadU32BE(src, prmaPos + 12) == NullOffset));
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show("Delete primitive failed: " + ex.Message); }
        }

        //--- Clear a whole root (Emitter Sets / Textures / PRMA), keeping the now-empty root section ------
        //Replaces the section's entire region with a clean 0x20 empty-container header (no children, count 0,
        //BinaryDataOffset NULL, NextSectionOffset -> the following top-level section right after the header).
        //Validated offline against GameResident for ESTA/TEXA/PRMA: the tree re-parses intact with the cleared
        //root at count 0. AlignTextureGap then re-pads the texture data (a no-op once TEXA is empty).
        private void ClearContainer(string sig)
        {
            byte[] src = BuildEftbBytes();
            int p = FindTopLevel(src, sig);
            if (p < 0) return;
            uint next = ReadU32BE(src, p + 12);
            int oldFootprint = (next != NullOffset) ? (int)next : src.Length - p;
            if (oldFootprint <= SecHeader) return;   //already just a header (empty)

            byte[] hdr = new byte[SecHeader];
            Array.Copy(src, p, hdr, 0, SecHeader);                                       //keep the original header bytes...
            WriteU32BE(hdr, 4, 0);                                                       //...then set the empty-container fields:
            WriteU32BE(hdr, 8, NullOffset);                                             //  no children
            WriteU32BE(hdr, 12, (next == NullOffset) ? NullOffset : (uint)SecHeader);   //  -> next top-level section, right after this header
            WriteU32BE(hdr, 0x14, NullOffset);                                          //  empty container -> NULL BinaryDataOffset
            WriteU16BE(hdr, 0x1C, 0);                                                   //  child count = 0

            byte[] o = new byte[src.Length - oldFootprint + SecHeader];
            Array.Copy(src, 0, o, 0, p);
            Array.Copy(hdr, 0, o, p, SecHeader);
            Array.Copy(src, p + oldFootprint, o, p + SecHeader, src.Length - p - oldFootprint);
            ReloadFromData(AlignTextureGap(o));
        }

        private static int TopLevelChildCount(byte[] d, string sig)
        {
            int p = FindTopLevel(d, sig);
            return (p >= 0) ? ReadU16BE(d, p + 0x1C) : 0;
        }

        //Count + name (up to max) the emitters that bind a texture sampler / a primitive mesh, for the clear warning.
        //Up to `max` "Set / Emitter" names (and the total count) of emitters referencing a texture/primitive, for
        //delete warnings. specificId 0 = any texture/primitive (Clear All); otherwise match that exact id/hash.
        private List<string> EmittersReferencing(bool textures, int max, out int total, uint specificId = 0)
        {
            var names = new List<string>(); total = 0;
            foreach (var node in TreeViewExtensions.Collect(Nodes))
            {
                if (!(node is SectionBase sb) || !(((SectionBase)node).BinaryData is Emitter em)) continue;
                bool uses = false;
                if (textures)
                {
                    if (em.Samplers != null)
                        foreach (var s in em.Samplers)
                            if (specificId != 0 ? s.TextureID == specificId
                                                : (s.TextureID != 0 && s.TextureID != NullOffset)) { uses = true; break; }
                }
                else
                {
                    uint h = em.PrimitiveHash;
                    uses = specificId != 0 ? h == specificId : (h != 0 && h != NullOffset);
                }
                if (uses) { total++; if (names.Count < max) names.Add(EmitterRefLabel(sb)); }
            }
            return names;
        }

        //"Set / Emitter" label for a referencing-emitter warning, so the user can find it in the tree.
        private static string EmitterRefLabel(SectionBase sb)
        {
            string parent = sb.Parent != null ? sb.Parent.Text : null;
            return string.IsNullOrEmpty(parent) ? sb.Text : parent + " / " + sb.Text;
        }

        //Right-click "Clear All ..." on the Emitter Sets / Textures / PRMA roots. The root stays; its contents go.
        //Dialogs are owned by the main window so they surface in front (see the save-dialog owner fix).
        public void ClearAllSets()
        {
            if (header == null || header.Signature != "EFTB") return;
            int cnt = TopLevelChildCount(data, "ESTA");
            if (cnt == 0) { MessageBox.Show(Runtime.MainForm, "There are no emitter sets to clear.", "Clear Emitter Sets"); return; }
            if (MessageBox.Show(Runtime.MainForm, $"Remove ALL {cnt} emitter set(s)? The Emitter Sets folder stays but will be empty.\n(Reload the file without saving to undo.)",
                "Clear Emitter Sets", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            ClearContainer("ESTA");
            SelectSectionPath("ESTA");
        }
        public void ClearAllTextures()
        {
            if (header == null || header.Signature != "EFTB") return;
            int cnt = TopLevelChildCount(data, "TEXA");
            if (cnt == 0) { MessageBox.Show(Runtime.MainForm, "There are no textures to clear.", "Clear Textures"); return; }
            int total; var names = EmittersReferencing(true, 5, out total);
            string warn = total > 0 ? $"\n\n{total} emitter(s) reference a texture and will lose it:\n  " + string.Join("\n  ", names) + (total > names.Count ? "\n  ..." : "") : "";
            if (MessageBox.Show(Runtime.MainForm, $"Remove ALL {cnt} texture(s)? The Textures folder stays but will be empty.{warn}\n(Reload the file without saving to undo.)",
                "Clear Textures", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            ClearContainer("TEXA");
            SelectSectionPath("TEXA");
        }
        public void ClearAllPrimitives()
        {
            if (header == null || header.Signature != "EFTB") return;
            int cnt = TopLevelChildCount(data, "PRMA");
            if (cnt == 0) { MessageBox.Show(Runtime.MainForm, "There are no primitives to clear.", "Clear Primitives"); return; }
            int total; var names = EmittersReferencing(false, 5, out total);
            string warn = total > 0 ? $"\n\n{total} emitter(s) reference a primitive and will draw no mesh:\n  " + string.Join("\n  ", names) + (total > names.Count ? "\n  ..." : "") : "";
            if (MessageBox.Show(Runtime.MainForm, $"Remove ALL {cnt} primitive(s)? The PRMA folder stays but will be empty.{warn}\n(Reload the file without saving to undo.)",
                "Clear Primitives", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            ClearContainer("PRMA");
            SelectSectionPath("PRMA");
        }

        //--- Export / import a single emitter as a toolbox-only "EFTX" bundle ----------------------------
        //The bundle carries the raw EMTR section bytes plus copies of the in-file textures / primitive it
        //references, so it can be re-imported into another .sesetlist. On import, a bundled resource is added
        //only if the target file doesn't already have that texture id / primitive hash (BotW ids are content
        //hashes, so a matching id IS the same resource (no remap needed). v1 bundles IN-FILE resources;
        //references that live in a sibling file are reported, not bundled (the target must provide them).
        //One bundled GX2 shader group (header ++ program) tagged with the donor file's local index, so an imported
        //emitter's vtx/frag index can be remapped to wherever the group lands in the target bundle.
        private class ShaderEntry { public bool isVtx; public uint donor; public byte[] group; }
        private class EftxData { public byte kind; public byte[] section; public List<TexEntry> texs = new List<TexEntry>(); public List<byte[]> prims = new List<byte[]>(); public List<ShaderEntry> shaders = new List<ShaderEntry>(); }

        private static void WriteEftx(string path, byte kind, byte[] section, List<TexEntry> texs, List<byte[]> prims, List<ShaderEntry> shaders)
        {
            using (var w = new System.IO.BinaryWriter(System.IO.File.Create(path)))
            {
                w.Write(new byte[] { (byte)'E', (byte)'F', (byte)'T', (byte)'X' });
                w.Write((uint)2);                 //version (2 = adds the shaders section; v1 readers/files still load)
                w.Write(kind);                    //0 = emitter, 1 = set
                w.Write((byte)0); w.Write((byte)0); w.Write((byte)0);
                w.Write(section.Length); w.Write(section);
                w.Write(texs.Count);
                foreach (var t in texs) { w.Write(ReadU32BE(t.Desc, 0x24)); w.Write(t.Desc.Length); w.Write(t.Data.Length); w.Write(t.Desc); w.Write(t.Data); }
                w.Write(prims.Count);
                foreach (var b in prims) { w.Write(ReadU32BE(b, 4)); w.Write(b.Length); w.Write(b); }
                w.Write(shaders == null ? 0 : shaders.Count);
                if (shaders != null) foreach (var s in shaders) { w.Write((byte)(s.isVtx ? 0 : 1)); w.Write(s.donor); w.Write(s.group.Length); w.Write(s.group); }
            }
        }

        private static EftxData ReadEftx(string path)
        {
            using (var r = new System.IO.BinaryReader(System.IO.File.OpenRead(path)))
            {
                byte[] m = r.ReadBytes(4);
                if (m.Length < 4 || m[0] != 'E' || m[1] != 'F' || m[2] != 'T' || m[3] != 'X') return null;
                uint ver = r.ReadUInt32(); if (ver != 1 && ver != 2) return null;
                var d = new EftxData();
                d.kind = r.ReadByte(); r.ReadBytes(3);
                d.section = r.ReadBytes(r.ReadInt32());
                int tc = r.ReadInt32();
                for (int i = 0; i < tc; i++) { r.ReadUInt32(); int dl = r.ReadInt32(); int al = r.ReadInt32(); var de = r.ReadBytes(dl); var da = r.ReadBytes(al); d.texs.Add(new TexEntry() { Desc = de, Data = da }); }
                int pc = r.ReadInt32();
                for (int i = 0; i < pc; i++) { r.ReadUInt32(); int bl = r.ReadInt32(); d.prims.Add(r.ReadBytes(bl)); }
                if (ver >= 2)
                {
                    int sc = r.ReadInt32();
                    for (int i = 0; i < sc; i++) { byte k = r.ReadByte(); uint donor = r.ReadUInt32(); int gl = r.ReadInt32(); byte[] g = r.ReadBytes(gl); d.shaders.Add(new ShaderEntry { isVtx = (k == 0), donor = donor, group = g }); }
                }
                return d;
            }
        }

        //Index every texture (id -> Desc+Data) and primitive (hash -> block) across THIS file and every OTHER
        //loaded file, so an exported emitter/set can bundle resources it references even when they live in a
        //sibling (e.g. a shared GameResident texture), as long as that sibling is also open. A resource in no
        //loaded file can't be bundled (reported as "external").
        private void BuildResourceIndex(byte[] src, out Dictionary<uint, TexEntry> texById, out Dictionary<uint, byte[]> primByHash)
        {
            texById = new Dictionary<uint, TexEntry>();
            primByHash = new Dictionary<uint, byte[]>();
            IndexTexaPrma(src, texById, primByHash);            //this file first
            foreach (var f in LoadedFiles)
            {
                if (f == this) continue;
                byte[] s; try { s = f.BuildEftbBytes(); } catch { continue; }
                IndexTexaPrma(s, texById, primByHash);          //add anything the siblings have that we don't
            }
        }
        private static void IndexTexaPrma(byte[] s, Dictionary<uint, TexEntry> texById, Dictionary<uint, byte[]> primByHash)
        {
            int tp, tfp; List<TexEntry> es;
            if (ParseTexa(s, out tp, out tfp, out es)) foreach (var e in es) { uint id = ReadU32BE(e.Desc, 0x24); if (!texById.ContainsKey(id)) texById[id] = e; }
            int pp, pfp; List<byte[]> bs;
            if (ParsePrma(s, out pp, out pfp, out bs)) foreach (var b in bs) if (b.Length >= 8) { uint h = ReadU32BE(b, 4); if (!primByHash.ContainsKey(h)) primByHash[h] = b; }
        }

        public void ExportEmitter(int setIndex, int emtrIndex, Emitter em, string path)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (emtrIndex < 0 || emtrIndex >= ePos.Count) return;

            byte[] emitterBytes = new byte[eSize[emtrIndex]];
            Array.Copy(src, ePos[emtrIndex], emitterBytes, 0, eSize[emtrIndex]);

            Dictionary<uint, TexEntry> texById; Dictionary<uint, byte[]> primByHash;
            BuildResourceIndex(src, out texById, out primByHash);

            var bTex = new List<TexEntry>(); var seenTex = new HashSet<uint>();
            var bPrim = new List<byte[]>();
            int external = 0;
            for (int slot = 0; slot < EmtSamplerCount; slot++)
            {
                uint id = (em != null) ? em.GetSamplerTextureId(slot) : 0;
                if (id == 0 || id == NullOffset || seenTex.Contains(id)) continue;
                seenTex.Add(id);
                TexEntry e;
                if (texById.TryGetValue(id, out e)) bTex.Add(e); else external++;
            }
            uint ph = (em != null) ? em.PrimitiveHash : 0;
            if (ph != 0 && ph != NullOffset)
            {
                byte[] b;
                if (primByHash.TryGetValue(ph, out b)) bPrim.Add(b); else external++;
            }

            var bShaders = CollectShaderEntries(src, new[] { em });
            try { WriteEftx(path, 0, emitterBytes, bTex, bPrim, bShaders); }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show(Runtime.MainForm, "Export failed: " + ex.Message, "Export Emitter"); return; }
            MessageBox.Show(Runtime.MainForm, $"Exported emitter to:\n{path}\n\nBundled {bTex.Count} texture(s), {bPrim.Count} primitive(s) and {bShaders.Count} shader(s)." +
                (external > 0 ? $"\n\n{external} resource(s) from other (unloaded) files were kept as references - they'll resolve when the target file provides them. (Load that file before exporting to bundle copies instead.)" : ""),
                "Export Emitter");
        }

        public void ImportEmitter(int setIndex, string path)
        {
            if (header == null || header.Signature != "EFTB") return;
            EftxData d;
            try { d = ReadEftx(path); }
            catch (Exception ex) { MessageBox.Show(Runtime.MainForm, "Couldn't read the file: " + ex.Message, "Import Emitter"); return; }
            if (d == null) { MessageBox.Show(Runtime.MainForm, "That isn't a toolbox emitter export (EFTX) file.", "Import Emitter"); return; }
            if (d.kind != 0) { MessageBox.Show(Runtime.MainForm, "That bundle is an emitter SET - import it onto the Emitter Sets folder.", "Import Emitter"); return; }
            try
            {
                AddBundledTextures(d.texs);   //each reuses the existing table or splices in the missing resource
                AddBundledPrims(d.prims);
                MergeImportedShaders(d);      //merge bundled shaders + re-point the imported emitter onto them
                SpliceImportedEmitter(setIndex, d.section);
                SelectSectionPath("ESTA", setIndex, int.MaxValue);   //land on the newly imported (last) emitter
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show(Runtime.MainForm, "Import failed: " + ex.Message, "Import Emitter"); }
        }

        //Add only the textures whose id is not already in TEXA (BotW texture ids are content hashes -> a matching
        //id is the same texture). One rebuild for the whole batch.
        private void AddBundledTextures(List<TexEntry> texs)
        {
            if (texs == null || texs.Count == 0) return;
            byte[] src = BuildEftbBytes();
            int texaPos, oldFp; List<TexEntry> entries;
            if (!ParseTexa(src, out texaPos, out oldFp, out entries) || texaPos < 0) return;
            var ids = new HashSet<uint>(); foreach (var e in entries) ids.Add(ReadU32BE(e.Desc, 0x24));
            bool added = false;
            foreach (var t in texs) { uint id = ReadU32BE(t.Desc, 0x24); if (!ids.Contains(id)) { entries.Add(t); ids.Add(id); added = true; } }
            if (added) SpliceTexa(src, texaPos, oldFp, BuildTexaBlock(entries, ReadU32BE(src, texaPos + 12) == NullOffset));
        }
        private void AddBundledPrims(List<byte[]> prims)
        {
            if (prims == null || prims.Count == 0) return;
            byte[] src = BuildEftbBytes();
            int prmaPos, oldFp; List<byte[]> blocks;
            if (!ParsePrma(src, out prmaPos, out oldFp, out blocks) || prmaPos < 0) return;
            var hashes = new HashSet<uint>(); foreach (var b in blocks) if (b.Length >= 8) hashes.Add(ReadU32BE(b, 4));
            bool added = false;
            foreach (var b in prims) if (b.Length >= 8) { uint h = ReadU32BE(b, 4); if (!hashes.Contains(h)) { blocks.Add(b); hashes.Add(h); added = true; } }
            if (added) SplicePrma(src, prmaPos, oldFp, BuildPrmaBlock(blocks, ReadU32BE(src, prmaPos + 12) == NullOffset));
        }

        //Bundle the vtx + frag shader groups the given emitters reference (deduped by local index), tagged with the
        //donor index, so an exported emitter/set carries its shaders and can render after import into another file.
        private List<ShaderEntry> CollectShaderEntries(byte[] src, IEnumerable<Emitter> ems)
        {
            var list = new List<ShaderEntry>();
            int shdb, gfd, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(src, out shdb, out gfd, out gfdLen, out blocks)) return list;
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);
            var seenV = new HashSet<uint>(); var seenF = new HashSet<uint>();
            if (ems != null) foreach (var em in ems)
            {
                if (em == null) continue;
                uint vi = em.ShaderVtxIndex, fi = em.ShaderFrgIndex;
                if (vi < (uint)vtx.Count && seenV.Add(vi)) list.Add(new ShaderEntry { isVtx = true, donor = vi, group = vtx[(int)vi] });
                if (fi < (uint)frag.Count && seenF.Add(fi)) list.Add(new ShaderEntry { isVtx = false, donor = fi, group = frag[(int)fi] });
            }
            return list;
        }

        //Merge an imported bundle's shader groups into this file (dedup by content) and re-point the imported
        //section's emitters from donor indices to wherever each group landed. No-op for v1 bundles (no shaders).
        private void MergeImportedShaders(EftxData d)
        {
            if (d == null || d.shaders == null || d.shaders.Count == 0) return;
            var vIds = new List<uint>(); var vG = new List<byte[]>(); var fIds = new List<uint>(); var fG = new List<byte[]>();
            foreach (var s in d.shaders) { if (s.isVtx) { vIds.Add(s.donor); vG.Add(s.group); } else { fIds.Add(s.donor); fG.Add(s.group); } }
            Dictionary<uint, int> vmap, fmap;
            if (MergeShaders(vIds, vG, fIds, fG, out vmap, out fmap)) RepointSectionShaders(d.section, vmap, fmap);
        }

        //Rewrite the vtx/frag shader index (+0x8C4 / +0x8C8) of every emitter in an EMTR or ESET section blob,
        //mapping donor indices to local ones via the maps MergeShaders returned.
        private static void RepointSectionShaders(byte[] section, Dictionary<uint, int> vmap, Dictionary<uint, int> fmap)
        {
            if (section == null || section.Length < 0x20) return;
            string sig = Encoding.ASCII.GetString(section, 0, 4);
            var eds = new List<int>();
            if (sig == "EMTR") { int bdo = (int)ReadU32BE(section, 0x14); eds.Add(bdo + 0x50); }
            else if (sig == "ESET")
            {
                uint sub = ReadU32BE(section, 8);
                if (sub != NullOffset)
                {
                    int emtr = (int)sub;
                    while (emtr + 0x20 <= section.Length && Encoding.ASCII.GetString(section, emtr, 4) == "EMTR")
                    {
                        int bdo = (int)ReadU32BE(section, emtr + 0x14);
                        eds.Add(emtr + bdo + 0x50);
                        uint en = ReadU32BE(section, emtr + 12);
                        if (en == NullOffset) break;
                        emtr += (int)en;
                    }
                }
            }
            foreach (var ed in eds)
            {
                if (ed + EmtShaderFrgOff + 4 > section.Length) continue;
                uint v = ReadU32BE(section, ed + EmtShaderVtxOff); if (vmap.ContainsKey(v)) WriteU32BE(section, ed + EmtShaderVtxOff, (uint)vmap[v]);
                uint f = ReadU32BE(section, ed + EmtShaderFrgOff); if (fmap.ContainsKey(f)) WriteU32BE(section, ed + EmtShaderFrgOff, (uint)fmap[f]);
            }
        }

        //Append the imported EMTR bytes to set #setIndex (same splice + chain fixes as AddEmitterTo; footprint
        //sizing and the ESET NextSectionOffset update that the emitter-splice fix added).
        private void SpliceImportedEmitter(int setIndex, byte[] emitterBytes)
        {
            byte[] src = BuildEftbBytes();
            int estaPos, esetPos; List<int> ePos, eSize;
            if (!LocateSet(src, setIndex, out estaPos, out esetPos, out ePos, out eSize)) return;
            if (ePos.Count == 0 || emitterBytes == null || emitterBytes.Length < 0x20) return;

            int L = emitterBytes.Length;
            int last = ePos[ePos.Count - 1], Ls = eSize[ePos.Count - 1], P = last + Ls;
            byte[] o = SpliceInsert(src, P, (byte[])emitterBytes.Clone());

            WriteU16BE(o, esetPos + 0x1C, (ushort)(ReadU16BE(o, esetPos + 0x1C) + 1));
            WriteU32BE(o, esetPos + EsetNumEmitterOff, ReadU32BE(o, esetPos + EsetNumEmitterOff) + 1);
            WriteU32BE(o, esetPos + 4, ReadU32BE(o, esetPos + 4) + (uint)L);
            uint esetNext = ReadU32BE(o, esetPos + 12);
            if (esetNext != NullOffset && esetPos + esetNext >= P) WriteU32BE(o, esetPos + 12, esetNext + (uint)L);
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)L);
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)L);

            WriteU32BE(o, last + 12, (uint)Ls);   //old last emitter -> new emitter
            WriteU32BE(o, P + 12, NullOffset);    //new emitter is the last

            var names = new List<string>();
            for (int i = 0; i < ePos.Count; i++) names.Add(ReadEmitterName(src, ePos[i]));
            WriteEmitterName(o, P, UniqueName(ReadEmitterName(o, P), names));

            ReloadFromData(AlignTextureGap(o));
        }

        public void ExportSet(int setIndex, List<Emitter> emitters, string path)
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int estaPos; List<int> setPos, setSize;
            if (!LocateAllSets(src, out estaPos, out setPos, out setSize)) return;
            if (setIndex < 0 || setIndex >= setPos.Count) return;

            uint estaNext = ReadU32BE(src, estaPos + 12);
            int estaEnd = (estaNext != NullOffset) ? estaPos + (int)estaNext : estaPos + (int)ReadU32BE(src, estaPos + 4);
            int fp = (setIndex < setPos.Count - 1) ? setPos[setIndex + 1] - setPos[setIndex] : estaEnd - setPos[setIndex];
            byte[] setBytes = new byte[fp];
            Array.Copy(src, setPos[setIndex], setBytes, 0, fp);

            Dictionary<uint, TexEntry> texById; Dictionary<uint, byte[]> primByHash;
            BuildResourceIndex(src, out texById, out primByHash);
            var bTex = new List<TexEntry>(); var seenTex = new HashSet<uint>();
            var bPrim = new List<byte[]>(); var seenPrim = new HashSet<uint>();
            int external = 0;
            if (emitters != null) foreach (var em in emitters)
            {
                if (em == null) continue;
                for (int slot = 0; slot < EmtSamplerCount; slot++)
                {
                    uint id = em.GetSamplerTextureId(slot);
                    if (id == 0 || id == NullOffset || seenTex.Contains(id)) continue;
                    seenTex.Add(id);
                    TexEntry e;
                    if (texById.TryGetValue(id, out e)) bTex.Add(e); else external++;
                }
                uint ph = em.PrimitiveHash;
                if (ph != 0 && ph != NullOffset && !seenPrim.Contains(ph))
                {
                    seenPrim.Add(ph);
                    byte[] b;
                    if (primByHash.TryGetValue(ph, out b)) bPrim.Add(b); else external++;
                }
            }

            var bShaders = CollectShaderEntries(src, emitters);
            try { WriteEftx(path, 1, setBytes, bTex, bPrim, bShaders); }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show(Runtime.MainForm, "Export failed: " + ex.Message, "Export Set"); return; }
            MessageBox.Show(Runtime.MainForm, $"Exported emitter set to:\n{path}\n\nBundled {bTex.Count} texture(s), {bPrim.Count} primitive(s) and {bShaders.Count} shader(s)." +
                (external > 0 ? $"\n\n{external} resource(s) from other (unloaded) files were kept as references - they'll resolve when the target file provides them. (Load that file before exporting to bundle copies instead.)" : ""),
                "Export Set");
        }

        public void ImportSet(string path)
        {
            if (header == null || header.Signature != "EFTB") return;
            EftxData d;
            try { d = ReadEftx(path); }
            catch (Exception ex) { MessageBox.Show(Runtime.MainForm, "Couldn't read the file: " + ex.Message, "Import Set"); return; }
            if (d == null) { MessageBox.Show(Runtime.MainForm, "That isn't a toolbox export (EFTX) file.", "Import Set"); return; }
            if (d.kind != 1) { MessageBox.Show(Runtime.MainForm, "That bundle is a single EMITTER - import it onto an emitter set instead.", "Import Set"); return; }
            try
            {
                AddBundledTextures(d.texs);
                AddBundledPrims(d.prims);
                MergeImportedShaders(d);      //merge bundled shaders + re-point the imported set's emitters onto them
                SpliceImportedSet(d.section);
                SelectSectionPath("ESTA", int.MaxValue);   //the newly imported (last) set
            }
            catch (Exception ex) { Console.WriteLine(ex); MessageBox.Show(Runtime.MainForm, "Import failed: " + ex.Message, "Import Set"); }
        }

        //Append the imported ESET bytes to ESTA (same splice as AddEmitterSet, with the imported set in place of
        //the template; handles an empty ESTA by restoring the child pointer).
        private void SpliceImportedSet(byte[] setBytes)
        {
            byte[] src = BuildEftbBytes();
            int estaPos; List<int> setPos, setSize;
            if (!LocateAllSets(src, out estaPos, out setPos, out setSize)) return;
            if (setBytes == null || setBytes.Length < 0x20) return;

            byte[] newset = (byte[])setBytes.Clone();
            int L = newset.Length;
            int P; byte[] o;
            if (setPos.Count == 0)
            {
                P = estaPos + 0x20;
                o = SpliceInsert(src, P, newset);
                WriteU32BE(o, estaPos + 8, 0x20);                 //restore child pointer
                WriteU16BE(o, estaPos + 0x1C, 1);
            }
            else
            {
                int last = setPos[setPos.Count - 1], Ls = setSize[setPos.Count - 1];
                P = last + Ls;
                o = SpliceInsert(src, P, newset);
                WriteU16BE(o, estaPos + 0x1C, (ushort)(ReadU16BE(o, estaPos + 0x1C) + 1));
                WriteU32BE(o, last + 12, (uint)Ls);              //old last set -> new set
            }
            WriteU32BE(o, P + 12, NullOffset);                   //new set is the last
            WriteU32BE(o, estaPos + 4, ReadU32BE(o, estaPos + 4) + (uint)L);
            uint estaNext = ReadU32BE(o, estaPos + 12);
            if (estaNext != NullOffset && estaPos + estaNext >= P) WriteU32BE(o, estaPos + 12, estaNext + (uint)L);

            var names = new List<string>();
            for (int i = 0; i < setPos.Count; i++) names.Add(ReadSetName(src, setPos[i]));
            WriteSetName(o, P, UniqueName(ReadSetName(o, P), names));

            ReloadFromData(AlignTextureGap(o));
        }

        //--- View the SHDA GX2 shader an emitter uses (decompiled to GLSL via the bundled gx2dec.exe) ------
        //Parse the GFD bundle inside SHDA: blocks = each whole BLK{ block (header + data). False if no GFD.
        private static bool ParseGfd(byte[] d, out int shdbPos, out int gfdPos, out int gfdLen, out List<byte[]> blocks)
        {
            shdbPos = -1; gfdPos = -1; gfdLen = 0; blocks = new List<byte[]>();
            int shda = FindTopLevel(d, "SHDA");
            if (shda < 0) return false;
            uint sub = ReadU32BE(d, shda + 8);
            if (sub == NullOffset) return false;
            shdbPos = shda + (int)sub;
            if (shdbPos + 0x20 > d.Length || Encoding.ASCII.GetString(d, shdbPos, 4) != "SHDB") return false;
            gfdLen = (int)ReadU32BE(d, shdbPos + 4);
            gfdPos = shdbPos + (int)ReadU32BE(d, shdbPos + 0x14);
            if (gfdPos + 4 > d.Length || Encoding.ASCII.GetString(d, gfdPos, 4) != "Gfx2") return false;
            int p = gfdPos + (int)ReadU32BE(d, gfdPos + 4), guard = 0;
            while (p + 0x20 <= d.Length && guard++ < 100000)
            {
                if (Encoding.ASCII.GetString(d, p, 4) != "BLK{") break;
                int hsz = (int)ReadU32BE(d, p + 4), dsz = (int)ReadU32BE(d, p + 0x14);
                byte[] b = new byte[hsz + dsz];
                Array.Copy(d, p, b, 0, hsz + dsz);
                blocks.Add(b);
                uint bt = ReadU32BE(d, p + 0x10);
                p += hsz + dsz;
                if (bt == 1) break; //EOF block
            }
            return true;
        }
        private static uint GfdBlockType(byte[] block) { return ReadU32BE(block, 0x10); }
        //Group blocks into vertex/fragment shaders; each group = its header block ++ its program block.
        private static void GroupGfd(List<byte[]> blocks, out List<byte[]> vtx, out List<byte[]> frag)
        {
            vtx = new List<byte[]>(); frag = new List<byte[]>();
            byte[] curHdr = null; bool curIsVtx = false;
            foreach (var b in blocks)
            {
                uint t = GfdBlockType(b);
                if (t == 3) { curHdr = b; curIsVtx = true; }           //vertex-shader header block
                else if (t == 6) { curHdr = b; curIsVtx = false; }     //fragment-shader header block
                else if ((t == 5 || t == 7) && curHdr != null)         //program block -> pair with the last header
                {
                    byte[] g = new byte[curHdr.Length + b.Length];
                    Array.Copy(curHdr, 0, g, 0, curHdr.Length);
                    Array.Copy(b, 0, g, curHdr.Length, b.Length);
                    (curIsVtx ? vtx : frag).Add(g);
                    curHdr = null;
                }
            }
        }

        //Per-file GX2 shader program-body hashes (set by MapShaderInfo): two files that ship a byte-identical
        //shader program share an entry (vertex + fragment pools kept separate). Drives the cross-file catalog.
        public HashSet<string> ShaderVtxProgHashes = new HashSet<string>();
        public HashSet<string> ShaderFrgProgHashes = new HashSet<string>();

        //Other open files whose fragment-shader bundle contains this exact GX2 program body. Lets the shader
        //overview show "[shared with GameResident]" so a once-decoded shader's meaning propagates by identity.
        public static List<string> ShaderSharers(string fragProgHash, PTCL self)
        {
            var r = new List<string>();
            if (string.IsNullOrEmpty(fragProgHash)) return r;
            try
            {
                foreach (var f in LoadedFiles.ToArray())
                {
                    if (f == null || f == self || f.TreeView == null || f.ShaderFrgProgHashes == null) continue;
                    if (f.ShaderFrgProgHashes.Contains(fragProgHash))
                        r.Add(string.IsNullOrEmpty(f.FileName) ? f.Text : f.FileName);
                }
            }
            catch { }
            return r;
        }

        //--- Shader bundling (GFD/Gfx2 surgery in SHDA) -------------------------------------------------
        //A shader = a group [header (type 3 vtx / 6 frag)][pad (type 2)][program (type 5 vtx / 7 frag)], all
        //vertex groups first then all fragment groups then EOF (type 1). An emitter selects its program by
        //INDEX: vertex-group ordinal at +0x8C4, fragment-group ordinal at +0x8C8. Program DATA sits on a 0x100
        //boundary (the pad before it is sized to suit). Header/program block CONTENT is position-independent, so
        //groups can be re-laid-out (added / removed / re-ordered) freely as long as the pad is recomputed.
        private const int GfdProgAlign = 0x100;

        //Split a group (header block ++ program block) back into the two blocks.
        private static void SplitGroup(byte[] g, out byte[] hdr, out byte[] prog)
        {
            int hlen = 0x20 + (int)ReadU32BE(g, 0x14);
            hdr = new byte[hlen]; Array.Copy(g, 0, hdr, 0, hlen);
            prog = new byte[g.Length - hlen]; Array.Copy(g, hlen, prog, 0, prog.Length);
        }
        //A fragment shader's GX2 samplerVarCount = how many texture samplers it declares (PSHhdr payload +0xD0;
        //a group's payload starts at group +0x20). -1 = not decodable.
        private static int FragSamplerVarCount(byte[] fragGroup)
        {
            int o = 0x20 + 0xD0;
            if (fragGroup == null || o + 4 > fragGroup.Length) return -1;
            return (int)ReadU32BE(fragGroup, o);
        }
        //SHA1 of a group's PROGRAM microcode (position-independent), so the same shader is recognised across
        //files even though it binds by local index. Backs the cross-file shader catalog (ShaderSharers).
        private static string ProgBodyHash(byte[] group)
        {
            byte[] hdr, prog; SplitGroup(group, out hdr, out prog);
            int start = prog.Length >= 0x20 ? 0x20 : 0; //skip the program block's own 0x20 BLK header
            using (var sha = System.Security.Cryptography.SHA1.Create())
                return BitConverter.ToString(sha.ComputeHash(prog, start, prog.Length - start)).Replace("-", "");
        }
        private static int FindGroupIndex(List<byte[]> groups, byte[] g)
        {
            for (int i = 0; i < groups.Count; i++)
                if (groups[i].Length == g.Length && System.Linq.Enumerable.SequenceEqual(groups[i], g)) return i;
            return -1;
        }
        private static void WriteGfdBlock(MemoryStream ms, uint type, byte[] data)
        {
            byte[] h = new byte[0x20];
            WriteSig(h, 0, "BLK{"); WriteU32BE(h, 4, 0x20); WriteU32BE(h, 8, 1); WriteU32BE(h, 0xC, 0);
            WriteU32BE(h, 0x10, type); WriteU32BE(h, 0x14, (uint)(data == null ? 0 : data.Length));
            ms.Write(h, 0, 0x20);
            if (data != null && data.Length > 0) ms.Write(data, 0, data.Length);
        }
        //Re-emit a complete GFD: header, every vertex group, every fragment group, EOF, with the pad before each
        //program block recomputed so the program DATA lands on a GfdProgAlign boundary.
        private static byte[] BuildGfd(byte[] gfdHeader, List<byte[]> vtxGroups, List<byte[]> fragGroups)
        {
            var ms = new MemoryStream();
            ms.Write(gfdHeader, 0, gfdHeader.Length);
            var all = new List<byte[]>(); all.AddRange(vtxGroups); all.AddRange(fragGroups);
            foreach (var g in all)
            {
                byte[] hdr, prog; SplitGroup(g, out hdr, out prog);
                ms.Write(hdr, 0, hdr.Length);
                long padStart = ms.Length;                          //program data = padStart + 0x20(pad hdr) + padLen + 0x20(prog hdr)
                int padLen = (int)((GfdProgAlign - ((padStart + 0x40) % GfdProgAlign)) % GfdProgAlign);
                WriteGfdBlock(ms, 2, new byte[padLen]);
                ms.Write(prog, 0, prog.Length);
            }
            WriteGfdBlock(ms, 1, null); //EOF
            return ms.ToArray();
        }
        //Merge bundled vertex/fragment groups into this file's GFD (dedup by content); returns donor index ->
        //new local index maps used to re-point imported emitters. The GFD is the last thing in the file, so
        //growing it just extends the file end (only SHDB.SectionSize needs updating).
        private bool MergeShaders(List<uint> vtxIds, List<byte[]> vtxGroups, List<uint> fragIds, List<byte[]> fragGroups,
                                  out Dictionary<uint, int> vtxMap, out Dictionary<uint, int> frgMap)
        {
            vtxMap = new Dictionary<uint, int>(); frgMap = new Dictionary<uint, int>();
            byte[] src = BuildEftbBytes();
            int shdbPos, gfdPos, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(src, out shdbPos, out gfdPos, out gfdLen, out blocks)) return false;
            byte[] gfdHeader = new byte[0x20]; Array.Copy(src, gfdPos, gfdHeader, 0, 0x20);
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);

            for (int i = 0; i < vtxGroups.Count; i++)
            {
                int idx = FindGroupIndex(vtx, vtxGroups[i]);
                if (idx < 0) { idx = vtx.Count; vtx.Add(vtxGroups[i]); }
                vtxMap[vtxIds[i]] = idx;
            }
            for (int i = 0; i < fragGroups.Count; i++)
            {
                int idx = FindGroupIndex(frag, fragGroups[i]);
                if (idx < 0) { idx = frag.Count; frag.Add(fragGroups[i]); }
                frgMap[fragIds[i]] = idx;
            }

            byte[] newGfd = BuildGfd(gfdHeader, vtx, frag);
            int tail = src.Length - (gfdPos + gfdLen);
            byte[] o = new byte[gfdPos + newGfd.Length + Math.Max(0, tail)];
            Array.Copy(src, 0, o, 0, gfdPos);
            Array.Copy(newGfd, 0, o, gfdPos, newGfd.Length);
            if (tail > 0) Array.Copy(src, gfdPos + gfdLen, o, gfdPos + newGfd.Length, tail);
            WriteU32BE(o, shdbPos + 4, (uint)newGfd.Length); //SHDB SectionSize tracks GFD length
            ReloadFromData(o);
            return true;
        }

        //Splice a rebuilt GFD back into SHDA: replace the GFD region in `src`, keep any trailing bytes, update
        //SHDB.SectionSize, and reload. The GFD is the last section so this only ever extends/shrinks the file end.
        //Shared by prune / clear (same tail logic as MergeShaders).
        private void ReplaceGfd(byte[] src, int gfdPos, int gfdLen, int shdbPos, byte[] newGfd)
        {
            int tail = src.Length - (gfdPos + gfdLen);
            byte[] o = new byte[gfdPos + newGfd.Length + Math.Max(0, tail)];
            Array.Copy(src, 0, o, 0, gfdPos);
            Array.Copy(newGfd, 0, o, gfdPos, newGfd.Length);
            if (tail > 0) Array.Copy(src, gfdPos + gfdLen, o, gfdPos + newGfd.Length, tail);
            WriteU32BE(o, shdbPos + 4, (uint)newGfd.Length);
            ReloadFromData(o);
        }

        //Absolute offset of every emitter's EmitterData struct in a file image (emtr + BinaryDataOffset + 0x50),
        //by walking ESTA -> ESET -> EMTR. The shader indices live at +0x8C4 (vtx) / +0x8C8 (frag) within it.
        private static List<int> EmitterDataOffsets(byte[] src)
        {
            var r = new List<int>();
            int esta = FindTopLevel(src, "ESTA");
            if (esta < 0) return r;
            uint sub = ReadU32BE(src, esta + 8);
            if (sub == NullOffset) return r;
            int eset = esta + (int)sub;
            while (eset + 0x20 <= src.Length && Encoding.ASCII.GetString(src, eset, 4) == "ESET")
            {
                uint esub = ReadU32BE(src, eset + 8);
                if (esub != NullOffset)
                {
                    int emtr = eset + (int)esub;
                    while (emtr + 0x20 <= src.Length && Encoding.ASCII.GetString(src, emtr, 4) == "EMTR")
                    {
                        int bdo = (int)ReadU32BE(src, emtr + 0x14);
                        r.Add(emtr + bdo + 0x50);
                        uint en = ReadU32BE(src, emtr + 12);
                        if (en == NullOffset) break;
                        emtr += (int)en;
                    }
                }
                uint nx = ReadU32BE(src, eset + 12);
                if (nx == NullOffset) break;
                eset += (int)nx;
            }
            return r;
        }

        //Right-click "Prune Unused Shaders" on the GTX Shader node: drop every vertex/fragment program no emitter
        //references, re-index the survivors, and re-point emitters. Dialog owned by the main window.
        public void PruneUnusedShaders()
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int shdbPos, gfdPos, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(src, out shdbPos, out gfdPos, out gfdLen, out blocks)) { MessageBox.Show(Runtime.MainForm, "This file has no SHDA shader bundle.", "Prune Shaders"); return; }
            byte[] gfdHeader = new byte[0x20]; Array.Copy(src, gfdPos, gfdHeader, 0, 0x20);
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);
            var eds = EmitterDataOffsets(src);
            var usedV = new HashSet<uint>(); var usedF = new HashSet<uint>();
            foreach (var ed in eds) { usedV.Add(ReadU32BE(src, ed + EmtShaderVtxOff)); usedF.Add(ReadU32BE(src, ed + EmtShaderFrgOff)); }
            var newV = new List<byte[]>(); var mapV = new Dictionary<int, int>();
            for (int i = 0; i < vtx.Count; i++) if (usedV.Contains((uint)i)) { mapV[i] = newV.Count; newV.Add(vtx[i]); }
            var newF = new List<byte[]>(); var mapF = new Dictionary<int, int>();
            for (int i = 0; i < frag.Count; i++) if (usedF.Contains((uint)i)) { mapF[i] = newF.Count; newF.Add(frag[i]); }
            int removed = (vtx.Count - newV.Count) + (frag.Count - newF.Count);
            if (removed == 0) { MessageBox.Show(Runtime.MainForm, "Every shader is used by at least one emitter - nothing to prune.", "Prune Shaders"); return; }
            if (MessageBox.Show(Runtime.MainForm, $"Remove {removed} unused shader(s)? ({vtx.Count - newV.Count} vertex, {frag.Count - newF.Count} fragment)\nReferenced shaders are kept and emitters re-indexed.\n(Reload the file without saving to undo.)",
                "Prune Shaders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            foreach (var ed in eds)
            {
                uint v = ReadU32BE(src, ed + EmtShaderVtxOff); if (mapV.ContainsKey((int)v)) WriteU32BE(src, ed + EmtShaderVtxOff, (uint)mapV[(int)v]);
                uint f = ReadU32BE(src, ed + EmtShaderFrgOff); if (mapF.ContainsKey((int)f)) WriteU32BE(src, ed + EmtShaderFrgOff, (uint)mapF[(int)f]);
            }
            ReplaceGfd(src, gfdPos, gfdLen, shdbPos, BuildGfd(gfdHeader, newV, newF));
            SelectSectionPath("SHDA");
        }

        //Right-click "Clear All Shaders": blank the whole bundle (header + EOF only). Irreversible (no GX2 compiler)
        //and leaves every emitter pointing at an empty pool, so it is heavily warned.
        public void ClearAllShaders()
        {
            if (header == null || header.Signature != "EFTB") return;
            byte[] src = BuildEftbBytes();
            int shdbPos, gfdPos, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(src, out shdbPos, out gfdPos, out gfdLen, out blocks)) { MessageBox.Show(Runtime.MainForm, "This file has no SHDA shader bundle.", "Clear Shaders"); return; }
            byte[] gfdHeader = new byte[0x20]; Array.Copy(src, gfdPos, gfdHeader, 0, 0x20);
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);
            if (vtx.Count + frag.Count == 0) { MessageBox.Show(Runtime.MainForm, "There are no shaders to clear.", "Clear Shaders"); return; }
            int emCount = EmitterDataOffsets(src).Count;
            if (MessageBox.Show(Runtime.MainForm, $"Remove ALL {vtx.Count} vertex + {frag.Count} fragment shader(s)?\n\nThis CANNOT be rebuilt in the toolbox (there is no GX2 shader compiler), and all {emCount} emitter(s) that bind a shader will be left pointing at an empty bundle.\n(Reload the file without saving to undo.)",
                "Clear Shaders", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            ReplaceGfd(src, gfdPos, gfdLen, shdbPos, BuildGfd(gfdHeader, new List<byte[]>(), new List<byte[]>()));
            SelectSectionPath("SHDA");
        }

        //Decompile the SHDA shaders the given emitter binds (by its vtx/frag index) and show the GLSL.
        public void ShowEmitterShader(Emitter em)
        {
            if (header == null || header.Signature != "EFTB") { MessageBox.Show(Runtime.MainForm, "Shader view is EFTB (Wii U) only.", "Shader"); return; }
            if (em == null) return;
            int shdb, gfd, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(data, out shdb, out gfd, out gfdLen, out blocks)) { MessageBox.Show(Runtime.MainForm, "This file has no SHDA shader bundle.", "Shader"); return; }
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);
            uint vi = em.ShaderVtxIndex, fi = em.ShaderFrgIndex;
            byte[] fragGroup = (fi < frag.Count) ? frag[(int)fi] : null;
            byte[] vtxGroup = (vi < vtx.Count) ? vtx[(int)vi] : null;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"This emitter uses vertex shader #{vi} and fragment (pixel) shader #{fi}.");
            sb.AppendLine($"The file's SHDA bundle holds {vtx.Count} vertex and {frag.Count} fragment shader(s).");
            sb.AppendLine();
            if (Gx2ShaderDecompiler.FindExe() == null)
            {
                sb.AppendLine("gx2dec.exe was not found, so the GX2 (Wii U Latte) bytecode can't be decompiled to GLSL.");
                sb.AppendLine("Put gx2dec.exe in the toolbox's Lib\\Plugins folder (next to FirstPlugin.Plg.dll), or set the GX2DEC_PATH environment variable.");
            }
            else
            {
                var ps = Gx2ShaderDecompiler.DecompileFragment(fragGroup);
                sb.AppendLine("================= FRAGMENT (pixel) shader =================");
                sb.AppendLine(ps.Error != null ? "(could not decompile) " + ps.Error
                    : (((ps.Mapping ?? "").Trim().Length > 0 ? ps.Mapping.Trim() + "\n\n" : "") + (ps.Glsl ?? "")));
                sb.AppendLine();
                var vs = Gx2ShaderDecompiler.DecompileVertex(vtxGroup, fragGroup);
                sb.AppendLine("================= VERTEX shader =================");
                sb.AppendLine(vs.Error != null ? "(could not decompile) " + vs.Error
                    : (((vs.Mapping ?? "").Trim().Length > 0 ? vs.Mapping.Trim() + "\n\n" : "") + (vs.Glsl ?? "")));
            }
            ShowTextWindow("Emitter shader - decompiled GLSL", sb.ToString());
        }

        //Summary popup for the "GTX Shader" (SHDB) node: it is one node = the file's whole GX2 shader bundle.
        //The shaders have no individual names (emitters bind by index) and the bundle is large, so listing or
        //decompiling all of them is pointless, so we report the counts and point at the per-emitter shader view.
        public void ShowShaderBundleSummary()
        {
            if (header == null || header.Signature != "EFTB") { MessageBox.Show(Runtime.MainForm, "Shader view is EFTB (Wii U) only.", "GTX Shader"); return; }
            int shdb, gfd, gfdLen; List<byte[]> blocks;
            if (!ParseGfd(data, out shdb, out gfd, out gfdLen, out blocks)) { MessageBox.Show(Runtime.MainForm, "This file has no SHDA shader bundle.", "GTX Shader"); return; }
            List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);
            MessageBox.Show(Runtime.MainForm,
                "This GTX Shader is the file's whole GX2 (Wii U) shader bundle:\n\n" +
                $"    {vtx.Count} vertex shader(s)\n" +
                $"    {frag.Count} fragment (pixel) shader(s)\n\n" +
                "The shaders have no individual names - each emitter binds one vertex and one fragment shader by index. " +
                "To see the actual shader code an emitter uses, right-click that emitter and choose \"View Shader (GLSL)...\".",
                "GTX Shader bundle");
        }

        private static void ShowTextWindow(string title, string body)
        {
            var form = new System.Windows.Forms.Form() { Text = title, Width = 880, Height = 700, StartPosition = System.Windows.Forms.FormStartPosition.CenterParent };
            var tb = new System.Windows.Forms.TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                Dock = System.Windows.Forms.DockStyle.Fill,
                ScrollBars = System.Windows.Forms.ScrollBars.Both,
                WordWrap = false,
                Font = new System.Drawing.Font("Consolas", 9f),
                Text = (body ?? "").Replace("\r\n", "\n").Replace("\n", "\r\n"),
            };
            form.Controls.Add(tb);
            form.Show(Runtime.MainForm);   //non-modal, owned by the main window
        }

        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(typeof(PTCL));
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(this, sfd.FileName);
        }

        //--- EFTB variable-size section support ------------------------------------------------------
        //When a texture or PRIM mesh changes size the bytes after it move, so every section offset that
        //spans it must be adjusted. Because all offsets are relative, we splice the new data in and then
        //recompute each offset through a cumulative position map. Validated against real files for arbitrary
        //grow/shrink deltas (the whole section tree re-parses intact afterwards).
        private class SectionRep
        {
            public int DataPos;          //absolute offset of the replaced data in the decompressed file
            public int OldLen;           //original data length
            public byte[] NewBytes;      //replacement bytes
            public int RawLen;           //NewBytes.Length
            public int SplicedLen;       //RawLen padded so delta keeps alignment
            public int Delta;            //SplicedLen - OldLen (bytes inserted/removed)
            public SectionBase SizeSection; //section whose +4 size field becomes RawLen (GX2B for a texture, PRIM for a mesh)
            public TEXR Tex;             //non-null for a texture replacement (extra header fields below)
            public SectionBase TexrHdr;  //the TEXR header section (texture only)
        }

        private byte[] RebuildSections(byte[] src, List<SectionRep> reps)
        {
            int ALIGN = GpuAlign; //keep inserted size a multiple of the single GPU grid so trailing textures stay aligned
            reps.Sort((a, b) => a.DataPos.CompareTo(b.DataPos));
            foreach (var r in reps)
            {
                r.RawLen = r.NewBytes.Length;
                int padSteps = (int)Math.Ceiling((double)(r.RawLen - r.OldLen) / ALIGN);
                r.SplicedLen = r.OldLen + ALIGN * padSteps;
                while (r.SplicedLen < r.RawLen) r.SplicedLen += ALIGN;
                r.Delta = r.SplicedLen - r.OldLen;
            }

            //Splice each new block in (zero-padded to SplicedLen)
            var ms = new MemoryStream();
            int prev = 0;
            foreach (var r in reps)
            {
                if (r.DataPos < prev) continue; //defensive: replaced data regions are disjoint and never overlap
                ms.Write(src, prev, r.DataPos - prev);
                ms.Write(r.NewBytes, 0, r.RawLen);
                for (int i = r.RawLen; i < r.SplicedLen; i++) ms.WriteByte(0);
                prev = r.DataPos + r.OldLen;
            }
            ms.Write(src, prev, src.Length - prev);
            byte[] nb = ms.ToArray();

            //Recompute every section offset through the position map
            var all = new List<SectionBase>();
            foreach (var s in header.Sections) CollectSections(s, all);
            foreach (var s in all)
            {
                long newPos = MapPos(s.Position, reps);
                RewriteOffset(nb, newPos + 8, s.SubSectionOffset, s.Position, reps);
                RewriteOffset(nb, newPos + 12, s.NextSectionOffset, s.Position, reps);
                RewriteOffset(nb, newPos + 20, s.BinaryDataOffset, s.Position, reps);
            }

            //Update each replaced block's section size, plus the texture header fields where applicable
            foreach (var r in reps)
            {
                long nsize = MapPos(r.SizeSection.Position, reps);
                WriteU32BE(nb, (int)nsize + 4, (uint)r.RawLen); //section data size
                if (r.Tex != null && r.TexrHdr != null)
                {
                    long nhdr = MapPos(r.TexrHdr.Position + r.TexrHdr.BinaryDataOffset, reps);
                    WriteU16BE(nb, (int)nhdr + 0, (ushort)r.Tex.Width);
                    WriteU16BE(nb, (int)nhdr + 2, (ushort)r.Tex.Height);
                    WriteU32BE(nb, (int)nhdr + 8, CompSelForFormat(r.Tex.SurfFormat)); //channel selector
                    WriteU32BE(nb, (int)nhdr + 12, r.Tex.MipCount);
                    WriteU32BE(nb, (int)nhdr + 16, Gx2FormatCode(r.Tex.SurfFormat));   //GX2 format code (game decodes from this)
                    WriteU32BE(nb, (int)nhdr + 20, r.Tex.TileMode);
                    WriteU32BE(nb, (int)nhdr + 28, r.Tex.ImageSize);
                    nb[(int)nhdr + 40] = (byte)r.Tex.SurfFormat;
                }
            }
            return nb;
        }

        private static void CollectSections(SectionBase s, List<SectionBase> outList)
        {
            outList.Add(s);
            foreach (var c in s.ChildSections) CollectSections(c, outList);
        }

        private static long MapPos(long oldPos, List<SectionRep> reps)
        {
            long p = oldPos;
            foreach (var r in reps)
                if (oldPos >= r.DataPos + r.OldLen) p += r.Delta;
            return p;
        }

        private static void RewriteOffset(byte[] nb, long fieldPos, uint off, long secPos, List<SectionRep> reps)
        {
            if (off == NullOffset) return;
            long newOff = MapPos(secPos + off, reps) - MapPos(secPos, reps);
            WriteU32BE(nb, (int)fieldPos, (uint)newOff);
        }

        private static void WriteU32BE(byte[] d, int o, uint v)
        {
            d[o] = (byte)(v >> 24); d[o + 1] = (byte)(v >> 16); d[o + 2] = (byte)(v >> 8); d[o + 3] = (byte)v;
        }

        private static void WriteU16BE(byte[] d, int o, ushort v)
        {
            d[o] = (byte)(v >> 8); d[o + 1] = (byte)v;
        }

        private static uint ReadU32BE(byte[] d, int o)
        {
            return (uint)((d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3]);
        }

        private static ushort ReadU16BE(byte[] d, int o)
        {
            return (ushort)((d[o] << 8) | d[o + 1]);
        }

        //Single canonical big-endian float (de)serialization, shared by Emitter and Primitive. ReadF32BE reuses a
        //thread-static 4-byte scratch buffer so per-vertex mesh decoding doesn't allocate a throwaway array per read.
        [ThreadStatic] private static byte[] f32Scratch;
        private static float ReadF32BE(byte[] d, int o)
        {
            if (f32Scratch == null) f32Scratch = new byte[4];
            f32Scratch[0] = d[o + 3]; f32Scratch[1] = d[o + 2]; f32Scratch[2] = d[o + 1]; f32Scratch[3] = d[o];
            return BitConverter.ToSingle(f32Scratch, 0);
        }
        private static void WriteF32BE(byte[] d, int o, float f)
        {
            byte[] b = BitConverter.GetBytes(f);
            d[o] = b[3]; d[o + 1] = b[2]; d[o + 2] = b[1]; d[o + 3] = b[0];
        }

        public class WiiU
        {

        }


        public static readonly uint NullOffset = 0xFFFFFFFF;

        public class Header
        {
            public BNTX BinaryTextureFile = null;

            public string Signature;

            public ushort GraphicsAPIVersion;
            public ushort VFXVersion;
            public ushort ByteOrderMark;
            public byte Alignment;
            public byte TargetOffset;

            public ushort Flag;
            public ushort BlockOffset;

            public uint DataAlignment;

            //For saving
            public List<SectionBase> Sections = new List<SectionBase>();

            private string UnknownString;

            public void Read(FileReader reader, PTCL ptcl)
            {
                uint Position = (uint)reader.Position; //Offsets are relative to this

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                Signature = reader.ReadString(4, Encoding.ASCII);

                if (Signature == "EFTB")
                {
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                    reader.Seek(48, SeekOrigin.Begin);
                }
                else if (Signature == "VFXB")
                {
                    uint padding = reader.ReadUInt32();
                    GraphicsAPIVersion = reader.ReadUInt16();
                    VFXVersion = reader.ReadUInt16();
                    ByteOrderMark = reader.ReadUInt16();
                    Alignment = reader.ReadByte();
                    TargetOffset = reader.ReadByte();
                    uint HeaderSize = reader.ReadUInt32();
                    Flag = reader.ReadUInt16();
                    BlockOffset = reader.ReadUInt16();
                    uint padding2 = reader.ReadUInt32();
                    uint FileSize = reader.ReadUInt32();

                    reader.Seek(Position + BlockOffset, SeekOrigin.Begin);
                }
                else
                {
                    throw new Exception("Unknown ptcl format. Signature not valid " + Signature);
                }


                while (reader.Position < reader.BaseStream.Length)
                {
                    SectionBase sectionBase = new SectionBase();
                    sectionBase.Read(reader, this);
                    Sections.Add(sectionBase);
                    ptcl.Nodes.Add(sectionBase);

                    if (sectionBase.NextSectionOffset == NullOffset)
                        break;
                }

                MapTextureIDs(ptcl);
                MapPrimitiveIDs(ptcl);
                MapShaderInfo(ptcl);

                reader.Dispose();
                reader.Close();
            }
            //Read-only tree row under the "GTX Shader" node (a summary line + one per program-in-use).
            public class ShaderNode : TreeNodeCustom
            {
                public ShaderNode(string text) { Text = text; }
            }
            //Decode the SHDA shader bundle: tag every emitter with pool sizes + its shader group bytes, compute
            //this file's program-body hashes (cross-file catalog), and hang a usage overview under the GTX Shader
            //node (which emitters use each vtx/frag program, sampler count, and whether it's shared with another
            //open file).
            private void MapShaderInfo(PTCL ptcl)
            {
                if (Signature != "EFTB") return;
                int shdbPos, gfdPos, gfdLen; List<byte[]> blocks;
                if (!ParseGfd(ptcl.data, out shdbPos, out gfdPos, out gfdLen, out blocks)) return;
                List<byte[]> vtx, frag; GroupGfd(blocks, out vtx, out frag);

                var fragSamplers = new List<int>();
                foreach (var g in frag) fragSamplers.Add(FragSamplerVarCount(g));
                ptcl.ShaderVtxProgHashes = new HashSet<string>();
                foreach (var g in vtx) ptcl.ShaderVtxProgHashes.Add(ProgBodyHash(g));
                ptcl.ShaderFrgProgHashes = new HashSet<string>();
                foreach (var g in frag) ptcl.ShaderFrgProgHashes.Add(ProgBodyHash(g));

                var usage = new SortedDictionary<long, List<string>>();
                SectionBase gtxNode = null;
                foreach (var node in TreeViewExtensions.Collect(ptcl.Nodes))
                {
                    var sec = node as SectionBase;
                    if (sec == null) continue;
                    if (sec.Signature == "SHDB" && gtxNode == null) gtxNode = sec;
                    if (!(sec.BinaryData is Emitter)) continue;
                    var em = (Emitter)sec.BinaryData;
                    em.ShaderVtxCount = vtx.Count;
                    em.ShaderFrgCount = frag.Count;
                    em.FragSamplerCount = em.ShaderFrgIndex < frag.Count ? fragSamplers[(int)em.ShaderFrgIndex] : -1;
                    em.FragGroupBytes = em.ShaderFrgIndex < frag.Count ? frag[(int)em.ShaderFrgIndex] : null;
                    em.VtxGroupBytes = em.ShaderVtxIndex < vtx.Count ? vtx[(int)em.ShaderVtxIndex] : null;
                    if (em.DataPosition >= 0x50 && em.DataPosition <= ptcl.data.Length)
                    {
                        em.HeadBytes = new byte[0x50];
                        Array.Copy(ptcl.data, em.DataPosition - 0x50, em.HeadBytes, 0, 0x50);
                    }
                    long key = ((long)em.ShaderVtxIndex << 32) | em.ShaderFrgIndex;
                    if (!usage.ContainsKey(key)) usage[key] = new List<string>();
                    var parentSet = sec.Parent as SectionBase;
                    string set = parentSet != null ? parentSet.Text : null;
                    usage[key].Add(string.IsNullOrEmpty(set) ? sec.Text : set + "/" + sec.Text);
                }
                if (gtxNode == null || (vtx.Count == 0 && frag.Count == 0)) return;

                gtxNode.Nodes.Add(new ShaderNode($"{vtx.Count} vertex / {frag.Count} fragment shaders, {usage.Count} programs in use"));
                foreach (var kv in usage)
                {
                    uint v = (uint)(kv.Key >> 32), f = (uint)(kv.Key & 0xFFFFFFFF);
                    string extra = "";
                    if (f < frag.Count)
                    {
                        if (fragSamplers[(int)f] >= 0) extra += $"  -  frag samples {fragSamplers[(int)f]} tex";
                        var sharers = ShaderSharers(ProgBodyHash(frag[(int)f]), ptcl); //byte-identical frag in another open file
                        if (sharers.Count > 0) extra += $"  [shared with {string.Join(", ", sharers)}]";
                    }
                    gtxNode.Nodes.Add(new ShaderNode($"Program  vtx {v} / frag {f}  -  used by {kv.Value.Count}: {string.Join(", ", kv.Value)}{extra}"));
                }
            }
            private void MapTextureIDs(PTCL ptcl)
            {
                List<TextureDescriptor> texDescp = new List<TextureDescriptor>();
                List<Emitter> emitters = new List<Emitter>();
                BNTX bntx = ptcl.header.BinaryTextureFile;
                List<TEXR> botwTex = new List<TEXR>(); //Used for BOTW

                foreach (var node in TreeViewExtensions.Collect(ptcl.Nodes))
                {
                    if (node is TextureDescriptor)
                        texDescp.Add((TextureDescriptor)node);
                    if (node is SectionBase && ((SectionBase)node).BinaryData is Emitter)
                        emitters.Add((Emitter)((SectionBase)node).BinaryData);
                    if (node is SectionBase && ((SectionBase)node).BinaryData is TEXR)
                        botwTex.Add((TEXR)((SectionBase)node).BinaryData);
                }

                int index = 0;
                //Show the "Textures" root folder for every EFTB file (even with 0 textures) so it remains after Clear All Textures.
                if (Signature == "EFTB")
                {
                    TextureFolder textureFolder = new TextureFolder("Textures");
                    ptcl.Nodes.Add(textureFolder);

                    //The unique textures shown in the tree, in "Texture N" order; handed to every emitter so the
                    //editor's per-slot dropdowns can offer them (mirrors AvailablePrimitives for the mesh dropdown).
                    List<TEXR> available = new List<TEXR>();

                    //Build the unique-texture folder ONCE, deduping by data-buffer identity. Kept O(emitters+textures)
                    //so the File Explorer stays responsive on big files like GameResident.sesetlist.
                    var seen = new HashSet<byte[]>();
                    foreach (TEXR tex in botwTex)
                        if (seen.Add(tex.data))
                        {
                            tex.Text = "Texture " + index++;
                            textureFolder.Nodes.Add(tex);
                            available.Add(tex);
                        }

                    //Resolve each emitter's bound textures by sampler TextureID (duplicate binds preserved).
                    foreach (var emitter in emitters)
                        foreach (TEXR tex in botwTex)
                            foreach (var sampler in emitter.Samplers)
                                if (sampler.TextureID == tex.TextureID)
                                    emitter.DrawableTex.Add(tex);

                    foreach (var emitter in emitters) emitter.AvailableTextures = available;
                    ptcl.FileTextures.Clear();               // republished on every reparse, so drop the prior pass first
                    ptcl.FileTextures.AddRange(available);   // publish for cross-file resolution (sibling files' emitters)

                    //Hide the whole TEXA section from the tree; its per-texture "Texture Info" nodes were
                    //redundant with the "Textures" folder built above. TEXA stays in the section model
                    //(Header.Sections), so saving and texture-replace still find it via CollectSections().
                    SectionBase texaNode = null;
                    foreach (TreeNode tn in ptcl.Nodes)
                        if (tn is SectionBase tsec && tsec.Signature == "TEXA") { texaNode = tsec; break; }
                    if (texaNode != null) ptcl.Nodes.Remove(texaNode);
                }


                if (bntx == null)
                    return;

                foreach (var emitter in emitters)
                {
                    foreach (var tex in texDescp)
                    {
                        foreach (var sampler in emitter.Samplers)
                        {
                            if (sampler.TextureID == tex.TextureID)
                            {
                                if (bntx.Textures.ContainsKey(tex.TexName))
                                {
                                    emitter.DrawableTex.Add(bntx.Textures[tex.TexName]);
                                }
                            }
                        }
                    }
                }
            }

            //Index the file's PRIM table and hand it to every emitter, so the emitter editor's mesh dropdown can
            //resolve each emitter's primitive hash (EmitterData +0x87C). EFTB-only layout.
            private void MapPrimitiveIDs(PTCL ptcl)
            {
                if (Signature != "EFTB") return;

                var prims = new List<Primitive>();
                var emitterSections = new List<SectionBase>();
                foreach (var node in TreeViewExtensions.Collect(ptcl.Nodes))
                {
                    if (node is Primitive)
                        prims.Add((Primitive)node);
                    if (node is SectionBase && ((SectionBase)node).BinaryData is Emitter)
                        emitterSections.Add((SectionBase)node);
                }

                for (int i = 0; i < prims.Count; i++)
                {
                    //Unique node text + DrawableContainer name per primitive: the viewport picks the active model
                    //by container NAME (ReloadDrawables -> SelectItemByText), so identical "Primitive Mesh" names
                    //made every click resolve to the first primitive.
                    prims[i].Index = i;
                    prims[i].Text = "Primitive Mesh " + i;
                    if (prims[i].DrawableContainer != null)
                        prims[i].DrawableContainer.Name = prims[i].Text;
                }

                foreach (var sec in emitterSections)
                    ((Emitter)sec.BinaryData).AvailablePrimitives = prims; //drives the emitter editor's mesh dropdown
                ptcl.FilePrimitives.Clear();           // republished on every reparse, so drop the prior pass first
                ptcl.FilePrimitives.AddRange(prims);   // publish for cross-file resolution (sibling files' emitters)
            }

            public class TextureFolder : TreeNodeCustom, IContextMenuNode, ITextureContainer
            {
                public TextureFolder(string text)
                {
                    Text = text;
                }

                public bool DisplayIcons => true;

                public ToolStripItem[] GetContextMenuItems()
                {
                    return new ToolStripItem[]
                    {
                        new ToolStripMenuItem("Create Texture", null, CreateTexture),
                        new ToolStripMenuItem("Clear All Textures", null, ClearAllTextures),
                        new ToolStripMenuItem("Export All Textures", null, ExportAll, Keys.Control | Keys.A),
                    };
                }

                //Import an image as a new texture in the shared table. The folder hangs off the PTCL node.
                private void CreateTexture(object sender, EventArgs args)
                {
                    PTCL ptcl = null;
                    for (System.Windows.Forms.TreeNode n = Parent; n != null; n = n.Parent)
                        if (n is PTCL) { ptcl = (PTCL)n; break; }
                    if (ptcl == null) return;

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Filter = "Microsoft DDS|*.dds|Supported Images|*.png;*.bmp;*.tga;*.tiff|All files (*.*)|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                        ptcl.AddTextureFromImage(ofd.FileName);
                }

                //Clear the whole shared texture table (the Textures folder stays, now empty).
                private void ClearAllTextures(object sender, EventArgs args)
                {
                    PTCL ptcl = null;
                    for (System.Windows.Forms.TreeNode n = Parent; n != null; n = n.Parent)
                        if (n is PTCL) { ptcl = (PTCL)n; break; }
                    if (ptcl != null) ptcl.ClearAllTextures();
                }

                public List<STGenericTexture> TextureList
                {
                    get
                    {
                        List<STGenericTexture> textures = new List<STGenericTexture>();
                        foreach (STGenericTexture node in Nodes)
                            textures.Add(node);

                        return textures;
                    }
                    set { }
                }

                private void ExportAll(object sender, EventArgs args)
                {
                    List<string> Formats = new List<string>();
                    Formats.Add("Microsoft DDS (.dds)");
                    Formats.Add("Portable Graphics Network (.png)");
                    Formats.Add("Joint Photographic Experts Group (.jpg)");
                    Formats.Add("Bitmap Image (.bmp)");
                    Formats.Add("Tagged Image File Format (.tiff)");

                    FolderSelectDialog sfd = new FolderSelectDialog();

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = sfd.SelectedPath;

                        BatchFormatExport form = new BatchFormatExport(Formats);
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            foreach (STGenericTexture tex in Nodes)
                            {
                                if (form.Index == 0)
                                    tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                                else if (form.Index == 1)
                                    tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                                else if (form.Index == 2)
                                    tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                                else if (form.Index == 3)
                                    tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                                else if (form.Index == 4)
                                    tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                            }
                        }
                    }
                }
            }

            private TreeNodeFile GetMagic(SectionBase section)
            {
                TreeNodeFile node = new TreeNodeFile();
                node.Text = section.Signature;

                foreach (var child in section.ChildSections)
                {
                    node.Nodes.Add(GetMagic(child));
                }

                return node;
            }
            public void Write(FileWriter writer)
            {
                writer.WriteSignature("VFXB");
                writer.Write(0x20202020);
                writer.Write(GraphicsAPIVersion);
                writer.Write(VFXVersion);
                writer.Write(ByteOrderMark);
                writer.Write(Alignment);
                writer.Write(TargetOffset);
                writer.Write(32);
                writer.Write(Flag);
                writer.Write(BlockOffset);
                writer.Write(0);
                long _ofsFileSize = writer.Position;
                writer.Write(0);
                writer.Seek(BlockOffset, SeekOrigin.Begin);

                foreach (var section in Sections)
                {
                    writer.Align(8);
                    section.Write(writer, this);
                }

                using (writer.TemporarySeek(_ofsFileSize, SeekOrigin.Begin))
                {
                    writer.Write(writer.BaseStream.Length);
                }

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        static bool ChildHasBinary = false;

        //  public static readonly uint NullOffset = 0xFFFFFFFF;


        public class Emitter : TreeNodeCustom
        {
            public List<STGenericTexture> DrawableTex = new List<STGenericTexture>();
            public List<SamplerInfo> Samplers = new List<SamplerInfo>();

            public STColor ConstantColor0;
            public STColor ConstantColor1;

            public STColor[] Color0Array = new STColor[8];
            public STColor[] Color1Array = new STColor[8];

            public STColor[] Color0AlphaArray = new STColor[8];
            public STColor[] Color1AlphaArray = new STColor[8];

            public STColor ConstantAlpha0
            {
                get
                {
                    return new STColor()
                    {
                        R = ConstantColor0.A,
                        G = ConstantColor0.A,
                        B = ConstantColor0.A,
                    };
                }
            }

            public STColor ConstantAlpha1
            {
                get
                {
                    return new STColor()
                    {
                        R = ConstantColor1.A,
                        G = ConstantColor1.A,
                        B = ConstantColor1.A,
                    };
                }
            }

            public enum ColorType
            {
                Constant,
                Random,
                Animated8Key,
            }

            public ColorType Color0Type;
            public ColorType Alpha0Type;
            public ColorType Color1Type;
            public ColorType Alpha1Type;

            private bool HasTime(STColor[] colors)
            {
                for (int i = 0; i < colors.Length; i++)
                    if (colors[i].Time != 0) return true;

                return false;
            }

            private void SetType(bool HasKeys, STColor[] colors, int type, bool isAlpha)
            {
                if (HasKeys)
                {
                    if (HasTime(colors))
                        SetType(type, isAlpha, ColorType.Animated8Key);
                    else
                        SetType(type, isAlpha, ColorType.Random);
                }
                else
                    SetType(type, isAlpha, ColorType.Constant);
            }

            private void SetType(int type, bool isAlpha, ColorType colorType)
            {
                if (type == 0)
                {
                    if (!isAlpha)
                        Color0Type = colorType;
                    else
                        Alpha0Type = colorType;
                }
                if (type == 1)
                {
                    if (!isAlpha)
                        Color1Type = colorType;
                    else
                        Alpha1Type = colorType;
                }
            }

            public uint Color0KeyCount;
            public uint Alpha0KeyCount;
            public uint Color1KeyCount;
            public uint Alpha1KeyCount;

            //--- Colour-track type conversion ---------------------------------------------------------------
            //The displayed type (Constant/Random/Animated8Key) is DERIVED (see SetType): key count 0 = Constant;
            //count>0 with no Time = Random; count>0 with a Time = Animated8Key. Converting therefore means
            //rewriting the count + the 8-key array, NOT a stored "type" field. The count persists via EmitterData
            //(+0x10/+0x18/+0x14/+0x1C, NOT re-emitted by Write), while the 8-key array and constant colour are the
            //in-memory fields Write overlays on save, so this edits both. New keys are seeded from the current
            //constant colour so the particle's look doesn't jump until the user edits the keys.
            //row: 0=Color0, 1=Color1, 2=Alpha0, 3=Alpha1. Returns true if anything changed.
            public bool SetColorTrackType(int row, ColorType target)
            {
                if (EmitterData == null) return false;
                STColor[] arr; int countOff; ColorType current;
                switch (row)
                {
                    case 0: arr = Color0Array;      countOff = 0x10; current = Color0Type; break;
                    case 1: arr = Color1Array;      countOff = 0x18; current = Color1Type; break;
                    case 2: arr = Color0AlphaArray; countOff = 0x14; current = Alpha0Type; break;
                    case 3: arr = Color1AlphaArray; countOff = 0x1C; current = Alpha1Type; break;
                    default: return false;
                }
                if (current == target || countOff + 4 > EmitterData.Length) return false;

                uint curCount = ReadU32BE(EmitterData, countOff);
                STColor rep = (current == ColorType.Constant) ? GetTrackConstant(row) : CloneColor(arr[0]);

                uint newCount;
                if (target == ColorType.Constant)
                {
                    newCount = 0;
                    SetTrackConstant(row, rep); //keep the visible colour
                }
                else
                {
                    bool fromConstant = (current == ColorType.Constant);
                    //Random needs >=1 key, Animated8Key needs >=2 to interpolate; cap at the 8-slot array.
                    newCount = target == ColorType.Animated8Key
                        ? (curCount >= 2 ? Math.Min(curCount, 8u) : 2u)
                        : (curCount >= 1 ? Math.Min(curCount, 8u) : 2u);

                    for (int i = 0; i < 8; i++)
                    {
                        if (fromConstant || i >= curCount) { arr[i].R = rep.R; arr[i].G = rep.G; arr[i].B = rep.B; }
                        if (target == ColorType.Random) arr[i].Time = 0f; //no timeline -> Random
                    }
                    if (target == ColorType.Animated8Key)
                        for (int i = 0; i < newCount; i++) //times span 0..1 so the last key carries a non-zero Time
                            arr[i].Time = newCount <= 1 ? 1f : (float)i / (newCount - 1);
                }

                WriteU32BE(EmitterData, countOff, newCount);
                SetTrackCountAndType(row, newCount, target);
                return true;
            }

            private static STColor CloneColor(STColor c) { return new STColor() { R = c.R, G = c.G, B = c.B, A = c.A, Time = c.Time }; }
            //Representative colour of a track's constant (alpha tracks carry the value in .A / in rep.R).
            private STColor GetTrackConstant(int row)
            {
                switch (row)
                {
                    case 0: return new STColor() { R = ConstantColor0.R, G = ConstantColor0.G, B = ConstantColor0.B };
                    case 1: return new STColor() { R = ConstantColor1.R, G = ConstantColor1.G, B = ConstantColor1.B };
                    case 2: return new STColor() { R = ConstantColor0.A, G = ConstantColor0.A, B = ConstantColor0.A };
                    default: return new STColor() { R = ConstantColor1.A, G = ConstantColor1.A, B = ConstantColor1.A };
                }
            }
            private void SetTrackConstant(int row, STColor rep)
            {
                switch (row)
                {
                    case 0: ConstantColor0.R = rep.R; ConstantColor0.G = rep.G; ConstantColor0.B = rep.B; break;
                    case 1: ConstantColor1.R = rep.R; ConstantColor1.G = rep.G; ConstantColor1.B = rep.B; break;
                    case 2: ConstantColor0.A = rep.R; break; //alpha stored in ConstantColor0.A
                    case 3: ConstantColor1.A = rep.R; break;
                }
            }
            private void SetTrackCountAndType(int row, uint count, ColorType t)
            {
                switch (row)
                {
                    case 0: Color0KeyCount = count; Color0Type = t; break;
                    case 1: Color1KeyCount = count; Color1Type = t; break;
                    case 2: Alpha0KeyCount = count; Alpha0Type = t; break;
                    case 3: Alpha1KeyCount = count; Alpha1Type = t; break;
                }
            }

            //--- Keyframe editing (add / remove / move) for the Animated8Key timeline -------------------------
            //Operate on a track's 8-slot STColor array (persisted by Write's overlay) + its key count (persisted
            //in EmitterData). Keys are kept time-sorted because the slider's gradient (and the game) expect it.
            public STColor[] GetColorArray(int row)
            {
                switch (row) { case 0: return Color0Array; case 1: return Color1Array; case 2: return Color0AlphaArray; default: return Color1AlphaArray; }
            }
            public uint GetColorKeyCount(int row)
            {
                switch (row) { case 0: return Color0KeyCount; case 1: return Color1KeyCount; case 2: return Alpha0KeyCount; default: return Alpha1KeyCount; }
            }
            private int ColorCountOffset(int row)
            {
                switch (row) { case 0: return 0x10; case 1: return 0x18; case 2: return 0x14; default: return 0x1C; }
            }
            //Write a track's key count to BOTH the in-memory field and EmitterData (+0x10/0x18/0x14/0x1C), where it
            //persists on save (Write re-emits the whole struct but not the count, so it must live in EmitterData).
            private void SetColorKeyCount(int row, uint n)
            {
                int off = ColorCountOffset(row);
                if (EmitterData != null && off + 4 <= EmitterData.Length) WriteU32BE(EmitterData, off, n);
                switch (row) { case 0: Color0KeyCount = n; break; case 1: Color1KeyCount = n; break; case 2: Alpha0KeyCount = n; break; default: Alpha1KeyCount = n; break; }
            }

            private static float Clamp01(float t) { return t < 0f ? 0f : (t > 1f ? 1f : t); }

            //Insert a keyframe at `time` (0..1), colour linearly interpolated from the surrounding keys so it sits
            //on the existing gradient. Returns the new key's index, or -1 if the track is full (8 keys).
            public int AddColorKey(int row, float time)
            {
                var arr = GetColorArray(row); int count = (int)GetColorKeyCount(row);
                if (count >= 8 || arr == null) return -1;
                time = Clamp01(time);
                int pos = count;
                for (int i = 0; i < count; i++) if (time < arr[i].Time) { pos = i; break; }
                STColor nk = InterpColorAt(arr, count, time);
                for (int i = count; i > pos; i--) arr[i] = arr[i - 1];
                arr[pos] = nk;
                SetColorKeyCount(row, (uint)(count + 1));
                return pos;
            }

            //Remove the keyframe at `index` (no minimum: a track may go to 0 keys, which derives to Constant).
            public bool RemoveColorKey(int row, int index)
            {
                var arr = GetColorArray(row); int count = (int)GetColorKeyCount(row);
                if (arr == null || index < 0 || index >= count) return false;
                for (int i = index; i < count - 1; i++) arr[i] = arr[i + 1];
                SetColorKeyCount(row, (uint)(count - 1));
                return true;
            }

            //Set a keyframe's time and re-sort the active keys; returns the index the key ended up at (it may move
            //past neighbours). Times are freely placed on 0..1, endpoints included.
            public int SetColorKeyTime(int row, int index, float time)
            {
                var arr = GetColorArray(row); int count = (int)GetColorKeyCount(row);
                if (arr == null || index < 0 || index >= count) return index;
                arr[index].Time = Clamp01(time);
                STColor moved = arr[index];
                var list = new List<STColor>();
                for (int i = 0; i < count; i++) list.Add(arr[i]);
                list.Sort((a, b) => a.Time.CompareTo(b.Time));
                for (int i = 0; i < count; i++) arr[i] = list[i];
                return list.IndexOf(moved);
            }

            private static STColor InterpColorAt(STColor[] arr, int count, float time)
            {
                if (count == 0) return new STColor() { R = 1f, G = 1f, B = 1f, A = 1f, Time = time };
                STColor lo = arr[0], hi = arr[count - 1];
                for (int i = 0; i < count; i++) if (arr[i].Time <= time) lo = arr[i];
                for (int i = count - 1; i >= 0; i--) if (arr[i].Time >= time) hi = arr[i];
                float span = hi.Time - lo.Time;
                float f = span <= 0f ? 0f : (time - lo.Time) / span;
                return new STColor()
                {
                    R = lo.R + (hi.R - lo.R) * f,
                    G = lo.G + (hi.G - lo.G) * f,
                    B = lo.B + (hi.B - lo.B) * f,
                    A = lo.A + (hi.A - lo.A) * f,
                    Time = time
                };
            }

            //Absolute offset of this emitter's data within the decompressed file.
            //Used on save to patch edited colour values back into the original bytes.
            public uint DataPosition;

            //Full emitter struct bytes (big endian). Parameter edits go into this buffer and, on save, the
            //whole struct is written back in place, so any field, documented or reverse-engineered, persists.
            public byte[] EmitterData;

            //Last "Probe" offset typed in the editor's Parameters tab. Stored here (not on the throwaway
            //EmitterParameters wrapper) so it survives switching between emitters.
            public string ProbeOffsetHex = "";

            private float GetFloat(int off)
            {
                if (EmitterData == null || off < 0 || off + 4 > EmitterData.Length) return 0f;
                return ReadF32BE(EmitterData, off);
            }
            private void SetFloat(int off, float v)
            {
                if (EmitterData == null || off < 0 || off + 4 > EmitterData.Length) return;
                WriteF32BE(EmitterData, off, v);
            }
            public float GetFloatAt(int off) { return GetFloat(off); }
            public void SetFloatAt(int off, float v) { SetFloat(off, v); }
            public uint GetU32At(int off) { return GetU32(off); }
            public void SetU32At(int off, uint v) { SetU32(off, v); }
            public byte GetByteAt(int off) { return (EmitterData != null && off >= 0 && off < EmitterData.Length) ? EmitterData[off] : (byte)0; }
            public void SetByteAt(int off, byte v) { if (EmitterData != null && off >= 0 && off < EmitterData.Length) EmitterData[off] = v; }

            // The color/alpha tracks are edited as STColor[] arrays (Color0Array etc.); they read from / save to the file at
            // the SAME struct offsets the renderer uses (color0 0x370, alpha0 0x3F0, color1 0x470, alpha1 0x4F0; consts 0x958/
            // 0x968) but only flush to EmitterData on Save. The live preview renders from EmitterData, so flush the arrays in
            // first -> a colour edit shows immediately. Format = (R,G,B,Time) per key, matching the renderer's read.
            public void FlushColorsToData()
            {
                if (EmitterData == null) return;
                WriteColorTrack(Color0Array, 0x370); WriteColorTrack(Color0AlphaArray, 0x3F0);
                WriteColorTrack(Color1Array, 0x470); WriteColorTrack(Color1AlphaArray, 0x4F0);
                if (ConstantColor0 != null) { SetFloat(0x958, ConstantColor0.R); SetFloat(0x95C, ConstantColor0.G); SetFloat(0x960, ConstantColor0.B); SetFloat(0x964, ConstantColor0.A); }
                if (ConstantColor1 != null) { SetFloat(0x968, ConstantColor1.R); SetFloat(0x96C, ConstantColor1.G); SetFloat(0x970, ConstantColor1.B); SetFloat(0x974, ConstantColor1.A); }
            }
            private void WriteColorTrack(STColor[] arr, int off)
            {
                if (arr == null) return;
                for (int k = 0; k < 8 && k < arr.Length; k++)
                {
                    if (arr[k] == null) continue;
                    SetFloat(off + k * 16, arr[k].R); SetFloat(off + k * 16 + 4, arr[k].G);
                    SetFloat(off + k * 16 + 8, arr[k].B); SetFloat(off + k * 16 + 12, arr[k].Time);
                }
            }

            private uint GetU32(int off)
            {
                if (EmitterData == null || off < 0 || off + 4 > EmitterData.Length) return 0;
                return ReadU32BE(EmitterData, off);
            }

            //Primitive-mesh reference: a hash stored at emitter offset 0x87C, matched against each PRIM block's
            //+0x04 id. 0 or 0xFFFFFFFF means this emitter draws no primitive (billboard / stripe particle).
            public uint PrimitiveHash { get { return GetU32(EmtPrimHashOff); } }
            public uint ShaderVtxIndex { get { return GetU32(EmtShaderVtxOff); } }   //index into the file's SHDA vertex-shader pool
            public uint ShaderFrgIndex { get { return GetU32(EmtShaderFrgOff); } }   //index into the file's SHDA fragment-shader pool

            //Shader pool sizes + decoded info, set by MapShaderInfo; drive the editor dropdowns + the usage view.
            public int ShaderVtxCount = 0;    //number of vertex shaders in this file's bundle
            public int ShaderFrgCount = 0;    //number of fragment shaders
            public int FragSamplerCount = -1; //texture samplers this emitter's fragment shader declares (-1 = unknown)
            public byte[] FragGroupBytes;     //this emitter's fragment-shader GFD group (header ++ program)
            public byte[] VtxGroupBytes;      //this emitter's vertex-shader GFD group
            public byte[] HeadBytes;          //the 0x50 data-frame head preceding EmitterData (name @0x10); head + EmitterData = the GPU-preview payload
            //Re-point this emitter to a different existing shader in the pool (index clamped); bakes on save.
            public void SetShaderVtxIndex(uint v) { if (ShaderVtxCount > 0 && v > ShaderVtxCount - 1) v = (uint)(ShaderVtxCount - 1); SetU32(EmtShaderVtxOff, v); }
            public void SetShaderFrgIndex(uint v) { if (ShaderFrgCount > 0 && v > ShaderFrgCount - 1) v = (uint)(ShaderFrgCount - 1); SetU32(EmtShaderFrgOff, v); }
            //The file's primitive table (set by MapPrimitiveIDs); drives the mesh dropdown in the emitter editor.
            public List<Primitive> AvailablePrimitives = new List<Primitive>();

            //All textures in the file (set by MapTextureIDs); drives the per-slot texture dropdowns in the editor.
            public List<TEXR> AvailableTextures = new List<TEXR>();
            //Byte offset of sampler 0's TextureID within EmitterData; captured from the version-dependent seek in Read.
            public int SamplerBaseOffset = 0x9A8;
            //If this emitter links a primitive hash that isn't in THIS file ("(external mesh)"), remember it so the
            //user can revert after switching the dropdown away. In-memory only, not persisted across save/reload.
            public uint ExternalPrimHash = 0;

            private void SetU32(int off, uint v)
            {
                if (EmitterData == null || off < 0 || off + 4 > EmitterData.Length) return;
                WriteU32BE(EmitterData, off, v);
            }

            //Labels for the editable mesh link. "none (billboarding)" = no primitive (the default billboard quad);
            //"(external mesh)" = a hash referenced by this emitter but absent from THIS file's primitive table.
            public const string PrimNoneLabel = "none (billboarding)";
            public const string PrimExternalLabel = "(external mesh)";

            //Dropdown choices: none + one entry per primitive in the file, plus "(external mesh)" when this emitter
            //currently or previously referenced a mesh from another file (so the user can revert to it).
            public List<string> PrimitiveOptions()
            {
                var list = new List<string>();
                list.Add(PrimNoneLabel);
                foreach (var p in AvailablePrimitives) list.Add("Primitive " + p.Index);
                if (ExternalPrimHash != 0 && ExternalPrimHash != 0xFFFFFFFF) list.Add(PrimExternalLabel);
                return list;
            }
            public string GetPrimitiveSelection()
            {
                uint h = PrimitiveHash;
                if (h == 0 || h == 0xFFFFFFFF) return PrimNoneLabel;
                foreach (var p in AvailablePrimitives) if (p.Hash == h) return "Primitive " + p.Index;
                ExternalPrimHash = h;          //remember it so the "(external mesh)" revert option stays available
                return PrimExternalLabel;
            }
            //Write the primitive descriptor: enable flag (0x874), 0x878, and the hash (0x87C). These mirror the
            //bytes of emitters that natively draw a primitive; "none (billboarding)" restores the billboard pattern.
            public void SetPrimitiveSelection(string sel)
            {
                if (AvailablePrimitives == null || AvailablePrimitives.Count == 0) return; //EFTB-only; nothing to link
                if (EmitterData == null || string.IsNullOrEmpty(sel)) return;
                if (sel == PrimExternalLabel)
                {
                    //Revert to the remembered external-mesh hash (no-op if we never observed one).
                    if (ExternalPrimHash != 0 && ExternalPrimHash != 0xFFFFFFFF)
                    { SetU32(EmtPrimEnableOff, EmtPrimEnableVal); SetU32(EmtPrimUnkOff, 0); SetU32(EmtPrimHashOff, ExternalPrimHash); }
                    return;
                }
                if (sel == PrimNoneLabel)
                {
                    SetU32(EmtPrimEnableOff, 0); SetU32(EmtPrimUnkOff, 0xFFFFFFFF); SetU32(EmtPrimHashOff, 0xFFFFFFFF);
                }
                else
                {
                    int idx;
                    if (int.TryParse(sel.Replace("Primitive", "").Trim(), out idx))
                        foreach (var p in AvailablePrimitives)
                            if (p.Index == idx) { SetU32(EmtPrimEnableOff, EmtPrimEnableVal); SetU32(EmtPrimUnkOff, 0); SetU32(EmtPrimHashOff, p.Hash); break; }
                }
            }

            //--- Texture samplers (Texture0/1/2) ------------------------------------------------------------
            //Empty slot = the sentinel 0xFFFFFFFFFFFFFFFF (both dwords FF). A used slot stores the texture id in
            //the low dword of a big-endian u64 (high dword 0). Wrap/filter/flag bytes occupy the trailing 24
            //bytes and differ between enabled and disabled, so enabling an empty slot copies a full sampler
            //template (from a sibling slot, else a baked default) rather than writing the id alone.
            public const string TexNoneLabel = "none";
            //An id not in THIS file's table. In shipped BotW this is NORMALLY a texture from a shared/global pool
            //(most emitters reference these; e.g. AssassinWindCutter has 1 local texture but its emitters sample
            //~7 shared ones); it can also be a locally-deleted texture. The hash is appended so the reference is
            //identifiable and re-selectable (re-applying the label restores the exact id).
            public const string TexExternalPrefix = "shared 0x";

            //A disabled sampler exactly as the game ships empty slots (id all-FF, wrap 0, canonical tail).
            private static readonly byte[] DisabledSampler = {
                0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF, 0x00,0x00, 0x00,0x00,0x41,0x7F,0xD7,0x0A,
                0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00 };
            //Fallback enabled sampler (a real slot: wrap 1/1, 0x417FD70A tail, enable flag at 0x18). The id bytes
            //are a placeholder the caller overwrites. Used only when an emitter has no enabled sibling to clone.
            private static readonly byte[] DefaultEnabledSampler = {
                0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x01,0x01, 0x00,0x00,0x41,0x7F,0xD7,0x0A,
                0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00, 0x01,0x00,0x00,0x00, 0x00,0x00,0x00,0x00 };

            private int SamplerOff(int slot) { return SamplerBaseOffset + slot * EmtSamplerStride; }
            private bool SamplerInRange(int slot)
            {
                int o = SamplerOff(slot);
                return EmitterData != null && slot >= 0 && slot < EmtSamplerCount && o + EmtSamplerStride <= EmitterData.Length;
            }
            private bool IsSamplerEmpty(int off)
            {
                return ReadU32BE(EmitterData, off) == 0xFFFFFFFF && ReadU32BE(EmitterData, off + 4) == 0xFFFFFFFF;
            }

            //Texture id bound to slot i, or 0xFFFFFFFF when the slot is the empty sentinel.
            public uint GetSamplerTextureId(int slot)
            {
                if (!SamplerInRange(slot)) return 0xFFFFFFFF;
                int o = SamplerOff(slot);
                if (IsSamplerEmpty(o)) return 0xFFFFFFFF;
                return ReadU32BE(EmitterData, o + 4); //id = low dword (high dword is 0 on a live slot)
            }

            //Dropdown choices for a texture slot: none + one entry per texture in the file.
            public List<string> TextureOptions()
            {
                var list = new List<string>();
                list.Add(TexNoneLabel);
                foreach (var t in AvailableTextures) list.Add(TexLabel(t));
                return list;
            }
            private static string TexLabel(TEXR t)
            {
                return string.IsNullOrEmpty(t.Text) ? ("Texture " + t.TextureID.ToString("x")) : t.Text;
            }
            //The file texture bound to slot i (null = none, or an id not present in this file's table).
            public TEXR GetSamplerTexture(int slot)
            {
                uint id = GetSamplerTextureId(slot);
                if (id == 0 || id == 0xFFFFFFFF) return null;
                foreach (var t in AvailableTextures) if (t.TextureID == id) return t;
                return FindGlobalTexture(id) as TEXR;   // cross-file: preview + click-navigate to a sibling file's texture
            }
            //Current dropdown value for slot i. A "shared 0x........" value means the id isn't in this file's
            //table (a shared/global-pool texture, the norm in shipped files, or a locally-deleted one).
            public string GetTextureSelection(int slot)
            {
                uint id = GetSamplerTextureId(slot);
                if (id == 0 || id == 0xFFFFFFFF) return TexNoneLabel;
                foreach (var t in AvailableTextures) if (t.TextureID == id) return TexLabel(t);
                return TexExternalPrefix + id.ToString("X8");
            }
            //Point slot i at a texture, clear it, or re-apply a shared/external id. Mirrors SetPrimitiveSelection:
            //edits EmitterData in place so the change bakes on save with no section shift / realign needed.
            public void SetTextureSelection(int slot, string sel)
            {
                if (!SamplerInRange(slot) || string.IsNullOrEmpty(sel)) return;
                int o = SamplerOff(slot);
                if (sel == TexNoneLabel) { Array.Copy(DisabledSampler, 0, EmitterData, o, EmtSamplerStride); return; }
                if (sel.StartsWith(TexExternalPrefix))
                {
                    //Re-apply a shared/external id (the hash is encoded in the label), letting the user revert after
                    //switching the slot away, exactly like the "(external mesh)" primitive case.
                    uint ext;
                    if (uint.TryParse(sel.Substring(TexExternalPrefix.Length), System.Globalization.NumberStyles.HexNumber, null, out ext))
                        AssignSampler(slot, ext);
                    return;
                }
                foreach (var t in AvailableTextures)
                    if (TexLabel(t) == sel) { AssignSampler(slot, t.TextureID); return; }
            }
            private void AssignSampler(int slot, uint id)
            {
                int o = SamplerOff(slot);
                if (IsSamplerEmpty(o)) //enabling a previously-empty slot: seed valid wrap/filter/flags first
                    Array.Copy(EnabledSamplerTemplate(slot), 0, EmitterData, o, EmtSamplerStride);
                WriteU32BE(EmitterData, o, 0);      //high dword 0
                WriteU32BE(EmitterData, o + 4, id); //texture id
            }
            //Borrow a full 0x20 enabled-sampler template so a freshly-enabled slot has valid parameters: prefer a
            //sibling slot already in use by THIS emitter, else the baked default.
            private byte[] EnabledSamplerTemplate(int targetSlot)
            {
                for (int s = 0; s < EmtSamplerCount; s++)
                {
                    if (s == targetSlot || !SamplerInRange(s)) continue;
                    int o = SamplerOff(s);
                    if (!IsSamplerEmpty(o))
                    {
                        var b = new byte[EmtSamplerStride];
                        Array.Copy(EmitterData, o, b, 0, EmtSamplerStride);
                        return b;
                    }
                }
                return (byte[])DefaultEnabledSampler.Clone();
            }

            //Documented EFTB emitter fields (ZeldaMods PTCL offset minus 0x50 = this tool's emitter offset).
            public float Radius { get { return GetFloat(0x360); } set { SetFloat(0x360, value); } }
            public float BlinkIntensity1 { get { return GetFloat(0x90); } set { SetFloat(0x90, value); } }
            public float BlinkIntensity2 { get { return GetFloat(0x94); } set { SetFloat(0x94, value); } }
            public float BlinkDuration1 { get { return GetFloat(0x98); } set { SetFloat(0x98, value); } }
            public float BlinkDuration2 { get { return GetFloat(0x9C); } set { SetFloat(0x9C, value); } }
            public float Scale0X { get { return GetFloat(0x5B0); } set { SetFloat(0x5B0, value); } }
            public float Scale0Y { get { return GetFloat(0x5B4); } set { SetFloat(0x5B4, value); } }

            // --- Empirically identified across all 866 .sesetlist files (statistical RE, 2026-06; struct-frame offsets) ---
            // Blend / render-state mode @0x8DC: strongest signal in the whole library (separation 0.77 over 8244 emitters).
            // Value 0 dominates additive-named emitters (fire/light/spark/glow), value 3 dominates alpha-named (smoke/dust/cloud),
            // value 4 = other (sub/mult?). So 0 => additive look, 3 => normal/alpha. Exact enum mapping pending Cemu confirmation.
            public uint BlendRenderMode { get { return GetU32(0x8DC); } set { SetU32(0x8DC, value); } }
            // Emission-shape / category selector @0x714 (values 0..21, clustered at 6/7/8). Likely emitFunction/volume-shape.
            public uint EmitShapeRaw { get { return GetU32(0x714); } set { SetU32(0x714, value); } }

            //Property wrapper shown in the editor's "Parameters" tab. Documented fields are named; the Probe
            //lets you read/write a float at any offset to reverse-engineer the undocumented ones (speed,
            //lifetime, emission rate, ...): edit, save, test in-game, repeat.
            // Enums for the Parameters PropertyGrid (rendered as dropdowns). Values = the on-disk struct byte/u32.
            public enum BlendMode : byte { Normal_Alpha = 0, Additive = 1 }
            public enum DisplaySideMode : byte { Both = 0, Front = 1, Back = 2 }
            public enum ZBufTestMode : byte { Normal = 0, Ignore_Z = 1 }
            public enum FragAlphaMode : byte { Default0 = 0, Multiply1 = 1, Subtract_Erosion3 = 3, Mode4 = 4 }
            public enum CombineOp : byte { Mul = 0, Add = 1, Sub = 2, Max = 3 }
            public enum EmitShape : uint { Point = 0, Circle = 1, CircleSameDivide = 2, FillCircle = 3, Sphere = 4,
                SphereSameDivide = 5, Sphere64 = 6, FillSphere = 7, Cylinder = 8, FillCylinder = 9, Box = 10,
                FillBox = 11, Line = 12, LineSameDivide = 13, Rectangle = 14 }
            public enum VtxTransformMode : uint { Billboard = 0, Plate_XY = 1, Plate_XZ = 2, Directional_Y = 3, Directional_Polygon = 4 }
            // GX2 texture address mode per sampler axis (struct sampler+0x08 wrapU / +0x09 wrapV). PINNED by RenderDoc cross-ref:
            // Smoke_Botttom Wrap/Clamp=1,2 ; Wind_sub Clamp/Clamp=2,2 ; Gdn_Target reticle ring Mirror/Mirror=0,0.
            public enum SamplerWrap : byte { Mirror = 0, Wrap = 1, Clamp = 2 }
            // Alpha-fluctuation waveform @0x99F, driving bank7 word0 bits 0/1/2 (the vertex shader sums one term per
            // waveform, so a value outside this set leaves alpha at zero and the emitter renders nothing). These three
            // are the whole library: 8 = 8806 emitters, 16 = 563, 32 = 357, never combined and never absent.
            public enum FluctuationWaveform : byte { Sine = 8, Random = 16, Blink = 32 }

            public class EmitterParameters
            {
                private Emitter e;
                public EmitterParameters(Emitter emitter) { e = emitter; }

                //--- Shader re-point: pick which compiled GX2 shader in this file's SHDA bundle this emitter binds.
                //The dropdown lists 0..pool-1 (also typeable); the change bakes on save. NOTE: the live preview runs
                //its own GLSL, not the file's GX2 shaders, so re-pointing won't change the preview; use right-click
                //"View Shader (GLSL)" to inspect the actual shader and test the look in-game.
                [Category("1c. Shader"), DisplayName("Vertex shader index"), TypeConverter(typeof(ShaderIndexConverter)),
                 Description("Index of the vertex shader this emitter binds in the file's SHDA bundle. Offset 0x8C4.")]
                public int ShaderVertexIndex { get { return (int)e.ShaderVtxIndex; } set { e.SetShaderVtxIndex((uint)Math.Max(0, value)); } }
                [Category("1c. Shader"), DisplayName("Fragment shader index"), TypeConverter(typeof(ShaderIndexConverter)),
                 Description("Index of the fragment (pixel) shader this emitter binds. Offset 0x8C8. Changes the look/combiner.")]
                public int ShaderFragmentIndex { get { return (int)e.ShaderFrgIndex; } set { e.SetShaderFrgIndex((uint)Math.Max(0, value)); } }
                [Category("1c. Shader"), DisplayName("Shader pool (read-only)"),
                 Description("Vertex / fragment shader counts in this file's SHDA bundle (the valid index range).")]
                public string ShaderPool { get { return e.ShaderVtxCount + " vertex / " + e.ShaderFrgCount + " fragment"; } }
                [Category("1c. Shader"), DisplayName("Fragment samples (read-only)"),
                 Description("Texture samplers this emitter's fragment shader declares (-1 = unknown).")]
                public int FragmentSamplerCount { get { return e.FragSamplerCount; } }

                //Dropdown of valid shader indices (0..pool-1) for the two index fields above; non-exclusive so a
                //value can still be typed. Reads the pool size off the wrapped emitter, vtx vs frag by property name.
                private class ShaderIndexConverter : Int32Converter
                {
                    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
                    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
                    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                    {
                        var ep = context != null ? context.Instance as EmitterParameters : null;
                        bool vtx = context != null && context.PropertyDescriptor != null && context.PropertyDescriptor.Name == "ShaderVertexIndex";
                        int n = ep == null ? 0 : (vtx ? ep.e.ShaderVtxCount : ep.e.ShaderFrgCount);
                        if (n < 0) n = 0;
                        var l = new int[n]; for (int i = 0; i < n; i++) l[i] = i;
                        return new StandardValuesCollection(l);
                    }
                }

                [Category("1. Documented"), DisplayName("Radius"), Description("Particle radius / base size (ZeldaMods). Offset 0x360.")]
                public float Radius { get { return e.Radius; } set { e.Radius = value; } }
                [Category("1. Documented"), DisplayName("Blink Intensity 1"), Description("ZeldaMods 'Blink Intensity 1'. Offset 0x90.")]
                public float BlinkIntensity1 { get { return e.BlinkIntensity1; } set { e.BlinkIntensity1 = value; } }
                [Category("1. Documented"), DisplayName("Blink Intensity 2"), Description("ZeldaMods 'Blink Intensity 2'. Offset 0x94.")]
                public float BlinkIntensity2 { get { return e.BlinkIntensity2; } set { e.BlinkIntensity2 = value; } }
                [Category("1. Documented"), DisplayName("Blink Duration 1"), Description("ZeldaMods 'Blink Duration 1'. Offset 0x98.")]
                public float BlinkDuration1 { get { return e.BlinkDuration1; } set { e.BlinkDuration1 = value; } }
                [Category("1. Documented"), DisplayName("Blink Duration 2"), Description("ZeldaMods 'Blink Duration 2'. Offset 0x9C.")]
                public float BlinkDuration2 { get { return e.BlinkDuration2; } set { e.BlinkDuration2 = value; } }
                [Category("1. Documented"), DisplayName("Fluctuation Waveform"), Description("Waveform the Blink Intensity/Duration pairs above drive the particle alpha with. Offset 0x99F: Sine(8), Random(16) or Blink(32). Every emitter in the library carries one of the three.")]
                public FluctuationWaveform Fluctuation { get { return (FluctuationWaveform)e.GetByteAt(0x99F); } set { e.SetByteAt(0x99F, (byte)value); } }
                [Category("1. Documented"), DisplayName("Scale X"), Description("First scale-array entry, X (ZeldaMods scale array). Offset 0x5B0.")]
                public float ScaleX { get { return e.Scale0X; } set { e.Scale0X = value; } }
                [Category("1. Documented"), DisplayName("Scale Y"), Description("First scale-array entry, Y. Offset 0x5B4.")]
                public float ScaleY { get { return e.Scale0Y; } set { e.Scale0Y = value; } }

                // ===== 1b. Scale Curve (8-key scale-over-life @0x5B0; renderer reads X=width, Y=height, Time=key time 0..1).
                //          Keys 0-3 exposed (covers nearly all curves). Set Key Count to the number of active keys (0 = const).
                [Category("1b. Scale Curve"), DisplayName("Scale Key Count"), Description("Active scale keys @0x20 (0 = use the constant Scale X/Y above).")]
                public uint ScaleKeyCount { get { return e.GetU32At(0x20); } set { e.SetU32At(0x20, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key0 X"), Description("@0x5B0")] public float Sk0X { get { return e.GetFloatAt(0x5B0); } set { e.SetFloatAt(0x5B0, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key0 Y"), Description("@0x5B4")] public float Sk0Y { get { return e.GetFloatAt(0x5B4); } set { e.SetFloatAt(0x5B4, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key0 Time"), Description("@0x5BC")] public float Sk0T { get { return e.GetFloatAt(0x5BC); } set { e.SetFloatAt(0x5BC, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key1 X"), Description("@0x5C0")] public float Sk1X { get { return e.GetFloatAt(0x5C0); } set { e.SetFloatAt(0x5C0, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key1 Y"), Description("@0x5C4")] public float Sk1Y { get { return e.GetFloatAt(0x5C4); } set { e.SetFloatAt(0x5C4, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key1 Time"), Description("@0x5CC")] public float Sk1T { get { return e.GetFloatAt(0x5CC); } set { e.SetFloatAt(0x5CC, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key2 X"), Description("@0x5D0")] public float Sk2X { get { return e.GetFloatAt(0x5D0); } set { e.SetFloatAt(0x5D0, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key2 Y"), Description("@0x5D4")] public float Sk2Y { get { return e.GetFloatAt(0x5D4); } set { e.SetFloatAt(0x5D4, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key2 Time"), Description("@0x5DC")] public float Sk2T { get { return e.GetFloatAt(0x5DC); } set { e.SetFloatAt(0x5DC, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key3 X"), Description("@0x5E0")] public float Sk3X { get { return e.GetFloatAt(0x5E0); } set { e.SetFloatAt(0x5E0, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key3 Y"), Description("@0x5E4")] public float Sk3Y { get { return e.GetFloatAt(0x5E4); } set { e.SetFloatAt(0x5E4, value); } }
                [Category("1b. Scale Curve"), DisplayName("Key3 Time"), Description("@0x5EC")] public float Sk3T { get { return e.GetFloatAt(0x5EC); } set { e.SetFloatAt(0x5EC, value); } }

                // ===== 1d. Particle Scale (the scale a particle is BORN at, which the scale curve above multiplies).
                //          This is the size the game's own vertex shader reads, NOT Radius @0x360 (the emission-volume
                //          extent: the Fire family carries 20-100 there against a true scale of 0.2-8).
                [Category("1d. Particle Scale"), DisplayName("Scale Start X"), Description("ptclScaleStart @0x978.")]
                public float ScaleStartX { get { return e.GetFloatAt(0x978); } set { e.SetFloatAt(0x978, value); } }
                [Category("1d. Particle Scale"), DisplayName("Scale Start Y"), Description("@0x97C.")]
                public float ScaleStartY { get { return e.GetFloatAt(0x97C); } set { e.SetFloatAt(0x97C, value); } }
                [Category("1d. Particle Scale"), DisplayName("Scale Start Z"), Description("@0x980. Read by mesh (PRIM) emitters; billboards are flat in XY.")]
                public float ScaleStartZ { get { return e.GetFloatAt(0x980); } set { e.SetFloatAt(0x980, value); } }
                [Category("1d. Particle Scale"), DisplayName("Scale Random X (%)"), Description("ptclScaleRandom @0x984, a PERCENT: a particle is born at Scale Start x (1 - u * percent/100), with one uniform u per particle shared by all three axes. Over 100 flips the sign (the game draws the mirrored particle).")]
                public float ScaleRandomX { get { return e.GetFloatAt(0x984); } set { e.SetFloatAt(0x984, value); } }
                [Category("1d. Particle Scale"), DisplayName("Scale Random Y (%)"), Description("@0x988.")]
                public float ScaleRandomY { get { return e.GetFloatAt(0x988); } set { e.SetFloatAt(0x988, value); } }
                [Category("1d. Particle Scale"), DisplayName("Scale Random Z (%)"), Description("@0x98C.")]
                public float ScaleRandomZ { get { return e.GetFloatAt(0x98C); } set { e.SetFloatAt(0x98C, value); } }

                // ===== 2. EMISSION =====
                [Category("2. Emission"), DisplayName("Lifespan (frames)"), Description("ptclMaxLifespan @0x6F0. <=1 = one-shot/burst sentinel.")]
                public float Lifespan { get { return e.GetFloatAt(0x6F0); } set { e.SetFloatAt(0x6F0, value); } }
                [Category("2. Emission"), DisplayName("Emit Rate"), Description("emissionRate / count @0x6F4.")]
                public float EmitRate { get { return e.GetFloatAt(0x6F4); } set { e.SetFloatAt(0x6F4, value); } }
                [Category("2. Emission"), DisplayName("Emit Interval (frames)"), Description("Frames between emissions @0x710.")]
                public uint EmitInterval { get { return e.GetU32At(0x710); } set { e.SetU32At(0x710, value); } }
                [Category("2. Emission"), DisplayName("End Frame"), Description("Emission cutoff @0x780. -1 = infinite (stream).")]
                public float EndFrame { get { return e.GetFloatAt(0x780); } set { e.SetFloatAt(0x780, value); } }
                [Category("2. Emission"), DisplayName("Emit Shape"), Description("emitFunction / volume type @0x714 (Point/Sphere/Cylinder/Box/Line/...). BotW may use values >14.")]
                public EmitShape Shape { get { return (EmitShape)e.GetU32At(0x714); } set { e.SetU32At(0x714, (uint)value); } }
                [Category("2. Emission"), DisplayName("Volume Scale X"), Description("Emission volume size X @0x80C.")]
                public float VolScaleX { get { return e.GetFloatAt(0x80C); } set { e.SetFloatAt(0x80C, value); } }
                [Category("2. Emission"), DisplayName("Volume Scale Y"), Description("@0x810.")]
                public float VolScaleY { get { return e.GetFloatAt(0x810); } set { e.SetFloatAt(0x810, value); } }
                [Category("2. Emission"), DisplayName("Volume Scale Z"), Description("@0x814.")]
                public float VolScaleZ { get { return e.GetFloatAt(0x814); } set { e.SetFloatAt(0x814, value); } }

                // ===== 3. MOTION =====
                [Category("3. Motion"), DisplayName("All-Dir Velocity"), Description("allDirVel @0x7B0: omnidirectional shape-burst speed (explosions).")]
                public float AllDirVel { get { return e.GetFloatAt(0x7B0); } set { e.SetFloatAt(0x7B0, value); } }
                [Category("3. Motion"), DisplayName("Dir Velocity"), Description("dirVel @0x7D4: directional speed along Dir (rain/aim).")]
                public float DirVel { get { return e.GetFloatAt(0x7D4); } set { e.SetFloatAt(0x7D4, value); } }
                [Category("3. Motion"), DisplayName("Dir X"), Description("Emission direction unit VEC3 @0x7C8 (cone axis). Rain/splash = (0,-1,0).")]
                public float DirX { get { return e.GetFloatAt(0x7C8); } set { e.SetFloatAt(0x7C8, value); } }
                [Category("3. Motion"), DisplayName("Dir Y"), Description("@0x7CC. -1=down, +1=up.")]
                public float DirY { get { return e.GetFloatAt(0x7CC); } set { e.SetFloatAt(0x7CC, value); } }
                [Category("3. Motion"), DisplayName("Dir Z"), Description("@0x7D0.")]
                public float DirZ { get { return e.GetFloatAt(0x7D0); } set { e.SetFloatAt(0x7D0, value); } }
                [Category("3. Motion"), DisplayName("Dispersion (rad)"), Description("Cone half-angle @0x7F4 (radians; pi default).")]
                public float Dispersion { get { return e.GetFloatAt(0x7F4); } set { e.SetFloatAt(0x7F4, value); } }
                [Category("3. Motion"), DisplayName("Arc Length (rad)"), Description("Azimuth span @0x7F0 (2pi default).")]
                public float ArcLength { get { return e.GetFloatAt(0x7F0); } set { e.SetFloatAt(0x7F0, value); } }
                [Category("3. Motion"), DisplayName("Air Resist"), Description("Velocity damping per frame @0x6DC (1=none, <1 decelerate).")]
                public float AirResist { get { return e.GetFloatAt(0x6DC); } set { e.SetFloatAt(0x6DC, value); } }
                [Category("3. Motion"), DisplayName("Momentum Random"), Description("Per-particle speed spread @0x7C4 (0..1).")]
                public float MomentumRandom { get { return e.GetFloatAt(0x7C4); } set { e.SetFloatAt(0x7C4, value); } }
                [Category("3. Motion"), DisplayName("Rotation Init (rad)"), Description("Initial Z-rotation @0x6C8 (2pi=random full turn).")]
                public float RotInit { get { return e.GetFloatAt(0x6C8); } set { e.SetFloatAt(0x6C8, value); } }
                [Category("3. Motion"), DisplayName("Angular Velocity (rad/f)"), Description("Spin speed @0x6D8.")]
                public float AngularVel { get { return e.GetFloatAt(0x6D8); } set { e.SetFloatAt(0x6D8, value); } }

                // ===== 4. RENDER =====
                [Category("4. Render"), DisplayName("Blend"), Description("blendType @0x88D. VERIFIED vs captured GPU blend (0x8DC was REFUTED).")]
                public BlendMode Blend { get { return (BlendMode)e.GetByteAt(0x88D); } set { e.SetByteAt(0x88D, (byte)value); } }
                [Category("4. Render"), DisplayName("Display Side"), Description("displaySideType @0x84F (cull): Both/Front/Back.")]
                public DisplaySideMode DisplaySide { get { return (DisplaySideMode)e.GetByteAt(0x84F); } set { e.SetByteAt(0x84F, (byte)value); } }
                [Category("4. Render"), DisplayName("Z-Buffer Test"), Description("zBufATestType @0x88E.")]
                public ZBufTestMode ZTest { get { return (ZBufTestMode)e.GetByteAt(0x88E); } set { e.SetByteAt(0x88E, (byte)value); } }
                [Category("4. Render"), DisplayName("Vertex Transform"), Description("vertexTransformMode @0x8F4. NOTE: value->mode map partly unverified for BotW; billboard(0) confirmed.")]
                public VtxTransformMode VtxTransform { get { return (VtxTransformMode)e.GetU32At(0x8F4); } set { e.SetU32At(0x8F4, (uint)value); } }
                [Category("4. Render"), DisplayName("Fragment Alpha Mode"), Description("fragmentAlphaMode @0x8A9. Mode 3 = subtract/erosion alpha (VERIFIED).")]
                public FragAlphaMode AlphaMode { get { return (FragAlphaMode)e.GetByteAt(0x8A9); } set { e.SetByteAt(0x8A9, (byte)value); } }
                [Category("4. Render"), DisplayName("Fragment Color Mode (raw)"), Description("fragmentColorMode @0x8A8 (RGB composite selector; int->formula partly undecoded).")]
                public byte ColorModeRaw { get { return e.GetByteAt(0x8A8); } set { e.SetByteAt(0x8A8, value); } }
                [Category("4. Render"), DisplayName("Tex Color Op (slot0 x slot1)"), Description("textureColorBlend @0x8AD.")]
                public CombineOp TexColorOp { get { return (CombineOp)e.GetByteAt(0x8AD); } set { e.SetByteAt(0x8AD, (byte)value); } }
                [Category("4. Render"), DisplayName("Slot2 Color Op"), Description("primitiveColorBlend @0x8AE.")]
                public CombineOp Slot2ColorOp { get { return (CombineOp)e.GetByteAt(0x8AE); } set { e.SetByteAt(0x8AE, (byte)value); } }
                [Category("4. Render"), DisplayName("Tex Alpha Op (slot0 x slot1)"), Description("textureAlphaBlend @0x8B1.")]
                public CombineOp TexAlphaOp { get { return (CombineOp)e.GetByteAt(0x8B1); } set { e.SetByteAt(0x8B1, (byte)value); } }
                [Category("4. Render"), DisplayName("Slot2 Alpha Op"), Description("primitiveAlphaBlend @0x8B2.")]
                public CombineOp Slot2AlphaOp { get { return (CombineOp)e.GetByteAt(0x8B2); } set { e.SetByteAt(0x8B2, (byte)value); } }
                [Category("4. Render"), DisplayName("Slot0 Flipbook Cols"), Description("Slot0 texture atlas columns @0x2B8. Flipbook grid is PER-SAMPLER (slot1 @0x308, slot2 @0x358).")]
                public float FlipCols { get { return e.GetFloatAt(0x2B8); } set { e.SetFloatAt(0x2B8, value); } }
                [Category("4. Render"), DisplayName("Slot0 Flipbook Rows"), Description("Slot0 texture atlas rows @0x2BC.")]
                public float FlipRows { get { return e.GetFloatAt(0x2BC); } set { e.SetFloatAt(0x2BC, value); } }
                [Category("4. Render"), DisplayName("Slot1 Flipbook Cols"), Description("Slot1 atlas columns @0x308 (e.g. ArrowHit_Fire/Fire = 2). Renderer (mesh path) samples slot1 on its OWN grid.")]
                public float FlipCols1 { get { return e.GetFloatAt(0x308); } set { e.SetFloatAt(0x308, value); } }
                [Category("4. Render"), DisplayName("Slot1 Flipbook Rows"), Description("Slot1 atlas rows @0x30C.")]
                public float FlipRows1 { get { return e.GetFloatAt(0x30C); } set { e.SetFloatAt(0x30C, value); } }
                [Category("4. Render"), DisplayName("Slot2 Flipbook Cols"), Description("Slot2 atlas columns @0x358.")]
                public float FlipCols2 { get { return e.GetFloatAt(0x358); } set { e.SetFloatAt(0x358, value); } }
                [Category("4. Render"), DisplayName("Slot2 Flipbook Rows"), Description("Slot2 atlas rows @0x35C.")]
                public float FlipRows2 { get { return e.GetFloatAt(0x35C); } set { e.SetFloatAt(0x35C, value); } }
                [Category("4. Render"), DisplayName("Static Flipbook Cell"), Description("Static cell index @0xD0.")]
                public uint StaticCell { get { return e.GetU32At(0xD0); } set { e.SetU32At(0xD0, value); } }

                // ===== 4b. Texture Sampler (GX2 address mode + reticle quadrant flag). Samplers @0x9A8, stride 0x20:
                //          wrapU @+0x08, wrapV @+0x09, maxAniso(float) @+0x0C, uv-expand flag @+0x17. =====
                [Category("4b. Texture Sampler"), DisplayName("Slot0 Wrap U"), Description("GX2 wrapU @ sampler0+0x08 (0x9B0): Mirror/Wrap/Clamp. PINNED by capture cross-ref. Mirror reflects a quadrant/half texture into a symmetric whole.")]
                public SamplerWrap Slot0WrapU { get { return (SamplerWrap)e.GetByteAt(0x9B0); } set { e.SetByteAt(0x9B0, (byte)value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot0 Wrap V"), Description("GX2 wrapV @ sampler0+0x09 (0x9B1): Mirror/Wrap/Clamp.")]
                public SamplerWrap Slot0WrapV { get { return (SamplerWrap)e.GetByteAt(0x9B1); } set { e.SetByteAt(0x9B1, (byte)value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot0 UV Expand (quadrant)"), Description("Flag @ sampler0+0x17 (0x9BF): the texture is a 1/2-scale quadrant. With Slot0 Wrap = Mirror the billboard samples [0,2] so the stored quarter mirror-tiles into the full sprite (e.g. Gdn_Target reticle ring). Renderer applies x2 only when this is set AND both axes are Mirror.")]
                public bool Slot0UvExpand { get { return e.GetByteAt(0x9BF) == 1; } set { e.SetByteAt(0x9BF, (byte)(value ? 1 : 0)); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot0 Max Anisotropy"), Description("maxAnisotropy @ sampler0+0x0C (0x9B4): 2.0 or ~16 (cosmetic filtering quality; NOT a UV scale).")]
                public float Slot0MaxAniso { get { return e.GetFloatAt(0x9B4); } set { e.SetFloatAt(0x9B4, value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot1 Wrap U"), Description("GX2 wrapU @ sampler1+0x08 (0x9D0).")]
                public SamplerWrap Slot1WrapU { get { return (SamplerWrap)e.GetByteAt(0x9D0); } set { e.SetByteAt(0x9D0, (byte)value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot1 Wrap V"), Description("GX2 wrapV @ sampler1+0x09 (0x9D1).")]
                public SamplerWrap Slot1WrapV { get { return (SamplerWrap)e.GetByteAt(0x9D1); } set { e.SetByteAt(0x9D1, (byte)value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot2 Wrap U"), Description("GX2 wrapU @ sampler2+0x08 (0x9F0).")]
                public SamplerWrap Slot2WrapU { get { return (SamplerWrap)e.GetByteAt(0x9F0); } set { e.SetByteAt(0x9F0, (byte)value); } }
                [Category("4b. Texture Sampler"), DisplayName("Slot2 Wrap V"), Description("GX2 wrapV @ sampler2+0x09 (0x9F1).")]
                public SamplerWrap Slot2WrapV { get { return (SamplerWrap)e.GetByteAt(0x9F1); } set { e.SetByteAt(0x9F1, (byte)value); } }

                [Category("5. Probe (any offset)"), DisplayName("Probe Offset (hex)"), Description("Type a hex offset into the emitter struct, e.g. 0x45C, then read/edit 'Probe Value' below to inspect any float in the struct.")]
                public string ProbeOffsetHex { get { return e.ProbeOffsetHex; } set { e.ProbeOffsetHex = value; } }
                [Category("5. Probe (any offset)"), DisplayName("Probe Value (float)"), Description("The float at 'Probe Offset (hex)'. Editing it writes that offset; save to persist. Use this to explore offsets not in the candidate list.")]
                public float ProbeValue
                {
                    get { int o; return TryHex(ProbeOffsetHex, out o) ? e.GetFloatAt(o) : 0f; }
                    set { int o; if (TryHex(ProbeOffsetHex, out o)) e.SetFloatAt(o, value); }
                }
                private static bool TryHex(string s, out int v)
                {
                    v = 0;
                    if (string.IsNullOrEmpty(s)) return false;
                    s = s.Trim();
                    if (s.StartsWith("0x") || s.StartsWith("0X")) s = s.Substring(2);
                    return int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out v);
                }
            }

            public void Read(FileReader reader, Header ptclHeader)
            {
                uint Position = (uint)reader.Position;
                DataPosition = Position;

                Color0Array = new STColor[8];
                Color1Array = new STColor[8];
                Color0AlphaArray = new STColor[8];
                Color1AlphaArray = new STColor[8];
                ConstantColor0 = new STColor();
                ConstantColor1 = new STColor();

                reader.ReadBytes(16); //Unknown padding
                Color0KeyCount = reader.ReadUInt32();
                Alpha0KeyCount = reader.ReadUInt32();
                Color1KeyCount = reader.ReadUInt32();
                Alpha1KeyCount = reader.ReadUInt32();
                uint scaleKeyCount = reader.ReadUInt32();

                //Seek to the contant colors
                if (ptclHeader.VFXVersion >= 37)
                    reader.Seek(Position + 2392, SeekOrigin.Begin);
                else if (ptclHeader.VFXVersion > 21)
                    reader.Seek(Position + 2384, SeekOrigin.Begin);
                else
                    reader.Seek(Position + 2392, SeekOrigin.Begin);

                ConstantColor0 = new STColor();
                ConstantColor0.R = reader.ReadSingle();
                ConstantColor0.G = reader.ReadSingle();
                ConstantColor0.B = reader.ReadSingle();
                ConstantColor0.A = reader.ReadSingle();

                ConstantColor1 = new STColor();
                ConstantColor1.R = reader.ReadSingle();
                ConstantColor1.G = reader.ReadSingle();
                ConstantColor1.B = reader.ReadSingle();
                ConstantColor1.A = reader.ReadSingle();

                //Seek to the random and animated color table
                reader.Seek(Position + 880, SeekOrigin.Begin);
                for (int i = 0; i < 8; i++)
                {
                    Color0Array[i] = new STColor();
                    Color0Array[i].R = reader.ReadSingle();
                    Color0Array[i].G = reader.ReadSingle();
                    Color0Array[i].B = reader.ReadSingle();
                    Color0Array[i].Time = reader.ReadSingle();
                }

                for (int i = 0; i < 8; i++)
                {
                    Color0AlphaArray[i] = new STColor();
                    Color0AlphaArray[i].R = reader.ReadSingle();
                    Color0AlphaArray[i].G = reader.ReadSingle();
                    Color0AlphaArray[i].B = reader.ReadSingle();
                    Color0AlphaArray[i].Time = reader.ReadSingle();
                }

                for (int i = 0; i < 8; i++)
                {
                    Color1Array[i] = new STColor();
                    Color1Array[i].R = reader.ReadSingle();
                    Color1Array[i].G = reader.ReadSingle();
                    Color1Array[i].B = reader.ReadSingle();
                    Color1Array[i].Time = reader.ReadSingle();
                }

                for (int i = 0; i < 8; i++)
                {
                    Color1AlphaArray[i] = new STColor();
                    Color1AlphaArray[i].R = reader.ReadSingle();
                    Color1AlphaArray[i].G = reader.ReadSingle();
                    Color1AlphaArray[i].B = reader.ReadSingle();
                    Color1AlphaArray[i].Time = reader.ReadSingle();

                    int alpha = Utils.FloatToIntClamp(Color1Array[i].A);
                }

                SetType(Color0KeyCount != 0, Color0Array, 0, false);
                SetType(Alpha0KeyCount != 0, Color0AlphaArray, 0, true);
                SetType(Color1KeyCount != 0, Color1Array, 1, false);
                SetType(Alpha1KeyCount != 0, Color1AlphaArray, 1, true);

                if (ptclHeader.VFXVersion >= 37)
                    SamplerBaseOffset = 2472;
                else if (ptclHeader.VFXVersion > 21)
                    SamplerBaseOffset = 2464;
                else
                    SamplerBaseOffset = 2472;
                reader.Seek(Position + SamplerBaseOffset, SeekOrigin.Begin);

                for (int i = 0; i < EmtSamplerCount; i++)
                {
                    SamplerInfo samplerInfo = new SamplerInfo();
                    samplerInfo.Read(reader);
                    Samplers.Add(samplerInfo);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                uint Position = (uint)writer.Position;

                //EFTB: write the whole struct first (carries any parameter edits), then overlay colours below.
                if (EmitterData != null && header != null && header.Signature == "EFTB")
                    writer.Write(EmitterData);

                //Seek to the contant colors
                if (header.VFXVersion >= 37)
                    writer.Seek(Position + 2392, SeekOrigin.Begin);
                else if (header.VFXVersion > 21)
                    writer.Seek(Position + 2384, SeekOrigin.Begin);
                else
                    writer.Seek(Position + 2392, SeekOrigin.Begin);

                writer.Write(ConstantColor0.R);
                writer.Write(ConstantColor0.G);
                writer.Write(ConstantColor0.B);
                writer.Write(ConstantAlpha0.R);
                writer.Write(ConstantColor1.R);
                writer.Write(ConstantColor1.G);
                writer.Write(ConstantColor1.B);
                writer.Write(ConstantAlpha1.R);

                writer.Seek(Position + 880, SeekOrigin.Begin);
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color0Array[i].R);
                    writer.Write(Color0Array[i].G);
                    writer.Write(Color0Array[i].B);
                    writer.Write(Color0Array[i].Time);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color0AlphaArray[i].R);
                    writer.Write(Color0AlphaArray[i].G);
                    writer.Write(Color0AlphaArray[i].B);
                    writer.Write(Color0AlphaArray[i].Time);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color1Array[i].R);
                    writer.Write(Color1Array[i].G);
                    writer.Write(Color1Array[i].B);
                    writer.Write(Color1Array[i].Time);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color1AlphaArray[i].R);
                    writer.Write(Color1AlphaArray[i].G);
                    writer.Write(Color1AlphaArray[i].B);
                    writer.Write(Color1AlphaArray[i].Time);
                }
            }

            public class SamplerInfo
            {
                public ulong TextureID;

                public void Read(FileReader reader)
                {
                    TextureID = reader.ReadUInt64();
                    byte wrapModeU = reader.ReadByte();
                    byte wrapMode = reader.ReadByte();
                    reader.Seek(22, SeekOrigin.Current);
                }
            }
            private Color ReadColorRgba(FileReader reader, int amount = 1)
            {
                Color[] colors = new Color[amount];
                for (int i = 0; i < 8; i++)
                {

                }
                float R = reader.ReadSingle();
                float G = reader.ReadSingle();
                float B = reader.ReadSingle();
                float A = reader.ReadSingle();

                int red = Utils.FloatToIntClamp(R);
                int green = Utils.FloatToIntClamp(G);
                int blue = Utils.FloatToIntClamp(B);
                int alpha = Utils.FloatToIntClamp(B);

                return Color.FromArgb(255, red, green, blue);
            }
            private Color ReadColorAnim(FileReader reader, int amount = 1)
            {
                float R = reader.ReadSingle();
                float G = reader.ReadSingle();
                float B = reader.ReadSingle();
                float unk = reader.ReadSingle();

                int red = Utils.FloatToIntClamp(R);
                int green = Utils.FloatToIntClamp(G);
                int blue = Utils.FloatToIntClamp(B);

                return Color.FromArgb(255, red, green, blue);
            }
            private Color ReadColorA(FileReader reader, int amount = 1)
            {
                float A = reader.ReadSingle();
                float unk = reader.ReadSingle();
                float unk2 = reader.ReadSingle();
                float unk3 = reader.ReadSingle();

                int alpha = Utils.FloatToIntClamp(A);

                return Color.FromArgb(alpha, 0, 0, 0);
            }
        }

    }
}
