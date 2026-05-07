using System.Collections;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public GameObject[] items;
    public ItemData[] itemData;

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
            int randomItem = Random.Range(0, items.Length);
        
            GameObject item = Instantiate(items[randomItem], transform.position, items[randomItem].transform.rotation);
        
            yield return new WaitForSeconds(Random.Range(startRandom, endRandom));
        }
    }
}
