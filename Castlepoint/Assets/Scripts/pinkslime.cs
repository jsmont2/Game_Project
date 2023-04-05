using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinkslime : MonoBehaviour
{
    private Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        anim.SetBool("hit", true);
        StartCoroutine(pinkslimeCo());
    }

    IEnumerator pinkslimeCo()
    {
        yield return new WaitForSeconds(.3f);
        this.gameObject.SetActive(false);
    }
}
