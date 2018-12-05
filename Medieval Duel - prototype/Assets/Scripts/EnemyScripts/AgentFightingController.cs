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
        public bool doAction = true;


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

        /// <summary>
        /// Condition for light attack calling
        /// </summary>
        private bool SetLightAttacking()
        {
            return !enemy.GetComponent<States>().isBlocking ? true : false;
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
