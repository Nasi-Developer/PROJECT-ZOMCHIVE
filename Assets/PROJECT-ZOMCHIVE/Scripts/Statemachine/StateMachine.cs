using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public abstract class StateMachine
    {
        protected IState currentState;

        public void ChangeState(IState newState)
        {
            currentState?.StateExit();

            currentState = newState;

            currentState.StateEnter();
        }

        public void HandleInput()
        {
            currentState.HandleInput();
        }
        public void Update()
        {
            currentState.Update();
        }
        public void PhysicsUpdate()
        {
            currentState.PhysicsUpdate();
        }
    }
}
