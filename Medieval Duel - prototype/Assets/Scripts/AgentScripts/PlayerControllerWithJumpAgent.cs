using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerWithJumpAgent : Agent {

    private PlayerStates state;
    private float walkSpeed = 5f, runSpeed = 7.0f;
    private StatsController stats;
    public float rollSpeed = 6.0f; //roll and rotate during roll speed
    public float rollStaminaCost = 40f;

    public List<GameObject> obstacles = new List<GameObject>();
    public List<GameObject> fences = new List<GameObject>();
    public Vector3 startPostion;
    public Vector3 targetPosition;
    public float obstaclesStartPositionX;
    public float obstaclesStartPositionY;

    public float fencesStartPositionY;
    Rigidbody rBody;
    RayPerception rayPer;
    public Transform Target;

    public bool isGrounded;
    
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

    private float previousDistance = float.MaxValue;

    const float rayDistance = 10f;
    float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
    string[] detectableObjects = new string[] { "Enemy", "Wall", "Fence" };
    public float distanceToTarget = float.MaxValue;
    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
        state = GetComponent<PlayerStates>();
        controller = GetComponent<CharacterController>();
        rayPer = GetComponent<RayPerception>();
        state.isAlive = true;
        startPostion = this.transform.position;
        targetPosition = Target.position;
        var obstacle = obstacles.ToArray();
        stats = GetComponent<StatsController>();
        obstaclesStartPositionX = obstacle[0].transform.position.x;
        obstaclesStartPositionY = obstacle[0].transform.position.y;
        var fence = fences.ToArray();
        fencesStartPositionY = fence[0].transform.position.y;

    }

    public override void AgentReset()
    {
        System.Random rnd = new System.Random();
        Target.position = new Vector3(targetPosition.x + rnd.Next(-4,4), 0.5f, targetPosition.z + rnd.Next(-4, 4));

        this.transform.position = startPostion;
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        stats.currentStamina = 100;

        int which = rnd.Next(0, 4);
        var pom2 = obstacles.ToArray();
        var pom3 = fences.ToArray();


        
        
        foreach (var x in obstacles)
        {            
            int pom = rnd.Next(-4, 4);
            x.transform.position = new Vector3(obstaclesStartPositionX + pom, obstaclesStartPositionY, x.transform.position.z);
        }

        foreach(var y in fences)
        {
            y.transform.position = new Vector3(y.transform.position.x, -2f, y.transform.position.z);
        }
        if(which <=3)
        {
            pom2[which].transform.position = new Vector3(obstaclesStartPositionX, -3f, pom2[which].transform.position.z);
            pom3[which].transform.position = new Vector3(pom3[which].transform.position.x, fencesStartPositionY, pom3[which].transform.position.z);
        }
        
    }

    public override void CollectObservations()
    {
        distanceToTarget = Vector3.Distance(this.transform.position,
                                          Target.position);


        if (this.transform.position.y > 1.5)
            isGrounded = false;
        else
            isGrounded = true;

        float a1 = this.transform.position.x / float.MaxValue;
        float a2 = this.transform.position.z / float.MaxValue;
        float a4 = Target.position.x / float.MaxValue;
        float a5 = Target.position.z / float.MaxValue;
        Vector3 relativePosition = Target.position - this.transform.position;

        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, -0.3f, -0.3f));
        AddVectorObs(relativePosition.x / float.MaxValue);
        AddVectorObs(relativePosition.z / float.MaxValue);
        AddVectorObs(distanceToTarget / float.MaxValue);
        AddVectorObs(previousDistance / float.MaxValue);
        AddVectorObs(a1);
        AddVectorObs(a2);
        AddVectorObs(a4);
        AddVectorObs(a5);
        AddVectorObs(stats.currentStamina / 100);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Wall")
        {
            AddReward(-0.2f);
        }
        
        //if (hit.gameObject.tag == "Ground")
        //    isGrounded = true;
    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        distanceToTarget = Vector3.Distance(this.transform.position,
                                                  Target.position);

        //rayPer.Perceive(rayDistance, rayAngles, detectableObjects, -0.3f, -0.3f, this, vectorAction);

        if (previousDistance > distanceToTarget)
        {           
            AddReward(0.05f);
        }


        if (stats.currentStamina < 25)
            AddReward(-0.05f);
        //if (previousDistance < distanceToTarget)
        //    AddReward(-0.1f);
        
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            AddReward(1.0f);
            Done();
        }

        


        AddReward(-0.005f);
        previousDistance = distanceToTarget;



        // Fell off platform
        //if (distanceToTarget > 10f)
        //{
        //    AddReward(-1.0f);
        //    Done();
        //}

        // Actions, size = 2
        //Vector3 controlSignal = Vector3.zero;
        //controlSignal.x = vectorAction[0];
        //controlSignal.z = vectorAction[1];
        //rBody.AddForce(controlSignal * speed);
        PlayerMovement(vectorAction);
        SetAnimatorValues(vectorAction);

    }




    //void Update () {
    //    if (state.isAlive)
    //    {
    //        PlayerMovement();
    //    }
    //    SetAnimatorValues();
    //}

    private void PlayerMovement(float[] action)
    {
        CheckIfPlayerSprint();
        CheckPlayerDirection(action);
        CheckIfPlayerJump(action);
        MovePlayer();
        RotatePlayer(action);

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
    private void CheckPlayerDirection(float[] action)
    {
        yStore = moveDirection.y;
        moveDirection = (transform.forward * action[1]) + (transform.right * action[0]); //moving on blue and red axis in world space
        moveDirection = moveDirection.normalized * moveSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
        moveDirection.y = yStore; //setting the y value of moveDirection
    }

    private void CheckIfPlayerJump(float[] action)
    {
        //implementation of jump
        if (isGrounded && stats.currentStamina >= 25)
        {
            moveDirection.y = 0f;
            if (action[2] > 0)
            {               
                moveDirection.y = jumpForce;
                stats.WasteStamina(25);
            }
        }
        //else
        //    moveDirection.y = 0;
    }

    private void MovePlayer()
    {


        //if (state.isRolling)
        //{
        //    state.isBlocking = false;
        //    state.isAttacking = false;
        //    state.lightAttack = false;
        //    state.heavyAttack = false;
        //    controller.Move(moveDirection * Time.deltaTime); //moving the player
        //    transform.rotation = Quaternion.Euler(0f, mainCamera.rotation.eulerAngles.y, 0f); //rotating player into camera rotation
        //    Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));
        //    playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rollSpeed * Time.deltaTime);
        //}
        //else
        //{
            moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime); //increasing the gravity value
            controller.Move(moveDirection * Time.deltaTime); //moving the player
        //}
    }

    private void RotatePlayer(float[] action)
    {
        if (action[0] != 0 || action[1] != 0)
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


    private void SetAnimatorValues(float[] action)
    {
        anim.SetBool("isGrounded", controller.isGrounded);
        anim.SetFloat("Speed", (Mathf.Abs(action[1]) + Mathf.Abs(action[0])));
        anim.SetBool("isRunning", state.isRunning);
        anim.SetBool("isWalking", state.isWalking);
        anim.SetBool("isAttacking", state.isAttacking);
        anim.SetBool("isBlocking", state.isBlocking);
        anim.SetBool("isRolling", state.isRolling);
        anim.SetBool("lightAttack",state.lightAttack);
        anim.SetBool("heavyAttack", state.heavyAttack);
        anim.SetBool("isAlive", state.isAlive);
    }



    private void CheckIfPlayerRoll()
    {
        //implementation of roll
        if (controller.isGrounded && state.isWalking)
        {
            if (Input.GetButtonDown("Jump") && stats.currentStamina > 0)
            {
                state.SetRolling();
                moveDirection = (transform.forward * Input.GetAxisRaw("Vertical")) + (transform.right * Input.GetAxisRaw("Horizontal")); //moving on blue and red axis in world space
                moveDirection = moveDirection.normalized * rollSpeed; //calculating the normalized value od moveDirection basing on setted moveSpeed
                StartCoroutine(RollRoutine());
                stats.WasteStamina(rollStaminaCost);
            }
        }
    }
    IEnumerator RollRoutine()
    {
        yield return new WaitForSeconds(1f);
        state.isRolling = false;
    }

}
