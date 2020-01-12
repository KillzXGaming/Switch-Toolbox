using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;
using Syroot.NintenTools.NSW.Bfres;
using Newtonsoft.Json;

namespace FirstPlugin
{
    public class YamlFscn
    {
        public static string ToJson(string Name, SceneAnim sceneAnim)
        {
            var config = new AnimConfig();
            config.ToYaml(sceneAnim);
            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }

        public static SceneAnim FromJson(string Name)
        {
            AnimConfig config = JsonConvert.DeserializeObject<AnimConfig>(
                System.IO.File.ReadAllText(Name));

            return config.FromYaml();
        }

        public static string ToYaml(string Name, SceneAnim sceneAnim)
        {
            var serializerSettings = new SerializerSettings()
            {
                //  EmitTags = false
            };

            serializerSettings.DefaultStyle = YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));

            var config = new AnimConfig();
            config.ToYaml(sceneAnim);

            var serializer = new Serializer(serializerSettings);
            string yaml = serializer.Serialize(config, typeof(AnimConfig));

            return yaml;
        }

        public static SceneAnim FromYaml(string Name)
        {
            var serializerSettings = new SerializerSettings()
            {
                // EmitTags = false
            };

            serializerSettings.DefaultStyle = YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));

            var serializer = new Serializer(serializerSettings);
            AnimConfig config = serializer.Deserialize<AnimConfig>(System.IO.File.ReadAllText(Name));

            return config.FromYaml();
        }

        public class AnimConfig
        {
            [JsonProperty(Order = 0)]
            public string Name { get; set; }
            [JsonProperty(Order = 1)]
            public string Path { get; set; }

            [JsonProperty(Order = 2)]
            public List<CameraAnimConfig> CameraAnimations = new List<CameraAnimConfig>();

            public void ToYaml(SceneAnim sceneAnim)
            {
                Name = sceneAnim.Name;
                Path = sceneAnim.Path;

                foreach (var cameraAnim in sceneAnim.CameraAnims)
                    CameraAnimations.Add(new CameraAnimConfig(cameraAnim));
            }

            public SceneAnim FromYaml()
            {
                SceneAnim anim = new SceneAnim();
                anim.Name = Name;
                anim.Path = Path;
                foreach (var camera in CameraAnimations)
                {
                    CameraAnim camAnim = new CameraAnim();
                    camAnim.Name = camera.Name;
                    camAnim.FrameCount = camera.FrameCount;
                    camAnim.BaseData = camera.BaseData;
                    if (camera.EulerZXY)
                        camAnim.Flags |= CameraAnimFlags.EulerZXY;
                    if (camera.Loop)
                        camAnim.Flags |= CameraAnimFlags.Looping;
                    if (camera.Perspective)
                        camAnim.Flags |= CameraAnimFlags.Perspective;

                    foreach (var curve in camera.Curves) {
                        AnimCurve animCurve = new AnimCurve();
                        animCurve.AnimDataOffset = ConvertOffset(curve.Type);
                        animCurve.Scale = 1;
                        animCurve.CurveType = AnimCurveType.Linear;

                        int FrameCount = curve.KeyFrames.Count;
                        animCurve.Frames = new float[FrameCount];
                        animCurve.Keys = new float[FrameCount, 2];

                        var values = curve.KeyFrames.Values.ToList();
                        for (int i = 0; i < curve.KeyFrames.Count; i++)
                        {
                            var KeyFrame = curve.KeyFrames.ElementAt(i);
                            animCurve.Frames[i] = KeyFrame.Key;
                            animCurve.Keys[i, 0] = KeyFrame.Value;

                            //Calculate delta
                            float Delta = 0;
                            if (i < values.Count - 1)
                                Delta = values[i + 1] - values[i];

                            animCurve.Keys[i, 1] = Delta;
                        }

                        animCurve.EndFrame = animCurve.Frames.Max();
                        if (animCurve.Keys.Length > 1)
                            animCurve.Delta = values[values.Count - 1] - values[0];

                        animCurve.KeyType = AnimCurveKeyType.Single;
                        animCurve.FrameType = AnimCurveFrameType.Single;


                        camAnim.Curves.Add(animCurve);
                    }

                    anim.CameraAnims.Add(camAnim);
                    anim.CameraAnimDict.Add(camAnim.Name);
                }

                return anim;
            }

            private static uint ConvertOffset(string type)
            {
                CameraOffsetType flags;
                bool isValid = Enum.TryParse(type, out flags);
                if (!isValid)
                    throw new Exception($"Invalid camera curve type {type}!");

                return (uint)flags;
            }
        }

        public class CameraAnimConfig
        {
            [JsonProperty(Order = 0)]
            public string Name { get; set; }

            [JsonProperty(Order = 1)]
            public bool Loop { get; set; }

            [JsonProperty(Order = 2)]
            public int FrameCount { get; set; }

            [JsonProperty(Order = 3)]
            public bool EulerZXY { get; set; }

            [JsonProperty(Order = 4)]
            public bool Perspective { get; set; }

            [JsonProperty(Order = 5)]
            public CameraAnimData BaseData { get; set; }

            [JsonProperty(Order = 6)]
            public List<CameraAnimCurve> Curves = new List<CameraAnimCurve>();

            public CameraAnimConfig() { }

            public CameraAnimConfig(CameraAnim camAnim)
            {
                Name = camAnim.Name;
                Loop = camAnim.Flags.HasFlag(CameraAnimFlags.Looping);
                EulerZXY = camAnim.Flags.HasFlag(CameraAnimFlags.EulerZXY);
                Perspective = camAnim.Flags.HasFlag(CameraAnimFlags.Perspective);

                BaseData = camAnim.BaseData;

                foreach (var curve in camAnim.Curves)
                    Curves.Add(new CameraAnimCurve(curve));
            }
        }

        public class CameraAnimCurve
        {
            [JsonProperty(Order = 0)]
            public string Type;

            [JsonProperty(Order = 1)]
            public Dictionary<int, float> KeyFrames { get; set; }

            public CameraAnimCurve() { }

            public CameraAnimCurve(AnimCurve curve) {
                KeyFrames = new Dictionary<int, float>();

                Type = ((CameraOffsetType)curve.AnimDataOffset).ToString();

                for (int f = 0; f < curve.Frames.Length; f++)
                {
                    int frame = (int)curve.Frames[f];
                    float Value = curve.Offset + curve.Keys[f, 0] * curve.Scale;
                    KeyFrames.Add(frame, Value);
                }
            }
        }

        //Offsets to get data from  "Values"
        public enum CameraOffsetType
        {
            ClipNear = 0,
            ClipFar = 4,
            AspectRatio = 8,
            FieldOFView = 12,
            PositionX = 16,
            PositionY = 20,
            PositionZ = 24,
            RotationX = 28,
            RotationY = 32,
            RotationZ = 36,
            Twist = 40,
        }
    }
}
