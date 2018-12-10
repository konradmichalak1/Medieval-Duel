using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.PlayerScripts
{
    public class PlayerFightingController:FightingController
    {
        protected override void Update()
        {       
            //checks if object is alive
            if (state.isAlive)
            {
                //each frame, executes fighting methods
                Block();
                HeavyAttack();
                LightAttack();
            }
            base.Update();
        }

        public override void LightAttack()
        {
            isLightAttacking = SetLightAttacking();
            base.LightAttack();
        }

        public override void HeavyAttack()
        {
            isHeavyAttacking = SetHeavyAttacking();
            base.HeavyAttack();
        }
        public override void Block()
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
