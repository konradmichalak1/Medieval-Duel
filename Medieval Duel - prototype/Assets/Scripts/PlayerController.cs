using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private PlayerStates state;
    private float walkSpeed = 2.5f, runSpeed = 7.0f;

    public float moveSpeed; //player movement speed
    public float jumpForce; //player jump force
    public CharacterController controller; //reference to player object 
    public Transform mainCamera; //object that rotate instead of player model
    public float rotateSpeed; //pivot rotation speed

    private Vector3 moveDirection; //value that describe in which direction player is going to move
    public float gravityScale; //multiplier of gravity force
    private float yStore; //auxiliary variable that stores current gravity force
    public Animator anim; //reference to animator object

    public GameObject playerModel; //reference to player model
	void Start () {
        state = GetComponent<PlayerStates>();
        controller = GetComponent<CharacterController>();
        state.isAlive = true;
	}
	void Update () {
        if (state.isAlive)
        {
            PlayerMovement();
        }
        SetAnimatorValues();
    }

    private void PlayerMovement()
    {
        CheckIfPlayerSprint();
        CheckPlayerDirection();

        //Moving is allowed when player is not attacking
        if (state.isRunning)
        {
            CheckIfPlayerJump();
        }

        if (!state.isRolling) //make sure player is not rolling
        {
            MovePlayer();
            RotatePlayer();
        }
    }

    private void CheckIfPlayerSprint(){
        if (Input.GetButton("Sprint"))
        {
            state.SetRunning();
            moveSpeed = runSpeed;
        }
        else {
            state.SetWalking();
            moveSpeed = walkSpeed;
        }
    }
    private void CheckPlayerDirection()
    {
        yStore = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //moving on blue and red axis in world space
        moveDirection = moveDirection.normalized * moveSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
        moveDirection.y = yStore; //setting the y value of moveDirection
    }

    private void CheckIfPlayerJump()
    {
        //implementation of jump
        if (controller.isGrounded && state.isRunning)
        { 
            moveDirection.y = 0f;
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpForce;
            }
        }
    }

    private void MovePlayer()
    {
        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime); //increasing the gravity value
        controller.Move(moveDirection * Time.deltaTime); //moving the player
    }

    private void RotatePlayer()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if (moveSpeed > walkSpeed)
            {
                rotateSpeed = runSpeed;
            }
            else
            {
                rotateSpeed = walkSpeed * 2;
            }
            transform.rotation = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f); //rotating player into camera rotation
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            state.SetStaying();
        }
    }


    private void SetAnimatorValues(){
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"))));
        anim.SetBool("isRunning", state.isRunning);
        anim.SetBool("isWalking", state.isWalking);
        anim.SetBool("isAttacking", state.isAttacking);
        anim.SetBool("isBlocking", state.isBlocking);
        anim.SetBool("isRolling", state.isRolling);
        anim.SetBool("lightAttack",state.lightAttack);
        anim.SetBool("heavyAttack", state.heavyAttack);
        anim.SetBool("isAlive", state.isAlive);
    }

}
