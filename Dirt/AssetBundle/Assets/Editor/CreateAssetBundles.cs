using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

public class CreateAssetBundles
{
    static string buildDirectory = "D:/SteamLibrary/steamapps/common/My Summer Car/ModAssetBundles";
    static string assetsDirectory = "D:/SteamLibrary/steamapps/common/My Summer Car/Mods/Assets";

    static Dictionary<string, Action> copyDictionary = new Dictionary<string, Action>
    {
		{"dirtmod.unity3d", () => CopyAssetBundle("dirtmod.unity3d", "DirtModRevived/dirtmod.unity3d")},
    };
    
    [MenuItem("AssetBundle/Build All", priority = 1)]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(buildDirectory);

        Debug.Log("AssetBundles built successfully!");
    }

    [MenuItem("AssetBundle/Build All and Copy", priority = 2)]
    static void BuildCopyAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(buildDirectory);

        foreach (Action copyAction in copyDictionary.Values) copyAction();

        Debug.Log("AssetBundles built successfully!");
    }

    [MenuItem("AssetBundle/Build Selected", priority = 20)]
    static void BuildSelectedAssetBundles()
    {
        BuildBundlesFromSelection(buildDirectory);

        Debug.Log("Asset Bundles built successfully!");
    }

    [MenuItem("AssetBundle/Build Selected and Copy", priority = 21)]
    static void BuildCopySelectedAssetBundles()
    {
        string[] bundles = BuildBundlesFromSelection(buildDirectory);

        if (bundles != null || bundles.Length > 0)
        {
            foreach (string bundle in bundles.Where(x => copyDictionary.ContainsKey(x)))
                copyDictionary[bundle]();

            Debug.Log("Asset Bundles built successfully!");
        }
    }

    [MenuItem("AssetBundle/Open Build Directory", priority = 80)]
    static void OpenBuildDirectory()
    {
        Process.Start(buildDirectory);
    }

    [MenuItem("AssetBundle/Open MSC Assets Directory", priority = 81)]
    static void OpenAssetsDirectory()
    {
        Process.Start(assetsDirectory);
    }

    [MenuItem("AssetBundle/Copy Selected Bundle(s)", priority = 100)]
    static void CopySelectedBundles()
    {
        var assets = Selection.objects.Where(o => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).ToArray();

        foreach (var bundleName in assets.Select(x => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(x)).assetBundleName).Distinct())
            if (copyDictionary.ContainsKey(bundleName)) copyDictionary[bundleName]();

        Debug.Log("AssetBundles copied!");
    }

    static void CopyAssetBundle(string bundleName, string destination)
    {
        File.Copy(Path.Combine(buildDirectory, bundleName), Path.Combine(assetsDirectory, destination), true);
    }

    static void RemoveAssetBundle(string bundleName)
    {
        if (File.Exists(Path.Combine(buildDirectory, bundleName)))
            File.Delete(Path.Combine(buildDirectory, bundleName));

        if (File.Exists(Path.Combine(buildDirectory, bundleName + ".manifest")))
            File.Delete(Path.Combine(buildDirectory, bundleName + ".manifest"));
    }

    static string[] BuildBundlesFromSelection(string directory)
    {
        if (Selection.objects.Count() > 0)
        {
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            HashSet<string> processedBundles = new HashSet<string>();

            var selected = Selection.objects.Where(x => !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(x)));

            foreach (var o in selected)
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);

                if (importer == null) continue;

                string assetBundleName = importer.assetBundleName;
                string assetBundleVariant = importer.assetBundleVariant;
                string assetBundleFullName = string.IsNullOrEmpty(assetBundleVariant) ? assetBundleName : assetBundleName + "." + assetBundleVariant;

                if (processedBundles.Contains(assetBundleFullName)) continue;
                processedBundles.Add(assetBundleFullName);

                assetBundleBuilds.Add(new AssetBundleBuild
                {
                    assetBundleName = assetBundleName,
                    assetBundleVariant = assetBundleVariant,
                    assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleFullName)
                });
            }

            BuildPipeline.BuildAssetBundles(directory, assetBundleBuilds.ToArray());

            return assetBundleBuilds.Select(x => x.assetBundleName).ToArray();
        }
        else return null;
    }
    //https://bitbucket.org/Unity-Technologies/assetbundledemo/src/00a6393792a438cfbe521e520136f515d728fa00/demo/Assets/AssetBundleManager/Editor/AssetbundlesMenuItems.cs
}
