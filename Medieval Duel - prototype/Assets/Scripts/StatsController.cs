﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsController : MonoBehaviour {

    public Slider healthBar;
    public Slider staminaBar;
    public float hp;
    public float currentHp;
    public float stamina;
    public float currentStamina;
    private bool canRegenerateStamina;
    public float staminaCooldown;

	void Start () {
        canRegenerateStamina = false;
        currentHp = hp;
        currentStamina = stamina;
        healthBar.value = CalculateHealth();
	}
	
	void Update () {
        RegenerateStamina();
	}

    public void WasteStamina(float staminaValue)
    {
        StopAllCoroutines();
        canRegenerateStamina = false;
        currentStamina -= staminaValue;
        staminaBar.value = CalculateStamina();
        StartCoroutine(StaminaRoutine());
    }

    public void RegenerateStamina()
    {
        if (currentStamina < stamina && canRegenerateStamina)
        {
            currentStamina += 1;
        }
        staminaBar.value = CalculateStamina();
    }

    IEnumerator StaminaRoutine()
    {
        yield return new WaitForSeconds(staminaCooldown);
        canRegenerateStamina = true;
    }

    private float CalculateStamina()
    {
        return currentStamina / stamina;
    }

    private float CalculateHealth()
    {
        return currentHp / hp;
    }

    public void DealDamage(float damageValue)
    {
        currentHp -= damageValue;
        healthBar.value = CalculateHealth();
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        currentHp = 0;
        Debug.Log("You died");
    }
}
