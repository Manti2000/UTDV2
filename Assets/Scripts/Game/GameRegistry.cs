using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRegistry : MonoBehaviour
{
    public static GameRegistry Instance;

    [SerializeField]
    private Block[] blocks;
    private Dictionary<string, int> nameIdDictionary = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
        nameIdDictionary.Clear();
        for(int i = 0; i < blocks.Length; i++)
        {
            if (!nameIdDictionary.ContainsKey(blocks[i].uniqueName))
            {
                nameIdDictionary.Add(blocks[i].uniqueName, i);
            }
            else
            {
                Debug.Log($"Block with unique name (\"{blocks[i].uniqueName}\") already exists!");
            }
        }
    }

    /// <summary>
    /// Returns the block ID for the given block name.
    /// <para>If the block with the given name doesn't exist, it returns -1.</para>
    /// </summary>
    /// <param name="uniquename"></param>
    /// <returns></returns>
    public int GetBlockIDByName(string uniquename)
    {
        if (nameIdDictionary.ContainsKey(uniquename))
        {
            return nameIdDictionary[uniquename];
        }
        else
        {
            return -1;
        }
    }

    public Block[] GetBlocks()
    {
        return blocks;
    }
    
}
