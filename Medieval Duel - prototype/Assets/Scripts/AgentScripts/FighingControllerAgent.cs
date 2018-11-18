using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FighingControllerAgent : MonoBehaviour {

    private PlayerStates state;
    private StatsController stats;

    public Collider[] attackHitboxes;
    public Animator anim; //reference to animator object
    public bool hitted; //is enemy hitted?

    public float lightAttackCooldown = 0.8f;
    public float heavyAttackCooldown = 1.3f;
    public GameObject enemy;
    public float damage;
    public float lightAttackStaminaCost;
    public float heavyAttackStaminaCost;
    void Start () {
        state = GetComponent<PlayerStates>();
        stats = GetComponent<StatsController>();
        state.isAttacking = false;
        hitted = false;
    }
	
    //Each frame, check if player attacking
	void Update () {
        enemy.gameObject.GetComponent<Renderer>().material.color = Color.blue;

        //first, let's check if player is alive
        if (state.isAlive)
        {
            LightAttack();
            HeavyAttack();
            Block();
        }
	}

    private void Block()
    {
        if(Input.GetMouseButton(1) && !state.isRunning && !state.isAttacking)
        {
            state.SetBlocking();
        }
        else
        {
            state.isBlocking = false;
        }
    }


    //Check if player press mouse button
    private void LightAttack()
    {
        //Attack allowed only when player is not running 
        if (CanAttack() && !Input.GetButton("HeavyAttack"))
        {
            damage = 10f; //Set light attack damage
            state.SetAttacking();
            state.lightAttack = true;
            StartCoroutine(LightAttackRoutine());
            stats.WasteStamina(lightAttackStaminaCost);
        }

        if (state.isAttacking)
        {
            LaunchAttack(attackHitboxes[0]);
        };
    }

    private void HeavyAttack()
    {
        if(CanAttack() && Input.GetButton("HeavyAttack"))
        {
            damage = 20f; //Set heavy attack damage
            state.SetAttacking();
            state.heavyAttack = true;
            StartCoroutine(HeavyAttackRoutine());
            stats.WasteStamina(heavyAttackStaminaCost);
        }
        if(state.isAttacking)
        {
            LaunchAttack(attackHitboxes[0]);
        }
    }

    //Check if player can attack
    private bool CanAttack()
    {
        if(Input.GetMouseButton(0) && !state.isRunning && !state.isAttacking && !state.isRolling && stats.currentStamina > 0)
        {
            return true;
        }
        return false;
    }
    //Light attack cooldown
    IEnumerator LightAttackRoutine()
    {
        yield return new WaitForSeconds(lightAttackCooldown);
        state.isAttacking = false;
        state.lightAttack = false;
        hitted = false;
    }

    //Heavy attack cooldown
    IEnumerator HeavyAttackRoutine()
    {
        yield return new WaitForSeconds(heavyAttackCooldown);
        state.isAttacking = false;
        state.heavyAttack = false;
        hitted = false;
    }

    //Search for collision
    private void LaunchAttack(Collider col)
    {
        Collider[] cols = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation, LayerMask.GetMask("Hitbox"));
        foreach (Collider c in cols)
        {
            if (c.name != name && !hitted)
            {
                Debug.Log(c.name);
                hitted = true;
                enemy.gameObject.GetComponent<Renderer>().material.color = Color.red;
                stats.DealDamage(damage);
            }
        }
    }


}
