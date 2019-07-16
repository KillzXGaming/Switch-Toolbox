using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;
using System.Drawing;
using System.IO;
using OpenTK;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace ColladaHelper
{
    public class DAEHelper
    {
        private static XmlAttribute createAttribute(XmlDocument doc, string att, string value)
        {
            XmlAttribute at = doc.CreateAttribute(att);
            at.Value = value;
            return at;
        }

        Dictionary<string, object> sourceLinks = new Dictionary<string, object>();

        List<ColladaImages> library_images = new List<ColladaImages>();
        List<ColladaMaterials> library_materials = new List<ColladaMaterials>();
        List<ColladaEffects> library_effects = new List<ColladaEffects>();
        List<ColladaGeometry> library_geometries = new List<ColladaGeometry>();
        List<ColladaController> library_controllers = new List<ColladaController>();
        ColladaVisualScene scene = new ColladaVisualScene();
        public int v1, v2, v3;

        #region ENUMS
        public enum ColladaPrimitiveType
        {
            None,
            polygons,
            polylist,
            triangles,
            trifans,
            tristrips,
            lines,
            linestrips
        }
        public enum SemanticType
        {
            None,
            POSITION,
            VERTEX,
            NORMAL,
            TEXCOORD,
            COLOR,
            WEIGHT,
            JOINT,
            INV_BIND_MATRIX,
            TEXTANGENT,
            TEXBINORMAL
        }

        #endregion

        #region Assets

        public void ParseAssets(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                if (node.Name.Equals("unit"))
                {

                }
                if (node.Name.Equals("up_axis"))
                {

                }
            }
        }

        #endregion

        #region Geometry

        public void ParseGeometry(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                ColladaGeometry g = new ColladaGeometry();
                g.Read(node);
                library_geometries.Add(g);
            }
        }

        public class ColladaGeometry
        {
            public string id;
            public string name;
            public ColladaMesh mesh;

            public void Read(XmlNode root)
            {
                id = (string)root.Attributes["id"].Value;
                name = (string)root.Attributes["name"].Value;
                mesh = new ColladaMesh();
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("mesh"))
                        mesh.Read(node);
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("geometry");
                node.Attributes.Append(createAttribute(doc, "id", id));
                node.Attributes.Append(createAttribute(doc, "name", name));

                // write mesh
                mesh.Write(doc, node);

                parent.AppendChild(node);
            }
        }

        public class ColladaMesh
        {
            public List<ColladaSource> sources = new List<ColladaSource>();
            public ColladaVertices vertices = new ColladaVertices();
            public List<ColladaPolygons> polygons = new List<ColladaPolygons>();

            public void Read(XmlNode root)
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("source"))
                    {
                        ColladaSource source = new ColladaSource();
                        source.Read(node);
                        sources.Add(source);
                    }
                    if (node.Name.Equals("vertices"))
                    {
                        vertices.Read(node);
                    }
                    if (node.Name.Equals("triangles"))
                    {
                        ColladaPolygons source = new ColladaPolygons();
                        source.type = ColladaPrimitiveType.triangles;
                        source.Read(node);
                        polygons.Add(source);
                    }
                    if (node.Name.Equals("polylist"))
                    {
                        ColladaPolygons source = new ColladaPolygons();
                        source.type = ColladaPrimitiveType.polylist;
                        source.Read(node);
                        polygons.Add(source);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("mesh");

                foreach (ColladaSource src in sources)
                {
                    src.Write(doc, node);
                }
                vertices.Write(doc, node);
                foreach (ColladaPolygons p in polygons)
                {
                    p.Write(doc, node);
                }

                parent.AppendChild(node);
            }
        }

        public enum ArrayType
        {
            float_array,
            Name_array
        }

        public class ColladaSource
        {
            public string id;

            public string[] data;
            public int count;
            public int stride;
            public ArrayType type;
            public List<string> accessor = new List<string>();

            public void Read(XmlNode root)
            {
                id = (string)root.Attributes["id"].Value;
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("float_array"))
                    {
                        count = int.Parse((string)node.Attributes["count"].Value);
                        data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                    }
                    if (node.Name.Equals("Name_array"))
                    {
                        count = int.Parse((string)node.Attributes["count"].Value);
                        data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                    }
                    if (node.Name.Equals("technique_common") && node.ChildNodes.Count > 0 && node.ChildNodes[0].Attributes["stride"] != null)
                    {
                        stride = int.Parse((string)node.ChildNodes[0].Attributes["stride"].Value);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("source");

                node.Attributes.Append(createAttribute(doc, "id", id));
                //node.Attributes.Append(createAttribute(doc, "count", count));

                XmlNode arr = doc.CreateElement(type + "");
                node.AppendChild(arr);
                arr.Attributes.Append(createAttribute(doc, "id", id + "-array"));
                arr.Attributes.Append(createAttribute(doc, "count", count + ""));
                string aa = "";
                foreach (string s in data) aa += s + " ";
                arr.InnerText = aa;

                XmlNode tc = doc.CreateElement("technique_common");
                node.AppendChild(tc);
                XmlNode accessor = doc.CreateElement("accessor");
                accessor.Attributes.Append(createAttribute(doc, "source", "#" + id + "-array"));
                accessor.Attributes.Append(createAttribute(doc, "count", (count / this.accessor.Count) + ""));
                if (this.accessor.Count > 0 && this.accessor[0].Equals("TRANSFORM"))
                    accessor.Attributes.Append(createAttribute(doc, "stride", 16 + ""));
                else
                    accessor.Attributes.Append(createAttribute(doc, "stride", this.accessor.Count + ""));
                tc.AppendChild(accessor);

                foreach (string param in this.accessor)
                {
                    XmlNode pa = doc.CreateElement("param");
                    accessor.AppendChild(pa);
                    pa.Attributes.Append(createAttribute(doc, "name", param));
                    if (param.Equals("TRANSFORM"))
                        pa.Attributes.Append(createAttribute(doc, "type", "float4x4"));
                    else
                        pa.Attributes.Append(createAttribute(doc, "type", type.ToString().Replace("_array", "")));
                }

                parent.AppendChild(node);
            }
        }

        public class ColladaVertices
        {
            public string id;
            public List<ColladaInput> inputs = new List<ColladaInput>();

            public void Read(XmlNode root)
            {
                id = (string)root.Attributes["id"].Value;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("input"))
                    {
                        ColladaInput input = new ColladaInput();
                        input.Read(node);
                        inputs.Add(input);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("vertices");

                node.Attributes.Append(createAttribute(doc, "id", id));

                foreach (ColladaInput input in inputs)
                {
                    input.Write(doc, node);
                }

                parent.AppendChild(node);
            }
        }

        public class ColladaPolygons
        {
            public ColladaPrimitiveType type = ColladaPrimitiveType.triangles;
            public List<ColladaInput> inputs = new List<ColladaInput>();
            public int[] p;
            public int count;
            public string materialid;

            public void Read(XmlNode root)
            {
                foreach (XmlAttribute att in root.Attributes)
                {
                    if (att.Name.Equals("material")) materialid = (string)att.Value;
                    if (att.Name.Equals("count")) int.TryParse((string)att.Value, out count);
                }

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("input"))
                    {
                        ColladaInput input = new ColladaInput();
                        input.Read(node);
                        inputs.Add(input);
                    }
                    if (node.Name.Equals("p"))
                    {
                        string[] ps = node.InnerText.Trim().Split(' ');
                        p = new int[ps.Length];
                        for (int i = 0; i < ps.Length; i++)
                            p[i] = int.Parse(ps[i]);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement(type.ToString());

                node.Attributes.Append(createAttribute(doc, "material", materialid));
                node.Attributes.Append(createAttribute(doc, "count", count + ""));

                foreach (ColladaInput input in inputs)
                {
                    input.Write(doc, node);
                }
                string p = "";
                foreach (int i in this.p)
                {
                    p += i + " ";
                }
                XmlNode pi = doc.CreateElement("p");
                pi.InnerText = p;
                node.AppendChild(pi);

                parent.AppendChild(node);
            }
        }

        public class ColladaInput
        {
            public SemanticType semantic;
            public string source;
            public int set = -99, offset = 0;

            public void Read(XmlNode root)
            {
                semantic = (SemanticType)Enum.Parse(typeof(SemanticType), (string)root.Attributes["semantic"].Value);
                source = (string)root.Attributes["source"].Value;
                if (root.Attributes["set"] != null)
                    int.TryParse((string)root.Attributes["set"].Value, out set);
                if (root.Attributes["offset"] != null)
                    int.TryParse((string)root.Attributes["offset"].Value, out offset);
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("input");

                node.Attributes.Append(createAttribute(doc, "semantic", semantic.ToString()));
                node.Attributes.Append(createAttribute(doc, "source", source));
                if (set != -99)
                    node.Attributes.Append(createAttribute(doc, "set", set + ""));
                if (offset != -99)
                    node.Attributes.Append(createAttribute(doc, "offset", offset + ""));

                parent.AppendChild(node);

            }
        }
        #endregion

        #region Materials
        // Images and Materials

        public void ParseMaterials(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                ColladaMaterials m = new ColladaMaterials();
                m.Read(node);
                library_materials.Add(m);
            }
        }

        public void ParseImages(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                ColladaImages m = new ColladaImages();
                m.Read(node);
                library_images.Add(m);
            }
        }

        public void ParseEffects(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                ColladaEffects m = new ColladaEffects();
                m.Read(node);
                library_effects.Add(m);
            }
        }

        public class ColladaImages
        {
            public string id, name, initref;

            public void Read(XmlNode root)
            {
                id = root.Attributes["id"].Value;
                //name = root.Attributes["name"].Value;
                foreach (XmlNode child in root.ChildNodes)
                {
                    if (child.Name.Equals("init_from"))
                    {
                        initref = child.InnerText;
                        if (initref.StartsWith("file://"))
                            initref = initref.Substring(7, initref.Length - 7);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("image");
                node.Attributes.Append(createAttribute(doc, "id", id));
                node.Attributes.Append(createAttribute(doc, "name", name));

                XmlNode init = doc.CreateElement("init_from");
                init.InnerText = initref;
                node.AppendChild(init);

                parent.AppendChild(node);
            }
        }
        public class ColladaMaterials
        {
            public string id, effecturl;

            public void Read(XmlNode root)
            {
                id = root.Attributes["id"].Value;
                foreach (XmlNode node in root.ChildNodes)
                    if (node.Name.Equals("instance_effect") && node.Attributes["url"] != null)
                        effecturl = node.Attributes["url"].Value;
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("material");
                node.Attributes.Append(createAttribute(doc, "id", id));

                XmlNode init = doc.CreateElement("instance_effect");
                init.Attributes.Append(createAttribute(doc, "url", effecturl));
                node.AppendChild(init);

                parent.AppendChild(node);
            }
        }
        public class ColladaEffects
        {
            public string id, name;
            public string source = "#";
            public ColladaSampler2D sampler;

            public void Read(XmlNode root)
            {
                foreach (XmlAttribute att in root.Attributes)
                {
                    if (att.Name.Equals("id")) id = att.Value;
                    if (att.Name.Equals("name")) name = root.Attributes["name"].Value;
                }

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("profile_COMMON"))
                    {
                        readEffectTechnique(node);
                    }
                }
            }

            private void readEffectTechnique(XmlNode root)
            {
                Dictionary<string, XmlNode> surfaces = new Dictionary<string, XmlNode>();
                Dictionary<string, XmlNode> samplers = new Dictionary<string, XmlNode>();
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("newparam") && node.ChildNodes[0].Name.Equals("surface"))
                        surfaces.Add(node.Attributes["sid"].Value, node.ChildNodes[0]);
                    if (node.Name.Equals("newparam") && node.ChildNodes[0].Name.Equals("sampler2D"))
                        samplers.Add(node.Attributes["sid"].Value, node.ChildNodes[0]);
                }
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("technique") && node.ChildNodes[0].Name.Equals("phong"))
                    {
                        foreach (XmlNode node1 in node.ChildNodes[0].ChildNodes)
                        {
                            if (node1.Name.Equals("diffuse"))
                            {
                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name.Equals("texture"))
                                    {
                                        string texture = node2.Attributes["texture"].Value;
                                        XmlNode temp = null;
                                        samplers.TryGetValue(texture, out temp);
                                        if (temp != null)
                                        {
                                            foreach (XmlNode node3 in temp.ChildNodes)
                                            {
                                                if (node3.Name.Equals("source"))
                                                {
                                                    //if you are reading this I am sorry
                                                    XmlNode temp2 = null;
                                                    surfaces.TryGetValue(node3.InnerText, out temp2);
                                                    if (temp2 != null)
                                                    {
                                                        texture = temp2.ChildNodes[0].InnerText;
                                                    }
                                                }
                                            }
                                        }
                                        source = "#" + texture;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // for writing
            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("effect");
                node.Attributes.Append(createAttribute(doc, "id", id));
                node.Attributes.Append(createAttribute(doc, "name", name));

                XmlNode prof = doc.CreateElement("profile_COMMON");
                node.AppendChild(prof);
                {
                    XmlNode np = doc.CreateElement("newparam");
                    prof.AppendChild(np);
                    np.Attributes.Append(createAttribute(doc, "sid", id + "-surface"));

                    XmlNode sur = doc.CreateElement("surface");
                    np.AppendChild(sur);
                    XmlNode init = doc.CreateElement("init_from");
                    sur.AppendChild(init);
                    init.InnerText = source.Replace("#", "");
                    sur.Attributes.Append(createAttribute(doc, "type", "2D"));
                }
                {
                    XmlNode np = doc.CreateElement("newparam");
                    prof.AppendChild(np);
                    np.Attributes.Append(createAttribute(doc, "sid", id + "-sampler"));
                    sampler.source = id + "-surface";
                    sampler.Write(doc, np);
                }
                {
                    XmlNode tech = doc.CreateElement("technique");
                    prof.AppendChild(tech);
                    tech.Attributes.Append(createAttribute(doc, "sid", "COMMON"));

                    XmlNode sur = doc.CreateElement("phong");
                    tech.AppendChild(sur);
                    XmlNode init = doc.CreateElement("diffuse");
                    sur.AppendChild(init);
                    XmlNode reff = doc.CreateElement("texture");
                    init.AppendChild(reff);
                    reff.Attributes.Append(createAttribute(doc, "texture", id + "-sampler"));
                    reff.Attributes.Append(createAttribute(doc, "texcoord", ""));
                }

                parent.AppendChild(node);
            }
        }

        public enum COLLADA_WRAPMODE
        {
            WRAP,
            REPEAT,
            CLAMP,
            CLAMP_TO_EDGE,
            MIRROR
        }
        public enum COLLADA_FILTER
        {
            NONE,
            NEAREST,
            LINEAR
        }

        public class ColladaSampler2D
        {
            public string source, url;
            public COLLADA_WRAPMODE wrap_s, wrap_t;
            public COLLADA_FILTER minfilter, magfilter;
            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("sampler2D");
                {
                    XmlNode src = doc.CreateElement("source");
                    src.InnerText = source;
                    node.AppendChild(src);
                }
                {
                    XmlNode src = doc.CreateElement("wrap_s");
                    src.InnerText = wrap_s.ToString();
                    node.AppendChild(src);
                }
                {
                    XmlNode src = doc.CreateElement("wrap_t");
                    src.InnerText = wrap_t.ToString();
                    node.AppendChild(src);
                }

                parent.AppendChild(node);
            }
        }

        #endregion

        #region Controllers

        public void ParseControllers(XmlNode root)
        {
            foreach (XmlNode node in root.ChildNodes)
            {
                ColladaController g = new ColladaController();
                g.Read(node);
                library_controllers.Add(g);
            }
        }

        public class ColladaController
        {
            public string id;
            public ColladaSkin skin = new ColladaSkin();

            public void Read(XmlNode root)
            {
                id = (string)root.Attributes["id"].Value;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("skin"))
                    {
                        skin.Read(node);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("controller");
                node.Attributes.Append(createAttribute(doc, "id", id));

                skin.Write(doc, node);

                parent.AppendChild(node);
            }
        }

        public class ColladaSkin
        {
            public string source;
            public Matrix4 mat = Matrix4.CreateScale(1, 1, 1);
            public List<ColladaSource> sources = new List<ColladaSource>();
            public ColladaJoints joints = new ColladaJoints();
            public ColladaVertexWeights weights = new ColladaVertexWeights();

            public void Read(XmlNode root)
            {
                source = (string)root.Attributes["source"].Value;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("bind_shape_matrix"))
                    {
                        string[] data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                        mat.M11 = float.Parse(data[0]); mat.M12 = float.Parse(data[1]); mat.M13 = float.Parse(data[2]); mat.M14 = float.Parse(data[3]);
                        mat.M21 = float.Parse(data[4]); mat.M22 = float.Parse(data[5]); mat.M23 = float.Parse(data[6]); mat.M24 = float.Parse(data[7]);
                        mat.M31 = float.Parse(data[8]); mat.M32 = float.Parse(data[9]); mat.M33 = float.Parse(data[10]); mat.M34 = float.Parse(data[11]);
                        mat.M41 = float.Parse(data[12]); mat.M42 = float.Parse(data[13]); mat.M43 = float.Parse(data[14]); mat.M44 = float.Parse(data[15]);
                    }
                    if (node.Name.Equals("source"))
                    {
                        ColladaSource source = new ColladaSource();
                        source.Read(node);
                        sources.Add(source);
                    }
                    if (node.Name.Equals("joints"))
                    {
                        joints.Read(node);
                    }
                    if (node.Name.Equals("vertex_weights"))
                    {
                        weights.Read(node);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("skin");
                node.Attributes.Append(createAttribute(doc, "source", source));

                XmlNode matrix = doc.CreateElement("matrix");
                node.AppendChild(matrix);
                matrix.InnerText = mat.M11 + " " + mat.M21 + " " + mat.M31 + " " + mat.M41
                    + " " + mat.M12 + " " + mat.M22 + " " + mat.M32 + " " + mat.M42
                    + " " + mat.M13 + " " + mat.M23 + " " + mat.M33 + " " + mat.M43
                    + " " + mat.M14 + " " + mat.M24 + " " + mat.M34 + " " + mat.M44;
                foreach (ColladaSource src in sources)
                {
                    src.Write(doc, node);
                }
                joints.Write(doc, node);
                weights.Write(doc, node);

                parent.AppendChild(node);
            }

        }

        public class ColladaJoints
        {
            public List<ColladaInput> inputs = new List<ColladaInput>();

            public void Read(XmlNode root)
            {
                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("input"))
                    {
                        ColladaInput input = new ColladaInput();
                        input.Read(node);
                        inputs.Add(input);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("joints");

                foreach (ColladaInput input in inputs)
                {
                    input.Write(doc, node);
                }

                parent.AppendChild(node);
            }
        }

        public class ColladaVertexWeights
        {
            public List<ColladaInput> inputs = new List<ColladaInput>();
            public int[] v, vcount;
            public int count;

            public void Read(XmlNode root)
            {
                count = int.Parse((string)root.Attributes["count"].Value);

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("input"))
                    {
                        ColladaInput input = new ColladaInput();
                        input.Read(node);
                        inputs.Add(input);
                    }
                    if (node.Name.Equals("vcount"))
                    {
                        string[] ps = node.InnerText.Trim().Split(' ');
                        vcount = new int[ps.Length];
                        for (int i = 0; i < ps.Length; i++)
                            vcount[i] = int.Parse(ps[i]);
                    }
                    if (node.Name.Equals("v"))
                    {
                        string[] ps = node.InnerText.Trim().Split(' ');
                        v = new int[ps.Length];
                        for (int i = 0; i < ps.Length; i++)
                            v[i] = int.Parse(ps[i]);
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("vertex_weights");
                node.Attributes.Append(createAttribute(doc, "count", vcount.Length.ToString()));

                foreach (ColladaInput input in inputs)
                {
                    input.Write(doc, node);
                }

                XmlNode vc = doc.CreateElement("vcount");
                XmlNode p = doc.CreateElement("v");
                node.AppendChild(vc);
                node.AppendChild(p);

                string ar = "";
                foreach (int i in vcount)
                    ar += i + " ";
                vc.InnerText = ar;

                ar = "";
                foreach (int i in v)
                    ar += i + " ";
                p.InnerText = ar;

                parent.AppendChild(node);
            }
        }

        #endregion

        #region Visual Nodes

        public class ColladaVisualScene
        {
            public List<ColladaNode> nodes = new List<ColladaNode>();
            public string id, name;
            public Dictionary<string, string> MaterialIds = new Dictionary<string, string>();

            public void Read(XmlNode root)
            {
                root = root.ChildNodes[0];
                id = (string)root.Attributes["id"].Value;
                name = (string)root.Attributes["name"].Value;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("node"))
                    {
                        ColladaNode n = new ColladaNode();
                        n.Read(node, null);
                        nodes.Add(n);
                        foreach (var v in n.materialIds)
                        {
                            if (!MaterialIds.ContainsKey(v.Key))
                                MaterialIds.Add(v.Key, v.Value);
                        }
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("library_visual_scenes");
                XmlNode vs = doc.CreateElement("visual_scene");
                vs.Attributes.Append(createAttribute(doc, "id", "VisualSceneNode"));
                vs.Attributes.Append(createAttribute(doc, "name", "rdmscene"));
                node.AppendChild(vs);

                foreach (ColladaNode no in nodes)
                {
                    no.Write(doc, vs);
                }


                parent.AppendChild(node);
            }
        }

        public class ColladaNode
        {
            public ColladaNode parent;
            public string id, name, type = "NODE", geomid, instance = "";
            public List<ColladaNode> children = new List<ColladaNode>();

            public Matrix4 mat = Matrix4.CreateScale(1, 1, 1);
            public Vector3 pos = new Vector3();
            public Vector3 scale = new Vector3();
            public Vector3 rot = new Vector3();

            // material
            public string materialSymbol, materialTarget;
            public Dictionary<string, string> materialIds = new Dictionary<string, string>();

            // instance geometry
            public string geom_id = "";

            public void Read(XmlNode root, ColladaNode parent)
            {
                this.parent = parent;
                id = (string)root.Attributes["id"].Value;
                name = (string)root.Attributes["name"].Value;
                if (root.Attributes["type"] != null)
                    type = (string)root.Attributes["type"].Value;

                foreach (XmlNode node in root.ChildNodes)
                {
                    if (node.Name.Equals("node"))
                    {
                        ColladaNode n = new ColladaNode();
                        n.Read(node, this);
                        children.Add(n);
                    }
                    else if (node.Name.Equals("matrix"))
                    {
                        string[] data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                        mat = new Matrix4();
                        mat.M11 = float.Parse(data[0]); mat.M12 = float.Parse(data[1]); mat.M13 = float.Parse(data[2]); mat.M14 = float.Parse(data[3]);
                        mat.M21 = float.Parse(data[4]); mat.M22 = float.Parse(data[5]); mat.M23 = float.Parse(data[6]); mat.M24 = float.Parse(data[7]);
                        mat.M31 = float.Parse(data[8]); mat.M32 = float.Parse(data[9]); mat.M33 = float.Parse(data[10]); mat.M34 = float.Parse(data[11]);
                        mat.M41 = float.Parse(data[12]); mat.M42 = float.Parse(data[13]); mat.M43 = float.Parse(data[14]); mat.M44 = float.Parse(data[15]);

                        pos = new Vector3(mat.M14, mat.M24, mat.M34);
                        scale = mat.ExtractScale();

                        mat.ClearScale();
                        mat.ClearTranslation();
                        mat.Invert();
                        var quat = mat.ExtractRotation();
                        rot = Toolbox.Library.AssimpHelper.ToEular(quat);
                        if (float.IsNaN(rot.X)) rot.X = 0;
                        if (float.IsNaN(rot.Y)) rot.Y = 0;
                        if (float.IsNaN(rot.Z)) rot.Z = 0;

                        mat.M11 = float.Parse(data[0]); mat.M12 = float.Parse(data[1]); mat.M13 = float.Parse(data[2]); mat.M14 = float.Parse(data[3]);
                        mat.M21 = float.Parse(data[4]); mat.M22 = float.Parse(data[5]); mat.M23 = float.Parse(data[6]); mat.M24 = float.Parse(data[7]);
                        mat.M31 = float.Parse(data[8]); mat.M32 = float.Parse(data[9]); mat.M33 = float.Parse(data[10]); mat.M34 = float.Parse(data[11]);
                        mat.M41 = float.Parse(data[12]); mat.M42 = float.Parse(data[13]); mat.M43 = float.Parse(data[14]); mat.M44 = float.Parse(data[15]);
                    }
                    else if (node.Name.Equals("extra"))
                    {

                    }
                    else if (node.Name.Equals("instance_controller") || node.Name.Equals("instance_geometry"))
                    {
                        if (node.Name.Equals("instance_geometry"))
                        {
                            geom_id = node.Attributes["url"].Value.Replace("#", "");
                        }
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name.Equals("bind_material"))
                            {
                                foreach (XmlNode node2 in node1.ChildNodes)
                                {
                                    if (node2.Name.Equals("technique_common"))
                                    {
                                        if (node2.ChildNodes[0].Attributes["symbol"] != null && !materialIds.ContainsKey(node2.ChildNodes[0].Attributes["symbol"].Value))
                                            materialIds.Add(node2.ChildNodes[0].Attributes["symbol"].Value, node2.ChildNodes[0].Attributes["target"].Value);
                                    }
                                }
                            }
                        }
                    }
                    else if (node.Name.Equals("translate"))
                    {
                        string[] data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                        pos = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                    }
                    else if (node.Name.Equals("scale"))
                    {
                        string[] data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                        scale = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                    }
                    else if (node.Name.Equals("rotate"))
                    {
                        string[] data = node.InnerText.Trim().Replace("\n", " ").Split(' ');
                        rot = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));
                    }
                }
            }

            public void Write(XmlDocument doc, XmlNode parent)
            {
                XmlNode node = doc.CreateElement("node");
                node.Attributes.Append(createAttribute(doc, "id", id));
                node.Attributes.Append(createAttribute(doc, "name", name));
                node.Attributes.Append(createAttribute(doc, "type", type));
                if (type.Equals("JOINT"))
                    node.Attributes.Append(createAttribute(doc, "sid", name));

                // transform matrix

                XmlNode matrix = doc.CreateElement("matrix");
                node.AppendChild(matrix);
                matrix.InnerText = mat.M11 + " " + mat.M21 + " " + mat.M31 + " " + mat.M41
                    + " " + mat.M12 + " " + mat.M22 + " " + mat.M32 + " " + mat.M42
                    + " " + mat.M13 + " " + mat.M23 + " " + mat.M33 + " " + mat.M43
                    + " " + mat.M14 + " " + mat.M24 + " " + mat.M34 + " " + mat.M44;

                // instance geometry (no rigging) instance controller for rigging
                if (!instance.Equals(""))
                {
                    XmlNode inst = doc.CreateElement(instance);
                    inst.Attributes.Append(createAttribute(doc, "url", geomid));
                    node.AppendChild(inst);
                    if (instance.Equals("instance_controller"))
                    {
                        XmlNode skel = doc.CreateElement("skeleton");
                        inst.AppendChild(skel);
                        skel.InnerText = "#Bone_0_id";
                    }
                    if (materialSymbol != null && materialTarget != null)
                    {
                        XmlNode bn = doc.CreateElement("bind_material");
                        inst.AppendChild(bn);
                        XmlNode tc = doc.CreateElement("technique_common");
                        bn.AppendChild(tc);
                        XmlNode im = doc.CreateElement("instance_material");
                        tc.AppendChild(im);
                        im.Attributes.Append(createAttribute(doc, "symbol", materialSymbol));
                        im.Attributes.Append(createAttribute(doc, "target", materialTarget));

                    }
                }


                foreach (ColladaNode no in children)
                {
                    no.Write(doc, node);
                }

                parent.AppendChild(node);
            }
        }

        #endregion
    }
}
