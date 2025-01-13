using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 10; // Verilen hasar
    [SerializeField] private float destroyTime = 5f; // Yok olma süresi

    void Start()
    {
        // Belirli bir süre sonra lazer nesnesini yok et
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer hedef bir Health bileşenine sahipse canını azalt
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Trash")
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
                Destroy(gameObject); // Lazer nesnesini yok et
            }
        }
    }
}
