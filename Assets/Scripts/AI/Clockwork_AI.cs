using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public enum RobotState
{
    Broken,
    Villager,
    Warrior,
    Farmer,
    Miner
}
public class Clockwork_AI : MonoBehaviour
{
    [Header("Robot Durumu")]
    public RobotState currentState = RobotState.Broken;
    public float moveSpeed = 2f; // Yapay zekanın hareket hızı
    public Transform spawnPoint;
    private Transform baseTransform; // Ana üssün transformu
    private bool isReachingBase = false;
    private Vector3 targetPosition; // Hedef pozisyo


    [Header("Coin UI")]
    [SerializeField] private Canvas my_canvas;
    public List<Transform> coin_holders = new List<Transform>();
    public int price = 1;


    private void Start()
    {
        // Başlangıçta hedef pozisyonu belirle
        baseTransform = GameObject.Find("Square").transform;
        SetRandomTargetPosition();

    }

    private void Update()
    {
        switch (currentState)
        {
            case RobotState.Broken:
                HandleBrokenState();
                break;
            case RobotState.Villager:
                HandleVillagerState();
                break;
            case RobotState.Warrior:
                HandleWarriorState();
                break;
            case RobotState.Farmer:
                HandleFarmerState();
                break;
            case RobotState.Miner:
                HandleMinerState();
                break;
        }
    }
    #region ChangeState
    public void ChangeState(RobotState newState)
    {
        currentState = newState;
        Debug.Log("Robot durumu değişti: " + newState.ToString());
    }
    #endregion




    private void HandleWarriorState()
    {
        // Savaşçı durumundaki davranışlar
    }

    private void HandleFarmerState()
    {
        // Çiftçi durumundaki davranışlar
    }

    private void HandleMinerState()
    {
        // Madenci durumundaki davranışlar
    }
    #region Broken Clockwork
    private void HandleBrokenState()
    {
        Patrol();
    }
    public void Repair()
    {
        if (currentState == RobotState.Broken)
        {
            ChangeState(RobotState.Villager);
            Debug.Log("Robot onarıldı ve köylü oldu.");
        }
    }
    private void Patrol()
    {
        MoveTowardsTarget();
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
    private void SetRandomTargetPosition()
    {
        // Rastgele bir pozisyon belirle
        targetPosition = GetRandomPosition(spawnPoint, 2f);

        // Hedef pozisyonu sınırlandır (Spawn noktalarının çevresinde kalması için)
    }
    void CatchCoin(Transform targetCoin)
    {
        float distance = Vector2.Distance(transform.position, targetCoin.position);
        float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
        float currentSpeed = moveSpeed * slowdownFactor;
        transform.position = Vector2.MoveTowards(transform.position, targetCoin.position, currentSpeed * Time.deltaTime);
    }
    #endregion




    #region Villager Clockwork
    private void HandleVillagerState()
    {
        PatrolBase();
    }
    private void PatrolBase()
    {
        // Ana üssün etrafında döner
        MoveTowardsBase();
        UpdateDirection();

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

    void SetRandomBasePatrolPos()
    {
        targetPosition = GetRandomPosition(baseTransform, 10f);
    }
    #endregion
    #region Used by all
    public Vector2 GetRandomPosition(Transform basePos, float patrolDistance)
    {
        // Referans transformdan rastgele bir pozisyon oluştur
        float randomX = Random.Range(basePos.position.x - patrolDistance, basePos.position.x + patrolDistance);

        // Y ve Z koordinatları sabit kalacak, sadece X koordinatı değişecek
        return new Vector2(randomX, basePos.position.y);
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

    #endregion

    // Oyuncu ile etkileşime girildiğinde çağrılacak metod
    public void Interact()
    {
        if (currentState == RobotState.Broken)
        {
            moveSpeed = 5f;
            currentState = RobotState.Villager;
            Debug.Log("Dost yapay zeka onarıldı ve ana üssün etrafında dolaşmaya başladı.");
        }

    }

}
