using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RollController : MonoBehaviour {

    /// <summary> Character movement controller </summary>
    protected CharacterController controller;
    /// <summary> Current object states such as: isWalking, isAttacking </summary>
    protected States state;
    /// <summary> Current object stats such us: health, stamina </summary>
    private StatsController stats;
    /// <summary> Value that describe in which direction player is going to move </summary>
    private Vector3 moveDirection;
    /// <summary> Character model </summary>
    public GameObject playerModel;
    /// <summary> Roll speed </summary>
    public float rollSpeed = 6.0f;
    /// <summary> Roll stamina cost </summary>
    public float rollStaminaCost = 40f;
    /// <summary> Roll cooldown [in seconds] </summary>
    public float rollCooldown = 1.0f;
    /// <summary> Describes if inheriting object invokes a function </summary>
    protected bool isRolling;
    void Start () {
        //Assign fields to components of this object
        controller = GetComponent<CharacterController>();
        state = GetComponent<States>();
        stats = GetComponent<StatsController>();
    }
	
	void Update () {
        //Check if player is alive
        if (state.isAlive) 
        {
            Roll();
            MoveCharacter();
        }
	}
    /// <summary>
    /// Makes the object do roll
    /// </summary>
    protected virtual void Roll()
    {
        //implementation of roll
        if (state.isWalking && !state.isImpact && !state.isShieldImpact)
        {
            if (isRolling && stats.currentStamina>0)
            {
                state.SetRolling();
                moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //moving on blue and red axis in world space
                moveDirection = moveDirection.normalized * rollSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
                StartCoroutine(RollRoutine());
                stats.WasteStamina(rollStaminaCost);
            }
        }
    }
    /// <summary>
    /// Shifts object position during roll
    /// </summary>
    private void MoveCharacter()
    {
        if (state.isRolling)
        {
            controller.Move(moveDirection * Time.deltaTime); //moving the player
            //transform.rotation = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f); //rotating player into camera rotation
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rollSpeed * Time.deltaTime);
        }
    }
    /// <summary>
    /// Cooldown roll - prevents rolling earlier than rollCooldown
    /// </summary>
    IEnumerator RollRoutine()
    {
        yield return new WaitForSeconds(rollCooldown);
        state.isRolling = false;
    }
}
