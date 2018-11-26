using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerRollController : RollController
    {
        protected override void Roll()
        {
            isRolling = SetRolling();
            base.Roll();
        }
        private bool SetRolling()
        {
            return Input.GetButtonDown("Jump") ? true : false;
        }

    }
}
