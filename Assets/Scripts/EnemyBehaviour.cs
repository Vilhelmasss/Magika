using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack, Dead };
    EnemyState currentState;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Vector3 startingPosition;
    private Vector3 patrolToPosition;
    private Animator animator;
    private float moveAwayRandom = 30f;
    // Start is called before the first frame update
    private float patrolSpeed = 2f;
    private float chaseSpeed = 6f;
    private float inIdleMax = 10f;
    private float inIdleCurr = 0f;
    private float inAttackMax = 3f;
    private float inAttackCurr = 0f;
    // private float 
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = gameObject.GetComponent<Animator>();
        currentState = EnemyState.Patrol;
        startingPosition = this.gameObject.transform.position;
        patrolToPosition = startingPosition;
        Vector3 closestNavMeshPosition = FindClosestNavMeshPosition(startingPosition, 10f);
        Debug.Log(closestNavMeshPosition);
        agent = GetComponent<NavMeshAgent>();
        ChangeToStatePatrol();
        FindNewMoveToPosition();
    }

    Vector3 FindClosestNavMeshPosition(Vector3 sourcePosition, float maxSearchDistance)
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(sourcePosition, out hit, maxSearchDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            Debug.LogWarning("No valid NavMesh position found within the given distance.");
            return sourcePosition;
        }
    }
    void Update()
    {
        Debug.Log($"Current State: {currentState}");
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (CheckHealth())
                    return;
                StatePatrol();
                break;

            case EnemyState.Idle:
                if (CheckHealth())
                    return;
                StateIdle();
                break;

            case EnemyState.Chase:
                if (CheckHealth())
                    return;
                StateChase();
                break;

            case EnemyState.Attack:
                if (CheckHealth())
                    return;
                StateAttack();
                break;

            case EnemyState.Dead:
                StateDeath();
                break;

            default:
                break;
        }

    }

// ------------------ STATE ACTIONS START ------------------

    private void StatePatrol()
    {
        CheckHealth();
        agent.destination = patrolToPosition;

        if (DistanceToPlayer() <= 15f)
        {
            Debug.Log("Should be chasing");
            ChangeToStateChase();
        }

        float distanceToPatrolPosition = Vector3.Distance(patrolToPosition, gameObject.transform.position);
        if (distanceToPatrolPosition <= 2f)
        {
            ChangeToStateIdle();
        }

    }

    private void StateIdle()
    {

        if (DistanceToPlayer() <= 15f)
        {
            Debug.Log("Should be chasing");
            ChangeToStateChase();
        }

        inIdleCurr -= Time.deltaTime;
        if (inIdleCurr <= 0)
        {
            ChangeToStatePatrol();
        }
    }

    private void StateDeath()
    {

    }

    private void StateChase()
    {
        CheckHealth();

        agent.destination = PlayerController.Instance.transform.position;

        // float distance = Vector3.Distance(gameObject.transform.position, PlayerController.Instance.transform.position);
        if (DistanceToPlayer() <= 2f)
        {
            ChangeToStateAttack();
        }
        if (DistanceToPlayer() >= 16f)
        {
            ChangeToStatePatrol();
        }
    }

    private void StateAttack()
    {
        CheckHealth();

        inAttackCurr -= Time.deltaTime;

        if (inAttackCurr <= 0)
        {
            // float distance = Vector3.Distance(gameObject.transform.position, PlayerController.Instance.transform.position);
            float distance = DistanceToPlayer();
            if (distance <= 4f)
            {
                PlayerController.Instance.GetComponent<HealthComponent>().TakeDamage(10f);
            }
            if (distance <= 2f)
            {
                ChangeToStateAttack();
                return;
            }
            ChangeToStateChase();
        }
    }

// ------------------ STATE ACTIONS END ------------------


// ------------------ CHANGE STATE START ------------------
    private void ChangeToStatePatrol()
    {
        FindNewMoveToPosition();
        currentState = EnemyState.Patrol;
        animator.SetTrigger("Walking");
        agent.speed = patrolSpeed;

    }

    private void ChangeToStateIdle()
    {
        agent.speed = 0f;
        inIdleCurr = inIdleMax;
        currentState = EnemyState.Idle;
        animator.SetTrigger("Taunt");
    }

    private void ChangeToStateChase()
    {
        agent.speed = chaseSpeed;
        animator.SetTrigger("Chase");
        currentState = EnemyState.Chase;
    }

    private void ChangeToStateAttack()
    {
        inAttackCurr = inAttackMax;
        agent.speed = 0.1f;
        animator.SetTrigger("Attack");
        currentState = EnemyState.Attack;
    }

    private void ChangeToStateDeath()
    {
        currentState = EnemyState.Dead;
        agent.enabled = false;
        // rb.isKinematic = false;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.enabled = false;


        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        animator.SetTrigger("Death");
    }

// ------------------ CHANGE STATE END ------------------


// ------------------ UTILITY START ------------------
    private bool CheckHealth()
    {
        if (gameObject.GetComponent<HealthComponent>() is null)
            return false;
        if (gameObject.GetComponent<HealthComponent>().GetHealth() <= 0)
        {
            ChangeToStateDeath();
            return true;
        }
        return false;
    }

    private void FindNewMoveToPosition()
    {
        patrolToPosition = FindClosestNavMeshPosition(startingPosition + new Vector3(Random.Range(0f, moveAwayRandom), 0f, Random.Range(0f, moveAwayRandom)), 10f);
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(gameObject.transform.position, PlayerController.Instance.transform.position);
    }

// ------------------ UTILITY END ------------------
}
