using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Base,
    Road,
    Plot

}


//The tile of the map
public class Tile : MonoBehaviour
{

    public TileType type;
    public Tower tower;

    [System.NonSerialized]
    public int tileX, tileY;

    [System.NonSerialized]
    public bool isVisible = true;


    //Set Tile data
    public Tile SetTile(int tileX, int tileY, TileType tileType = TileType.Base)
    {
        this.type = tileType;
        this.tileX = tileX;
        this.tileY = tileY;

        return this;
        
    }


    public void SetTileVisible(bool visible)
    {
      
            isVisible = visible;
            GetComponent<MeshRenderer>().enabled = visible;
            
        
    }

    
}
