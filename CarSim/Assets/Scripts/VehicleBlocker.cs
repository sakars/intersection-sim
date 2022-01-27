using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBlocker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.transform.CompareTag("Car") && collision.transform != transform.parent) transform.parent.GetComponent<Vehicle>().blockedAmount++;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.transform.CompareTag("Car") && collision.transform != transform.parent) transform.parent.GetComponent<Vehicle>().blockedAmount--;
    }
}
