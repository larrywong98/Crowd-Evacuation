using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public static class Map
{
    public static GameObject person;
    public static Tilemap tilemap;
    public static List<Vector2> exits = new List<Vector2>();
    public static int width;
    public static int height;
    public static Node[,] grid;
    public static void map()
    {
        BoundsInt bounds = tilemap.cellBounds;
        height = bounds.size.y;
        width = bounds.size.x;
        grid = new Node[width, height];
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileBase tile = allTiles[x + y * width];
                grid[x, y] = new Node(null, new Vector2(x + 0.5f, y + 0.5f));
                if (tile != null)
                {
                    grid[x, y].SetWall(1);
                }
                else
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    {
                        exits.Add(new Vector2(x, y));
                    }
                }
            }
        }
    }
}
public class Node
{
    private Vector2 position;
    private Node parent;
    private float g;
    private float h;
    private float f;
    private int isWall;

    public Node(Node p, Vector2 pos)
    {
        this.position = pos;
        this.parent = p;
        this.g = float.PositiveInfinity;
        this.h = float.PositiveInfinity;
        this.f = float.PositiveInfinity;
        this.isWall = 0;

    }
    public void SetWall(int t)
    {
        this.isWall = t;
    }
    public int GetWall()
    {
        return this.isWall;
    }
    public float Getf()
    {
        return this.f;
    }
    public void Calculatef(float scale)
    {
        this.f = this.g* scale + this.h;
    }
    public float Getg()
    {
        return this.g;
    }
    public void Setg(float g)
    {
        this.g = g;
    }
    public float Geth()
    {
        return this.h;
    }
    public void Seth(float h)
    {
        this.h = h;
    }
    public Node GetParent()
    {
        return this.parent;
    }
    public void SetParent(Node p)
    {
        this.parent = p;
    }
    public Vector2 GetPosition()
    {
        return this.position;
    }
}
