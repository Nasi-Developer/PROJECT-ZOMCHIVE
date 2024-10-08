using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerMovementState : IState
    {
        public virtual void StateEnter()
        {
            Debug.Log("State : " + GetType().Name); 
        }

        public virtual void StateExit()
        {
            
        }
        public virtual void HandleInput()
        {
            
        }

        public virtual void PhysicsUpdate()
        {
           
        }


        public void Update()
        {
            
        }
    }
}
