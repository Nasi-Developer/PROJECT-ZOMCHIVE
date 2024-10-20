using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerSprintingState : PlayerMovingState
    {
        private PlayerSprintData sprintData;
        private bool keepSprinting;
        private float startTime;
        private bool shouldResetSprintState;

        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        #region IState Methods
        public override void StateEnter()
        {

            stateMachine.ReusableData.MovementSpeedModifer = sprintData.SpeedModifier;

            base.StateEnter();

            stateMachine.ReusableData.CurrentJumpForce = airborneData.JumpData.StrongForce;

            shouldResetSprintState = true;

            startTime = Time.time;
        }

        public override void Update()
        {
            base.Update();

            if (keepSprinting)
            {
                return;
            }

            if (Time.time < startTime + sprintData.SprintToRunTime)
            {
                // ��� �ð� < ��� �ð� + �����ð�(1��)�� ���

                return;
            }

            StopSprinting();

        }

        public override void StateExit()
        {
            base.StateExit();

            if (shouldResetSprintState)
            {
                keepSprinting = false;
                stateMachine.ReusableData.ShouldSprint = false;
            }

            
        }

        #endregion

        #region Main Methods
        private void StopSprinting()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                // 1�� �̳��� Sprint key�� �°�, �Է��� ���� ��� ���� ����(Hard Stop)�� ��ȯ
            }

            stateMachine.ChangeState(stateMachine.RunningState);
            // �Է� ������ �׳� Running ���·� ��ȯ

        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.playerActions.Sprint.performed += OnSprintPerformed;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            stateMachine.Player.Input.playerActions.Sprint.performed -= OnSprintPerformed;
        }

        protected override void OnFall()
        {
            shouldResetSprintState = false;
            base.OnFall();
        }
        #endregion

        #region Input Methods
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            keepSprinting = true;

            stateMachine.ReusableData.ShouldSprint = true;
        }

        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            shouldResetSprintState = false;

            base.OnJumpStarted(context);
        }
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}
