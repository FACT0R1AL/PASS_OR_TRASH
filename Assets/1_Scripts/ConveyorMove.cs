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
        collision.transform.position += moveDir * moveSpeed * Time.deltaTime;
        Debug.Log(collision.gameObject.name);
    }
}
