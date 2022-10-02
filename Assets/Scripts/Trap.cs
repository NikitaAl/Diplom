using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    
    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(100);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * 1f, ForceMode2D.Impulse);
        }
    }

}
