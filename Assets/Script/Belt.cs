using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Belt : MonoBehaviour
{
    public List<GameObject> onBelt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    private void OnTriggerEnter(Collider collision)
    {
        onBelt.Add(collision.gameObject);
        Debug.Log(collision.gameObject.GetComponent<Controller>().on_belt);
        collision.gameObject.GetComponent<Controller>().on_belt = 1;
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    onBelt.Add(collision.gameObject);
    //    collision.gameObject.GetComponent<Controller>().on_belt = 1;
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    onBelt.Remove(collision.gameObject);
    //}
}
