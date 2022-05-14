using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("In-game settings")]
    public WorldSettings Settings;
    public Vector2Int RoadEndPoints;
    public float RoadInset;
    [System.NonSerialized]
    public SpriteAtlas spriteAtlas;
    public Material TerrainMaterial;


    private Chunk[,] chunks;
    private int MapPerimeter { get { return 2 * (Settings.mapWidth) + 2 * (Settings.mapHeight - 2); } }
    public List<Road> roadPoints;

    private void Awake()
    {
        instance = this;
        update = false;
        Destroy(planeMeshRenderer.gameObject);
        Destroy(terrainMeshRenderer);
        Destroy(meshFilter);
    }

    /// <summary>
    /// Generates the world.
    /// </summary>
    public void Generate()
    {
        if (Settings.chunksPerRow <= 0)
            Settings.chunksPerRow = 1;

        chunks = new Chunk[Settings.chunksPerRow, Settings.chunksPerRow];

        //Generate terrain height map
        Texture2D noise = NoiseGenerator.GenerateNoiseMap(Settings.mapWidth, Settings.mapHeight, 1, Settings.mapScale, Settings.octaves, Settings.lacunarity, Settings.persistence, Settings.Offset);
        Texture2D smoother = NoiseGenerator.GenerateNoiseMap(Settings.mapWidth, Settings.mapHeight, 1, Settings.mapScale, 1, Settings.lacunarity, Settings.persistence, Settings.Offset);

        Texture2D heightMap = Settings.SmoothTerrain ? NoiseGenerator.GetMultipliedNoiseMap(noise, smoother) : noise;

        //Generate road
        if (Settings.GenerateRoad)
        {
            Vector2 pos1 = GetPositionOnPerimeter(RoadEndPoints.x);
            Vector2 pos2 = GetPositionOnPerimeter(RoadEndPoints.y);

            Vector2 midPos = new Vector2Int(Settings.mapWidth / 2, Settings.mapHeight / 2);

            Vector2 insetPos1 = pos1 + (midPos - pos1).normalized * RoadInset;
            Vector2 insetPos2 = pos2 + (midPos - pos2).normalized * RoadInset;

            roadPoints = new List<Road>();
            Road currentRoad = RoadFinder.SearchRoad(Vector2Int.RoundToInt(insetPos1), Vector2Int.RoundToInt(insetPos2), heightMap, true);

            roadPoints.Add(currentRoad);
            while (currentRoad.type != RoadType.End)
            {
                roadPoints.Add(currentRoad);
                currentRoad = currentRoad.Next[0];
            }
            
        }

        //Blend the road with the terrain map
        Texture2D slopeMap = Settings.GenerateRoad ? NoiseGenerator.GetSlopeMap(RoadFinder.PathTexture, Settings.RoadThickness) : null;
        Texture2D finalHeightMap = slopeMap ? NoiseGenerator.GetMultipliedNoiseMap(slopeMap, heightMap) : heightMap;

        //Create the terrain appearance translation
        int[,] translation = TerrainTranslator.GetTranslation(finalHeightMap, (roadPoints, Settings.RoadThickness));

        //Create the sprite atlas for the terrain
        spriteAtlas = new SpriteAtlas();
        spriteAtlas.Create(GameRegistry.Instance.GetBlocks());

        int chunkWidth = (Settings.mapWidth - 2) / Settings.chunksPerRow;
        int chunkHeight = (Settings.mapHeight - 2) / Settings.chunksPerRow;

        //Generate the chunks
        for (int x = 0; x < Settings.chunksPerRow; x++)
        {
            for (int y = 0; y < Settings.chunksPerRow; y++)
            {
                GenerateChunk(finalHeightMap, translation, x, y, chunkWidth, chunkHeight);
            }
        }
    }

    private void GenerateChunk(Texture2D finalHeightMap, int[,] translation, int x, int y, int chunkWidth, int chunkHeight)
    {
        GameObject gameObj = new GameObject();
        gameObj.name = $"WorldChunk( {x}, {y} )";
        Chunk chunk = gameObj.AddComponent<Chunk>();
        Vector2 chunkPos = new Vector2(x * chunkWidth, y * chunkHeight);

        Vector4 cutout = new Vector4(chunkPos.x, chunkPos.y, chunkWidth + 2, chunkHeight + 2);

        chunk.MeshMaterial = TerrainMaterial;
        chunk.Generate(finalHeightMap, translation, spriteAtlas, chunkPos - Vector2.one, Settings.maxHeight, transform, cutout);

        chunks[x, y] = chunk;
    }

    private Vector2Int GetPositionOnPerimeter(int length)
    {


        //This code makes sure that you can input any number
        if (length < 0)
        {
            length += Mathf.FloorToInt((float)length / MapPerimeter) * -(MapPerimeter);
        }
        else if (length == 0) length = MapPerimeter;
        //---------------------------------------------------



        int[] size = { Settings.mapWidth - 1, Settings.mapHeight - 1 };
        int[] s = new int[2];
        int i = 0;
        int len = length - size[i];

        int side = 0;

        //loop while the current side length is not the correct side
        while (len > 0)
        {

            side = i % 4;


            s[i % 2] += size[i % 2] * GetSign(side);

            i++;
            len -= size[i % 2];

        }

        int l = len + size[i % 2];


        if (l != 0)//if the given length is not in the corner
        {

            s[i % 2] += l * GetSign(i % 4);

            //int cD = roadRadius + minCornerDistance;

        }

        if (len == 0)//if the given length is in the corner, it gets pushed backwards
        {

            //s[i % 2] += (roadRadius + minCornerDistance) * GetSign((i + 2) % 4);

        }

        return new Vector2Int(s[0], s[1]);


    }

    private int GetSign(int i)
    {
        return Mathf.RoundToInt(i / 3F) == 0 ? 1 : -1;
    }


    //EDITOR STUFF---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    [Header("Editor testing")]
    public bool update = false;
    public Texture2D noiseMapTexture;
    public Texture2D noiseMapRoad;
    public Texture2D VoxelTerrainTexture;
    public MeshRenderer planeMeshRenderer;
    public MeshRenderer terrainMeshRenderer;
    public MeshFilter meshFilter;

    /// <summary>
    /// Generates a noismap for testing purposes.
    /// </summary>
    public void GenerateNoiseMap()
    {
        if (planeMeshRenderer)
        {
            Texture2D a = NoiseGenerator.GenerateNoiseMap(Settings.mapWidth, Settings.mapHeight, 1, Settings.mapScale, Settings.octaves, Settings.lacunarity, Settings.persistence, Settings.Offset);
            Texture2D b = NoiseGenerator.GenerateNoiseMap(Settings.mapWidth, Settings.mapHeight, 1, Settings.mapScale, 1, Settings.lacunarity, Settings.persistence, Settings.Offset);

            noiseMapRoad = Settings.SmoothTerrain ? NoiseGenerator.GetMultipliedNoiseMap(a, b) : a;

            planeMeshRenderer.material.mainTexture = noiseMapRoad;
        }
    }

    /// <summary>
    /// Generates a terrain in the editor for testing purposes.
    /// </summary>
    public void GenerateTerrainInEditor()
    {
        if (terrainMeshRenderer)
        {
            spriteAtlas = new SpriteAtlas();
            spriteAtlas.Create(GameRegistry.Instance.GetBlocks());

            if (Settings.GenerateRoad) RoadFinder.SearchRoad(GetPositionOnPerimeter(RoadEndPoints.x), GetPositionOnPerimeter(RoadEndPoints.y), noiseMapRoad, true);

            Texture2D slopeMap = Settings.GenerateRoad ? NoiseGenerator.GetSlopeMap(RoadFinder.PathTexture, Settings.RoadThickness) : null;

            Texture2D multiplied = slopeMap ? NoiseGenerator.GetMultipliedNoiseMap(slopeMap, noiseMapRoad) : noiseMapRoad;


            planeMeshRenderer.material.mainTexture = multiplied;

            //GenerateChunk(multiplied, )
        }
    }
}

