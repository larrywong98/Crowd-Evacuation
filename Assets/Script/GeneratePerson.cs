using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GeneratePerson : MonoBehaviour
{
    public GameObject person;
    private Node[,] grid;
    [SerializeField] private Rigidbody2D rb;
    public Tilemap tilemap;
    private float speed;
    private List<Vector2> exits = new List<Vector2>();
    private int height;
    private int width;
    private Vector2 dest;
    void Start()
    {
        //map=new Map();
        //this.grid = map.grid;
        BoundsInt bounds = tilemap.cellBounds;
        this.height = bounds.size.y;
        this.width = bounds.size.x;
        this.grid = new Node[this.width, this.height]; 
        //map =new Map();
        //this.grid = map.grid;
        //this.exits = map.exits;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int y = 0; y < this.height; y++)
        {
            for (int x = 0; x < this.width; x++)
            {
                //Vector3Int coordinate = new Vector3Int(x, y, (int)this.tilemap.transform.position.y);
                //Vector3 tilePosition = this.tilemap.CellToWorld(coordinate);
                //Debug.Log(tilePosition);
                TileBase tile = allTiles[x + y * this.width];
                this.grid[x, y] = new Node(null, new Vector2(x + 0.5f, y + 0.9f));
                if (tile != null)
                {
                    this.grid[x, y].SetWall(1);
                }
                else
                {
                    if (x == 0 || y == 0 || x == this.width - 1 || y == this.height - 1)
                    {
                        exits.Add(new Vector2(x, y));
                    }
                }
            }

        }
        //foreach(var e in exits)
        //{
        //    Debug.Log(e);
        //}


        float diameter;
        float new_x; 
        float new_y; 
        int count = 250;
        while (count>0){
            diameter = Random.Range(0.5f, 0.72f);
            //diameter = 0.5f;
            new_x = Random.Range(0.5f, 27.5f);
            new_y = Random.Range(0.5f, 33.5f);
            if (grid[(int)new_x, (int)new_y].GetWall() == 1) { continue; }
            GameObject new_obj = Instantiate(person, new Vector3(new_x, new_y, 0), Quaternion.identity);
            new_obj.transform.localScale = new Vector3(diameter, diameter, 1);
            count--;
        }
    }
          
    // Update is called once per frame
    void Update()
    {
        
    }
}
