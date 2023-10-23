using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int damage = 4;
    private int playerMask;

    [SerializeField] private Transform explosionPoint;
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
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(explosionPoint.transform.position, explosionRadius, playerMask);

        foreach (Collider2D obj in hitObjects)
        {
            obj.GetComponent<PlayerCombat>().Hitted(transform, damage);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerCombat>().Hitted(transform, damage);

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(explosionPoint.transform.position, explosionRadius);
        Gizmos.color = Color.blue;
    }
}