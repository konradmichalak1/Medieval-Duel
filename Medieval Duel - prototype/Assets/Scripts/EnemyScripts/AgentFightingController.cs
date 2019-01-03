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
            base.LightAttack();
        }

        public override void HeavyAttack()
        {
            isHeavyAttacking = true;
            base.HeavyAttack();
        }

        public override void Block()
        {
            isBlocking = true;
            base.Block();
        }


    }
}
