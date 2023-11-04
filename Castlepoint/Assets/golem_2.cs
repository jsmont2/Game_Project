using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
public class golem_2 : Enemy // inherits everything from enemy script including mono behavior
                             //side note: I had to get the flashred function from Enemy so that's why I inherited from Enemy this time
{
    private bool golemIsAtt;
    [SerializeField] private GameObject laserAttObject;
    [SerializeField] private List<GameObject> laserAttProjectiles;
    private float laserAttTimer;
    // Start is called before the first frame update
    void Start()
    {
        laserAttTimer = 0f;
        laserAttObject.SetActive(false);
        foreach (var item in laserAttProjectiles)
        {
            item.SetActive(false);
        }
        golemIsAtt = false;
        charType = characterType.boss;
        currentState = characterState.idle;
        myRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;    //allows the enemy to find the player and chase him
        
    }
    private void Update()
    {
        laserAttTimer += Time.deltaTime * .1f;
        if(laserAttTimer >= .5f)
        {
            laserAttTimer = 0f;
            FireLaserAtt();
        }
        if(golemIsAtt == true)//aims golems eye laser at the player, will follow player while firing
        {
        Vector3 targetDir = target.position - laserAttObject.transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        laserAttObject.transform.rotation = Quaternion.Slerp(laserAttObject.transform.rotation, q, Time.deltaTime * 2.5f);
        }
        if (currentState == characterState.invulnerable)
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("hit", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("invulnerable", true);
        }
        if (currentState == characterState.stagger)
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("invulnerable", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("hit", true);
        }
        if(currentState == characterState.idle)
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("invulnerable", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("hit", false);
        }
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CheckDistance();
    }
    private IEnumerator Invulnerable()
    {
        ChangeState(characterState.invulnerable);
        yield return new WaitForSeconds(10f);
        ChangeState(characterState.idle);
    }
    private void FireLaserAtt()
    {
        StartCoroutine(LaserAtt());
    }
    private IEnumerator LaserAtt()
    {        
        ChangeState(characterState.attack);        
        anim.SetBool("laserAtt", true);
        golemIsAtt = true;
        yield return new WaitForSeconds(.5f);        
        laserAttObject.SetActive(true);        
        //float angle = Vector3.Angle(targetDir, laserAttObject.transform.forward);
        //laserAttObject.transform.RotateAround(laserAttObject.transform.position, Vector3.back, angle);
        yield return new WaitForSeconds(.6f);
        foreach (var item in laserAttProjectiles)
        {
            item.SetActive(true);
        }
        yield return new WaitForSeconds(1f);
        foreach (var item in laserAttProjectiles)
        {
            item.SetActive(false);
        }
        golemIsAtt = false;
        laserAttObject.SetActive(false);
        
        anim.SetBool("laserAtt", false);
        ChangeState(characterState.idle);
        laserAttTimer = 0f;
    }    
    
}
