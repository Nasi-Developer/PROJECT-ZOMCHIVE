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
                return; // �����̴� ���¸� return. �������� ������ AddForce ���� ��.
            }

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            // GetMovementDirection()�� �����ϸ� �� �� ������..

            characterRotationDirection.y = 0f;
        }
        #endregion
    }
}
