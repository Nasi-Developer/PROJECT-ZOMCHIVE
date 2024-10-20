using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private PlayerJumpData jumpData;
        private bool shouldKeepRotating;
        private bool canStartFalling;
        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            jumpData = airborneData.JumpData;
        }

        #region IState Methods
        public override void StateEnter()
        {

            base.StateEnter();
            stateMachine.ReusableData.MovementSpeedModifer = 0f;
            stateMachine.ReusableData.RotationData = jumpData.RotationData;
            stateMachine.ReusableData.MovementDecelerationForce = jumpData.DecelerationForce;

            shouldKeepRotating = stateMachine.ReusableData.MovementInput != Vector2.zero;

            Jump();
        }
        public override void Update()
        {
            base.Update();

            if (!canStartFalling && IsMovingUp(0f)) // Player Rigidbody의 Velocity가 0f보다 크면 
            {
                canStartFalling = true;

            }

            if (!canStartFalling || GetPlayerVerticalVelocity().y > 0)
            {
                return;
            }

            stateMachine.ChangeState(stateMachine.FallingState);
        }
        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (shouldKeepRotating)
            {
                RotateTowardsTargetRotation();
            }

            if (IsMovingUp())
            {
                DecelerateVertically();
            }
        }
        public override void StateExit()
        {
            base.StateExit();

            canStartFalling = false;

            SetBaseRotationData();
        }
        #endregion

        #region Main Methods
        private void Jump()
        {
            Vector3 jumpForce = stateMachine.ReusableData.CurrentJumpForce;

            Vector3 jumpDirection = stateMachine.Player.transform.forward;

            if (shouldKeepRotating)
            {
                jumpDirection = GetTargetRotationDirection(stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);
            
            if(Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, jumpData.JumpToGroundRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);
                // float groundAngle = Vector3.Dot(hit.normal, Vector3.up);

                if (IsMovingUp())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }

                if (IsMovingDown())
                {
                    float forceModifier = jumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                    jumpForce.y *= forceModifier;
                }
            }

            ResetVelocity(); // Rigidbody Velocity 초기화해서 의도치 않은 이동 방지.

            stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        #endregion

        #region Reusable Methods
        protected override void ResetSprintState()
        {
            
        }   
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {

        }
        #endregion
    }
}
