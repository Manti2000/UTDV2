using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAtlas
{
    public Texture2D AtlasTexture;
    private Dictionary<int, (SideFace, Rect)[]> Map = new Dictionary<int, (SideFace, Rect)[]>();
    
    public (SideFace, Rect)[] GetTexCoordsForBlockID(int id)
    {
        if (Map.ContainsKey(id))
        {
            return Map[id];
        }
        else
        {
            Debug.LogWarning($"No block registered in the atlas with the id {id}.");
        }
        return null;
    }

    /// <summary>
    /// Creates the sprite atlas from the given block array.
    /// </summary>
    /// <param name="blocks"></param>
    public void Create(Block[] blocks)
    {
        //Gather textures from registered blocks
        List<Texture2D> textures = new List<Texture2D>();
        for(int i = 0; i < blocks.Length; i++)
        {
            for(int k = 0; k < blocks[i].settings.sides.Length; k++)
            {
                textures.Add(blocks[i].settings.sides[k].sprite.texture);
            }
        }
        
        //length of atlas sides
        int length = GetSideLengthFromCount(textures.Count);

        //Create sprite atlas
        AtlasTexture = new Texture2D(length * 128, length * 128);
        AtlasTexture.filterMode = FilterMode.Point;
        AtlasTexture.wrapModeU = TextureWrapMode.Clamp;
        AtlasTexture.wrapMode = TextureWrapMode.Clamp;
        AtlasTexture.wrapModeV = TextureWrapMode.Clamp;
        AtlasTexture.wrapModeW = TextureWrapMode.Clamp;

        Rect[] rects = AtlasTexture.PackTextures(textures.ToArray(), 10);

        //Creating a padding from the textures sides
        Color[] colors = AtlasTexture.GetPixels();
        foreach (Rect uvrect in rects)
        {
            int x = (int)(uvrect.x * AtlasTexture.width);
            int y = (int)(uvrect.y * AtlasTexture.height);
            int width = (int)(uvrect.width * AtlasTexture.width);
            int height = (int)(uvrect.height * AtlasTexture.height);

            //Padding bottom
            int py = y + height, px;
            for (px = x; px < x + width; px++)
            {

                colors[(py) * AtlasTexture.width + px] = colors[(py - 1) * AtlasTexture.width + px];
            }

            //Padding top
            py = y;
            if (py > 0)
            {
                for (px = x; px < x + width; px++)
                {
                    colors[(py - 1) * AtlasTexture.width + px] = colors[(py) * AtlasTexture.width + px];
                }
            }

            //Padding left
            px = x;
            if (px > 0)
            {
                for (py = y; py < y + height; py++)
                {

                    colors[(py) * AtlasTexture.width + px - 1] = colors[(py) * AtlasTexture.width + px];
                }
            }

            //Padding right
            px = x + width;
            for (py = y; py < y + height; py++)
            {

                colors[(py) * AtlasTexture.width + px] = colors[(py) * AtlasTexture.width + px - 1];
            }
        }

        AtlasTexture.SetPixels(colors);
        AtlasTexture.Apply();


        //Filling up the dictionary
        for (int index = 0, rectIndex = 0; index < blocks.Length; index++)
        {
            (SideFace, Rect)[] faces = new (SideFace, Rect)[blocks[index].settings.sides.Length];
            for (int i = 0; i < blocks[index].settings.sides.Length; i++, rectIndex++)
            {
                faces[i].Item1 = blocks[index].settings.sides[i].side;
                faces[i].Item2 = rects[rectIndex];
            }
            Map.Add(index, faces);
        }
        
    }

    private int GetSideLengthFromCount(int textureCount)
    {
        return Mathf.CeilToInt(textureCount / 2F);
    }
}

