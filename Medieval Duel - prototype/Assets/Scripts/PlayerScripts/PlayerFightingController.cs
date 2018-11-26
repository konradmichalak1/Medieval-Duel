using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public class PlayerFightingController:FightingController
    {

        protected override void LightAttack()
        {
            isLightAttacking = SetLightAttacking();
            base.LightAttack();
        }

        protected override void HeavyAttack()
        {
            isHeavyAttacking = SetHeavyAttacking();
            base.HeavyAttack();
        }
        protected override void Block()
        {
            isBlocking = SetBlocking();
            base.Block();
        }

        /// <summary> Checks user input </summary>
        private bool SetLightAttacking()
        {
            return Input.GetMouseButton(0) && !Input.GetButton("HeavyAttack") ? true : false;
        }

        private bool SetHeavyAttacking()
        {
            return Input.GetMouseButton(0) && Input.GetButton("HeavyAttack") ? true : false;
        }

        private bool SetBlocking()
        {
            return Input.GetMouseButton(1) ? true : false;
        }


    }
}
