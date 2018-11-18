using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatesAgent : MonoBehaviour
{
    public bool isRunning; //is character running?
    public bool isWalking; //is character walking?
    public bool isAttacking; //is character attacking?
    public bool isBlocking; //is character using shield to block?
    public bool isRolling; //is character rolling?
    public bool lightAttack; 
    public bool heavyAttack;
    public bool isAlive; //is player alive?

    public void SetPlayerDied()
    {
        isRunning = false;
        isWalking = false;
        isAttacking = false;
        isBlocking = false;
        isRolling = false;
        lightAttack = false;
        heavyAttack = false;
        isAlive = false;
    }

    public void SetRunning(){
        isRunning = true;
        isWalking = false;
        isAttacking = false;
        lightAttack = false;
        heavyAttack = false;
        isBlocking = false;
    }

    public void SetWalking(){
            isWalking = true;
            isRunning = false;
    }

    public void SetStaying(){
        isWalking = false;
        isRunning = false;
    }

    public void SetAttacking()
    {
        isBlocking = false;
        isAttacking = true;
    }

    public void SetBlocking()
    {
        isBlocking = true;
        isAttacking = false;
    }
    public void SetRolling()
    {
        isRolling = true;
        isWalking = false;
        isRunning = false;
        isAttacking = false;
    }

}


