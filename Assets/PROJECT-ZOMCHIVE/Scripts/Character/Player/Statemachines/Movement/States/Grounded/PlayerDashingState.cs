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
        private bool shouldKeepRotating;
        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            dashData = movementData.DashData;   
        }

        #region IState Methods
        public override void StateEnter()
        {

            stateMachine.ReusableData.MovementSpeedModifer = dashData.SpeedModifier;
            base.StateEnter();
            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;
            stateMachine.ReusableData.RotationData = dashData.RotationData;

            AddForceOnTransitionFromStationaryState();

            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

            UpdateConsecutiveDashes(); 

            startTime = Time.time;
        }

        public override void StateExit()
        {
            base.StateExit();

            SetBaseRotationData();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!shouldKeepRotating)
            {
                return;
            }

            RotateTowardsTargetRotation();
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
        private void AddForceOnTransitionFromStationaryState() // �������� �ʴ� ���¿��� �뽬�� �� Force
        {
            if(stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return; // �����̴� ���¸� return. �������� ������ AddForce ���� ��.
            }

            Vector3 characterRotationDirection = stateMachine.Player.transform.forward;
            // GetMovementDirection()�� �����ϸ� �� �� ������..

            characterRotationDirection.y = 0f;

            UpdateTargetRotation(characterRotationDirection, false);

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

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.playerActions.Movement.performed += OnMovementPerfomed;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            stateMachine.Player.Input.playerActions.Movement.performed -= OnMovementPerfomed;
        }
        #endregion

        #region Input Methods
        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
           
        }

        private void OnMovementPerfomed(InputAction.CallbackContext context)
        {
            shouldKeepRotating = true;  
        }
        #endregion
    }
}
