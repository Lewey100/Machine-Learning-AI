using UnityEngine;
using System.Collections;

public class hit : MonoBehaviour
{
 
    public Vector3 init_pos;
    public Quaternion init_rotation;
    public bool crash;

    // Use this for initialization
    void Start()
    {     
        crash = false;
        init_pos = transform.position;
        init_rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other) //If car hits a wall, it has failed.
    { 
       crash = true;
    }
}
