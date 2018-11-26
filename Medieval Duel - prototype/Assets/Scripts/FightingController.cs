using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all fields and methods that are necessary to implement the fight
/// </summary>
public abstract class FightingController : MonoBehaviour {

    /// <summary> Current object states such as: isWalking, isAttacking </summary>
    protected States state;
    /// <summary> Current object stats such us: health, stamina </summary>
    private StatsController stats;
    /// <summary> Array of box colliders that belongs to current object.
    /// Box colliders search collision with all object that belongs to 'Hitbox' layer.
    /// It ignore itself. </summary>
    public Collider[] attackHitboxes;
    /// <summary> It is set to true, if enemy is hitted. </summary>
    public bool hitted;
    /// <summary> Value that stores object damage, depending on the type of attack </summary>
    private float actualDamage;
    /// <summary> Light attack damage </summary>
    public float lightAttackDamage = 10f;
    /// <summary> Heavy attack damage </summary>
    public float heavyAttackDamage = 20f;
    /// <summary> Light attack stamina cost </summary>
    public float lightAttackStaminaCost = 25f;
    /// <summary> Heavy attack stamina cost </summary>
    public float heavyAttackStaminaCost = 40f;
    /// <summary> Light attack cooldown [in seconds] </summary>
    public float lightAttackCooldown = 0.6f;
    /// <summary> Heavy attack cooldown [in seconds] </summary>
    public float heavyAttackCooldown = 1.4f;
    /// <summary> Describes if inheriting object invokes a function </summary>
    protected bool isBlocking, isLightAttacking, isHeavyAttacking;

    void Start () {
        //Assign fields to components of this object
        state = GetComponent<States>();
        stats = GetComponent<StatsController>();
        state.isAttacking = false;
        hitted = false;
    }
	
    //Code that is executed each frame
	protected virtual void Update () {
        //checks if object is alive
        if (state.isAlive)
        {
            //each frame, executes fighting methods
            LightAttack();
            HeavyAttack();
            Block();
        }
    }

    /// <summary> Makes the object do block with shield </summary>
    protected virtual void Block()
    {
        // if inheriting object invokes block, doesn't running and doesn't attacking, then set its state to blocking
        if (!state.isImpact && stats.currentStamina > 0 && isBlocking && !state.isRunning && !state.isAttacking && !state.isRolling)
        {
            state.SetBlocking();
        }
        else
        {
            state.isBlocking = false;
        }
    }
    /// <summary> Makes the object do light attack </summary>
    protected virtual void LightAttack()
    {
        //if inheriting object invokes light attack, and can attack
        if (isLightAttacking && CanAttack())
        {
            actualDamage = lightAttackDamage ; //set light attack damage
            state.SetAttacking();
            state.lightAttack = true;
            hitted = true; //makes that first 1/8 animation does not deal damage
            StartCoroutine(LightAttackRoutine()); //run cooldown
            stats.WasteStamina(lightAttackStaminaCost);
        }

        if (state.isAttacking)
        {
            LaunchAttack(attackHitboxes[0]);
        }
    }
    /// <summary> Makes the object do heavy attack </summary>
    protected virtual void HeavyAttack()
    {
        //if inheriting object invokes light attack, and can attack
        if (isHeavyAttacking && CanAttack())
        {
            actualDamage = heavyAttackDamage; //set heavy attack damage
            state.SetAttacking();
            state.heavyAttack = true;
            hitted = true; //makes that first half animation does not deal damage
            StartCoroutine(HeavyAttackRoutine()); //run cooldown
            stats.WasteStamina(heavyAttackStaminaCost);
        }

        if(state.isAttacking)
        {
            LaunchAttack(attackHitboxes[0]);
        }
    }

    /// <summary> Checks if object can attack. </summary>
    /// <returns>Returns true if object can attack, else returns false</returns>
    private bool CanAttack()
    {
        return !state.isShieldImpact && !state.isImpact && !state.isRunning && !state.isAttacking && !state.isRolling && stats.currentStamina > 0 ? true : false;
    }
    /// <summary> Cooldown light attack - prevents attacking earlier than lightAttackCooldown </summary>
    /// <returns></returns>
    IEnumerator LightAttackRoutine()
    {
        yield return new WaitForSeconds(lightAttackCooldown/4);
        hitted = false; //after 1/8 of animation, damage can be dealt to an enemy
        yield return new WaitForSeconds(lightAttackCooldown*0.75f);
        state.isAttacking = false;
        state.lightAttack = false;
        hitted = false;
    }

    /// <summary> Cooldown heavy attack - prevents attacking earlier than heavyAttackCooldown </summary>
    /// <returns></returns>
    IEnumerator HeavyAttackRoutine()
    {
        yield return new WaitForSeconds(heavyAttackCooldown/2);
        hitted = false; //after half of animation, damage can be dealt to an enemy
        yield return new WaitForSeconds(heavyAttackCooldown/2);
        state.isAttacking = false;
        state.heavyAttack = false;
        hitted = false;
    }

    /// <summary> Search for collision with all object that belongs to 'Hitbox' layer. </summary>
    /// <param name="col">Array of box colliders that belongs to current object</param>
    private void LaunchAttack(Collider col)
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Hitbox"));
        foreach (Collider c in cols)
        {
            //The following condition checks, if object is not trying to hit itselfs and if enemy has been hitted during this attack.
            if (c.name != name && !hitted)
            {
                //Set flag, that object has been hitted and can't be hitted again during this attack
                hitted = true;

                //Deal damage to hitted object based on following conditions.
                if (c.GetComponent<States>().isBlocking)
                {
                    if (state.heavyAttack)
                    {
                        c.GetComponent<States>().SetImpact();
                        StartCoroutine(c.GetComponent<FightingController>().ImpactRoutine());
                        c.GetComponent<StatsController>().DealDamage(actualDamage);
                    }
                    //If target is blocking, and this object launch light attack, set target field 'isShieldImpact' as true.
                    else if(state.lightAttack)
                    {
                        c.GetComponent<States>().SetShieldImpact();
                        c.GetComponent<StatsController>().WasteStamina(5f);
                        StartCoroutine(c.GetComponent<FightingController>().ShieldImpactRoutine());
                    }
                }
                else if(c.GetComponent<States>().isRolling)
                {
                    if(state.lightAttack) c.GetComponent<StatsController>().DealDamage(actualDamage);
                }
                else if(!c.GetComponent<States>().isImpact)
                {
                    c.GetComponent<StatsController>().DealDamage(actualDamage);
                    c.GetComponent<States>().SetImpact();
                    StartCoroutine(c.GetComponent<FightingController>().ImpactRoutine());
                }

            }
        }
    }

    /// <summary> Cooldown shield impact - prevent doing anything while impact </summary>
    /// <returns></returns>
    IEnumerator ShieldImpactRoutine()
    {
        yield return new WaitForSeconds(0.15f);
        state.isShieldImpact = false;
    }

    /// <summary>
    /// Cooldown impact - prevent doing anything while impact
    /// </summary>
    /// <returns></returns>
    IEnumerator ImpactRoutine()
    {
        yield return new WaitForSeconds(0.22f);
        state.isImpact = false;
    }

}
