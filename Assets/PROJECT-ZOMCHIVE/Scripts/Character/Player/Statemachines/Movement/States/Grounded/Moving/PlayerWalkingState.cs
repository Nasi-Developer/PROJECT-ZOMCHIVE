using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerWalkingState : PlayerMovingState
    {
        private PlayerWalkData walkData;
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            walkData = movementData.WalkData;
        }


        #region IState Methods
        public override void StateEnter()
        {

            stateMachine.ReusableData.MovementSpeedModifer = movementData.WalkData.SpeedModifer;

            stateMachine.ReusableData.BackwardsCameraRecenteringData = movementData.WalkData.BackwardsCameraRecenteringData;

            base.StateEnter();

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.WeakForce;
        }

        public override void StateExit()
        {
            base.StateExit();

            SetBaseCameraRecenteringData();
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

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}
