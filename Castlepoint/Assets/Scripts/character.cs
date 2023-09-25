using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class character : MonoBehaviour
{
    // Character script for both the player and enemies
    public enum characterState  // the character state machine for all "players" in the game
    {
        idle,
        walk,
        attack,
        stagger,
        interact
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
            rb.GetComponent<character>().currentState = characterState.stagger;
            animator.SetBool("hit", true);
            yield return new WaitForSeconds(kt);
            animator.SetBool("hit", false);
            rb.velocity = Vector2.zero;
            rb.GetComponent<character>().currentState = characterState.idle;
            rb.velocity = Vector2.zero;
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            if (this.charType == characterType.enemy)
            {
                int newRand = Random.Range(1, 11);
                if (newRand == 2)
                {
                    Instantiate(heartForEnemy, transform.position, Quaternion.identity);
                }
            }
            this.gameObject.SetActive(false);
        }
    }

    public float getDmg()
    {
        return damage;
    }

    public float getThrust()
    {
        return thrust;
    }


    // Start is called before the first frame update
    void Start()
    {
        isElevated = false;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public new objectType GetType()
    {
        return objType;
    }
    public float getknockTime()
    {
        return knockTime;
    }

}
