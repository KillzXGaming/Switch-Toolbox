#include <fbxsdk.h>
#include <map>
#include <string>
#include <cmath>

#define EXPORT extern "C" __declspec(dllexport)

extern FbxManager* lSdkManager;
extern FbxScene* lScene;
extern std::map<std::string, FbxNode*> NodeMap;

namespace
{
    enum AnimChannel
    {
        ChannelTranslateX = 0,
        ChannelTranslateY = 1,
        ChannelTranslateZ = 2,
        ChannelRotateX = 3,
        ChannelRotateY = 4,
        ChannelRotateZ = 5,
        ChannelScaleX = 6,
        ChannelScaleY = 7,
        ChannelScaleZ = 8,
    };

    enum AnimInterpolation
    {
        InterpLinear = 0,
        InterpConstant = 1,
        InterpCubic = 2,
    };

    FbxAnimStack* gCurrentAnimStack = nullptr;
    FbxAnimLayer* gCurrentAnimLayer = nullptr;
    double gCurrentFrameRate = 30.0;

    FbxTime::EMode GetTimeMode(double frameRate)
    {
        if (std::fabs(frameRate - 24.0) < 0.0001) return FbxTime::eFrames24;
        if (std::fabs(frameRate - 25.0) < 0.0001) return FbxTime::ePAL;
        if (std::fabs(frameRate - 30.0) < 0.0001) return FbxTime::eFrames30;
        if (std::fabs(frameRate - 48.0) < 0.0001) return FbxTime::eFrames48;
        if (std::fabs(frameRate - 50.0) < 0.0001) return FbxTime::eFrames50;
        if (std::fabs(frameRate - 60.0) < 0.0001) return FbxTime::eFrames60;
        return FbxTime::eCustom;
    }

    FbxAnimCurve* GetCurve(FbxNode* node, int channel)
    {
        if (node == nullptr || gCurrentAnimLayer == nullptr)
            return nullptr;

        switch (channel)
        {
        case ChannelTranslateX:
            return node->LclTranslation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_X, true);
        case ChannelTranslateY:
            return node->LclTranslation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Y, true);
        case ChannelTranslateZ:
            return node->LclTranslation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Z, true);
        case ChannelRotateX:
            return node->LclRotation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_X, true);
        case ChannelRotateY:
            return node->LclRotation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Y, true);
        case ChannelRotateZ:
            return node->LclRotation.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Z, true);
        case ChannelScaleX:
            return node->LclScaling.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_X, true);
        case ChannelScaleY:
            return node->LclScaling.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Y, true);
        case ChannelScaleZ:
            return node->LclScaling.GetCurve(gCurrentAnimLayer, FBXSDK_CURVENODE_COMPONENT_Z, true);
        default:
            return nullptr;
        }
    }
}

EXPORT void Exp_Anim_Reset()
{
    gCurrentAnimStack = nullptr;
    gCurrentAnimLayer = nullptr;
    gCurrentFrameRate = 30.0;
}

EXPORT bool Exp_Anim_Begin(const char* name, double frameRate, double startFrame, double endFrame, int /*looping*/)
{
    if (lScene == nullptr || lSdkManager == nullptr)
        return false;

    if (frameRate <= 0.0)
        frameRate = 30.0;

    gCurrentFrameRate = frameRate;

    const char* stackName = (name != nullptr && name[0] != '\0') ? name : "Take001";
    gCurrentAnimStack = FbxAnimStack::Create(lScene, stackName);
    gCurrentAnimLayer = FbxAnimLayer::Create(lScene, "BaseLayer");
    gCurrentAnimStack->AddMember(gCurrentAnimLayer);

    FbxTime::EMode timeMode = GetTimeMode(frameRate);
    lScene->GetGlobalSettings().SetTimeMode(timeMode);
    if (timeMode == FbxTime::eCustom)
        lScene->GetGlobalSettings().SetCustomFrameRate(frameRate);
    FbxTime::SetGlobalTimeMode(timeMode, frameRate);

    FbxTime start;
    start.SetSecondDouble(startFrame / frameRate);
    FbxTime stop;
    stop.SetSecondDouble(endFrame / frameRate);

    FbxTimeSpan timeSpan(start, stop);
    lScene->GetGlobalSettings().SetTimelineDefaultTimeSpan(timeSpan);
    gCurrentAnimStack->LocalStart.Set(start);
    gCurrentAnimStack->LocalStop.Set(stop);
    gCurrentAnimStack->ReferenceStart.Set(start);
    gCurrentAnimStack->ReferenceStop.Set(stop);

    return true;
}

EXPORT bool Exp_Anim_SetBoneDefaults(
    const char* boneName,
    double tx, double ty, double tz,
    double rx, double ry, double rz,
    double sx, double sy, double sz)
{
    if (boneName == nullptr)
        return false;

    auto nodeIt = NodeMap.find(boneName);
    if (nodeIt == NodeMap.end())
        return false;

    FbxNode* node = nodeIt->second;
    node->LclTranslation.Set(FbxDouble3(tx, ty, tz));
    node->LclRotation.Set(FbxDouble3(rx, ry, rz));
    node->LclScaling.Set(FbxDouble3(sx, sy, sz));
    return true;
}

EXPORT bool Exp_Anim_AddKey(
    const char* boneName,
    int channel,
    double frame,
    double value,
    int interpolation,
    double tangentIn,
    double tangentOut)
{
    if (boneName == nullptr || gCurrentAnimLayer == nullptr)
        return false;

    auto nodeIt = NodeMap.find(boneName);
    if (nodeIt == NodeMap.end())
        return false;

    FbxAnimCurve* curve = GetCurve(nodeIt->second, channel);
    if (curve == nullptr)
        return false;

    if (gCurrentFrameRate <= 0.0)
        gCurrentFrameRate = 30.0;

    FbxTime time;
    time.SetSecondDouble(frame / gCurrentFrameRate);

    curve->KeyModifyBegin();
    int keyIndex = curve->KeyAdd(time);
    curve->KeySetValue(keyIndex, static_cast<float>(value));

    switch (interpolation)
    {
    case InterpConstant:
        curve->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationConstant);
        curve->KeySetConstantMode(keyIndex, FbxAnimCurveDef::eConstantStandard);
        break;
    case InterpCubic:
        curve->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationCubic);
        curve->KeySetTangentMode(keyIndex, FbxAnimCurveDef::eTangentUser);
        curve->KeySetLeftDerivative(keyIndex, static_cast<float>(tangentIn));
        curve->KeySetRightDerivative(keyIndex, static_cast<float>(tangentOut));
        break;
    case InterpLinear:
    default:
        curve->KeySetInterpolation(keyIndex, FbxAnimCurveDef::eInterpolationLinear);
        break;
    }

    curve->KeyModifyEnd();
    return true;
}

EXPORT void Exp_Anim_End()
{
    gCurrentAnimLayer = nullptr;
    gCurrentAnimStack = nullptr;
}
