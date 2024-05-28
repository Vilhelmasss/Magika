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
    private float chaseSpeed = 3.5f;
    private float inIdleMax = 10f;
    private float inIdleCurr = 0f;
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

    // Update is called once per frame
    Vector3 FindClosestNavMeshPosition(Vector3 sourcePosition, float maxSearchDistance)
    {
        NavMeshHit hit;
        // Use NavMesh.SamplePosition to find the nearest point on the navmesh within maxSearchDistance
        if (NavMesh.SamplePosition(sourcePosition, out hit, maxSearchDistance, NavMesh.AllAreas))
        {
            // Return the position of the hit
            return hit.position;
        }
        else
        {
            // If no point is found, return the original source position (or handle it appropriately)
            Debug.LogWarning("No valid NavMesh position found within the given distance.");
            return sourcePosition;
        }
    }
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                StatePatrol();
                break;
            case EnemyState.Idle:
                StateIdle();
                break;

            case EnemyState.Chase:
                agent.speed = chaseSpeed;
                agent.destination = PlayerController.Instance.transform.position;
                break;

            case EnemyState.Dead:
                StateDeath();
                break;

            case EnemyState.Attack:
                break;


            default:
                break;
        }

    }

    private void StatePatrol()
    {
        agent.destination = patrolToPosition;
        float distanceToPatrolPosition = Vector3.Distance(patrolToPosition, gameObject.transform.position);
        if (distanceToPatrolPosition <= 2f)
        {
            ChangeToStateIdle();
        }

    }

    private void StateIdle()
    {
        inIdleCurr -= Time.deltaTime;
        if (inIdleCurr <= 0)
        {
            ChangeToStatePatrol();
        }
    }

    private void StateDeath()
    {

    }

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

    private void ChangeToStateDeath()
    {
        currentState = EnemyState.Dead;
        agent.enabled = false;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        
        capsule.height = 0f;
        capsule.radius = 0f;

        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        animator.SetTrigger("Death");
    }

    private void FindNewMoveToPosition()
    {
        patrolToPosition = FindClosestNavMeshPosition(startingPosition + new Vector3(Random.Range(0f, moveAwayRandom), 0f, Random.Range(0f, moveAwayRandom)), 10f);
    }
}
