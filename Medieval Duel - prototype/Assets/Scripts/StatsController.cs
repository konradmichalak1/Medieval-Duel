using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : MonoBehaviour {
    /// <summary> Current object states such as: isWalking, isAttacking </summary>
    private States state;
    /// <summary> Health bar from user interface - shows current HP </summary>
    public Slider healthBar;
    /// <summary> Stamina bar from user interface - shows current SP </summary>
    public Slider staminaBar;
    /// <summary> Maximum health points value </summary>
    public float maxHealthPoints = 100;
     /// <summary> Maximum stamina points value </summary>
    public float maxStaminaPoints = 100;
    /// <summary> Current hp value </summary>
    public float currentHp;
    /// <summary> Current sp value </summary>
    public float currentStamina;
    /// <summary> If true, stamina is regenerating. </summary>
    private bool canRegenerateStamina;
    /// <summary> Time needed, to start regenerate stamina. </summary>
    public float staminaCooldown = 1.0f;

	void Start () {
        //Initialize values
        state = GetComponent<States>();
        canRegenerateStamina = false;
        currentHp = maxHealthPoints;
        currentStamina = maxStaminaPoints;
        healthBar.value = CalculateHealth();
	}
	
	void Update () {
        //Regenerate stamina only if object is alive
        if (state.isAlive)
        {
            RegenerateStamina();
        }
	}

    /// <summary>Decreases stamina points by the value of the argument </summary>
    /// <param name="staminaValue">How many stamina points subtract?</param>
    public void WasteStamina(float staminaValue)
    {
        StopAllCoroutines();
        canRegenerateStamina = false;
        currentStamina -= staminaValue;
        staminaBar.value = CalculateStamina();
        StartCoroutine(StaminaRoutine());
    }

    /// <summary>Regenerate stamina points </summary>
    public void RegenerateStamina()
    {
        if (currentStamina < maxStaminaPoints && canRegenerateStamina)
        {
            currentStamina += 5;
        }
        if (currentStamina > maxStaminaPoints) currentStamina = maxStaminaPoints;
        staminaBar.value = CalculateStamina();
    }
    /// <summary> Cooldown stamina regenerate - prevents regenerating earlier than staminaCooldown </summary>
    IEnumerator StaminaRoutine()
    {
        yield return new WaitForSeconds(staminaCooldown);
        canRegenerateStamina = true;
    }

    /// <summary> Calculates percentage value of current stamina points </summary>
    private float CalculateStamina()
    {
        return currentStamina / maxStaminaPoints;
    }

    /// <summary> Calculates percentage value of current health points </summary>
    private float CalculateHealth()
    {
        return currentHp / maxHealthPoints;
    }

    /// <summary> Decreases health points of this object </summary>
    public void DealDamage(float damageValue)
    {
        currentHp -= damageValue;
        healthBar.value = CalculateHealth();
        if (currentHp <= 0)
        {
            Die();
        }
    }
    /// <summary> Set object state to 'Dead' </summary>
    private void Die()
    {
        currentHp = 0;
        state.SetPlayerDied();
    }
}
