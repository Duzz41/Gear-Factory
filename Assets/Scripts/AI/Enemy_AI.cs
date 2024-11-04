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

            // Check if within attack range
            if (Vector2.Distance(transform.position, closestTarget.position) <= attackRange)
            {
                StartAttack();
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

    private void StartAttack()
    {
        currentSpeed = 0; // Stop moving when attacking
        if (anim != null)
        {
            anim.SetBool("isAttacking", true);
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
