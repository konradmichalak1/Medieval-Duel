using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    /// <summary>
    /// is object running?
    /// </summary>
    public bool isRunning;
    /// <summary>
    /// is object walking?
    /// </summary>
    public bool isWalking;
    /// <summary>
    /// is object attacking?
    /// </summary>
    public bool isAttacking;
    /// <summary>
    /// is object blocking?
    /// </summary>
    public bool isBlocking;
    /// <summary>
    /// is object rolling?
    /// </summary>
    public bool isRolling;
    /// <summary>
    /// is object currently light attacking?
    /// </summary>
    public bool lightAttack; 
    /// <summary>
    /// is object currently heavy attacking?
    /// </summary>
    public bool heavyAttack;
    /// <summary>
    /// is object alive?
    /// </summary>
    public bool isAlive;
    /// <summary>
    /// If object has been hitted, unables any action and run 'Impact' animation.
    /// </summary>
    public bool isImpact;
    /// <summary>
    /// If object has been hitted but already blocking with shield, unables any action and run 'ShieldImpact' animation.
    /// </summary>
    public bool isShieldImpact;

    /// <summary>
    /// Set all states to false - object is dead
    /// </summary>
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
    /// <summary>
    /// Set appropriate states to make object running
    /// </summary>
    public void SetRunning(){
        isRunning = true;
        isWalking = false;
        isAttacking = false;
        lightAttack = false;
        heavyAttack = false;
        isBlocking = false;
    }

    /// <summary>
    /// Set appropriate states to make object walking
    /// </summary>
    public void SetWalking(){
            isWalking = true;
            isRunning = false;
    }
    /// <summary>
    /// Set appropriate states to make object staying
    /// </summary>
    public void SetStaying(){
        isWalking = false;
        isRunning = false;
    }
    /// <summary>
    /// Set appropriate states to make object attacking
    /// </summary>
    public void SetAttacking()
    {
        isBlocking = false;
        isAttacking = true;
    }
    /// <summary>
    /// Set appropriate states to make object blocking
    /// </summary>
    public void SetBlocking()
    {
        isBlocking = true;
        isAttacking = false;
    }
    /// <summary>
    /// Set appropriate states to make object rolling
    /// </summary>
    public void SetRolling()
    {
        isRolling = true;
        isWalking = false;
        isRunning = false;
        isAttacking = false;
        isBlocking = false;
        lightAttack = false;
        heavyAttack = false;
    }

    public void SetImpact()
    {
        isImpact = true;
        isWalking = false;
        isAttacking = false;
        lightAttack = false;
        heavyAttack = false;
    }
    public void SetShieldImpact()
    {
        isShieldImpact = true;
        isRunning = false;
        isAttacking = false;
        isRolling = false;
        lightAttack = false;
        heavyAttack = false;
    }
}


