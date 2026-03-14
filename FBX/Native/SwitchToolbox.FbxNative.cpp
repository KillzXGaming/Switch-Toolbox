#include <fbxsdk.h>
#include <vector>
#include <map>
#include <string>
#include <iostream>

#define EXPORT extern "C" __declspec(dllexport)

// Global Manager (Simple singleton for this specific task)
FbxManager* lSdkManager = nullptr;
FbxScene* lScene = nullptr;
FbxIOSettings* lIos = nullptr;
FbxExporter* lExporter = nullptr;

// Helper to keep track of created nodes to parent them correctly
std::map<std::string, FbxNode*> NodeMap;
std::vector<FbxNode*> MeshNodes;
std::map<std::string, FbxSurfacePhong*> MaterialMap;

enum MaterialTextureType {
    MaterialTextureDiffuse = 0,
    MaterialTextureNormal = 1,
    MaterialTextureSpecular = 2,
    MaterialTextureEmission = 3,
    MaterialTextureAlpha = 4,
    MaterialTextureNormalFlatColor = 5,
};

struct MaterialTextureBinding {
    std::string textureName;
    std::string filePath;
    int textureType = MaterialTextureDiffuse;
};

std::map<std::string, std::vector<MaterialTextureBinding>> MaterialTextureMap;

static void ConnectMaterialTexture(FbxSurfacePhong* material, const MaterialTextureBinding& binding) {
    if (material == nullptr || lScene == nullptr)
        return;
    if (binding.textureName.empty() || binding.filePath.empty())
        return;

    FbxFileTexture* texture = FbxFileTexture::Create(lScene, binding.textureName.c_str());
    texture->SetFileName(binding.filePath.c_str());
    texture->SetMappingType(FbxTexture::eUV);
    texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
    texture->SetSwapUV(false);
    texture->SetTranslation(0.0, 0.0);
    texture->SetScale(1.0, 1.0);
    texture->SetRotation(0.0, 0.0);
    texture->UVSet.Set("UVMap");

    FbxProperty targetProperty;
    switch (binding.textureType) {
    case MaterialTextureNormalFlatColor:
        texture->SetTextureUse(FbxTexture::eBumpNormalMap);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sNormalMap);
        if (!targetProperty.IsValid())
            targetProperty = material->FindProperty(FbxSurfaceMaterial::sBump);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);
        material->BumpFactor.Set(1.0);
        break;
    case MaterialTextureNormal:
        texture->SetTextureUse(FbxTexture::eBumpNormalMap);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sNormalMap);
        if (!targetProperty.IsValid())
            targetProperty = material->FindProperty(FbxSurfaceMaterial::sBump);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);
        material->BumpFactor.Set(1.0);
        break;
    case MaterialTextureSpecular:
        texture->SetTextureUse(FbxTexture::eStandard);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sSpecular);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);
        material->SpecularFactor.Set(1.0);
        break;
    case MaterialTextureEmission:
        texture->SetTextureUse(FbxTexture::eLightMap);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sEmissive);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);
        material->EmissiveFactor.Set(1.0);
        break;
    case MaterialTextureAlpha:
        texture->SetTextureUse(FbxTexture::eStandard);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sTransparencyFactor);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);

        targetProperty = material->FindProperty(FbxSurfaceMaterial::sTransparentColor);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);

        material->TransparentColor.Set(FbxDouble3(1.0, 1.0, 1.0));
        material->TransparencyFactor.Set(1.0);
        break;
    case MaterialTextureDiffuse:
    default:
        texture->SetTextureUse(FbxTexture::eStandard);
        targetProperty = material->FindProperty(FbxSurfaceMaterial::sDiffuse);
        if (targetProperty.IsValid())
            targetProperty.ConnectSrcObject(texture);
        material->DiffuseFactor.Set(1.0);
        break;
    }
}

static void ApplyMaterialTextures(const std::string& materialName, FbxSurfacePhong* material) {
    auto it = MaterialTextureMap.find(materialName);
    if (it == MaterialTextureMap.end())
        return;

    for (const auto& binding : it->second) {
        ConnectMaterialTexture(material, binding);
    }
}

EXPORT void Exp_CreateContext() {
    if (lSdkManager == nullptr) {
        lSdkManager = FbxManager::Create();
        lIos = FbxIOSettings::Create(lSdkManager, IOSROOT);
        lSdkManager->SetIOSettings(lIos);
    }
}

EXPORT void Exp_DestroyContext() {
    if (lExporter) lExporter->Destroy();
    if (lScene) lScene->Destroy();
    // Manager is usually kept alive or destroyed at very end, but for DLL we can clean up
    if (lSdkManager) lSdkManager->Destroy();

    lSdkManager = nullptr;
    lScene = nullptr;
    lExporter = nullptr;
    NodeMap.clear();
    MeshNodes.clear();
    MaterialMap.clear();
    MaterialTextureMap.clear();
}

EXPORT bool Exp_Initialize(const char* filename) {
    if (!lSdkManager) Exp_CreateContext();

    lScene = FbxScene::Create(lSdkManager, "SwitchToolboxExport");

    // Axis Setup (Switch/OpenGL is usually Y Up or Z Up, FBX defaults to Y Up)
    // DAE exporter handles rotation, we might need to set axis system here if needed
    // FbxAxisSystem::MayaZUp.ConvertScene(lScene); // Example if needed

    // Set units to Meters (1.0 = 1m). This matches Blender's default and eliminates 
    // the need for 100x scaling during import. 
    FbxSystemUnit::m.ConvertScene(lScene);

    lExporter = FbxExporter::Create(lSdkManager, "");

    // Initialize the exporter.
    bool lExportStatus = lExporter->Initialize(filename, -1, lSdkManager->GetIOSettings());

    if(!lExportStatus) {
        return false;
    }

    // Set format to Binary (0) or ASCII (1) - usually binary is detected by -1 but we want to be sure
    // We will let the user or default decide, but -1 tries to detect from extension
    return true;
}

// Revised Skeleton / Node Handling
EXPORT void Exp_CreateNode(const char* name, const char* parentName, double* matrix) {
    FbxNode* lNode = FbxNode::Create(lScene, name);

    // Convert double* matrix to FbxAMatrix
    FbxAMatrix lMtx;
    for (int i = 0; i < 4; i++) {
        for (int j = 0; j < 4; j++) {
            lMtx[i][j] = matrix[i * 4 + j];
        }
    }

    // Set local transform from matrix
    lNode->LclTranslation.Set(lMtx.GetT());
    lNode->LclRotation.Set(lMtx.GetR());
    lNode->LclScaling.Set(lMtx.GetS());

    // Switch Toolbox uses X, Y, Z order (Rotation order ZYX in FBX means X*Y*Z application)
    lNode->RotationOrder.Set(FbxEuler::eOrderZYX);

    // Skeleton Attribute
    FbxSkeleton* lSkeletonAttribute = FbxSkeleton::Create(lSdkManager, name);
    lSkeletonAttribute->SetSkeletonType(FbxSkeleton::eLimbNode);
    lNode->SetNodeAttribute(lSkeletonAttribute);

    NodeMap[name] = lNode;

    if (parentName && strlen(parentName) > 0) {
        if (NodeMap.find(parentName) != NodeMap.end()) {
            NodeMap[parentName]->AddChild(lNode);
        } else {
             lScene->GetRootNode()->AddChild(lNode);
        }
    } else {
        lScene->GetRootNode()->AddChild(lNode);
    }
}

EXPORT void Exp_AddPose(const char* name) {
    FbxPose* lPose = FbxPose::Create(lSdkManager, name);
    lPose->SetIsBindPose(true);

    for (auto const& [nodeName, nodePtr] : NodeMap) {
        FbxAMatrix lMtx = nodePtr->EvaluateGlobalTransform();
        lPose->Add(nodePtr, lMtx);
    }
    lScene->AddPose(lPose);
}

// Mesh Construction
struct TempVertex {
    double px, py, pz;
    double nx, ny, nz;
    double tx, ty, tz;
    double bx, by, bz;
    std::map<int, std::pair<double, double>> uvs;
    std::map<int, std::tuple<double, double, double, double>> colors;
    int bIndices[4];
    double bWeights[4];
};

std::vector<TempVertex> currentVertices;
std::vector<int> currentIndices;
std::string currentMeshName;
std::string currentMatName;

EXPORT void Exp_StartMesh(const char* name) {
    currentMeshName = name;
    currentVertices.clear();
    currentIndices.clear();
    currentMatName = "";
}

EXPORT void Exp_SetMeshMaterial(const char* matName) {
    currentMatName = matName;
}

EXPORT void Exp_AddVertexExtended(
    double px, double py, double pz,
    double nx, double ny, double nz,
    int b0, int b1, int b2, int b3,
    double w0, double w1, double w2, double w3
) {
    TempVertex v;
    v.px = px; v.py = py; v.pz = pz;
    v.nx = nx; v.ny = ny; v.nz = nz;
    v.tx = 0; v.ty = 0; v.tz = 0;
    v.bx = 0; v.by = 0; v.bz = 0;
    v.bIndices[0] = b0; v.bIndices[1] = b1; v.bIndices[2] = b2; v.bIndices[3] = b3;
    v.bWeights[0] = w0; v.bWeights[1] = w1; v.bWeights[2] = w2; v.bWeights[3] = w3;
    currentVertices.push_back(v);
}

EXPORT void Exp_AddVertexTangent(double x, double y, double z) {
    if (currentVertices.empty()) return;
    currentVertices.back().tx = x;
    currentVertices.back().ty = y;
    currentVertices.back().tz = z;
}

EXPORT void Exp_AddVertexBitangent(double x, double y, double z) {
    if (currentVertices.empty()) return;
    currentVertices.back().bx = x;
    currentVertices.back().by = y;
    currentVertices.back().bz = z;
}

EXPORT void Exp_AddVertexUV(int index, double u, double v) {
    if (currentVertices.empty()) return;
    currentVertices.back().uvs[index] = {u, v};
}

EXPORT void Exp_AddVertexColor(int index, double r, double g, double b, double a) {
    if (currentVertices.empty()) return;
    currentVertices.back().colors[index] = {r, g, b, a};
}

EXPORT void Exp_AddIndex(int index) {
    currentIndices.push_back(index);
}

std::vector<std::string> BoneNames;
EXPORT void Exp_RegisterBone(int id, const char* name) {
    if (id >= BoneNames.size()) {
        BoneNames.resize(id + 1);
    }
    BoneNames[id] = name;
}

// Map to store cluster matrices provided by C#
struct ClusterMatrices {
    FbxAMatrix meshTransform;
    FbxAMatrix linkTransform;
};
std::map<std::pair<std::string, std::string>, ClusterMatrices> ClusterMatrixMap;

EXPORT void Exp_SetClusterMatrices(const char* meshName, const char* boneName, double* meshMtx, double* linkMtx) {
    ClusterMatrices mats;
    for(int i=0; i<4; i++) {
        for(int j=0; j<4; j++) {
            mats.meshTransform[i][j] = meshMtx[i*4+j];
            mats.linkTransform[i][j] = linkMtx[i*4+j];
        }
    }
    ClusterMatrixMap[{meshName, boneName}] = mats;
}

EXPORT void Exp_EndMeshWithSkinning() {
    FbxMesh* lMesh = FbxMesh::Create(lScene, currentMeshName.c_str());

    lMesh->InitControlPoints(currentVertices.size());
    FbxVector4* controlPoints = lMesh->GetControlPoints();

    FbxGeometryElementNormal* lElementNormal = lMesh->CreateElementNormal();
    lElementNormal->SetMappingMode(FbxGeometryElement::eByControlPoint);
    lElementNormal->SetReferenceMode(FbxGeometryElement::eDirect);

    // Collect all unique UV, Color, Tangent, and Bitangent indices used in this mesh
    std::map<int, FbxGeometryElementUV*> uvElements;
    std::map<int, FbxGeometryElementVertexColor*> colorElements;
    FbxGeometryElementNormal* lElementTangent = nullptr;
    FbxGeometryElementNormal* lElementBitangent = nullptr;
    bool hasTangents = false;
    bool hasBitangents = false;

    for (const auto& v : currentVertices) {
        for (const auto& uvPair : v.uvs) {
            int idx = uvPair.first;
            if (uvElements.find(idx) == uvElements.end()) {
                std::string uvName = "UVMap" + (idx == 0 ? "" : std::to_string(idx));
                FbxGeometryElementUV* lElementUV = lMesh->CreateElementUV(uvName.c_str());
                lElementUV->SetMappingMode(FbxGeometryElement::eByControlPoint);
                lElementUV->SetReferenceMode(FbxGeometryElement::eDirect);
                uvElements[idx] = lElementUV;
            }
        }
        for (const auto& colPair : v.colors) {
            int idx = colPair.first;
            if (colorElements.find(idx) == colorElements.end()) {
                FbxGeometryElementVertexColor* lElementColor = lMesh->CreateElementVertexColor();
                lElementColor->SetMappingMode(FbxGeometryElement::eByControlPoint);
                lElementColor->SetReferenceMode(FbxGeometryElement::eDirect);
                colorElements[idx] = lElementColor;
            }
        }
        // Check if any vertex has tangents or bitangents
        if (v.tx != 0 || v.ty != 0 || v.tz != 0)
            hasTangents = true;
        if (v.bx != 0 || v.by != 0 || v.bz != 0)
            hasBitangents = true;
    }

    // Create tangent and bitangent elements if needed
    if (hasTangents) {
        lElementTangent = lMesh->CreateElementNormal();
        lElementTangent->SetMappingMode(FbxGeometryElement::eByControlPoint);
        lElementTangent->SetReferenceMode(FbxGeometryElement::eDirect);
    }
    if (hasBitangents) {
        lElementBitangent = lMesh->CreateElementNormal();
        lElementBitangent->SetMappingMode(FbxGeometryElement::eByControlPoint);
        lElementBitangent->SetReferenceMode(FbxGeometryElement::eDirect);
    }

    FbxSkin* lSkin = FbxSkin::Create(lScene, "");
    std::map<int, FbxCluster*> clusterMap;

    for (size_t i = 0; i < currentVertices.size(); i++) {
        TempVertex& v = currentVertices[i];
        controlPoints[i] = FbxVector4(v.px, v.py, v.pz);
        lElementNormal->GetDirectArray().Add(FbxVector4(v.nx, v.ny, v.nz));

        // Add UVs
        for (auto& pair : uvElements) {
            int idx = pair.first;
            if (v.uvs.find(idx) != v.uvs.end()) {
                pair.second->GetDirectArray().Add(FbxVector2(v.uvs[idx].first, v.uvs[idx].second));
            } else {
                pair.second->GetDirectArray().Add(FbxVector2(0, 0)); // default if missing
            }
        }

        // Add Colors
        for (auto& pair : colorElements) {
            int idx = pair.first;
            if (v.colors.find(idx) != v.colors.end()) {
                auto& c = v.colors[idx];
                pair.second->GetDirectArray().Add(FbxColor(std::get<0>(c), std::get<1>(c), std::get<2>(c), std::get<3>(c)));
            } else {
                pair.second->GetDirectArray().Add(FbxColor(1, 1, 1, 1)); // default if missing
            }
        }

        // Add Tangents
        if (hasTangents && lElementTangent != nullptr) {
            lElementTangent->GetDirectArray().Add(FbxVector4(v.tx, v.ty, v.tz, 1.0));
        }

        // Add Bitangents
        if (hasBitangents && lElementBitangent != nullptr) {
            lElementBitangent->GetDirectArray().Add(FbxVector4(v.bx, v.by, v.bz, 1.0));
        }

        for (int b = 0; b < 4; b++) {
             int boneID = v.bIndices[b];
             double weight = v.bWeights[b];
             if (weight <= 0.0001) continue;

             if (clusterMap.find(boneID) == clusterMap.end()) {
                 if (boneID < BoneNames.size()) {
                     std::string bName = BoneNames[boneID];
                     if (NodeMap.find(bName) != NodeMap.end()) {
                         FbxCluster* cluster = FbxCluster::Create(lScene, "");
                         cluster->SetLink(NodeMap[bName]);
                         cluster->SetLinkMode(FbxCluster::eTotalOne);

                         // Apply Bind Pose Matrices if provided
                         auto it = ClusterMatrixMap.find({currentMeshName, bName});
                         if (it != ClusterMatrixMap.end()) {
                              cluster->SetTransformMatrix(it->second.meshTransform);
                              cluster->SetTransformLinkMatrix(it->second.linkTransform);
                         }

                         clusterMap[boneID] = cluster;
                     }
                 }
             }

             if (clusterMap.find(boneID) != clusterMap.end()) {
                 clusterMap[boneID]->AddControlPointIndex(i, weight);
             }
        }
    }

    if (clusterMap.size() > 0) {
        for(auto const& [key, val] : clusterMap) {
            lSkin->AddCluster(val);
        }
        lMesh->AddDeformer(lSkin);
    }

    for(size_t i=0; i < currentIndices.size(); i+=3) {
        lMesh->BeginPolygon();
        lMesh->AddPolygon(currentIndices[i]);
        lMesh->AddPolygon(currentIndices[i+1]);
        lMesh->AddPolygon(currentIndices[i+2]);
        lMesh->EndPolygon();
    }

    FbxNode* lNode = FbxNode::Create(lScene, currentMeshName.c_str());
    lNode->SetNodeAttribute(lMesh);
    lScene->GetRootNode()->AddChild(lNode);

    if (!currentMatName.empty() && MaterialMap.find(currentMatName) != MaterialMap.end()) {
         lNode->AddMaterial(MaterialMap[currentMatName]);
    }
}

EXPORT void Exp_AddMaterial(const char* name, double r, double g, double b) {
    FbxSurfacePhong* lMaterial = FbxSurfacePhong::Create(lScene, name);
    lMaterial->Diffuse.Set(FbxDouble3(r, g, b));
    lMaterial->DiffuseFactor.Set(1.0);
    lMaterial->Ambient.Set(FbxDouble3(0.0, 0.0, 0.0));
    lMaterial->AmbientFactor.Set(1.0);
    lMaterial->Specular.Set(FbxDouble3(0.0, 0.0, 0.0));
    lMaterial->SpecularFactor.Set(0.0);
    lMaterial->Emissive.Set(FbxDouble3(0.0, 0.0, 0.0));
    lMaterial->EmissiveFactor.Set(0.0);
    lMaterial->TransparencyFactor.Set(0.0);
    MaterialMap[name] = lMaterial;
    ApplyMaterialTextures(name, lMaterial);
}

EXPORT void Exp_SetMaterialShininess(const char* matName, double shininess) {
    if (matName == nullptr)
        return;

    auto matIt = MaterialMap.find(matName);
    if (matIt == MaterialMap.end() || matIt->second == nullptr)
        return;

    matIt->second->Shininess.Set(shininess);
}

EXPORT void Exp_AddMaterialTexture(const char* matName, const char* textureName, const char* filePath, int textureType) {
    if (matName == nullptr || textureName == nullptr || filePath == nullptr)
        return;

    MaterialTextureBinding binding;
    binding.textureName = textureName;
    binding.filePath = filePath;
    binding.textureType = textureType;

    if (binding.textureName.empty() || binding.filePath.empty())
        return;

    auto& bindings = MaterialTextureMap[matName];
    for (const auto& existing : bindings) {
        if (existing.textureType == binding.textureType &&
            existing.textureName == binding.textureName &&
            existing.filePath == binding.filePath) {
            return;
        }
    }

    bindings.push_back(binding);

    auto matIt = MaterialMap.find(matName);
    if (matIt != MaterialMap.end()) {
        ConnectMaterialTexture(matIt->second, binding);
    }
}

EXPORT bool Exp_Save() {
    if (!lExporter) return false;
    return lExporter->Export(lScene);
}
