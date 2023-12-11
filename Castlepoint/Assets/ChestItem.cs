using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestItem : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private List<GameObject> itemsInChest;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public void DestroyItem()
    {
        Destroy(item);
    }
    public void ChooseItem(int index)
    {
        item = itemsInChest[index];
        item.SetActive(true);
    }
}
