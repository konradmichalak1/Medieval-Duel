using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollControllerAgent : MonoBehaviour {

    public CharacterController controller; //reference to player object 
    private States state;
    private StatsController stats;
    private Vector3 moveDirection;
    public Transform mainCamera; //object that rotate instead of player model
    public GameObject playerModel; //reference to player model
    public float rollSpeed = 6.0f; //roll and rotate during roll speed
    public float rollStaminaCost = 40f;

    void Start () {
        controller = GetComponent<CharacterController>();
        state = GetComponent<States>();
        stats = GetComponent<StatsController>();
    }
	
	void Update () {
        //First check if player is alive
        if (state.isAlive) 
        {
            CheckIfPlayerRoll();
            MovePlayer();
        }
	}
    private void CheckIfPlayerRoll()
    {
        //implementation of roll
        if (controller.isGrounded && state.isWalking)
        {
            if (Input.GetButtonDown("Jump") && stats.currentStamina>0)
            {
                state.SetRolling();
                moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //moving on blue and red axis in world space
                moveDirection = moveDirection.normalized * rollSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
                StartCoroutine(RollRoutine());
                stats.WasteStamina(rollStaminaCost);
            }
        }
    }
    private void MovePlayer()
    {
        if (state.isRolling)
        {
            state.isBlocking = false;
            state.isAttacking = false;
            state.lightAttack = false;
            state.heavyAttack = false;
            controller.Move(moveDirection * Time.deltaTime); //moving the player
            transform.rotation = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f); //rotating player into camera rotation
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rollSpeed * Time.deltaTime);
        }
    }
    IEnumerator RollRoutine()
    {
        yield return new WaitForSeconds(1f);
        state.isRolling = false;
    }
}
