using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatProjectile : MonoBehaviour
{
    public int damage = 3;
    private int playerMask;

    [SerializeField] private float explosionRadius = .5f;

    private void Awake()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    private void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }

    public void OnHitExplosion()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerMask);

        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(transform, 3);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().Hitted(transform, 2);

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.color = Color.blue;
    }
}