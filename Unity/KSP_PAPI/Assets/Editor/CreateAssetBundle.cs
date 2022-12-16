using UnityEngine;
using UnityEditor;

public class CreateAssetBundle : MonoBehaviour
{
    [MenuItem("KSP_PAPI/Build Asset Bundles")]
    static void BuildABs()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

        buildMap[0].assetBundleName = "ksp_papi.assetbundle";

        string[] papiAssets = new string[5];
        papiAssets[0] = "Assets/KSP_PAPI/KSP_PAPI.fbx";
        papiAssets[1] = "Assets/KSP_PAPI/KSP_PAPI.png";
        papiAssets[2] = "Assets/KSP_PAPI/KSP_PAPI.mat";
        papiAssets[3] = "Assets/KSP_PAPI/KSP_PAPI.shader";
        papiAssets[4] = "Assets/KSP_PAPI/KSP_PAPI.prefab";

        buildMap[0].assetNames = papiAssets;
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}