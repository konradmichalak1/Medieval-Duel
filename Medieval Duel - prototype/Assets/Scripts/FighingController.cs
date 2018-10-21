using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FighingController : MonoBehaviour {

    private PlayerStates state;
    private StatsController stats;

    public Collider[] attackHitboxes;
    public Animator anim; //reference to animator object
    public bool hitted; //is enemy hitted?

    public float lightAttackCooldown = 0.8f;
    public GameObject enemy;
    public float damage;
    public float lightAttackStaminaCost;
    void Start () {
        state = GetComponent<PlayerStates>();
        stats = GetComponent<StatsController>();
        state.isAttacking = false;
        hitted = false;
    }
	
    //Each frame, check if player attacking
	void Update () {
        enemy.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        Attack();
        Block();
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
    private void Attack()
    {
        //Attack allowed only when player is not running 
        if (Input.GetMouseButton(0) && !state.isAttacking && !state.isRunning && stats.currentStamina>0)
        {
            state.SetAttacking();
            StartCoroutine(AttackRoutine());
            stats.WasteStamina(lightAttackStaminaCost);
        }

        if (state.isAttacking)
        {
            LaunchAttack(attackHitboxes[0]);
        };
    }

    //Attack cooldown
    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(lightAttackCooldown);
        state.isAttacking = false;
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
