using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class character : MonoBehaviour
{
    // Character script for both the player and enemies
    public enum characterState  // the character state machine for all "players" in the game
    {
        idle,
        walk,
        attack,
        stagger,
        interact,
        dead
    }

    public enum characterType
    {
        player,
        enemy
    }

    public GameObject heartForEnemy;
    public characterState currentState;
    public float speed;
    public Rigidbody2D rb;
    public characterType charType;
    public float maxHealth;
    public float health;
    public string enemyName;
    public int baseAttack;
    public float moveSpeed;
    public SpriteRenderer sprite;
    //public Animator hitAnim; //tried to set the animation for when the enemy gets hit
    public float knockTime;
    public float thrust;
    public float damage;
    public GameObject thisObject;
    public Animator animator;
    [SerializeField]
    public bool isElevated;
    public GameObject smallerPinkSlimePrefab;

    // For enemy hit sound FX
    public AudioClip enemyHitSoundFX;
    private AudioSource enemyhitSoundFX;

    // Start is called before the first frame update
    void Start()
    {
        isElevated = false;

        // For hit sound FX
        enemyhitSoundFX = GetComponent<AudioSource>();
        enemyhitSoundFX.clip = enemyHitSoundFX;
    }

    public void Knock(Transform other, float force, float kt) // Knockback
    {
        Rigidbody2D hit = this.GetComponent<Rigidbody2D>();
        Vector2 difference = hit.transform.position - other.position;
        difference = difference.normalized * force;
        hit.AddForce(difference, ForceMode2D.Impulse);
        if (hit != null)
        {
            StartCoroutine(KnockCo(hit, kt));
        }
    }


    private IEnumerator KnockCo(Rigidbody2D rb, float kt)
    {
        if (rb != null)
        {
            UnityEngine.Debug.Log(this);
            rb.GetComponent<character>().currentState = characterState.stagger;
            this.GetComponent<Animator>().SetBool("hit", true);
            yield return new WaitForSeconds(kt);
            UnityEngine.Debug.Log("COLLISION");
            this.GetComponent<Animator>().SetBool("hit", false);
            rb.velocity = Vector2.zero;
            rb.GetComponent<character>().currentState = characterState.idle;
            
        }
    }

    public void TakeDamage(float dmg)
    {
        // Is this where we put the sound for enemy hits??
        // enemyhitSoundFX.PlayOneShot(enemyHitSoundFX);

        health -= dmg;
        if (health <= 0)
        {
            if (this.charType == characterType.enemy)
            {
                UnityEngine.Debug.Log("PLAYING ENEMY HIT SOUND");
                int newRand = Random.Range(1, 11);
                if (newRand == 2)
                {
                    Instantiate(heartForEnemy, transform.position, Quaternion.identity);
                }
                //I moved "this.gameObject.SetActive(false);" here to make the player not dissapear and let the health code run through to make him lose a heart and play the death anim; though there may be a way to work around this 
                StartCoroutine(EnemyDeathAnimAndDestroy());
                currentState = characterState.dead;
                
            }
            //"this.gameObject.SetActive(false);" was here previously 
        }

        if (health <= 0 && gameObject.CompareTag("big_pinkslime"))
        {
            if (this.charType == characterType.enemy)
            {

                StartCoroutine(EnemyDeathAnimAndDestroy());
                // Spawn the first smaller pink slime
                Instantiate(smallerPinkSlimePrefab, transform.position + new Vector3(-.5f, 0, 0), Quaternion.identity);

                // Spawn the second smaller pink slime
                Instantiate(smallerPinkSlimePrefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);

                // Spawn the third smaller pink slime
                Instantiate(smallerPinkSlimePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                currentState = characterState.dead;

            }
        }
    }

    private IEnumerator EnemyDeathAnimAndDestroy()
    {
        // Play enemy death animation
        animator.SetTrigger("isDead");

        // Wait for the duration of the death animation
        yield return new WaitForSeconds(0.5f); // Replace ANIMATION_DURATION with the actual duration of your death animation.

        // Disable the game object after the animation has played
        this.gameObject.SetActive(false);
        UnityEngine.Debug.Log("Playing Pink slime death anim");

    }

    public float getDmg()
    {
        return damage;
    }

    public float getThrust()
    {
        return thrust;
    }    

    // Update is called once per frame
    void Update()
    {
    }
    /*public new objectType GetType()
    {
        return objType;
    }*/
    public float getknockTime()
    {
        return knockTime;
    }

}
