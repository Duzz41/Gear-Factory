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
    Worker,
    Miner
}
public class Clockwork_AI : MonoBehaviour
{
    [Header("Robot Durumu")]
    public RobotState currentState = RobotState.Broken;
    public float moveSpeed = 2f; // Yapay zekanın hareket hızı
    [SerializeField] private float currentSpeed;
    public Transform spawnPoint;
    private Transform baseTransform; // Ana üssün transformu
    public Vector3 targetPosition; // Hedef pozisyo
    Transform closestCoin;
    Transform closestTarget;
    private Transform closestTool;

    public List<GameObject> coins = new List<GameObject>();
    [SerializeField] Animator anim;
    [SerializeField] GameObject[] sprites;

    [SerializeField] private GameObject laserPrefab; // Lazer prefabını buraya atayın
    [SerializeField] private Transform firePoint; // Lazerin fırlayacağı nokta
    [SerializeField] private float laserSpeed = 10f;

    [SerializeField] private float fireCooldown = 2f; // Ateş etme arasındaki süre
    [SerializeField] private float stopDistance = 4f; // Hedefe yaklaştığında duracağı mesafe

    [SerializeField] private float workerDistance = 1f;
    // Transform closestTarget;
    private float lastFireTime; // Son ateş zamanı
    private void Start()
    {
        anim = sprites[0].GetComponent<Animator>();
        // Başlangıçta hedef pozisyonu belirle
        /*for (int i = 0; i < sprites.Length; i++)
         {
             sprites[i].SetActive(false);
         }*/
        baseTransform = GameObject.Find("GearFactory").transform;
        SetRandomTargetPosition();

        walls_parent = GameObject.Find("#WALLS").transform;

    }
    private void Update()
    {
        anim.SetFloat("Speed", currentSpeed);
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
            case RobotState.Worker:
                HandleWorkerState();
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
        UpdateSpritesBasedOnState(newState);
    }
    private void UpdateSpritesBasedOnState(RobotState state)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            // State'in indeksini sprite dizisindeki sırasıyla eşleştiriyoruz
            sprites[i].SetActive(i == (int)state);
            anim = sprites[(int)state].GetComponent<Animator>();
        }
    }
    #endregion    
    private void HandleMinerState()
    {
        // Madenci durumundaki davranışlar
    }
    #region Worker
    private void HandleWorkerState()
    {
        closestTarget = FindWork();

        if (closestTarget != null)
        {

            Debug.Log("İŞ bulll");
            BuildTarget(closestTarget);
            UpdateDirection(closestTarget.position);

        }
        else
        {
            PatrolBase();
        }
    }
    Transform FindWork()
    {
        float closestDistance = 45f;
        foreach (GameObject constr in GameManager.instance.constBuildings)
        {
            if (constr != null)
            {
                float distanceToTrash = Vector2.Distance(transform.position, constr.transform.position);
                if (distanceToTrash < closestDistance)
                {

                    closestTarget = constr.transform;
                    return closestTarget;
                }
            }
            else
            {
                return null;
            }
        }
        return closestTarget;
    }
    void BuildTarget(Transform target)
    {
        if (GameManager.instance._isDay == false && (target.position.x < left_wall.position.x || target.position.x > right_wall.position.x))
        {
            return;
        }
        UpdateDirection(target.position);
        float distance = Vector2.Distance(transform.position, target.position);

        // Yeterince yaklaşmamışsa hareket etmeye devam et
        if (distance > workerDistance)
        {

            float currentSpeed = moveSpeed * 1.25f;
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        }

        if (distance <= workerDistance)
        {
            // GameManager.instance.constBuildings.Remove(target.gameObject);
            BuildingTarget(target);
        }
    }
    private bool isConstructionSoundPlaying = false;
    void BuildingTarget(Transform target)
    {
        Debug.Log(target.name);
        if (anim != null)
        {
            anim.SetTrigger("Work"); // Attack animasyonunu çalıştır
        }
        if (!isConstructionSoundPlaying)
        {

            AudioManager.instance.PlaySfx("Construction");
            isConstructionSoundPlaying = true; // Ses çalmaya başladı, bayrağı güncelle
        }

        StartCoroutine(ConstructionCoroutine(target));

        // Lazer nesnesini oluştur
    }
    private IEnumerator ConstructionCoroutine(Transform target)
    {
        // Construction duration (e.g., 2 seconds)
        yield return new WaitForSeconds(4f);
        isConstructionSoundPlaying = false;
        // Complete construction
        Wall wall = target.GetComponent<Wall>();
        if (wall != null)
        {
            Debug.Log("Construction completed!");
            anim.ResetTrigger("Work");
            wall.CompleteConstruction();
            closestTarget = null;
            // Resume worker's patrol or walking behavior
        }
    }
    #endregion
    #region WARRIOR
    private void HandleWarriorState()
    {
        Transform closestTarget = FindEnemyOrTrash();

        if (closestTarget != null)
        {
            AttackTarget(closestTarget);
            UpdateDirection(closestTarget.position);
        }
        else
        {
            //if (GameManager.instance._isDay)
            PatrolBase();
            //else
            //NightBehaviour();
        }
        // Savaşçı durumundaki davranışlar
    }
    Transform FindEnemyOrTrash()
    {

        float closestDistance = 150f;
        foreach (GameObject trashs in GameManager.instance.gearTrashs)
        {
            if (trashs != null)
            {
                float distanceToTrash = Vector2.Distance(transform.position, trashs.transform.position);
                if (distanceToTrash < closestDistance)
                {

                    closestTarget = trashs.transform;
                    return closestTarget;
                }
            }
            else
            {
                return null;
            }
        }
        foreach (GameObject enemy in GameManager.instance.humans)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestTarget = enemy.transform;
                    return closestTarget;
                }
            }
            else
            {
                return null;
            }
        }
        return closestTarget;
    }


    void AttackTarget(Transform target)
    {
        UpdateDirection(target.position);
        float distance = Vector2.Distance(transform.position, target.position);

        // Yeterince yaklaşmamışsa hareket etmeye devam et
        if (distance > stopDistance)
        {
            float currentSpeed = moveSpeed * 1.25f;
            Vector2 targetPosition = new Vector2(target.position.x, 1f);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        }

        // Saldırı mesafesinde lazer ateşle
        if (distance <= stopDistance)
        {
            FireLaser(target);
        }
    }
    void FireLaser(Transform target)
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack"); // Attack animasyonunu çalıştır
        }
        // Cooldown kontrolü
        if (Time.time - lastFireTime < fireCooldown)
        {
            return; // Cooldown süresi dolmadıysa çık
        }

        // Lazer nesnesini oluştur
        if (laserPrefab != null && firePoint != null)
        {
            GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

            // Lazerin yönünü hesapla
            Vector2 direction = (target.position - firePoint.position).normalized;

            // Rigidbody2D üzerinden hareket uygula
            Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * laserSpeed;
            }

            // Son ateş zamanını güncelle
            lastFireTime = Time.time;
        }
    }
    #endregion
    #region Broken Clockwork


    private void HandleBrokenState()
    {
        Transform closestCoin = FindClosestCoin();
        if (closestCoin != null)
        {
            CatchCoin(closestCoin);
            UpdateDirection(closestCoin.position);
        }
        else
            Patrol();
    }




    private void Patrol()
    {
        MoveTowardsTarget();
        UpdateDirection(targetPosition);
    }
    private void MoveTowardsTarget()
    {
        float distance = Vector2.Distance(transform.position, targetPosition);
        float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
        currentSpeed = moveSpeed * slowdownFactor;
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
        float distance = Vector2.Distance(transform.position, targetCoin.position);
        float slowdownFactor = Mathf.Clamp01(distance / 2f);
        currentSpeed = moveSpeed / slowdownFactor;
        Vector2 targetPos = new Vector2(targetCoin.position.x, 1f);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
        if (distance < 0.5f)
        {
            ChangeState(RobotState.Villager);
            CollectGear(targetCoin.gameObject);

            MoveTowardsBase();

            if (!GameManager.instance.attackableObjects.Contains(gameObject))
            {
                GameManager.instance.attackableObjects.Add(gameObject);
            }
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
        UpdateDirection(targetPosition);

    }



    //Parayı aldıktan sonra köy merkezine ulaşana kadar köy merkezine yönelmesini sağlayan method.
    private void MoveTowardsBase()
    {
        if (currentState == RobotState.Broken)
        {
            targetPosition = new Vector2(baseTransform.position.x, 1f);
            float distance = Vector2.Distance(transform.position, targetPosition);
            currentSpeed = moveSpeed * 1.5f;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (distance < 1f)
            {
                SetRandomBasePatrolPos();
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, targetPosition);
            float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
            currentSpeed = (moveSpeed * 1.5f) * slowdownFactor;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            if (distance < 0.5f)
            {

                SetRandomBasePatrolPos();

            }

        }
    }

    private Transform FindClosestTool()
    {
        float closestDistance = 30f;

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
        UpdateDirection(targetTool.position);
        float distance = Vector2.Distance(transform.position, targetTool.position);
        float slowdownFactor = Mathf.Clamp01(distance / 1.5f);
        currentSpeed = (moveSpeed * 1.5f) * slowdownFactor;

        Vector2 targetPosition = new Vector2(targetTool.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        if (distance < 2.5f)
        {


            switch (targetTool.parent.parent.name)
            {
                case "MinerFactory":
                    ChangeState(RobotState.Miner);
                    break;
                case "WorkerFactory":
                    ChangeState(RobotState.Worker);
                    break;
                case "MilitaryFactory":
                    ChangeState(RobotState.Warrior);
                    break;
            }

            targetTool.parent.parent.GetComponent<Building>().RemoveTool(targetTool);

            targetTool.parent = this.transform;
            targetTool.tag = "Untagged";
            Destroy(targetTool.gameObject);

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



    private void UpdateDirection(Vector3 target)
    {
        // Hedef pozisyona olan X mesafesini kontrol et
        if (target.x > transform.position.x)
        {
            // Hedef sağda
            if (transform.localScale.x < 0) Flip(); // Eğer nesne sola bakıyorsa, sağa döndür
        }
        else if (target.x < transform.position.x)
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


    private void NightBehaviour()
    {
        if (GameManager.instance._isDay == false)
        {
            if (currentState == RobotState.Warrior)
            {
                FindLastWall();
                if (transform.position.x < 0)
                {
                    transform.position = Vector2.MoveTowards(transform.position, left_wall.position, currentSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, right_wall.position, currentSpeed * Time.deltaTime);
                }
            }
            else if (currentState == RobotState.Worker)
            {

            }
        }
    }

    public Transform walls_parent;
    private Transform left_wall;
    private Transform right_wall;
    private void FindLastWall()
    {
        for (int i = 0; i < walls_parent.childCount; i++)
        {

            if (i == 0)
            {
                left_wall = walls_parent.GetChild(i);
                right_wall = walls_parent.GetChild(i);
            }
            else
            {
                if (walls_parent.GetChild(i).position.x < left_wall.position.x)
                {
                    left_wall = walls_parent.GetChild(i);
                }
                else if (walls_parent.GetChild(i).position.x > right_wall.position.x)
                {
                    right_wall = walls_parent.GetChild(i);
                }
            }

        }


    }

    #endregion
}
