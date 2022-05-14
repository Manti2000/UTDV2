using System;
using UnityEngine;

[System.Serializable]
public class Block
{
    public string uniqueName;
    public BlockSettings settings;
}

[System.Serializable]
public struct BlockSettings
{
    /// <summary>
    /// Side textures of a block.
    /// <para>0 - NORTH</para>
    /// <para>1 - WEST</para>
    /// <para>2 - EAST</para>
    /// <para>3 - SOUTH</para>
    /// </summary>
    public SideTexture[] sides;
}

[System.Serializable]
public struct SideTexture
{
    public SideFace side;
    public Sprite sprite;
}

