using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class OneWayPlatformerCharacter : MonoBehaviour
{
    private Rigidbody2D _rb2d;
    private bool _ignorePlatform = false;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // If character positon + heght is greater than colider position, ignore collision
        _ignorePlatform = transform.position.y - GetComponent<Collider2D>().bounds.extents.y > GameObject.Find("OneWayPlatform").transform.position.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform") && _ignorePlatform)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>(), false);
        }
    }
}
