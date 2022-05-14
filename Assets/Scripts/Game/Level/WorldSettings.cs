using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WorldSettings
{
    [Min(1)]
    public int mapWidth, mapHeight;
    public int chunksPerRow;
    [Min(0.1F)]
    public float pixelsPerUnit;
    public float mapScale;
    [Range(1, 16)]
    public int octaves;
    public float lacunarity, persistence;
    public float maxHeight;
    public Vector2 Offset;
    public bool GenerateRoad;
    public bool SmoothTerrain;
    public float RoadThickness;
}
