using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public bool isRunning; //is character running?
    public bool isWalking; //is character walking?
    public bool isAttacking; //is character attacking?
    public bool isBlocking; //is character using shield to block?

    public void SetRunning(){
            isRunning = true;
            isWalking = false;
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

}


