using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoadType
{
    /// <summary>
    /// The end point of the road.
    /// </summary>
    End,
    /// <summary>
    /// The start point of the road.
    /// </summary>
    Start,
    /// <summary>
    /// The point between the end and the start point of the road.
    /// </summary>
    Path
}

public class Road
{

    public int X { get { return Position.x; } } 
    public int Y { get { return Position.y; } }
    public readonly Vector2Int Position;

    /// <summary>
    ///<para>true - Starting point</para>
    ///<para>null - Connecting point</para>
    ///<para>false - End point</para>
    /// </summary>
    public RoadType type = RoadType.Path;
    public List<Road> Next = new List<Road>();

    /// <summary>
    ///<para>true - Starting point</para>
    ///<para>null - Connecting point</para>
    ///<para>false - End point</para>
    /// </summary>
    public Road(Vector2Int Position, RoadType type)
    {
        this.Position = Position;
        this.type = type;
    }

    public void AddNextRoadPoint(Road road)
    {
        Next.Add(road);
    }

}
