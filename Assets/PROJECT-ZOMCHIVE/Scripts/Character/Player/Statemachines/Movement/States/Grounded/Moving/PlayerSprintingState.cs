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

        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            sprintData = movementData.SprintData;
        }

        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();

            stateMachine.ReusableData.MovementSpeedModifer = sprintData.SpeedModifier;

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
                // 경과 시간 < 경과 시간 + 유지시간(1초)일 경우

                return;
            }

            StopSprinting();

        }

        public override void StateExit()
        {
            base.StateExit();

            keepSprinting = false;
        }

        #endregion

        #region Main Methods
        private void StopSprinting()
        {
            if(stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                // 1초 이내에 Sprint key를 뗏고, 입력이 없을 경우 정지 상태(Hard Stop)로 전환
            }

            stateMachine.ChangeState(stateMachine.RunningState);
            // 입력 있으면 그냥 Running 상태로 전환

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
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            keepSprinting = true;
        }
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.HardStoppingState);
        }
        #endregion
    }
}
