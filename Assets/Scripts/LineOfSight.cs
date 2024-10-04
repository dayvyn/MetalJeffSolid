using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public enum GuardState
{
    PATROL,
    SEEN,
    INVESTIGATE,
    NONE
}


public class LineOfSight : MonoBehaviour
{
    PlayerController player;
    Transform playerTarget;
    NavMeshAgent agent;
    public GuardState state = GuardState.PATROL;
    float chaseTimer = 0;
    bool seen;

    // Start is called before the first frame update
    void OnEnable()
    {
        player = FindAnyObjectByType<PlayerController>();
        playerTarget = player.transform;
        agent = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 directionToTarget = (playerTarget.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;
        Ray ray = new Ray(transform.position, playerTarget.position.normalized);
        LayerMask playerLayer = LayerMask.GetMask("Player");
        NavMeshHit hit;
        bool seeable = !agent.Raycast(playerTarget.position, out hit);
        seen = seeable;
        bool playerCrouched = player.Crouched;
        float dot = Vector3.Dot(forwardDirection,directionToTarget);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < 1.3f)
        {
            SceneManager.LoadScene(1);
        }
        if (dot > 0.4f)
        {
            if (seen && Vector3.Distance(playerTarget.transform.position, transform.position) < 10)
            {
                state = GuardState.SEEN;
                chaseTimer = 0;
                if (Vector3.Distance(playerTarget.transform.position, transform.position) < 1.5f)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }
        else if (playerTarget.gameObject.GetComponent<Rigidbody>().velocity.x > 0.3f && seeable && !playerCrouched|| playerTarget.gameObject.GetComponent<Rigidbody>().velocity.z > 0.3f && seeable && !playerCrouched)
        {
            if (state == GuardState.PATROL)
            state = GuardState.INVESTIGATE;
            else if (state == GuardState.INVESTIGATE)
            {
                state = GuardState.NONE;
            }
        }
    }
    private void Update()
    {
        if (state == GuardState.SEEN && !seen)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer > 1f)
            {
                state = GuardState.INVESTIGATE;
            }
        }
    }

}
