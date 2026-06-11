using System.Collections;
using UnityEngine;

[System.Serializable]
public class Products
{
    public GameObject product;
    
    public GameObject[] easyTrashes;
    public GameObject[] mediumTrashes;
    public GameObject[] hardTrashes;
}

public class SpawnItem : MonoBehaviour
{
    public Products[] products;
    
    public float startRandom;
    public float endRandom;

    void Start()
    {
        StartCoroutine(Spawn());
    }
    
    IEnumerator Spawn()
    {
        while (true)
        {
            int randomProduct = Random.Range(0, products.Length);

            int qualityLevel = Random.Range(0, 4);

            if (qualityLevel == 0)
            {
                GameObject product = Instantiate(products[randomProduct].product, 
                    transform.position, 
                    products[randomProduct].product.transform.rotation);
            }
            else
            {
                // easy : 50%
                // medium : 30%
                // hard : 20%
            }
        
            yield return new WaitForSeconds(Random.Range(startRandom, endRandom));
        }
    }
}
