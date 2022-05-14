using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class MapGeneratorEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        Map gen = (Map) target;

        if (DrawDefaultInspector())
        {
            if (gen.update)
            {
                gen.GenerateNoiseMap();
                gen.GenerateTerrainInEditor();
            }
        }

        //if (gen.spriteAtlas != null && gen.spriteAtlas.AtlasTexture != null) GUI.DrawTexture(new Rect(10, 10, 100, 100), gen.spriteAtlas.AtlasTexture, ScaleMode.ScaleToFit, true, 10F);

        if (GUILayout.Button("Generate noise map"))
        {
            gen.GenerateNoiseMap();
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            gen.GenerateTerrainInEditor();
        }


    }

    

    

}
