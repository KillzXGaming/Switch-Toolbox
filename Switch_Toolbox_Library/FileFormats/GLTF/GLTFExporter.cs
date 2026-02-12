using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

namespace Toolbox.Library.GLTFModel
{
    public class GLTFExporter
    {
        private SceneBuilder Scene;
        private MaterialBuilder DefaultMaterial;

        public GLTFExporter()
        {
            Scene = new SceneBuilder();
            DefaultMaterial = new MaterialBuilder();
        }

        public void Write(string FileName)
        {
            var model = Scene.ToGltf2();
            model.SaveGLTF(FileName);
        }
    }
}
