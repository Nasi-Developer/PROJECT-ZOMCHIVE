using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private PlayerFallData fallData;
        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            fallData = airborneData.FallData;
        }
        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();

            ResetVerticalVelocity();

            stateMachine.ReusableData.MovementSpeedModifer = 0f;
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
        #endregion

        #region Input Methods
        #endregion
    }
}
