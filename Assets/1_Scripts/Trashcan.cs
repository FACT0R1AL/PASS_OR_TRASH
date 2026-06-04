using UnityEngine;

public class Trashcan : MonoBehaviour
{
    public delegate void EndingTrash();
    public static event EndingTrash OnEndTrash;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            
            Destroy(other.gameObject);
            
            if (!itemData.isTrashProduct)
            {
                OnEndTrash?.Invoke();
                Debug.Log("THERE IS PRODUCT AHHHHHHHHH!!!!");
            }
        }
    }
}
