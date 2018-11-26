using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class AgentRollController : RollController
    {
        protected override void Roll()
        {
            state.isWalking = true;

            //isRolling = true;
            //isRolling = SetRolling();
            base.Roll();
        }
        /// <summary>
        /// DO ZAIMPLEMENTOWANIA - WARUNKI WYWOŁANIA ROLLA DLA AGENTÓW
        /// </summary>
        private bool SetRolling()
        {
            return false;
        }
    }
}
