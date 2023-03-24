using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Controller : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    public Tilemap tilemap;
    private float speed;
    private List<Vector2> exits = new List<Vector2>();
    private int height;
    private int width;
    private int flag;
    private Node[,] grid;
    private Vector2 dest;
    private int collide;
    public int on_belt;
    void Start()
    {
        this.flag=0;
        this.collide = 0;
        this.speed = 100.0f;
        //UnityEngine.Random.Range(50.0f, 100f);
        BoundsInt bounds = tilemap.cellBounds;
        this.height = bounds.size.y;
        this.width = bounds.size.x;
        this.grid = new Node[this.width, this.height]; ;
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
                this.grid[x, y] = new Node(null, new Vector2(x + 0.5f, y + 0.5f));
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

    }
    Vector2 Find_possible_exit(Vector2 cur)
    {
        float min_dis = float.PositiveInfinity;
        Vector2 exit=new Vector2();
        foreach(var e in exits)
        {
            float dis=Vector2.Distance(e, cur);
            if (dis < min_dis)
            {
                min_dis = dis;
                exit = e;
            }
        }
        return exit + new Vector2(0.5f, 0.5f);
    }

    List<Vector2> GetPath(Node current_node)
    {
        Node current = current_node;
        List<Vector2> path = new List<Vector2>();
        while (current != null)
        {
            path.Add(current.GetPosition());
            current = current.GetParent();
        }
        path.Reverse();
        return path;
    }

    List<Vector2> astar(Vector2 destination)
    {
        //Node end_node = this.grid[(int)destination.x, (int)destination.y];
        List<Node> open_list = new List<Node>();
        List<Node> close_list = new List<Node>();
        int curx = (int)transform.position.x;
        int cury = (int)transform.position.y;
        int destx = (int)destination.x;
        int desty = (int)destination.y;
        //Node start_node = this.grid[curx, cury];
        this.grid[curx, cury].Setg(0);
        this.grid[curx, cury].Seth(Math.Abs(this.grid[curx, cury].GetPosition().x - destination.x) + 
                                   Math.Abs(this.grid[curx, cury].GetPosition().y - destination.y));
        this.grid[curx, cury].Calculatef(1f);
        open_list.Add(this.grid[curx, cury]);


        int tmp = 0;
        while (open_list.Count > 0 && tmp < 1000)
        {
            tmp++;
            //Get current node
            Node current_node = open_list[0];
            for (int i = 1; i < open_list.Count; i++)
            {
                if (open_list[i].Getf() < current_node.Getf())
                {
                    current_node = open_list[i];
                }
            }

            // found
            if (current_node.GetPosition() == this.grid[destx, desty].GetPosition())
            {
                return GetPath(current_node);
            }
            open_list.Remove(current_node);
            close_list.Add(current_node);

            // Children
            List<Vector2> neighbours = new List<Vector2>() {
                new Vector2(0, -1),
                new Vector2(0, 1),
                new Vector2(-1, 0),
                new Vector2(1, 0),
                new Vector2(-1, -1),
                new Vector2(-1, 1),
                new Vector2(1, -1),
                new Vector2(1, 1)
            };
            List<Node> children = new List<Node>();
            foreach (var neighbor in neighbours)
            {
                Vector2 neighbor_pos = current_node.GetPosition() + neighbor;
                int nbx = (int)neighbor_pos.x;
                int nby = (int)neighbor_pos.y;
                if (neighbor_pos.x >= this.grid.GetLength(0) || neighbor_pos.x < 0 || neighbor_pos.y >= this.grid.GetLength(1) || neighbor_pos.y < 0 ||
                    this.grid[nbx, nby].GetWall() == 1) 
                    continue;
                //Node new_node = this.grid[(int)neighbor_pos.x, (int)neighbor_pos.y];
                //new_node.SetParent(current_node);
                children.Add(this.grid[nbx, nby]);
                //children.Add(new Node(current_node, neighbor_pos));
            }
            foreach (var child in children)
            {
                if (close_list.Contains(child)) continue;
                float g = current_node.Getg() + 1;
                if (g < child.Getg())
                {
                    //Debug.DrawLine(new Vector3(child.GetPosition().x, child.GetPosition().y, 0), new Vector3(current_node.GetPosition().x, current_node.GetPosition().y, 0), Color.green, 1000f);
                    float h = Math.Abs(child.GetPosition().x - this.grid[destx,desty].GetPosition().x) + 
                              Math.Abs(child.GetPosition().y - this.grid[destx, desty].GetPosition().y);
                    child.SetParent(current_node);
                    child.Setg(g);
                    child.Seth(h);
                    if ((child.GetPosition().x == current_node.GetPosition().x - 1 && child.GetPosition().y == current_node.GetPosition().y - 1) ||
                        (child.GetPosition().x == current_node.GetPosition().x + 1 && child.GetPosition().y == current_node.GetPosition().y + 1) ||
                        (child.GetPosition().x == current_node.GetPosition().x - 1 && child.GetPosition().y == current_node.GetPosition().y + 1) ||
                        (child.GetPosition().x == current_node.GetPosition().x + 1 && child.GetPosition().y == current_node.GetPosition().y - 1))
                    {
                        child.Calculatef(1.4f);
                    }
                    else
                    {
                        child.Calculatef(1f);
                    }
                    if (!open_list.Contains(child))
                    {
                        open_list.Add(child);
                    }
                }
                
            }
        }
        return null;
    }

    IEnumerator Move(List<Vector2> astar_path)
    {
        yield return new WaitForSeconds(1f);
        foreach (var p in astar_path)
        {
            while ((transform.position - new Vector3(p.x, p.y, 0)).sqrMagnitude > 0.5f)
            {
                if (this.on_belt == 1)
                {
                    break;
                }
                //rb.velocity = VelocityMoveTowards(transform.position, p, speed * Time.deltaTime);
                if (this.collide == 0) { 
                    transform.position = Vector2.MoveTowards(transform.position, p, speed * Time.deltaTime);
                }
                yield return new WaitForSeconds(0.1f);
                
            }
        }
        if (this.on_belt == 1)
        {
            while ((transform.position - new Vector3(this.dest.x, this.dest.y, 0)).sqrMagnitude > 1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(this.dest.x, this.dest.y, 0), speed * Time.deltaTime);
                //rb.velocity = VelocityMoveTowards(transform.position, new Vector3(this.dest.x, this.dest.y, 0), speed * Time.deltaTime);
            }
        }
        Destroy(gameObject);
    }
    //Vector2 VelocityMoveTowards(Vector2 current, Vector2 target, float maxDistanceDelta)
    //{
        
    //    float angle = Mathf.Atan((target.y - current.y) / (target.x - current.x));
    //    Vector2 returnValue;
    //    returnValue.x = Mathf.Cos(angle) * maxDistanceDelta;
    //    returnValue.y = Mathf.Sin(angle) * maxDistanceDelta;
    //    if (current.x + returnValue.x > target.x || current.y + returnValue.y > target.y)
    //    {
    //        returnValue.x = target.x - current.x;
    //        returnValue.y = target.y - current.y;
    //    }
    //    if (this.on_belt == 1) { return Vector2.zero; }
    //    return returnValue;
    //}
    IEnumerator StopMove()
    {
        this.collide = 1;
        float distance=Math.Abs(this.dest.x - transform.position.x)+Math.Abs(this.dest.y-transform.position.y);
        yield return new WaitForSeconds(distance / 20.0f);//UnityEngine.Random.Range(0,distance/15.0f));
        this.collide = 0;
    }

    void Update()
    {
        if (this.flag == 0)
        {
            this.dest = Find_possible_exit(transform.position);
            List<Vector2> astar_path = new List<Vector2>();
            astar_path = astar(this.dest);
            if (astar_path != null)
            {
                StartCoroutine(Move(astar_path));


            }
            this.flag = 1;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(StopMove());
            //Debug.Log("contact");
        }
    }
}


//this.horizontal = 0.0f;
//this.vertical = 0.0f;
//if ((transform.position - new Vector3(this.dest.x, this.dest.y, 0)).sqrMagnitude <= 0.1f)
//{
//    Destroy(gameObject);
//}
//timer+= Time.deltaTime;
//if (timer > 5.0f)
//{
//    break;
//}
//float offset_x = UnityEngine.Random.Range(0, -other.transform.position.x + transform.position.x);
//float offset_y = UnityEngine.Random.Range(0,- other.transform.position.y + transform.position.y);
//this.next_node = this.next_node + new Vector2(offset_x,offset_y);
//Vector2 prev=new Vector2();
//foreach (var p in astar_path)
//{
//    //rb.velocity = new Vector2(p.x-prev.x, p.y-prev.y).normalized * speed;
//    //transform.position = new Vector3(p.x, p.y, 0);
//    var step = speed * Time.deltaTime;
//    transform.position = Vector3.MoveTowards(transform.position, new Vector3(p.x,p.y,0), step);
//    //prev = p;
//    yield return new WaitForSeconds(0.5f);
//}
//Vector3 vec = new Vector3(this.horizontal, this.vertical, 0);
//vec = vec.normalized;
//rb.velocity = new Vector2(vec.x, vec.y) * speed;
//if (Input.GetKey(KeyCode.W))
//{
//    this.vertical = 1.0f;
//}
//if (Input.GetKey(KeyCode.A))
//{
//    this.horizontal = -1.0f;
//}
//if (Input.GetKey(KeyCode.S))
//{
//    this.vertical = -1.0f;
//}
//if (Input.GetKey(KeyCode.D))
//{
//    this.horizontal = 1.0f;
//}

//StartCoroutine(ExecuteAfterTime(5));
//rb.AddForce(vec * speed);
//cameraTransform.position = new Vector3(transform.position.x, transform.position.y, -10);

//horizontal = Input.GetAxis("Horizontal");
//vertical = Input.GetAxis("Vertical");
//Vector2 prev = new Vector2(0, 0);
//if (prev != Vector2.zero)
//    Debug.DrawLine(new Vector3(p.x, p.y, 0), new Vector3(prev.x, prev.y, 0), Color.green, 1000f);
//prev = p;
//Debug.Log(p);