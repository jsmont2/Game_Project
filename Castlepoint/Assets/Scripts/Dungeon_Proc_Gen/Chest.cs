using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]private List<GameObject> items;
    private Animator anim;
    private bool hasKey;
    private int itemCount;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetBool("Locked", true);
    }
    public void OpenChest()
    {
        anim.SetBool("Locked", false);
    }
}
