using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public TerrainMeshData Data;
    
    public Material MeshMaterial;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public void Generate(Texture2D heightmap, int[,] translation, SpriteAtlas spriteAtlas, Vector2 pos, float maxHeight, Transform parent = null, Vector4 cutout = new Vector4())
    {
        transform.parent = parent;

        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Data = TerrainGenerator.CreateVoxelTerrain(heightmap, maxHeight, translation, spriteAtlas, cutout);

        meshFilter.sharedMesh = Data.GetMesh();

        meshRenderer.material = MeshMaterial;
        meshRenderer.material.mainTexture = spriteAtlas.AtlasTexture;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

        transform.position = new Vector3(pos.x, 0, pos.y);

        meshCollider = gameObject.AddComponent<MeshCollider>();
    }
}
