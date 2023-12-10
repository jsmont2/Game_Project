using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindDATA : MonoBehaviour
{
    public GameObject theData;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(FindDataPersistence);
    }

    // Update is called once per frame
   void FindDataPersistence()
    {
        theData = GameObject.Find("DataPersistenceManager");
        theData.GetComponent<DataPersistenceManager>().SaveGame();
    }
}
