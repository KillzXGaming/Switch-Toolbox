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
}

EXPORT bool Exp_Initialize(const char* filename) {
    if (!lSdkManager) Exp_CreateContext();

    lScene = FbxScene::Create(lSdkManager, "SwitchToolboxExport");
    
    // Axis Setup (Switch/OpenGL is usually Y Up or Z Up, FBX defaults to Y Up)
    // DAE exporter handles rotation, we might need to set axis system here if needed
    // FbxAxisSystem::MayaZUp.ConvertScene(lScene); // Example if needed

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
    double u0x, u0y;
    double c0r, c0g, c0b, c0a;
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

EXPORT void Exp_AddVertex(
    double px, double py, double pz,
    double nx, double ny, double nz,
    double u0x, double u0y,
    double cr, double cg, double cb, double ca,
    int b0, int b1, int b2, int b3,
    double w0, double w1, double w2, double w3
) {
    TempVertex v;
    v.px = px; v.py = py; v.pz = pz;
    v.nx = nx; v.ny = ny; v.nz = nz;
    v.u0x = u0x; v.u0y = u0y;
    v.c0r = cr; v.c0g = cg; v.c0b = cb; v.c0a = ca;
    v.bIndices[0] = b0; v.bIndices[1] = b1; v.bIndices[2] = b2; v.bIndices[3] = b3;
    v.bWeights[0] = w0; v.bWeights[1] = w1; v.bWeights[2] = w2; v.bWeights[3] = w3;
    currentVertices.push_back(v);
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

    FbxGeometryElementUV* lElementUV = lMesh->CreateElementUV("UVMap");
    lElementUV->SetMappingMode(FbxGeometryElement::eByControlPoint);
    lElementUV->SetReferenceMode(FbxGeometryElement::eDirect);

    FbxGeometryElementVertexColor* lElementColor = lMesh->CreateElementVertexColor();
    lElementColor->SetMappingMode(FbxGeometryElement::eByControlPoint);
    lElementColor->SetReferenceMode(FbxGeometryElement::eDirect);

    FbxSkin* lSkin = FbxSkin::Create(lScene, "");
    std::map<int, FbxCluster*> clusterMap;

    for (size_t i = 0; i < currentVertices.size(); i++) {
        TempVertex& v = currentVertices[i];
        controlPoints[i] = FbxVector4(v.px, v.py, v.pz);
        lElementNormal->GetDirectArray().Add(FbxVector4(v.nx, v.ny, v.nz));
        lElementUV->GetDirectArray().Add(FbxVector2(v.u0x, v.u0y));
        lElementColor->GetDirectArray().Add(FbxColor(v.c0r, v.c0g, v.c0b, v.c0a));

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
    lMaterial->Specular.Set(FbxDouble3(0.0, 0.0, 0.0));
    lMaterial->Emissive.Set(FbxDouble3(0.0, 0.0, 0.0));
    MaterialMap[name] = lMaterial;
}

EXPORT bool Exp_Save() {
    if (!lExporter) return false;
    return lExporter->Export(lScene);
}

