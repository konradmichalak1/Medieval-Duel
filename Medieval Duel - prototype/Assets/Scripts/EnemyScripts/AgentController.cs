using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    class AgentController : MoveController
    {
        protected override void Update()
        {   //Checks if object is alive
            if (state.isAlive)
            {
               // CharacterMovement();
            }
            SetAnimatorValues();
        }
    }
}
