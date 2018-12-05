﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    public class PlayerController : MoveController
    {
        protected void Update()
        {
            //Checks if object is alive
            if (state.isAlive)
            {
                CharacterMovement();
            }
            SetAnimatorValues();
        }

    }
}
