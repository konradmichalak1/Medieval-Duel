using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {

    /// <summary> Current object states such as: isWalking, isAttacking </summary>
    public States state;
    /// <summary> Current object stats such us: health, stamina </summary>
    public StatsController stats;
    /// <summary> Movement speed </summary>
    public float walkSpeed = 2.5f, runSpeed = 7.0f;
    /// <summary> Character current movement speed </summary>
    public float moveSpeed = 3f;
    /// <summary> Character jump force </summary>
    public float jumpForce = 13f;
    /// <summary> Character movement controller </summary>
    public CharacterController controller;
    /// <summary> Camera that rotate instead of player model </summary>
    public Transform mainCamera;
    /// <summary> Pivot rotation speed </summary>
    public float rotateSpeed = 10f;
    /// <summary> Value that describe in which direction player is going to move </summary>
    protected Vector3 moveDirection;
    /// <summary> Multiplier of gravity force </summary>
    public float gravityScale = 4f;
    /// <summary> Auxiliary variable that stores current gravity force </summary>
    private float yStore;
    /// <summary> Reference to animator object </summary>
    public Animator anim;
    /// <summary> Reference to player model </summary>
    public GameObject playerModel; 

	void Start () {
        //Assign fields to components of this object
        state = GetComponent<States>();
        controller = GetComponent<CharacterController>();
        stats = GetComponent<StatsController>();
        state.isAlive = true;
	}
    

    protected void CharacterMovement()
    {
        Sprint();
        CheckCharacterDirection();

        //Moving is allowed when player is not attacking
        if (state.isRunning)
        {
            Jump();
        }

        if (!state.isRolling) //make sure player is not rolling
        {
            MoveCharacter();
            RotateCharacter();
        }
    }

    public void CharacterMovement(float[] action)
    {
        Sprint();
        CheckCharacterDirection(action);
        Jump(action);
        MoveCharacter();
        RotateCharacter(action);
    }

    /// <summary>  </summary>
    protected void Sprint(){
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
    private void CheckCharacterDirection()
    {
        yStore = moveDirection.y;
        moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //moving on blue and red axis in world space
        moveDirection = moveDirection.normalized * moveSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
        moveDirection.y = yStore; //setting the y value of moveDirection
    }
    private void CheckCharacterDirection(float[] action)
    {
        yStore = moveDirection.y;
        moveDirection = (transform.forward * action[1]) + (transform.right * action[0]); //moving on blue and red axis in world space
        moveDirection = moveDirection.normalized * moveSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
        moveDirection.y = yStore; //setting the y value of moveDirection
    }

    protected void Jump()
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
    private void Jump(float[] action)
    {
        if (action.Length == 3)
        {
            //implementation of jump
            if (controller.isGrounded && stats.currentStamina >= 25)
            {
                moveDirection.y = 0f;
                if (action[2] > 0)
                {
                    moveDirection.y = jumpForce;
                    stats.WasteStamina(25);
                }
            }
        }
    }
    private void MoveCharacter()
    {
        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime); //increasing the gravity value
        controller.Move(moveDirection * Time.deltaTime); //moving the player
    }

    private void RotateCharacter()
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
    private void RotateCharacter(float[] action)
    {
        if (action[0] != 0 || action[1] != 0)
        {

            rotateSpeed = walkSpeed * 2;

            transform.rotation = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f); //rotating player into camera rotation
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
        else
        {
            state.SetStaying();
        }
    }

    protected void SetAnimatorValues(){
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetBool("isRunning", state.isRunning);
        anim.SetBool("isWalking", state.isWalking);
        anim.SetBool("isAttacking", state.isAttacking);
        anim.SetBool("isBlocking", state.isBlocking);
        anim.SetBool("isRolling", state.isRolling);
        anim.SetBool("lightAttack",state.lightAttack);
        anim.SetBool("heavyAttack", state.heavyAttack);
        anim.SetBool("isAlive", state.isAlive);
        anim.SetBool("isImpact", state.isImpact);
        anim.SetBool("isShieldImpact", state.isShieldImpact);
    }
    public void SetAnimatorValues(float[] action)
    {
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(action[1]) + Mathf.Abs(action[0])));
        anim.SetBool("isRunning", state.isRunning);
        anim.SetBool("isWalking", state.isWalking);
        anim.SetBool("isAttacking", state.isAttacking);
        anim.SetBool("isBlocking", state.isBlocking);
        anim.SetBool("isRolling", state.isRolling);
        anim.SetBool("lightAttack", state.lightAttack);
        anim.SetBool("heavyAttack", state.heavyAttack);
        anim.SetBool("isAlive", state.isAlive);
        anim.SetBool("isImpact", state.isImpact);
        anim.SetBool("isShieldImpact", state.isShieldImpact);
    }

}
