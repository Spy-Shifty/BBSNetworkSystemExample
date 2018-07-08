using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TerrainTools {

    [MenuItem("TerrainTools/Enable Splatmaps")]
    private static void EnableSplatmaps() {

        Object activeObject = Selection.activeObject;
        if(!(activeObject is TerrainData)) {
            Debug.LogError("Selected Object isn't an TerrainData Asset");
            return;
        }

        Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(activeObject));
        foreach (Object o in data) {
            if (o is Texture2D) {
                (o as Texture2D).hideFlags = HideFlags.None;
                Debug.Log(o);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(o));
            }
        }
    }

    [MenuItem("TerrainTools/Disable Basemap")]
    private static void DisableBasemap() {

        Object activeObject = Selection.activeObject;
        Terrain terrain = (activeObject as GameObject)?.GetComponent<Terrain>();
        if (!terrain) {
            Debug.LogError("Selected Object doesn't have an Terrain");
            return;
        }

        terrain.basemapDistance = 10000000;
        Debug.Log(terrain.name + "  " + terrain.basemapDistance);
    }
}
