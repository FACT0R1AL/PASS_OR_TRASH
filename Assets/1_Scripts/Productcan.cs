using UnityEngine;

public class Productcan : MonoBehaviour
{
    public delegate void EndingProduct();
    public static event EndingProduct OnEndProduct;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            ItemData itemData = other.GetComponent<ItemData>();
            
            Destroy(other.gameObject);
            
            if (itemData.isTrashProduct)
            {
                OnEndProduct?.Invoke();
                Debug.Log("THERE IS TRASH AHHHHHHHHH!!!!");
            }
        }
    }
}
