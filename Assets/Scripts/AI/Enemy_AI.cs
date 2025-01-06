using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public float moveSpeed = 2f; // Yapay zekanın hareket hızı

    private float currentSpeed;
    public Vector3 targetPosition; // Hedef pozisyo
    public float attackRange = 1f;
    [SerializeField] Transform closestTarget;
    [SerializeField] Animator anim;
    public int attackDamage = 1; // Saldırı başına hasar

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        // Find the closest target
        closestTarget = FindClosestCoin();

        if (closestTarget != null)
        {
            MoveTowardsTarget();
            FlipTowardsTarget();
            // Check if within attack range
            if (Vector2.Distance(transform.position, closestTarget.position) <= attackRange)
            {
                StartAttack(closestTarget.gameObject);
            }
            else
            {
                StopAttack();
            }
        }
    }

    private Transform FindClosestCoin()
    {
        float closestDistance = 100f;

        foreach (GameObject target in GameManager.instance.attackableObjects)
        {
            if (target != null)
            {
                float distanceToCoin = Vector2.Distance(transform.position, target.transform.position);
                if (distanceToCoin < closestDistance)
                {
                    closestTarget = target.transform;
                    closestDistance = distanceToCoin;
                }
            }
        }

        return closestTarget;
    }
    private void MoveTowardsTarget()
    {
        // Move towards the closest target
        Vector2 direction = (closestTarget.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(closestTarget.position.x, 1f), currentSpeed * Time.deltaTime);

        // Trigger walking animation if an animator is attached
        if (anim != null)
        {
            anim.SetFloat("Speed", currentSpeed);
        }
    }
    private void FlipTowardsTarget()
    {
        // Hedefe doğru bak
        if (closestTarget != null)
        {
            Vector3 scale = transform.localScale;
            if (closestTarget.position.x > transform.position.x)
            {
                scale.x = Mathf.Abs(scale.x); // Sağ tarafa bak
            }
            else if (closestTarget.position.x < transform.position.x)
            {
                scale.x = -Mathf.Abs(scale.x); // Sol tarafa bak
            }
            transform.localScale = scale;
        }
    }
    private void StartAttack(GameObject target)
    {

        currentSpeed = 0; // Stop moving when attacking
        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
        }



    }

    public void Hit()
    {
        if (closestTarget != null)
        {
            if (closestTarget.GetComponent<Wall>() != null)
            {
                Wall wall = closestTarget.GetComponent<Wall>();
                if (wall != null)
                {
                    AudioManager.instance.PlaySfx("HumanAttack");
                    wall.TakeDamage(attackDamage);
                }
            }
            else if (closestTarget.GetComponent<Health>() != null)
            {
                Health health = closestTarget.GetComponent<Health>();
                if (health != null)
                {
                    AudioManager.instance.PlaySfx("HumanAttack");
                    health.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void StopAttack()
    {
        currentSpeed = moveSpeed; // Resume moving speed when not attacking
        if (anim != null)
        {
            anim.SetBool("isAttacking", false);
            //anim.SetBool("isWalking", true);
        }
    }
}
