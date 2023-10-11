using System.Collections.Specialized;
//using System.Runtime.Intrinsics; 
using System.Numerics;
//using System.Threading.Tasks.Dataflow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class PlayerController : character
{

    private Vector3 change;

    private Vector3 move;
    private Animator animator;
    public GameObject projectile;
    public List<string> sceneList;
    private int sceneListSize;
    [SerializeField] public bool hasKey;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        sceneList.Add(SceneManager.GetActiveScene().name);
    }
    public AudioClip arrowThrowSound;
    private AudioSource arrowthrowSound;
    public Signal reduceMagic;
    public Inventory playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        currentState = characterState.idle;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        arrowthrowSound = GetComponent<AudioSource>();
        arrowthrowSound.clip = arrowThrowSound;
    }
    void Update()
    {
        if (Input.GetButtonDown("attack") && (currentState != characterState.death || currentState != characterState.attack || currentState != characterState.stagger))
        {
            StartCoroutine(AttackCo());
        }

        else if (Input.GetButtonDown("Second Weapon") && (currentState != characterState.death || currentState != characterState.attack || currentState != characterState.stagger))//press m to fire arrow
        {
            StartCoroutine(SecondAttackCo());
            arrowthrowSound.Play();
        }
        if (SceneManager.GetActiveScene().name != sceneList[sceneListSize])
        {
            sceneList.Add(SceneManager.GetActiveScene().name);
            sceneListSize++;
            StartPos();
        }
    }

    // Update is called once per frame
    void FixedUpdate() //changed Update to FixedUpdate to fix jitter bug
    {
        if (currentState != characterState.stagger)
        {
            move = Vector3.zero;
            move.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw normalizes the movement so that with each digital press the speed goes to the correct value
            move.y = Input.GetAxisRaw("Vertical");
            if (currentState == characterState.idle || currentState == characterState.walk)
            {
                UpdateAnimationAndMove();
            }

        }
    }
    private void StartPos()
    {

        if (sceneList.Count >= 2)
        {
            if (sceneList[sceneListSize - 1] == "dungeon_proc_gen_test")
            { this.gameObject.transform.position = new Vector3(40.5f, 31f, 0); }
            if (sceneList[sceneListSize - 1] == "overworld")
            { this.gameObject.transform.position = new Vector3(0, 13, 0); }
            Debug.Log(this.transform.position);
        }
    }
    void UpdateAnimationAndMove()
    {
        if (move != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", move.x);
            animator.SetFloat("moveY", move.y);
            animator.SetBool("moving", true);

        }
        else
        {
            animator.SetBool("moving", false);

        }
    }
    void MoveCharacter() //function that moves player
    {
        //making change to change.normalized normalized the speed when moving the player diagonally
        rb.MovePosition(transform.position + move.normalized * speed * Time.deltaTime);
    }
    public IEnumerator PlayDeathAnimationAndLoadGameOver()
    {
        this.GetComponent<Animator>().SetBool("isDead", true);
        this.GetComponent<character>().currentState = characterState.death;
        // Wait for the duration of the death animation
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(1f);

        // Load the game over scene
        SceneManager.LoadScene("game_over");
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = characterState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.15f);
        currentState = characterState.walk;
    }


    private IEnumerator SecondAttackCo()
    {
        //animator.SetBool("attacking", true);
        currentState = characterState.attack;
        yield return null;
        MakeArrow();
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.15f);
        currentState = characterState.walk;
    }

    private void MakeArrow()
    {
        if (playerInventory.currentMagic > 0)
        {
            Vector2 temp = new Vector2(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
            Arrow arrow = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.Setup(temp, ChooseArrowDirection());
            playerInventory.ReduceMagic(arrow.magicCost);
            reduceMagic.Raise();
        }
    }
    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }


}
