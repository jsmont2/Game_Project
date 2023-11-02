using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class rangedSpider : MonoBehaviour
{
    public float projectileRadius;
    public float attackRadius;

    // For web pellet
    public Transform player;
    public GameObject webPellet;
    private float shootCooldown;
    public float startShootCooldown;

    // Start is called before the first frame update
    void Start()
    {
        // Web Pellet
        shootCooldown = startShootCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // For web pellet
        if (Vector3.Distance(player.position, transform.position) <= projectileRadius && Vector3.Distance(player.position, transform.position) > attackRadius) 
        {
            Vector2 direction = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);
            transform.up = direction;
            if (shootCooldown <= 0)
            {
                Instantiate(webPellet, transform.position, transform.rotation);
                shootCooldown = startShootCooldown;
            }
            else
            {
                shootCooldown -= Time.deltaTime;
            }
        }
    }
}
