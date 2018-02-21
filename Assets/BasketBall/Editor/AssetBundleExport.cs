/* ****************************************************************************
 * Product:    Basketball
 * Developer:  Mark Kreitler - markkreitler@protonmail.com
 * Company:    DefaultCompany
 * Date:       20/02/2018 11:21
 *****************************************************************************/
// Create an AssetBundle for Windows.
using UnityEngine;
using UnityEditor;

public class BuildAssetBundlesExample : MonoBehaviour
{
    [MenuItem("Assets/Build Asset Bundles")]
    static void BuildABs()
    {
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/Bundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
    }
}
