using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinkslime : Enemy // inherates everything from enemy script including mono behavior
{
    private Animator anim;
    public Transform target;
    public float chaseRadius;
    public float attackRadius;
    public Transform homePosition;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        
    }

    // Update is called once per frame
    void Update()
    
    {
        CheckDistance();
    }
    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) <= chaseRadius && Vector3.Distance(target.position, transform.position) > attackRadius)// this check the distance between enemy and player to dictate what the enemy will do.
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
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
