using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
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
    public Vector3 targetPosition; // Hedef pozisyo
    [SerializeField] Transform closestCoin;
    [SerializeField] private Transform closestTool;

    public List<GameObject> coins = new List<GameObject>();


    private void Start()
    {
        // Başlangıçta hedef pozisyonu belirle
        baseTransform = GameObject.Find("GearFactory").transform;
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
        Transform closestCoin = FindClosestCoin();
        if (closestCoin != null)
        {
            CatchCoin(closestCoin);
        }
        else
            Patrol();
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
    //Yapay zekanın en yakınında kalan gear'i bulan metod
    private Transform FindClosestCoin()
    {
        float closestDistance = 5f;

        foreach (GameObject coin in GameManager.instance.coins)
        {
            if (coin != null)
            {
                float distanceToCoin = Vector2.Distance(transform.position, coin.transform.position);
                if (distanceToCoin < closestDistance)
                {

                    closestCoin = coin.transform;
                    return closestCoin;
                }
            }
            else
            {
                return null;
            }
        }

        return closestCoin;
    }

    void CatchCoin(Transform targetCoin)
    {
        UpdateDirection();
        float distance = Vector2.Distance(transform.position, targetCoin.position);
        float currentSpeed = moveSpeed * 1.25f;
        transform.position = Vector2.MoveTowards(transform.position, targetCoin.position, currentSpeed * Time.deltaTime);
        if (distance < 0.5f)
        {
            CollectGear(targetCoin.gameObject);
            MoveTowardsBase();
            ChangeState(RobotState.Villager);
        }
    }


    void CollectGear(GameObject gear)
    {
        GameManager.instance.coins.Remove(gear);
        coins.Add(gear);
        Destroy(gear);
        closestCoin = null;
    }

    #endregion




    #region Villager Clockwork
    private void HandleVillagerState()
    {
        Transform closestTool = FindClosestTool();
        if (closestTool != null)
        {
            CatchTool(closestTool);
        }
        else
        {
            PatrolBase();
        }

    }



    //Köylü olduktan sonra ana üssün etrafında görev alana kadar volta atmasını sağlayan kısım.
    private void PatrolBase()
    {
        // Ana üssün etrafında döner
        MoveTowardsBase();
        UpdateDirection();

    }



    //Parayı aldıktan sonra köy merkezine ulaşana kadar köy merkezine yönelmesini sağlayan method.
    private void MoveTowardsBase()
    {
        if (currentState == RobotState.Broken)
        {
            targetPosition = new Vector2(baseTransform.position.x, 1f);
            float distance = Vector2.Distance(transform.position, targetPosition);
            float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
            float currentSpeed = moveSpeed * slowdownFactor;

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (distance < 0.5f)
            {
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
            if (distance < 0.5f)
            {

                SetRandomBasePatrolPos();

            }

        }
    }

    private Transform FindClosestTool()
    {
        float closestDistance = 7f;

        GameObject[] tools = GameObject.FindGameObjectsWithTag("Tool");
        foreach (GameObject tool in tools)
        {
            if (tool != null)
            {
                float distanceToTool = Vector2.Distance(transform.position, tool.transform.position);
                if (distanceToTool < closestDistance)
                {
                    closestTool = tool.transform;
                    return closestTool;
                }
            }
            else
            {
                return null;
            }
        }


        return closestTool;
    }
    private void CatchTool(Transform targetTool)
    {
        UpdateDirection();
        float distance = Vector2.Distance(transform.position, targetTool.position);
        float currentSpeed = moveSpeed * 1.25f;

        Vector2 targetPosition = new Vector2(targetTool.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        if (distance < 0.5f)
        {


            switch (targetTool.parent.parent.name)
            {
                case "MinerFactory":
                    ChangeState(RobotState.Miner);
                    break;
                case "FarmerFactory":
                    ChangeState(RobotState.Farmer);
                    break;
                case "MilitaryFactory":
                    ChangeState(RobotState.Warrior);
                    break;

            }

            targetTool.parent.parent.GetComponent<Building>().RemoveTool(targetTool);
            targetTool.parent = this.transform;
            targetTool.tag = "Untagged";


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
        return new Vector2(randomX, 1f);
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
