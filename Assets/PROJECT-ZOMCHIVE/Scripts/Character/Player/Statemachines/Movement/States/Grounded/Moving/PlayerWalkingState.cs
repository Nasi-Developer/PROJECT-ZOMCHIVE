using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerWalkingState : PlayerMovingState
    {
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }


        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();

            stateMachine.ReusableData.MovementSpeedModifer = movementData.WalkData.SpeedModifer;
        }
        #endregion

        #region Input Methods
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            stateMachine.ChangeState(stateMachine.RunningState);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.LightStoppingState);
        }
        #endregion
    }
}
