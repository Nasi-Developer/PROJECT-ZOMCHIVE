using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerGroundedState : PlayerMovementState
    {
        private SlopeData slopData;
        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
            slopData = stateMachine.Player.ColliderUtility.SlopeData;
        }
        #region IState Methods

        public override void StateEnter()
        {
            base.StateEnter();

            UpdateShouldSprintState();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();

           
        }

        #endregion

        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardRayFromCapsuleCenter = new Ray(capsuleColliderInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardRayFromCapsuleCenter, out RaycastHit hit, slopData.FloatRayDistance,
                                stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                // float groundAngle = Vector3.Angle(hit.normal, -downwardRayFromCapsuleCenter.direction);
                float cosAngle = Vector3.Dot(hit.normal, -downwardRayFromCapsuleCenter.direction);
                float groundAngle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

                float slopeSpeedModifer = SetSlopeSpeedModiferOnAngle(groundAngle);

                if (slopeSpeedModifer == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint =
                stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderCenterInLocalSpace.y *
                stateMachine.Player.transform.localScale.y - hit.distance;

                // 캐릭터의 크기가 작아지거나, 커질 것을 대비하여 localscale.y를 곱한다.

                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * slopData.StepReachForce - GetPlayerVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
                // 수직 힘이라 겹쳐도 상관 X
            }
        }

        private float SetSlopeSpeedModiferOnAngle(float angle)
        {
            float slopeSpeedModifier = movementData.SlopeSpeedAngles.Evaluate(angle);

            stateMachine.ReusableData.MovementOnSlopeSpeedModifer = slopeSpeedModifier;

            return slopeSpeedModifier;
        }
        private void UpdateShouldSprintState()
        {
            if (!stateMachine.ReusableData.ShouldSprint)
            {
                return;
            }

            if (stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            stateMachine.ReusableData.ShouldSprint = false;
        }
        private bool IsThereGroundUnderneath()
        {
            BoxCollider groundCheckCollider = stateMachine.Player.ColliderUtility.TriggerColliderData.GroundCheckCollider;

            Vector3 groundColliderCenterInWorldSpace = groundCheckCollider.bounds.center;

            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, groundCheckCollider.bounds.extents,
                       groundCheckCollider.transform.rotation, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

            return overlappedGroundColliders.Length > 0;

        }

        #endregion
        #region Reuseable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            stateMachine.Player.Input.playerActions.Movement.canceled += OnMovementCanceled;
            stateMachine.Player.Input.playerActions.Dash.started += OnDashStarted;
            stateMachine.Player.Input.playerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            stateMachine.Player.Input.playerActions.Movement.canceled -= OnMovementCanceled;
            stateMachine.Player.Input.playerActions.Dash.started -= OnDashStarted;
            stateMachine.Player.Input.playerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove() // 이동 상태 업데이트 함수
        {
            if (stateMachine.ReusableData.ShouldSprint)
            {
                stateMachine.ChangeState(stateMachine.SprintingState);
                return;
            }

            if (stateMachine.ReusableData.ShouldWalk)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
        }

        protected override void OnContactWithGroundExited(Collider collider)
        {
            base.OnContactWithGroundExited(collider);

            if (IsThereGroundUnderneath())
            {
                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = stateMachine.Player.ColliderUtility.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - stateMachine.Player.ColliderUtility.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

            if(!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, movementData.GroundToFallRayDistance, stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }

            
        }

        protected virtual void OnFall()
        {
            stateMachine.ChangeState(stateMachine.FallingState);
        }

        #endregion

        #region Input Methods
        protected virtual void OnMovementCanceled(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        protected virtual void OnDashStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.DashingState);
        }
        protected virtual void OnJumpStarted(InputAction.CallbackContext context)
        {
            stateMachine.ChangeState(stateMachine.JumpingState);
        }
        #endregion
    }
}
