using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VertexPos
{
    BottomLeft,
    BottomRight,
    TopRight,
    TopLeft
}

public enum SideFace
{
    South,
    East,
    North,
    West,
    Top,
    AllSides,
    All,
    Unknown
}

public class TerrainGenerator
{
    public static TerrainMeshData CreateVoxelTerrain(Texture2D map, float height, int[,] translation, SpriteAtlas spriteAtlas, Vector4 cutout = new Vector4())
    {

        int mapWidth, mapHeight;
        if(cutout.z <= 0 || cutout.w <= 0)
        {
            mapWidth = map.width;
            mapHeight = map.height;
            cutout.x = 0;
            cutout.y = 0;
        }
        else
        {
            mapWidth = (int)cutout.z;
            mapHeight = (int)cutout.w;
        }

        //Creating the voxels
        List<Vector3> vertices = new List<Vector3>();
        Voxel[,] voxels = new Voxel[mapWidth, mapHeight];

        for (int x = 0; x < voxels.GetLength(0); x++)
        {
            for (int y = 0; y < voxels.GetLength(1); y++)
            {
                Voxel voxel = new Voxel(x, y, (map.GetPixel((int)cutout.x + x, (int)cutout.y + y).r * height));
                voxels[x, y] = voxel;

                int blockID = translation[(int)cutout.x + x, (int)cutout.y + y];
                voxels[x, y].blockID = blockID;
            }
        }

        //Creating the vertices
        for (int x = 1; x < voxels.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < voxels.GetLength(1) - 1; y++)
            {

                voxels[x, y].CreateAllFaceVertices(vertices, GetNeighbours(voxels[x, y], voxels, false, true));

            }
        }


        //Create triangles
        List<int> triangles = new List<int>();

        for (int x = 1; x < voxels.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < voxels.GetLength(1) - 1; y++)
            {
                foreach (Face face in voxels[x, y].faces)
                {

                    face.CreateFace(triangles);

                }
            }
        }

        //UV mapping
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int x = 1; x < voxels.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < voxels.GetLength(1) - 1; y++)
            {
                SetUVCoordinates(ref voxels[x, y], spriteAtlas.GetTexCoordsForBlockID(voxels[x, y].blockID), vertices, uvs);
            }
        }


        TerrainMeshData meshData = new TerrainMeshData();
        meshData.vertices = vertices.ToArray();
        meshData.triangles = triangles.ToArray();
        meshData.uvs = uvs;

        return meshData;
    }

    private static void SetUVCoordinates(ref Voxel voxel, (SideFace, Rect)[] sides, List<Vector3> vertices, Vector2[] uvs)
    {
        foreach ((SideFace, Rect) side in sides)
        {
            if (side.Item1 == SideFace.Unknown)
                continue;

            foreach (Face face in voxel.faces)
            {
                if (side.Item1 == SideFace.AllSides && face.side == SideFace.Top)
                    continue;
                if (side.Item1 != SideFace.All && side.Item1 != SideFace.AllSides && face.side != side.Item1)
                    continue;

                float heightDif = Mathf.Clamp(Mathf.Abs((vertices[face.vertices[(int)VertexPos.TopLeft]] - vertices[face.vertices[(int)VertexPos.BottomLeft]]).y), 0, 1);

                if (heightDif == 0)
                    heightDif = 1;

                Rect rect = side.Item2;
                Vector2 topLeft = new Vector2(rect.x, rect.y + (rect.height - rect.height * heightDif)),
                        topRight = new Vector2(rect.x + rect.width, rect.y + (rect.height - rect.height * heightDif)),
                        bottomLeft = new Vector2(rect.x, rect.y + rect.height),
                        bottomRight = new Vector2(rect.x + rect.width, rect.y + rect.height);


                uvs[face.vertices[(int)VertexPos.BottomLeft]] = bottomLeft;
                uvs[face.vertices[(int)VertexPos.BottomRight]] = bottomRight;
                uvs[face.vertices[(int)VertexPos.TopLeft]] = topLeft;
                uvs[face.vertices[(int)VertexPos.TopRight]] = topRight;

            }
        }
    }

    private static Voxel[] GetNeighbours(Voxel voxel, Voxel[,] map, bool corners = true, bool filterByHeight = false)
    {
        List<Voxel> sides = new List<Voxel>();

        for (int y = voxel.y - 1; y < voxel.y + 2; y++)
        {

            for (int x = voxel.x - 1; x < voxel.x + 2; x++)
            {

                if (!(x < map.GetLength(0) && x >= 0 && y < map.GetLength(1) && y >= 0) || (map[x, y] == null) || ((voxel.x == x) && (voxel.y == y)) || (!corners && (x != voxel.x && y != voxel.y)))
                    continue;

                if (filterByHeight && voxel.height <= map[x, y].height)
                    continue;


                sides.Add(map[x, y]);
            }
        }

        return sides.ToArray();
    }

}

public class Face
{
    public int[] vertices = new int[4];
    public SideFace side;

    public void SetVertex(VertexPos pos, int vertexIndex)
    {
        vertices[(int)pos] = vertexIndex;
    }

    public void CreateFace(List<int> triangles)
    {
        //Create top face
        triangles.Add(this.vertices[0]);
        triangles.Add(this.vertices[3]);
        triangles.Add(this.vertices[1]);

        triangles.Add(this.vertices[3]);
        triangles.Add(this.vertices[2]);
        triangles.Add(this.vertices[1]);

    }

    
}

public class Voxel
{
    public int x, y, blockID;
    public Vector3 Position { get { return new Vector3(x, height, y); } }
    public float height;
    public List<Face> faces = new List<Face>();
    public Face topFace { get { return faces[faces.Count - 1]; } }

    public Voxel(int x, int y, float height)
    {
        this.x = x;
        this.y = y;
        this.height = height;
    }

    public void CreateAllFaceVertices(List<Vector3> vertices, Voxel[] neighbours)
    {
        Vector3 direction;
        Vector3 normal;


        foreach (Voxel n in neighbours)
        {
            direction = new Vector3(n.x - x, 0, n.y - y);
            normal = new Vector3(-direction.z, 0, direction.x);

            float heightDif = height - n.height - 0.5F;

            CreateSideFaceVertices(vertices, direction, normal, heightDif, GetSideFromDirection(direction));
        }

        direction = new Vector3(0, 0.5F, 0);
        normal = new Vector3(-0.5F, 0, 0);

        //TOP FACE
        CreateSideFaceVertices(vertices, direction, normal, 0.5F, SideFace.Top);

    }

    private void CreateSideFaceVertices(List<Vector3> vertices, Vector3 direction, Vector3 normal, float heightDifference = 0.5F, SideFace side = SideFace.Unknown)
    {
        direction = direction.normalized / 2;

        normal = normal.normalized / 2;

        Vector3 Up = Vector3.Cross(normal, direction).normalized;
        
        Vector3 vertexTopLeft = Position + direction + normal + Up / 2;
        Vector3 vertexTopRight = vertexTopLeft - normal * 2;

        Vector3 vertexBottomRight = Position + direction - normal - Up * heightDifference;
        Vector3 vertexBottomLeft = vertexBottomRight + normal * 2;

        vertices.Add(vertexBottomLeft);
        vertices.Add(vertexBottomRight);
        vertices.Add(vertexTopRight);
        vertices.Add(vertexTopLeft);

        Face face = new Face();

        for (int i = 0; i < 4; i++)
        {
            face.SetVertex((VertexPos)(i), vertices.Count - i - 1);
        }

        face.side = side;
        faces.Add(face);
    }

    private SideFace GetSideFromDirection(Vector3 direction)
    {
        if(direction.x > 0 && direction.z == 0)
        {
            return SideFace.East;
        }
        if (direction.x < 0 && direction.z == 0)
        {
            return SideFace.West;
        }
        if (direction.x == 0 && direction.z > 0)
        {
            return SideFace.North;
        }
        if (direction.x == 0 && direction.z < 0)
        {
            return SideFace.South;
        }
        return SideFace.Unknown;
    }
}

public struct TerrainMeshData
{
    public Voxel[,] voxels;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    public Mesh GetMesh(string name = "")
    {
        Mesh mesh = new Mesh();
        mesh.name = name;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }
}

