using System.Collections.Specialized;
//using System.Runtime.Intrinsics; 
using System.Numerics;
//using System.Threading.Tasks.Dataflow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using UnityEngine.SceneManagement;

public class PlayerController_COPY : character
{

    private Vector3 change;

    private Vector3 move;
    private Animator playerAnimator;

    //For heart system
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // For heart sound FX's
    public AudioClip heartUpSound;
    private AudioSource heartSound;

    // For magic sound FX's
    public AudioClip magicUpSound;
    private AudioSource magicUp;

    // For Arrows
    public GameObject projectile;
    public Signal reduceMagic;
    public Inventory playerInventory;
    public AudioClip arrowThrowSound;
    private AudioSource arrowthrowSound;

    // For Box
    private Vector2 boxPushSpeed;

    // Level Up System
    [SerializeField] int currentExperience, maxExperience, currentLevel;



    // For GameManager
    private GameManager gameManager;

    // For changing player skins when leveling up
    public XpController xpController;      // Reference to your XpController script
    public GameObject playerObjectLevel1;  // Reference to the player game object at level 1
    public GameObject playerObjectLevel2;  // Reference to the player game object at level 2
    public GameObject playerObjectLevel3;  // Reference to the player game object at level 3
    public CameraMovement cameraMovement;

    private Vector3 initialPlayerPosition;
    private Vector3 initialPlayerPositionLevel2;
    private Vector3 initialPlayerPositionLevel3;



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
        gameManager = GameObject.FindObjectOfType<GameManager>();

        // Ensuring the archer and spell caster objects are not null
        if (xpController == null || playerObjectLevel1 == null || playerObjectLevel2 == null)
        {
            Debug.LogError("Missing references in the PlayerController script. Please assign them in the Unity Editor.");
        }

    }
    void Update()
    {
        // Attack
        if (Input.GetButtonDown("attack") && currentState != characterState.attack && currentState != characterState.stagger)
        {
            StartCoroutine(AttackCo());
        }
        else if (Input.GetButtonDown("Second Weapon") && currentState != characterState.attack && currentState != characterState.stagger && gameObject.name != "player") //press m to fire arrow
        {
            StartCoroutine(SecondAttackCo());
            //play arrow sound here
            Debug.Log("Playing Arrow Sound");
        }
        // Health
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {

                hearts[i].sprite = emptyHeart;

            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;

            }
        }

        if (health <= 0)
        {
            Debug.Log("playing death anim");
            currentState = characterState.dead;
            StartCoroutine(PlayDeathAnimationAndLoadGameOver());
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
            rb.velocity = Vector2.zero; // No input, stop moving
            animator.SetBool("moving", false);

        }
    }
    void MoveCharacter() //function that moves player
    {
        //making change to change.normalized normalized the speed when moving the player diagonally
        rb.MovePosition(transform.position + move.normalized * speed * Time.deltaTime);
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



    IEnumerator PlayDeathAnimationAndLoadGameOver()
    {
        // Play the death animation
        //Animator animator = thePlayer.GetComponent<Animator>();
        Debug.Log("Playing Death Animation");
        animator.SetTrigger("isDead");

        // Wait for the duration of the death animation
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        yield return new WaitForSeconds(0.8f);

        // Load the game over scene
        SceneManager.LoadScene("game_over");
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("overworld");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "enemy")
        {
            // lose 1 heart if colliding with enemy
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("isHurt");



        }

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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("box"))
        {
            // Stop applying force when not pushing
            Rigidbody2D boxRB = collision.collider.GetComponent<Rigidbody2D>();
            boxRB.velocity = Vector2.zero;
            animator.SetBool("isPushing", false);

        }
    }

    private void OnTriggerEnter2D(Collider2D other) // moved the heartup to the oncollisionenter2d above
    {
        //Debug.Log("A trigger happened");
        if (other.tag == "heartUp" && health != maxHealth)
        {
            Debug.Log("Picked up heart");
            other.gameObject.SetActive(false);
            health += 1;
            heartSound.Play();
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
