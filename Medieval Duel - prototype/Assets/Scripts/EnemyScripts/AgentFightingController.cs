using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.EnemyScripts
{
    public class AgentFightingController : FightingController
    {
        /// <summary> Reference to enemy object, for agent enemy is Player or other agent. </summary>
        public GameObject enemy;

        protected override void Update()
        {  
            base.Update();
        }

        public override void LightAttack()
        {
            isLightAttacking = true;
            //isLightAttacking = SetLightAttacking();
            base.LightAttack();
        }

        public override void HeavyAttack()
        {
            isHeavyAttacking = true;
            //isHeavyAttacking = SetHeavyAttacking();
            base.HeavyAttack();
        }

        public override void Block()
        {
            isBlocking = true;
            //isBlocking = SetBlocking();
            base.Block();
        }

        /// <summary>
        /// Condition for light attack calling
        /// </summary>
        private bool SetLightAttacking()
        {
            return true;
        }
        /// <summary>
        /// Condition for heavy attack calling
        /// </summary>
        private bool SetHeavyAttacking()
        {
            return enemy.GetComponent<States>().isBlocking ? true : false;
        }
        /// <summary>
        /// Condition for shield block calling
        /// </summary>
        private bool SetBlocking()
        {
            return enemy.GetComponent<States>().lightAttack ? true : false;
        }

    }
}
