using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin.GFMDLStructs;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BufferToStruct
    {
        public static Model LoadFile(System.IO.Stream stream)
        {
             var buffer = FlatBuffers.Gfbmdl.Model.GetRootAsModel(
                    new FlatBuffers.ByteBuffer(stream.ToBytes()));

            Model model = new Model();
            model.Version = buffer.Version;
            model.Bounding = ConvertBounding(buffer.Bounding);

            model.TextureNames = new List<string>();
            model.ShaderNames = new List<string>();
            model.MaterialNames = new List<string>();

            for (int i = 0; i < buffer.TextureNamesLength; i++)
                model.TextureNames.Add(buffer.TextureNames(i));

            for (int i = 0; i < buffer.ShaderNamesLength; i++)
                model.ShaderNames.Add(buffer.ShaderNames(i));

            for (int i = 0; i < buffer.MaterialNamesLength; i++)
                model.MaterialNames.Add(buffer.MaterialNames(i));

            model.Materials = new List<Material>();
            for (int i = 0; i < buffer.MaterialsLength; i++)
            {
                var mat = buffer.Materials(i).Value;
                List<MatColor> colors = new List<MatColor>();
                List<MatSwitch> switches = new List<MatSwitch>();
                List<MatFloat> values = new List<MatFloat>();
                List<TextureMap> textures = new List<TextureMap>();

                MaterialCommon common = null;
                if (mat.Common != null)
                {
                    var com = mat.Common.Value;

                    List<MatColor> sharedColors = new List<MatColor>();
                    List<MatSwitch> sharedSwitches = new List<MatSwitch>();
                    List<MatInt> sharedValues = new List<MatInt>();

                    for (int j = 0; j < com.ColorsLength; j++)
                    {
                        var col = com.Colors(j).Value;
                        sharedColors.Add(new MatColor()
                        {
                            Color = ConvertColor(col.Color),
                            Name = col.Name,
                        });
                    }

                    for (int j = 0; j < com.ValuesLength; j++)
                    {
                        var val = com.Values(j).Value;
                        sharedValues.Add(new MatInt()
                        {
                            Value = val.Value,
                            Name = val.Name,
                        });
                    }

                    for (int j = 0; j < com.SwitchesLength; j++)
                    {
                        var val = com.Switches(j).Value;
                        sharedSwitches.Add(new MatSwitch()
                        {
                            Value = val.Value,
                            Name = val.Name,
                        });
                    }

                    common = new MaterialCommon()
                    {
                        Switches = sharedSwitches,
                        Values = sharedValues,
                        Colors = sharedColors,
                    };
                }

                for (int j = 0; j < mat.TextureMapsLength; j++)
                {
                    var tex = mat.TextureMaps(j).Value;
                    TextureParams parameters = null;
                    if (tex.Params != null)
                    {
                        var param = tex.Params.Value;
                        parameters = new TextureParams()
                        {
                            Unknown1 = param.Unknown1,
                            WrapModeX = (uint)param.WrapModeX,
                            WrapModeY = (uint)param.WrapModeY,
                            WrapModeZ = (uint)param.WrapModeZ,
                            Unknown5 = param.Unknown5,
                            Unknown6 = param.Unknown6,
                            Unknown7 = param.Unknown7,
                            Unknown8 = param.Unknown8,
                            lodBias = param.LodBias,
                        };
                    }
                    textures.Add(new TextureMap()
                    {
                        Index = (uint)tex.Index,
                        Sampler = tex.Sampler,
                        Params = parameters,
                    });
                }

                for (int j = 0; j < mat.ColorsLength; j++)
                {
                    var col = mat.Colors(j).Value;
                    colors.Add(new MatColor()
                    {
                        Color = ConvertColor(col.Color),
                        Name = col.Name,
                    });
                }

                for (int j = 0; j < mat.ValuesLength; j++)
                {
                    var val = mat.Values(j).Value;
                    values.Add(new MatFloat()
                    {
                        Value = val.Value,
                        Name = val.Name,
                    });
                }

                for (int j = 0; j < mat.SwitchesLength; j++)
                {
                    var val = mat.Switches(j).Value;
                    switches.Add(new MatSwitch()
                    {
                        Value = val.Value,
                        Name = val.Name,
                    });
                }

                if (mat.Parameter3 != i)
                    Console.WriteLine($"Expected {i} for shader index, got " + mat.Parameter3);
                if (mat.Parameter4 != -1)
                    Console.WriteLine("Expected -1 for param 4, got " + mat.Parameter4);
                if (mat.Parameter5 != i)
                    Console.WriteLine($"Expected {i} for param 5, got " + mat.Parameter5);

                model.Materials.Add(new Material()
                {
                    Name = mat.Name,
                    Switches = switches,
                    Colors = colors,
                    Values = values,
                    Parameter1 = mat.Parameter1,
                    Parameter2 = mat.Parameter2,
                    Parameter3 = mat.Parameter3,
                    ShaderIndex = mat.ShaderIndex,
                    Parameter4 = mat.Parameter4,
                    Parameter5 = mat.Parameter5,
                    RenderLayer = mat.RenderLayer,
                    ShaderGroup = mat.ShaderGroup,
                    Unknown1 = mat.Unknown1,
                    Unknown2 = mat.Unknown2,
                    Unknown3 = mat.Unknown3,
                    Unknown4 = mat.Unknown4,
                    Unknown5 = mat.Unknown5,
                    Unknown6 = mat.Unknown6,
                    Unknown7 = mat.Unknown7,
                    Common = common,
                    TextureMaps = textures,
                });
            }

            model.Groups = new List<Group>();
            for (int i = 0; i < buffer.GroupsLength; i++)
            {
                var group = buffer.Groups(i).Value;
                model.Groups.Add(new Group()
                {
                    BoneIndex = group.BoneIndex,
                    Layer = group.Layer,
                    MeshIndex= group.MeshIndex,
                    Bounding = ConvertBounding(group.Bounding),
                });
            }

            model.Meshes = new List<Mesh>();
            for (int i = 0; i < buffer.MeshesLength; i++)
            {
                var mesh = buffer.Meshes(i).Value;

                List<MeshAttribute> attributes = new List<MeshAttribute>();
                for (int j = 0; j < mesh.AttributesLength; j++)
                {
                    var att = mesh.Attributes(j).Value;
                    attributes.Add(new MeshAttribute()
                    {
                        BufferFormat = att.BufferFormat,
                        ElementCount = att.ElementCount,
                        VertexType = att.VertexType,
                    });
                }

                List<MeshPolygon> polygons = new List<MeshPolygon>();
                for (int j = 0; j < mesh.PolygonsLength; j++)
                {
                    var poly = mesh.Polygons(j).Value;
                    polygons.Add(new MeshPolygon()
                    {
                        MaterialIndex = poly.MaterialIndex,
                        Faces = poly.GetFacesArray().ToList(),
                    });
                }

                model.Meshes.Add(new Mesh()
                {
                    Attributes = attributes,
                    Polygons = polygons,
                    Data = mesh.GetDataArray().ToList(),
                });
            }

            model.Bones = new List<Bone>();
            for (int i = 0; i < buffer.BonesLength; i++)
            {
                var bone = buffer.Bones(i).Value;
                BoneRigidData rigidData = null;
                if (bone.RigidCheck != null)
                {
                    rigidData = new BoneRigidData();
                    rigidData.Unknown1 = (uint)bone.RigidCheck.Value.Unknown1;
                }

                model.Bones.Add(new Bone()
                {
                    Name = bone.Name,
                    BoneType = bone.BoneType,
                    Parent = bone.Parent,
                    Scale = ConvertVec3(bone.Scale),
                    Rotation = ConvertVec3(bone.Rotation),
                    Translation = ConvertVec3(bone.Translation),
                    RadiusEnd = ConvertVec3(bone.RadiusEnd),
                    RadiusStart = ConvertVec3(bone.RadiusStart),
                    SegmentScale = bone.Visible,
                    Zero = bone.Zero,
                    RigidCheck = rigidData,
                });
            }

            model.CollisionGroups = new List<CollisionGroup>();
            for (int i = 0; i < buffer.CollisionGroupsLength; i++)
            {
                var colGroup = buffer.CollisionGroups(i).Value;
                List<uint> children = colGroup.GetBoneChildrenArray().ToList();

                model.CollisionGroups.Add(new CollisionGroup()
                {
                    BoneIndex = colGroup.BoneIndex,
                    Unknown1 = colGroup.Unknown1,
                    Bounding = ConvertBounding(colGroup.Bounding),
                    BoneChildren = children,
                });
            }

            model.Unknown = new List<UnknownEmpty>();

            return model;
        }

        public static BoundingBox ConvertBounding(FlatBuffers.Gfbmdl.BoundingBox? buffer)
        {
            if (buffer != null && buffer.HasValue)
            {
                BoundingBox bounding = new BoundingBox();
                bounding.MinX = buffer.Value.MinX;
                bounding.MinY = buffer.Value.MinY;
                bounding.MinZ = buffer.Value.MinZ;
                bounding.MaxX = buffer.Value.MaxX;
                bounding.MaxY = buffer.Value.MaxY;
                bounding.MaxZ = buffer.Value.MaxZ;
                return bounding;
            }
            else
                return null;
        }

        public static Vector3 ConvertVec3(FlatBuffers.Gfbmdl.Vector3? vec3)
        {
            if (vec3 != null && vec3.HasValue)
                return new Vector3(vec3.Value.X, vec3.Value.Y, vec3.Value.Z);
            else
                return null;
        }

        public static Vector4 ConvertVec4(FlatBuffers.Gfbmdl.Vector4? vec3)
        {
            if (vec3 != null && vec3.HasValue)
                return new Vector4(vec3.Value.X, vec3.Value.Y, vec3.Value.Z, vec3.Value.W);
            else
                return null;
        }

        public static ColorRGB32 ConvertColor(FlatBuffers.Gfbmdl.ColorRGB32? col)
        {
            if (col != null && col.HasValue)
                return new ColorRGB32(col.Value.R, col.Value.G, col.Value.B);
            else
                return null;
        }
    }
}
