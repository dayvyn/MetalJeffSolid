using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuardMichael : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] target;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed = 15;
    NavMeshPath navPath;
    Queue<Vector3> remainingPoints;
    Vector3 currentTargetPoint;
    bool waiting = false;
    float waitTimer = 0;
    float chaseTimer = 0;
    LineOfSight guardLOS;
    GuardState state;
    PlayerController playerTarget;
    Vector3 startLeft;
    Vector3 startRight;
    bool gotStart = false;
    bool lookLeft = false;
    bool lookRight = false;
    bool gotLastPlayerPos = false;
    [SerializeField] float investigateSpeed;
    Vector3 startForward;
    Vector3 new_forward;
    Vector3 lastPlayerPos;

    // Update is called once per frame
    void Update()
    {
        state = guardLOS.state;
        switch (state)
        {
            case GuardState.PATROL:
                UpdatePatrol();
                break;
            case GuardState.SEEN:
                UpdateSeen();
                break;
            case GuardState.INVESTIGATE:
                UpdateInvestigate();
                break;
            case GuardState.NONE:
                UpdateNone();
                break;
        }
    }

    private void Start()
    {
        navPath = new NavMeshPath();
        remainingPoints = new Queue<Vector3>();
        guardLOS = GetComponentInChildren<LineOfSight>();
        QueuePoints(true);
        currentTargetPoint = remainingPoints.Dequeue();
        playerTarget = FindAnyObjectByType<PlayerController>();
    }

    void QueuePoints(bool forwardOrBackward)
    {
        if (forwardOrBackward)
        for (int k = 0; k < target.Length; k++)
        {
            remainingPoints.Enqueue(target[k].position);
        }
        else
        for (int i = target.Length - 1; i >= 0; i--)
        {
            remainingPoints.Enqueue(target[i].position);
        }
    }
    void UpdatePatrol()
    {
        agent.isStopped = false;
        gotLastPlayerPos = false;
        gotStart = false;
        float distToPoint = Vector3.Distance(currentTargetPoint, transform.position);
        if (distToPoint < 0.6f)
        {
            if (remainingPoints.Count > 0)
            {
                currentTargetPoint = remainingPoints.Dequeue();
                speed = 3;
            }
            else if (remainingPoints.Count == 0)
            {
                speed = 0;
                waitTimer += Time.deltaTime;
                if (Mathf.Ceil(waitTimer) > 2f)
                {
                    waitTimer = 0;
                    QueuePoints(waiting);
                    waiting = !waiting;
                }
            }
        }
        if (currentTargetPoint != null)
        {
            agent.SetDestination(currentTargetPoint);
        }

    }

    void UpdateSeen()
    {
        agent.isStopped = false;
        gotLastPlayerPos = false;
        gotStart = false;
        agent.SetDestination(playerTarget.transform.position);
    }

    private void UpdateInvestigate()
    {
        if (!gotLastPlayerPos)
        {
            lastPlayerPos = playerTarget.transform.position;
            agent.SetDestination(lastPlayerPos);
            gotLastPlayerPos = true;
        }
        if (Vector3.Distance(transform.position, lastPlayerPos) < 1)
        {
            agent.isStopped = true;
            if (!gotStart)
            {
                if (!lookLeft && !lookRight)
                {
                    startLeft = -transform.right;
                    startRight = transform.right;
                    startForward = transform.forward;
                    gotStart = true;
                }
            }
            else if (!lookLeft)
            {
                Quaternion rotTarget = Quaternion.LookRotation(startLeft);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, investigateSpeed * Time.deltaTime);
                if (Mathf.Abs(Mathf.Abs(rotTarget.y) - Mathf.Abs(transform.rotation.y)) < 0.02f)
                {
                    lookLeft = true;
                }
            }
            else if (!lookRight)
            {
                Quaternion rotTarget = Quaternion.LookRotation(startRight);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, investigateSpeed * Time.deltaTime);
                if (Mathf.Abs(Mathf.Abs(rotTarget.y) - Mathf.Abs(transform.rotation.y)) < 0.02f)
                {
                    lookRight = true;
                }
            }
            else if (lookLeft && lookRight)
            {
                Quaternion rotTarget = Quaternion.LookRotation(startForward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, investigateSpeed * Time.deltaTime);
                if (Mathf.Abs(Mathf.Abs(rotTarget.y) - Mathf.Abs(transform.rotation.y)) < 0.02f)
                {
                    lookRight = false;
                    lookLeft = false;
                    gotStart = false;
                    guardLOS.state = GuardState.PATROL;
                    gotLastPlayerPos = false;
                }
            }
        }

     }
    void UpdateNone()
    {
        gotLastPlayerPos = false;
        agent.isStopped = false;
        guardLOS.state = GuardState.INVESTIGATE;
    }
}
