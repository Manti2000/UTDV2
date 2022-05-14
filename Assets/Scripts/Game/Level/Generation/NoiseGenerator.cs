using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{

    /// <summary>
    /// Generates a perlin noise map.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="scale"></param>
    /// <param name="Offset"></param>
    /// <returns></returns>
    public static Texture2D GenerateNoiseMap(int width, int height, float pixelsPerUnit, float scale, int octaves, float lacunarity, float persistence, Vector2 Offset)
    {
        if (pixelsPerUnit <= 0.0001F)
                pixelsPerUnit = 0.0001F;

        float stepX = (width) / (width * pixelsPerUnit - 1);
        float stepY = (height) / (height * pixelsPerUnit - 1);

        width = (int)(width * pixelsPerUnit);
        height = (int)(height * pixelsPerUnit);

        Color[] colors = new Color[width * height];

        float max = float.MinValue;
        float min = float.MaxValue;

        for (int k = 0; k < height; k++)
        {
            for (int i = 0; i < width; i++)
            {
                float perlinValue = 0;
                float frequency = 1;
                float amplitude = 1;

                for(int j = 0; j < octaves; j++)
                {
                    perlinValue += Mathf.PerlinNoise(((i + Offset.x) * stepX) / scale * frequency, ((k + Offset.y) * stepY) / scale * frequency) * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                //perlinValue *= Mathf.PerlinNoise(((i + 10000) * stepX) / scale, ((k + 10000) * stepY) / scale);


                if (perlinValue < min)
                {
                    min = perlinValue;
                }
                if(perlinValue > max)
                {
                    max = perlinValue;
                }

                colors[k * width + i] = new Color(1, 1, 1) * perlinValue;
            }
        }

        for (int k = 0; k < height; k++)
        {
            for (int i = 0; i < width; i++)
            {

                float val = colors[k * width + i].r;



                float v = Mathf.InverseLerp(min, max, val);



                colors[k * width + i] = new Color(1, 1, 1) * v;

            }
        }


        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;

    }

    public static Texture2D GetMultipliedNoiseMap(Texture2D map1, Texture2D map2)
    {

        if(map1.width != map2.width || map1.height != map2.height)
        {
            Debug.LogWarning("Map multiplication can't be done: The maps are not the same size!");
            return null;
        }

        int height = map1.height, width = map1.width;

        Color[] colors = new Color[width * height];

        for (int k = 0; k < height; k++)
        {
            for (int i = 0; i < width; i++)
            {

                float val1 = map1.GetPixel(i, k).r;
                float val2 = map2.GetPixel(i, k).r;

                colors[k * width + i] = Color.white * val1 * val2;

            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;

        texture.SetPixels(colors);

        texture.Apply();

        return texture;
    }

    public static Texture2D GetSlopeMap(Texture2D map, float multiplier)
    {
        int width = map.width, height = map.height;


        Texture2D texture = new Texture2D(width, height);
        List<Vector2Int> basePixels = new List<Vector2Int>();
        Color[] mapPixels = map.GetPixels();
        Color[] pixels = new Color[width * height];

        for (int k = 0; k < height; k++)
        {

            for (int i = 0; i < width; i++)
            {
                int index = k * width + i;

                if (mapPixels[index] == Color.black) basePixels.Add(new Vector2Int(i, k));


            }

        }

        for (int k = 0; k < height; k++)
        {

            for (int i = 0; i < width; i++)
            {
                int index = k * width + i;

                pixels[index] = GetSValue(i, k, basePixels, map, multiplier);

            }

        }

        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point;

        texture.Apply();

        return texture;

    }

    //Generates a white dotted map based off of the peaks of a noise map(perlin)
    public static Texture2D GetPeakMap(Texture2D texture)
    {
        Color[] noiseMap = texture.GetPixels();
        Color[] peakMap = new Color[noiseMap.Length];


        for (int k = 0; k < texture.height; k++)
        {


            for (int i = 0; i < texture.width; i++)
            {
                int index = k * texture.width + i;
                float height = noiseMap[index].r;

                bool peakH = true;
                bool peakL = true;

                //Comparing the height to the neighboring values

                for (int y = k - 1; y < k + 2; y++)
                {

                    for (int x = i - 1; x < i + 2; x++)
                    {

                        int index2 = 0;



                        if (!(x < texture.width && x >= 0 && y < texture.height && y >= 0) || (i == x) && (k == y))
                            continue;

                        index2 = y * texture.width + x;


                        if (height <= noiseMap[index2].r)
                        {
                            peakH = false;

                        }

                        if (height >= noiseMap[index2].r)
                        {
                            peakL = false;

                        }

                        if (!peakH && !peakL)
                        {

                            break;
                        }


                    }

                    if (!peakH && !peakL)
                    {

                        break;
                    }

                }



                if (peakH == true)
                {
                    peakMap[index] = Color.white;
                }
                else if (peakL == true)
                {

                    peakMap[index] = Color.white;


                }
                else
                {
                    peakMap[index] = Color.black;
                }


            }
        }

        Texture2D peakTex = new Texture2D(texture.width, texture.height);
        peakTex.SetPixels(peakMap);
        peakTex.filterMode = FilterMode.Point;
        peakTex.Apply();

        return peakTex;

    }

    //Generates crosses on white dots
    public static Texture2D GetCrossMap(Texture2D peakMap, float scale)
    {
        Color[] peaks = peakMap.GetPixels();
        Color[] crosses = new Color[peaks.Length];
        for (int k = 0; k < peakMap.height; k++)
        {

            for (int i = 0; i < peakMap.width; i++)
            {
                int index = k * peakMap.width + i;



                if (peaks[index] == Color.white)
                {

                    float perlin = Mathf.PerlinNoise(i * scale, k * scale);

                    if (perlin >= 0.5)
                    {
                        for (int x = 0; x < peakMap.width; x++)
                        {
                            crosses[index - i + x] = Color.white;
                        }
                    }
                    else
                    {

                        for (int y = 0; y < peakMap.height; y++)
                        {
                            crosses[y * peakMap.width + i] = Color.white;
                        }
                    }

                }

            }

        }

        Texture2D tex = new Texture2D(peakMap.width, peakMap.height);
        tex.SetPixels(crosses);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        return tex;

    }

    //Generates a staircase pyramid like map from a peak map
    public static Texture2D GetPyramidMap(Texture2D peakMap)
    {

        List<Vector2Int> peaks = new List<Vector2Int>();
        Color[] pPixels = peakMap.GetPixels();
        Color[] tPixels = new Color[pPixels.Length];


        for (int k = 0; k < peakMap.height; k++)
        {

            for (int i = 0; i < peakMap.width; i++)
            {
                int index = k * peakMap.width + i;

                if (pPixels[index] == Color.black) peaks.Add(new Vector2Int(i, k));


            }

        }



        for (int k = 0; k < peakMap.height; k++)
        {

            for (int i = 0; i < peakMap.width; i++)
            {
                int index = k * peakMap.width + i;

                tPixels[index] = GetPValue(i, k, peaks);
            }
        }


        Texture2D texture = new Texture2D(peakMap.width, peakMap.height);
        texture.SetPixels(tPixels);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        return texture;

    }

    //Used to get a value with the given coordinates for the pyramid map
    private static Color GetPValue(int x, int y, List<Vector2Int> peaks)
    {
        Vector2Int closestPeak = Vector2Int.zero;
        float min = float.MaxValue;

        //Getting the closest peak point
        for (int i = 0; i < peaks.Count; i++)
        {
            float sqrDistance = Mathf.Pow(x - peaks[i].x, 2) + Mathf.Pow(y - peaks[i].y, 2);

            if(sqrDistance < min)
            {
                min = sqrDistance;
                closestPeak = peaks[i];
            }

        }

        Vector2Int placement = (new Vector2Int(x, y) - closestPeak);
        float multiplier = 0;

        //determine where it is on the pyramid
        if(Mathf.Abs(placement.x) > Mathf.Abs(placement.y))
        {
            multiplier = Mathf.Abs(placement.x);
        }
        else if(placement.x < Mathf.Abs(placement.y))
        {
            multiplier = Mathf.Abs(placement.y);

        }else 
        { 
        
            multiplier = Mathf.Abs(placement.x);
        }

        if(multiplier != 0)
        {
            return Color.white * multiplier / 10f;
        }

        return Color.black;        

    }

    private static Color GetSValue(int x, int y, List<Vector2Int> basePixels, Texture2D map, float multiplier)
    {
        float smallestDistance = float.MaxValue;
        foreach(Vector2Int pos in basePixels)
        {
            float sqrDistance = (new Vector2(pos.x - x, pos.y - y)).sqrMagnitude;

            if(sqrDistance < smallestDistance)
            {
                smallestDistance = sqrDistance;
            }
        }

        float value = 1 - (1F / (smallestDistance * map.GetPixel(x, y).r / multiplier));

        return Color.white * Mathf.Max(0, value);
    }

}
