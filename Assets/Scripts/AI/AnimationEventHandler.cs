using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    public Enemy_AI enemyAI; // Enemy_AI script'ine referans





    //Diğer AI lar için kullanıldığında hata vererbilir
    // Animation Event ile bu metot çağrılacak
    void Awake()
    {
        if (enemyAI == null)
        {
            enemyAI = GetComponentInParent<Enemy_AI>();
        }
    }
    public void DealDamage()
    {
        if (enemyAI != null)
        {
            enemyAI.Hit(); // Enemy_AI'deki DealDamage metodunu çağır
        }
        else
        {
            Debug.LogWarning("Enemy_AI referansı atanmadı!");
        }
    }
}
