using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.EnemyScripts
{
    public class AgentFightingController : FightingController
    {
        protected override void LightAttack()
        {
            //Należy odkomentować po zaimplementowaniu warunków ataku i bloku dla agenta
            //isLightAttacking = true;
            //isLightAttacking = SetLightAttacking();
            base.LightAttack();
        }

        protected override void HeavyAttack()
        {
            //isHeavyAttacking = SetHeavyAttacking();
            base.HeavyAttack();
        }
        protected override void Block()
        {
            //isBlocking = SetBlocking();
            isBlocking = true; //tymczasowo by testować blokowanie
            base.Block();
        }

        /// <summary>
        /// DO ZAIMPLEMENTOWANIA - WARUNKI WYWOŁANIA ATAKÓW I BLOKOWANIA DLA AGENTÓW
        /// </summary>
        private bool SetLightAttacking()
        {
            return true;
        }
        /// <summary>
        /// DO ZAIMPLEMENTOWANIA - WARUNKI WYWOŁANIA ATAKÓW I BLOKOWANIA DLA AGENTÓW
        /// </summary>
        private bool SetHeavyAttacking()
        {
            return true;
        }
        /// <summary>
        /// DO ZAIMPLEMENTOWANIA - WARUNKI WYWOŁANIA ATAKÓW I BLOKOWANIA DLA AGENTÓW
        /// </summary>
        private bool SetBlocking()
        {
            return true;
        }
    }
}
