using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData;
        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;
        }

        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();

            stateMachine.ReusableData.MovementSpeedModifer = dashData.SpeedModifier;

            AddForceOnTransitionFromStationaryState();
        }
        #endregion

        #region Main Methods
        private void AddForceOnTransitionFromStationaryState()
        {
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return; // 움직이는 상태면 return. 움직이지 않으면 AddForce 해줄 것.
            }

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            // GetMovementDirection()을 재사용하면 될 것 같은데..

            characterRotationDirection.y = 0f;
        }
        #endregion
    }
}
