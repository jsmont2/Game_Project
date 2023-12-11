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
    [SerializeField] private GameObject armAttProjectile;
    private bool armAttProjectileActive;
    private float laserAttTimer;
    private float armAttTimer;
    private float goInvulnerable;
    // Start is called before the first frame update
    void Start()
    {
        goInvulnerable = maxHealth;
        laserAttTimer = 0f;
        armAttTimer = 0f;
        laserAttObject.SetActive(false);
        armAttProjectileActive = false;
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
        CheckDmg();
        if (currentState != characterState.invulnerable)
        { FireLaserAtt(); FireArmAtt(); }
        AimAtPlayer(armAttProjectile, 1.3f);//continually aim at player
        AimAtPlayer(laserAttObject, 2f);//continually aim at player
        ControlAnimStates();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (golemIsAtt == false)//if the golem is not already attacking
        {
            CheckDistance();//see if player is within attacking distance
        }
    }
    private void ControlAnimStates()
    {
        if (currentState == characterState.invulnerable)//set all animator state machines
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("hit", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("invulnerable", true);
        }
        if (currentState == characterState.stagger)//set all animator state machines
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("invulnerable", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("hit", true);
        }
        if (currentState == characterState.idle || currentState == characterState.attack)//set all animator state machines
        {
            anim.SetBool("wakeUp", false);
            anim.SetBool("invulnerable", false);
            anim.SetBool("shockwave", false);
            anim.SetBool("hit", false);
        }
    }
    private void CheckDmg()
    {
        if (goInvulnerable - health >= 10)
        {
            goInvulnerable = health;
            StartCoroutine(Invulnerable());
        }
    }
    private IEnumerator Invulnerable()
    {
        ChangeState(characterState.invulnerable);//set state machine to invulnerable
        yield return new WaitForSeconds(20f);//Allow golem to to set and be invulnerable for set time
        ChangeState(characterState.idle);//reset state machine back to idle
    }
    private void FireArmAtt()
    {
        if(!anim.GetBool("armAtt"))//if the golem is not using the arm attack already
        { armAttTimer += Time.deltaTime * .1f; }//increment arm attack counter
        if(armAttTimer >= .5f && !anim.GetBool("armAtt"))//if the golem has charged up arm attack
        {
            armAttTimer = 0f;//reset the arm attack counter
            StartCoroutine(ArmAtt());//begin to fire the arm attack
        }
    }
    private IEnumerator ArmAtt()
    {
        ChangeState(characterState.attack);//change state machine to attack      
        anim.SetBool("armAtt", true);//set animator state machine to arm attack
        golemIsAtt = true;//set boolean for golem is attacking to true
        yield return new WaitForSeconds(.48f);
        armAttProjectileActive = true;
        GameObject temp = Instantiate(armAttProjectile, armAttProjectile.GetComponent<Transform>()) as GameObject;
        armAttProjectile.GetComponent<Transform>().DetachChildren();
        temp.SetActive(true);
        //temp.GetComponent<Arrow>().Setup(temp.transform.position, AimArmAtPlayer(temp, 5f));
        yield return new WaitForSeconds(3f);
        Destroy(temp);
        armAttProjectileActive = false;
        ChangeState(characterState.idle);
        armAttProjectile.SetActive(false);
        anim.SetBool("armAtt", false);
        golemIsAtt = false;
    }
    private void FireLaserAtt()
    {
        if (!anim.GetBool("laserAtt"))//if the golem is not using the laser attack already
        { laserAttTimer += Time.deltaTime * .1f; }//increment laser attack timer
        if (laserAttTimer >= 2f && !anim.GetBool("laserAtt"))//if the golem has charged up laser attack
        {
            laserAttTimer = 0f;//reset the laser beam attack counter
            StartCoroutine(LaserAtt());//begin to fire the laser
        }
    }
    private IEnumerator LaserAtt()
    {
        ChangeState(characterState.attack);//change state machine to attack      
        anim.SetBool("laserAtt", true);//set animator state machine to laser attack
        golemIsAtt = true;//set boolean for golem is attacking to true
        yield return new WaitForSeconds(.5f); // wait for half a second      
        laserAttObject.SetActive(true);//Enable the laser attack in scene to begin animation from eye
        yield return new WaitForSeconds(.6f);//wait for half a second
        foreach (var item in laserAttProjectiles)//enable the laser beam to sync with eye beam animation
        {
            item.SetActive(true);
        }
        yield return new WaitForSeconds(1f);//wait for a second to allow beam to last
        foreach (var item in laserAttProjectiles)//diable laser beam
        {
            item.SetActive(false);
        }
        laserAttObject.SetActive(false);//disable eye portion of laser beam
        yield return new WaitForSeconds(2.0f);//hold golem in place for 2 seconds to allow an attack from player
        golemIsAtt = false;//set boolean for golem is attacking to false
        anim.SetBool("laserAtt", false);//set animator state machine laser attack to false(allow idle to resume)
        ChangeState(characterState.idle);//change state machine back to idle

    }
    private void AimAtPlayer(GameObject projectile, float rateOfChange)
    {
        Vector3 targetDir = target.position - projectile.transform.position;//will be used to calibrate aiming laser at player
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;//store the angle for sprite to pivot and adjust
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);//store the angle in quarternion
        projectile.transform.rotation = Quaternion.Slerp(projectile.transform.rotation, q, Time.deltaTime * rateOfChange);//rotate laser beam towards player
    }
    Vector3 AimArmAtPlayer(GameObject projectile, float rateOfChange)
    {
        Vector3 targetDir = target.position - projectile.transform.position;//will be used to calibrate aiming laser at player
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;//store the angle for sprite to pivot and adjust
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);//store the angle in quarternion
        projectile.transform.rotation = Quaternion.Slerp(projectile.transform.rotation, q, Time.deltaTime * rateOfChange);//rotate laser beam towards player
        return targetDir;
    }
}
