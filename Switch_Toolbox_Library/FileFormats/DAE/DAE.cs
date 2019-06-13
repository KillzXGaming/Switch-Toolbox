using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collada141;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using ColladaHelper;
using OpenTK;
using Switch_Toolbox.Library.Rendering;

namespace Switch_Toolbox.Library
{
    public class DAE : DAEHelper
    {
        public List<STGenericObject> objects = new List<STGenericObject>();
        public List<STGenericMaterial> materials = new List<STGenericMaterial>();
        public STSkeleton skeleton;
        public List<string> BoneNames = new List<string>();

        public bool UseTransformMatrix = true;

        Dictionary<string, Vertex> VertexSkinSources = new Dictionary<string, Vertex>();
        Dictionary<string, Matrix4> MatrixSkinSources = new Dictionary<string, Matrix4>();

        public bool LoadFile(string FileName)
        {
            COLLADA collada = COLLADA.Load(FileName);

            foreach (var item in collada.Items)
            {
                if (item is library_geometries)
                {
                    LoadGeometry((library_geometries)item);
                }
            }

            return true;
        }

        private void SetControllers(COLLADA collada)
        {
           
        }


        private void LoadGeometry(library_geometries Geometries)
        {
            foreach (var geom in Geometries.geometry)
            {
                var mesh = geom.Item as mesh;
                if (mesh == null)
                    continue;

                Console.WriteLine(geom.name);

                foreach (var source in mesh.source)
                {
                    var float_array = source.Item as float_array;
                    if (float_array == null)
                        continue;

                    Console.Write("Geometry {0} source {1} : ", geom.id, source.id);
                    foreach (var mesh_source_value in float_array.Values)
                        Console.Write("{0} ", mesh_source_value);
                    Console.WriteLine();
                }
            }
        }

        private List<STGenericObject> CreateGenericObjects(string Name, library_geometries Geometries)
        {
            List<STGenericObject> objects = new List<STGenericObject>();
            foreach (var geom in Geometries.geometry)
            {
                var daeMesh = geom.Item as mesh;
                if (daeMesh == null)
                    continue;

                STGenericObject mesh = new STGenericObject();
                mesh.ObjectName = Name;

                foreach (var source in daeMesh.source)
                {
                    var float_array = source.Item as float_array;
                    if (float_array == null)
                        continue;
                }
                objects.Add(mesh);
            }
            return objects;
        }
    }
}
