using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoadFinder : MonoBehaviour
{
    public static Texture2D PathTexture;
    
    /// <summary>
    /// Generates a road on to the terrain with A*, and returns the starting road point.
    /// </summary>
    /// <param name="StartPoint"></param>
    /// <param name="EndPoint"></param>
    /// <param name="map"></param>
    /// <param name="pathTexture"></param>
    /// <returns></returns>
    public static Road SearchRoad(Vector2Int StartPoint, Vector2Int EndPoint, Texture2D map, bool pathTexture = false)
    {
        if(pathTexture)
            PathTexture = new Texture2D(map.width, map.height);

        int index = 0;
        Color[] mapPixels = map.GetPixels();

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        Node startNode = new Node(StartPoint.x, StartPoint.y);
        Node targetNode = new Node(EndPoint.x, EndPoint.y);

        open.Add(startNode);

       // Debug.Log("CONTAINS: " + open.Contains(startNode));


        int c = 0;
        while (open.Count > 0 && c <= 10000)
        {
            //Debug.Log(c);
            //c++;

            Node currentNode = open[0];


            //Checking if from the open list anybody has a lower fcost then the current one
            foreach (Node node in open)
            {

                //Debug.Log("neigbour:   " + node.fCost + "    current: " + currentNode.fCost);

                if (node.fCost < currentNode.fCost || (node.fCost == currentNode.fCost && node.hCost < currentNode.hCost))
                {

                    currentNode = node;

                }

            }

            open.Remove(currentNode);
            closed.Add(currentNode);

            //Retrace path if it has reached the destination - found the road
            if (currentNode.Equals(targetNode))
            {
                targetNode.parent = currentNode;

                return RetracePath(startNode, currentNode);
                
               
            }


            //Get neigbours and assign the g and h cost to them if needed
            Node[] neighbours = GetNeighbourNodes(currentNode, new Vector2Int(map.width, map.height), true);

            foreach (Node node in neighbours) //loop through the neigbours
            {
                if (closed.Contains(node)) continue;

                index = currentNode.y * PathTexture.width + currentNode.x;

                float mapValue = map.GetPixel(node.x, node.y).r;
                //Calculate the g cost from the current to one of its neigbours
                float movementCost = currentNode.gCost + GetDistance(currentNode, node, mapValue);

                if(movementCost > node.gCost || !open.Contains(node))
                {
                    index = node.y * PathTexture.width + node.x;

                    //mapValue = map.GetPixel(targetNode.x, targetNode.y).r;

                    //Set the costs of the node
                    node.gCost = movementCost;
                    node.hCost = GetDistance(node, targetNode, mapValue);
                    node.parent = currentNode;

                    //If the open list does not contain the node then add it
                    if (!open.Contains(node))
                    {
                        open.Add(node);
                    }

                }

            }

            //if(pathTexture)
                //CreateTexture(startNode, targetNode, open, closed, currentNode, map);
            //yield return new WaitForSeconds(0.001F);

        } //End of while loop
        return null;

    }

    /// <summary>
    ///Given the starting and ending node it returns a road path.
    /// </summary>
    static Road RetracePath(Node startNode, Node endNode)
    {

        Road currentRoad = new Road(new Vector2Int(endNode.x, endNode.y), RoadType.End);
        Node currentNode = endNode;
        SetRoadPixel(currentRoad);

        //loop while the we have not arrived to the start location
        while (!currentNode.Equals(startNode))
        {
            currentNode = currentNode.parent;
            Road road = new Road(new Vector2Int(currentNode.x, currentNode.y), RoadType.Path);
            road.AddNextRoadPoint(currentRoad);
            currentRoad = road;
            SetRoadPixel(currentRoad);
        }
        currentRoad.type = RoadType.Start;


        return currentRoad;

    }

    /// <summary>
    /// <param>Returns a number based on the distance between two nodes. Used for heuristic value.</param>
    /// </summary>
    static float GetDistance(Node nodeA, Node nodeB, float mapValue = 0)
    {
        Vector2 distanceVector = new Vector2(Mathf.Abs(nodeB.x - nodeA.x), Mathf.Abs(nodeB.y - nodeA.y));

        if (distanceVector.x > distanceVector.y)
            return (14 * distanceVector.y + 10 * (distanceVector.x - distanceVector.y)) * mapValue;

        return (14 * distanceVector.x + 10 * (distanceVector.y - distanceVector.x)) * mapValue;

    }

    /// <summary>
    /// Gets the neigbouring nodes of the given node.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="mapSize"></param>
    /// <param name="corners"></param>
    /// <returns></returns>
    static Node[] GetNeighbourNodes(Node node, Vector2Int mapSize, bool corners = true)
    {
        List<Node> sides = new List<Node>();

        for (int y = node.y - 1; y < node.y + 2; y++)
        {

            for (int x = node.x - 1; x < node.x + 2; x++)
            {

                if (!(x < mapSize.x && x >= 0 && y < mapSize.y && y >= 0) || ((node.x == x) && (node.y == y)) || (!corners && (x != node.x && y != node.y)))
                    continue;

                
                sides.Add(new Node(x, y));
            }
        }

        return sides.ToArray();

    }

    /// <summary>
    /// Used for creating a texture for the pathfinding algorithm, if needed.
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <param name="open"></param>
    /// <param name="closed"></param>
    /// <param name="currentNode"></param>
    /// <param name="Map"></param>
    static void CreateTexture(Node startNode, Node targetNode, List<Node> open, List<Node> closed, Node currentNode, Texture2D Map)
    {
        
        Color[] colors = Map.GetPixels();
        int index = 0;

        foreach (Node node in open)
        {
            index = node.y * PathTexture.width + node.x;
            colors[index] = Color.Lerp(Color.green, colors[index], 0.9F);
        }

        foreach (Node node in closed)
        {
            index = node.y * PathTexture.width + node.x;
            colors[index] = Color.Lerp(Color.red, colors[index], 0.9F);

        }

        Node curr = currentNode;
        while (!curr.Equals(startNode))
        {
            index = curr.y * PathTexture.width + curr.x;

            colors[index] = Color.Lerp(Color.white, colors[index], 0.3F);
            curr = curr.parent;

        }

        index = startNode.y * PathTexture.width + startNode.x;
        colors[index] = Color.blue;

        index = targetNode.y * PathTexture.width + targetNode.x;
        colors[index] = Color.cyan;

        index = currentNode.y * PathTexture.width + currentNode.x;
        colors[index] = Color.yellow;

        PathTexture.filterMode = FilterMode.Point;
        PathTexture.SetPixels(colors);
        PathTexture.Apply();

    }

    static void SetRoadPixel(Road road)
    {
        PathTexture.filterMode = FilterMode.Point;
        PathTexture.SetPixel(road.X, road.Y, new Color(0, 0, 0));
        PathTexture.Apply();
    }

    /// <summary>
    /// The Node class used by the pathfinder to find the right path
    /// </summary>
    class Node : IEquatable<Node>
    {
        public int x, y;
        public float gCost, hCost;
        public float fCost { get => gCost + hCost; }
        public Node parent;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Node node = obj as Node;
            if (node == null) return false;
            else return Equals(node);
        }

        public bool Equals(Node node)
        {
            return this.x == node.x && this.y == node.y;
        }
    }

}


