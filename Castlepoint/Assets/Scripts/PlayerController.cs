using System.Collections.Specialized;
//using System.Runtime.Intrinsics; 
using System.Numerics;
//using System.Threading.Tasks.Dataflow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class PlayerController : character, IDataPersistence
{
    public void LoadData(GameData data)
    {
        //thisPlayer = data.player;
        this.transform.position = data.currentPosition;
        this.health = data.currentHealth;
        this.hasKey = data.hasKey;
    }
    public void SaveData(GameData data)
    {
        Debug.Log("Saving Player");
        //data.player = thisPlayer;
        if (this != null)
        {
            data.currentPosition = this.transform.position;
            data.currentHealth = this.health;
            data.hasKey = this.hasKey;
        }

    }
    public GameObject thisPlayer;
    private Vector3 change;

    private Vector3 move;
    private Animator animator;
    public GameObject projectile;
    public List<string> sceneList;
    private int sceneListSize;
    [SerializeField] public bool hasKey;
    private void Awake()
    {   
        sceneList.Add(SceneManager.GetActiveScene().name);
    }
    public AudioClip arrowThrowSound;
    private AudioSource arrowthrowSound;
    public Signal reduceMagic;
    public Inventory playerInventory;
    private bool touchingChest;

    // For GameManager
    //private GameManager gameManager;

    // For changing player skins when leveling up
    public XpController xpController;      // Reference to your XpController script
    public GameObject playerObjectLevel1;  // Reference to the player game object at level 1
    public GameObject playerObjectLevel2;  // Reference to the player game object at level 2
    public GameObject playerObjectLevel3;  // Reference to the player game object at level 3
    public CameraMovement cameraMovement;
    private Vector3 initialPlayerPosition;
    private Vector3 initialPlayerPositionLevel2;
    private Vector3 initialPlayerPositionLevel3;
    // For heart sound FX's
    public AudioClip heartUpSound;
    private AudioSource heartSound;
    // For magic sound FX's
    public AudioClip magicUpSound;
    private AudioSource magicUp;
    // For Box
    private Vector2 boxPushSpeed;
    // Level Up System
    [SerializeField] int currentExperience, maxExperience, currentLevel;
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

        // heart sound pick up sound fx
        heartSound = GetComponent<AudioSource>();
        heartSound.clip = heartUpSound;

        // magic sound pick up sound fx
        magicUp = GetComponent<AudioSource>();
        magicUp.clip = magicUpSound;

        // Find the GameManager in the scene
        //gameManager = GameObject.FindObjectOfType<GameManager>();

        // Ensuring the archer and spell caster objects are not null
        if (xpController == null || playerObjectLevel1 == null || playerObjectLevel2 == null)
        {
            Debug.LogError("Missing references in the PlayerController script. Please assign them in the Unity Editor.");
        }
    }
    void Update()
    {
        if (touchingChest == false && Input.GetButtonDown("attack") && (currentState != characterState.death || currentState != characterState.attack || currentState != characterState.stagger))
        {
            StartCoroutine(AttackCo());
        }
        else if (Input.GetButtonDown("Second Weapon") && (currentState != characterState.death || currentState != characterState.attack || currentState != characterState.stagger))//press m to fire arrow
        {
            StartCoroutine(SecondAttackCo());
            //arrowthrowSound.Play();
        }
        if (SceneManager.GetActiveScene().name != sceneList[sceneListSize])
        {
            sceneList.Add(SceneManager.GetActiveScene().name);
            sceneListSize++;
            StartPos();
        }
        // Change skins when leveling up 
        if (xpController.level == 2)  // Change the level as needed
        {
            initialPlayerPositionLevel2 = playerObjectLevel1.transform.position;
            ChangeToPlayerObject2(playerObjectLevel2);

        }
        else if (xpController.level == 3)
        {
            initialPlayerPositionLevel3 = playerObjectLevel2.transform.position;
            ChangeToPlayerObject3(playerObjectLevel3);
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
            if (sceneList[sceneListSize - 1] == "dungeon_proc_gen_test" && sceneList[sceneListSize] == "OverworldJohn")
            { this.gameObject.transform.position = new Vector3(40.5f, 31f, 0); }
            if (sceneList[sceneListSize - 1] == "OverworldJohn" && sceneList[sceneListSize] == "dungeon_proc_gen_test")
            { this.gameObject.transform.position = new Vector3(0, 13, 0); }
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
            arrowthrowSound.PlayOneShot(arrowThrowSound);
        }
    }
    Vector3 ChooseArrowDirection()
    {
        float temp = Mathf.Atan2(animator.GetFloat("moveY"), animator.GetFloat("moveX")) * Mathf.Rad2Deg;
        return new Vector3(0, 0, temp);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("box"))
        {
            if (animator.GetBool("moving"))
            {
                animator.SetBool("isPushing", true);
                // Apply force to the box only when pushing
                Rigidbody2D boxRB = collision.collider.GetComponent<Rigidbody2D>();
                boxRB.velocity = new Vector2(move.x, move.y) * boxPushSpeed;
            }
            else
            {
                animator.SetBool("isPushing", false);

            }
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Chest")//if player is touching a chest
        {
            touchingChest = true;
            if (Input.GetButtonDown("attack"))//if player tries to open the chest
            {
                other.gameObject.GetComponent<Chest>().OpenChest();
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Chest")
        {
            touchingChest = false;
        }
        if (other.collider.CompareTag("box"))
        {
            // Stop applying force when not pushing
            Rigidbody2D boxRB = other.collider.GetComponent<Rigidbody2D>();
            boxRB.velocity = Vector2.zero;
            animator.SetBool("isPushing", false);

        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "laser")
        {
            this.gameObject.GetComponent<character>().TakeDamage(1f);
        }

       

        if (other.tag == "magicUp")
        {
            magicUp.PlayOneShot(magicUpSound);
        }

    }
    private void LevelUp()
    {
        // Code to change the sprite skins goes here
        // After leveling up to the level 2, the player skin should switch to the default skin
        // Then from level 2 to level 3, the spell caster

        currentLevel++;
        currentExperience = 0;
        maxExperience += 100;
    }

    private void ChangeToPlayerObject2(GameObject newPlayerObject)
    {
        // Record the position of the current player
        initialPlayerPosition = gameObject.transform.position;

        // Deactivate the current player object
        gameObject.SetActive(false);

        // Activate the new player object
        newPlayerObject.SetActive(true);

        // Set the position of the new player to the recorded position
        playerObjectLevel2.transform.position = GetInitialPlayerPosition();

        cameraMovement.SetTarget(newPlayerObject.transform);

        // Optionally, you can also reposition the player or perform other actions
        // based on the transition from one object to another.
    }

    private void ChangeToPlayerObject3(GameObject newPlayerObject)
    {
        // Record the position of the current player
        initialPlayerPosition = gameObject.transform.position;

        // Deactivate the current player object
        gameObject.SetActive(false);

        // Activate the new player object
        newPlayerObject.SetActive(true);

        // Set the position of the new player to the recorded position
        playerObjectLevel3.transform.position = GetInitialPlayerPosition();

        cameraMovement.SetTarget(newPlayerObject.transform);

        // Optionally, you can also reposition the player or perform other actions
        // based on the transition from one object to another.
    }

    public Vector3 GetInitialPlayerPosition()
    {
        return initialPlayerPosition;
    }

    public Vector3 GetInitialPlayerPositionLevel2()
    {
        return initialPlayerPositionLevel2;
    }

    public Vector3 GetInitialPlayerPositionLevel3()
    {
        return initialPlayerPositionLevel3;
    }

    

}
