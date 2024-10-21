using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Clockwork_AI : MonoBehaviour
{
    public float moveSpeed = 2f; // Yapay zekanın hareket hızı
    public Transform baseTransform; // Ana üssün transformu
    public bool isBroken = true; // Yapay zekanın bozulma durumu
    public bool isReachingBase = false;
    private bool isPatrollingBase = false; // Ana üs etrafında devriye atıp atmadığını kontrol eder
    private Vector3 targetPosition; // Hedef pozisyon

    public Transform spawnPoint;

    private void Start()
    {
        // Başlangıçta hedef pozisyonu belirle
        baseTransform = GameObject.Find("Base").transform;
        SetRandomTargetPosition();

    }

    private void Update()
    {
        if (isPatrollingBase)
        {

            PatrolBase();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        MoveTowardsTarget();
        UpdateDirection();
    }


    private void PatrolBase()
    {
        // Ana üssün etrafında döner
        MoveTowardsBase();
        UpdateDirection();

    }
    private void MoveTowardsTarget()
    {
        float distance = Vector2.Distance(transform.position, targetPosition);
        float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
        float currentSpeed = moveSpeed * slowdownFactor;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // Hedef pozisyona doğru hareket et
        if (distance < 0.5f)
        {
            SetRandomTargetPosition();
        }
    }
    public Vector2 GetRandomPosition(Transform basePos, float patrolDistance)
    {
        // Referans transformdan rastgele bir pozisyon oluştur
        float randomX = Random.Range(basePos.position.x - patrolDistance, basePos.position.x + patrolDistance);

        // Y ve Z koordinatları sabit kalacak, sadece X koordinatı değişecek
        return new Vector2(randomX, basePos.position.y);
    }
    private void SetRandomTargetPosition()
    {
        // Rastgele bir pozisyon belirle
        targetPosition = GetRandomPosition(spawnPoint, 2f);

        // Hedef pozisyonu sınırlandır (Spawn noktalarının çevresinde kalması için)
        Debug.Log("Hedef pozisyonu: " + targetPosition.x);
    }
    void SetRandomBasePatrolPos()
    {
        targetPosition = GetRandomPosition(baseTransform, 10f);
    }

    private void MoveTowardsBase()
    {
        if (isReachingBase == false)
        {

            targetPosition = new Vector2(baseTransform.position.x, baseTransform.position.y);
            float distance = Vector2.Distance(transform.position, targetPosition);
            float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
            float currentSpeed = moveSpeed * slowdownFactor;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 1f)
            {

                isReachingBase = true;
                moveSpeed = 2f;
                SetRandomBasePatrolPos();
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, targetPosition);
            float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
            float currentSpeed = moveSpeed * slowdownFactor;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
            {

                SetRandomBasePatrolPos();

            }

        }
    }

    private void UpdateDirection()
    {
        // Hedef pozisyona olan X mesafesini kontrol et
        if (targetPosition.x > transform.position.x)
        {
            // Hedef sağda
            if (transform.localScale.x < 0) Flip(); // Eğer nesne sola bakıyorsa, sağa döndür
        }
        else if (targetPosition.x < transform.position.x)
        {
            // Hedef solda
            if (transform.localScale.x > 0) Flip(); // Eğer nesne sağa bakıyorsa, sola döndür
        }
    }

    private void Flip()
    {
        // Karakterin yerel ölçeğini ters çevirerek yönünü değiştir
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // X eksenindeki ölçeği ters çevir
        transform.localScale = localScale;
    }
    // Oyuncu ile etkileşime girildiğinde çağrılacak metod
    public void Interact()
    {
        if (isBroken)
        {
            moveSpeed = 5f;
            isBroken = false; // Bozulma durumunu değiştir
            Debug.Log("Dost yapay zeka onarıldı ve ana üssün etrafında dolaşmaya başladı.");
            isPatrollingBase = true; // Ana üs etrafında dolaşmaya başla
        }

    }
}
