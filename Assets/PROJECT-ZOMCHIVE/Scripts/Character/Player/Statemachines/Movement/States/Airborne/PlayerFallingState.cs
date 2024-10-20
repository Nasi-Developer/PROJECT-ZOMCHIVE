using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private PlayerFallData fallData;
        private Vector3 playerPositionOnEnter;
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            fallData = airborneData.FallData;
        }
        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();

            stateMachine.ReusableData.MovementSpeedModifer = 0f;
            playerPositionOnEnter = stateMachine.Player.transform.position;

            ResetVerticalVelocity();


        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            LimitVerticalVelocity();
        }

        #endregion

        #region Main Methods
        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            if(playerVerticalVelocity.y >= -fallData.FallSpeedLimit)
            {
                return;
            }

            Vector3 LimitedVelocity = new Vector3(0f, -fallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);

            stateMachine.Player.Rigidbody.AddForce(LimitedVelocity, ForceMode.VelocityChange);
        }
        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()
        {
           
        }

        protected override void OnContactWithGround(Collider collider) // ÃßÈÄ ³«ÇÏ µ¥¹ÌÁö or ³«°ø
        {
            float fallDistance = Mathf.Abs(playerPositionOnEnter.y - stateMachine.Player.transform.position.y);

            if (fallDistance < fallData.minimumDistanceToBeConsideredHardFall)
            {
                stateMachine.ChangeState(stateMachine.LightLandingState);

                return;
            }

            if (stateMachine.ReusableData.ShouldWalk && stateMachine.ReusableData.ShouldSprint || stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.HardLandingState);

                return;
            }

            stateMachine.ChangeState(stateMachine.RollingState);
        }
        #endregion

        #region Input Methods
        #endregion
    }
}
