using UnityEngine;

public class ItemMove : MonoBehaviour
{
    void Update()
    {
        Collider[] conveyors = new Collider[5];
        int count = Physics.OverlapBoxNonAlloc(transform.position, transform.localScale, conveyors, Quaternion.identity, LayerMask.GetMask("Conveyor"));

        if (count > 0)
        {
            GameObject conveyor = conveyors[0].gameObject;
            transform.position += conveyor.GetComponent<Conveyor>().moveSpeed * conveyor.GetComponent<Conveyor>().moveDir * Time.deltaTime;
        }
    }
}
