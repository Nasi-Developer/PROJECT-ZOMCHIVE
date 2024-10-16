using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private PlayerDashData dashData;
        private float startTime; // �뽬 ��� �ð�
        private int consecutiveDashesUsed; // ���� �뽬 ��� ��(ī��Ʈ)
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

            UpdateConsecutiveDashes(); 

            startTime = Time.time;
        }

        public override void OnAnimationEnterEvent()
        {
            base.OnAnimationEnterEvent();

            if (stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardStoppingState);

                return;
            }

            stateMachine.ChangeState(stateMachine.SprintingState);
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

            stateMachine.Player.Rigidbody.velocity = characterRotationDirection * GetMovementSpeed();
        }

        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                consecutiveDashesUsed = 0;
            }

            ++consecutiveDashesUsed;

            if (consecutiveDashesUsed == dashData.ConsecutiveDashsLimitAmount)
            {
                consecutiveDashesUsed = 0;
                stateMachine.Player.Input.DisableActionFor(stateMachine.Player.Input.playerActions.Dash, dashData.DashLimitReachedCooldown);

            }
        }

        private bool IsConsecutive()
        {
            return Time.time > startTime + dashData.TimeToBeConsideredConsecutive;
        }
        #endregion

        #region Input Methods
        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
           
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            
        }
        #endregion
    }
}
