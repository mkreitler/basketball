/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       20/02/2018 11:21
 *****************************************************************************/

using UnityEngine;
using UnityEditor;

public class BuildAssetBundlesExample : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundles/Mac")]
    static void BuildABs()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/Basketball/BasketballBundles_mac", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
    }

    [MenuItem("Assets/Build Asset Bundles/iOS")]
    static void BuildABsIOS()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/Basketball/BasketballBundles_iOS", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    [MenuItem("Assets/Build Asset Bundles/Android")]
    static void BuildABsAndroid()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/Basketball/BasketballBundles_android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
