using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody playerRB;
    float forwardBack;
    float leftRight;
    Camera mainCam;
    [SerializeField] float moveSpeed;
    public bool Crouched { get { return crouched; } private set { crouched = false; } }
    bool crouched;
    public UnityAction<bool> CrouchedEvent;
    private void OnEnable()
    {
        playerRB = GetComponent<Rigidbody>();
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        leftRight = Input.GetAxis("Horizontal");
        forwardBack = Input.GetAxis("Vertical");

        var camForward = mainCam.transform.forward;
        var camRight = mainCam.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        var playerMoveVector = camForward * forwardBack + camRight * leftRight;

        playerRB.velocity = new Vector3(playerMoveVector.x * moveSpeed, playerRB.velocity.y, playerMoveVector.z * moveSpeed);
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            crouched = true;
            moveSpeed /= 2;
        }
        else if (ctx.canceled)
        {
            crouched = false;
            moveSpeed *= 2;
        }
        CrouchedEvent.Invoke(crouched);
    }
}
