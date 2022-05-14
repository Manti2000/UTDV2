using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTranslator
{
   public static int[,] GetTranslation(Texture2D heightMap, (List<Road>, float) roadData)
   {
        int width = heightMap.width, height = heightMap.height;
        int[,] translation = new int[width, height];

        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string uniqueName = Translate(x, y, heightMap.GetPixel(x, y).r, roadData);

                int blockID = GameRegistry.Instance.GetBlockIDByName(uniqueName);

                translation[x, y] = blockID == -1 ? 0 : blockID;
            }
        }

        return translation;
   }

    /// <summary>
    /// Returns the name of the block.
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    private static string Translate(int x, int y, float height, (List<Road>, float) roadData)
    {
        if (roadData.Item1 != null)
        {
            float val = GetSlopeValue(x, y, roadData.Item1, roadData.Item2);
            if (val == 0)
                return "road";
        }

        if (height <= 0.1F)
        {
            return "sand";
        }
        else
        {
            return "dirt";
        }
    }

    private static string TranslateRoad()
    {
        return "road";
    }

    private static float GetSlopeValue(int x, int y, List<Road> roadPoints, float multiplier)
    {
        float smallestDistance = float.MaxValue;
        foreach (Road road in roadPoints)
        {
            float sqrDistance = (new Vector2(road.X - x, road.Y - y)).sqrMagnitude;

            if (sqrDistance < smallestDistance)
            {
                smallestDistance = sqrDistance;
            }
        }

        float value = 1 - (1F / (smallestDistance / multiplier));

        return Mathf.Max(0, value);
    }
}
