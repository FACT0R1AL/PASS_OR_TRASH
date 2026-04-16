using UnityEngine;
using System.Collections;

public class ConveyorMove : MonoBehaviour
{
    public Vector3 moveDir;
    public float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.transform.parent.CompareTag("Conveyor"))
        {
            collision.transform.parent = gameObject.transform;
            collision.transform.position += moveDir * moveSpeed * Time.deltaTime;
            Debug.Log(collision.gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        collision.transform.parent = null;
    }
}
