using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Health : MonoBehaviour
{
    [SerializeField] private string hitSoundName;
    [SerializeField] private string dieSoudName;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] GameObject gear;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        AudioManager.instance.PlaySfx(hitSoundName);
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} has {currentHealth} health left!");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        if (gameObject.tag != "Player")
        {
            AudioManager.instance.PlaySfx(dieSoudName);
            GameObject newCoin = Instantiate(gear, this.transform.position, Quaternion.identity);
            GameManager.instance.coins.Add(newCoin);
            Debug.Log($"{gameObject.name} has been destroyed!");
            if (gameObject.tag == "Trash")
            {
                GameManager.instance.gearTrashs.Remove(gameObject);
            }
            else
            {
                GameManager.instance.humans.Remove(gameObject);
            }
            Destroy(gameObject);

        }
        else
        {
            Debug.Log($"{gameObject.name} has been destroyed!");

            Destroy(gameObject);
            SceneManager.LoadScene("Emre");
        }

    }
}
