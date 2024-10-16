using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZOMCHIVE
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine stateMachine;
        protected PlayerGroundedData movementData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;
            movementData = stateMachine.Player.Data.GroundedData;
            
            InitializeData();
        }

        private void InitializeData()
        {
            stateMachine.ReusableData.TimeToReachTargetRotation.y = movementData.BaseRotationData.TargetRotationReachTime.y;
        }

        #region IState Interface
        public virtual void StateEnter()
        {
            Debug.Log("State : " + GetType().Name);

            AddInputActionsCallbacks();
        }

        public virtual void StateExit()
        {
            RemoveInputActionsCallbacks();
        }

        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public virtual void Update()
        {
            
        }
        public virtual void OnAnimationEnterEvent()
        {
            
        }

        public virtual void OnAnimationExitEvent()
        {
            
        }

        public virtual void OnAnimationTransitionEvent()
        {
            
        }
        #endregion

        #region Main Methods
        private void ReadMovementInput()
        {
            stateMachine.ReusableData.MovementInput = stateMachine.Player.Input.playerActions.Movement.ReadValue<Vector2>();
        }

        private void Move()
        {
            if (stateMachine.ReusableData.MovementInput == Vector2.zero || stateMachine.ReusableData.MovementSpeedModifer == 0f)
            {
                return;
            }
            // 유저 인풋이 없거나(speedModifer가 0일 경우), 캐릭터가 움직이지 않도록 한다.

            Vector3 movementDirection = GetMovementDirection();
            // Vector2 형태의 유저 입력 값을 Vector3로 변환하여, 캐릭터의 이동 방향을 구한다.

            float targetRotationYAngle = Rotate(movementDirection);
            // 유저 입력 방향을 기반으로 캐릭터가 회전해야 할 각도를 계산하고, 캐릭터를 해당 방향으로 회전시킨다.

            Vector3 targetRotaitonDirection = GetTargetRotationDirection(targetRotationYAngle);
            // 목표 회전 각도를 기반으로 캐릭터가 향해야 할 최종 방향 벡터를 구한다.

            float movementSpeed = GetMovementSpeed();
            // 이동 속도를 구한다. 기본 속도(baseSpeed)와 속도 보정값(speedModifer)을 곱하여 최종 이동 속도를 계산.

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            // 플레이어의 현재 수평 속도를 가져온다. y 축 속도는 제외된다 (지면과의 상호작용을 무시하기 위해).

            stateMachine.Player.Rigidbody.AddForce(targetRotaitonDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
            // 플레이어의 수평 속도를 제거한 후 목표 방향으로 힘을 가한다.
            // ForceMode.VelocityChange는 즉각적인 속도 변화(마찰과 질량 무시)를 의미한다.
        }

        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);
            // 유저 인풋 방향 + 카메라 각도를 더한 각도 값

            RotateTowardsTargetRotation();

            return directionAngle;
        }

        private void UpdateTargetRotationData(float targetAngle)
        {
            stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
        }

        private float AddCameraRotationtoAngle(float directionAngle)
        {
            directionAngle += stateMachine.Player.MainCameraTransform.eulerAngles.y;

            if (directionAngle > 360f)
            {
                directionAngle -= 360f;
            }

            // Atan2를 사용해 얻은 유저 인풋의 방향 각도에 카메라 가로축의 각도(0~360)을 더한다.

            return directionAngle;
        }

        private static float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            // 유저 입력 값을 Vector3로 만든 direction을 Atan2를 사용하여 유저가 어느 방향을 향하고 싶은지 방향을 구함
            // Atan2는 -180 ~ +180을 반환하므로, 음의 각도가 나올 것을 대비하여 +360f를 조건에 따라 더해준다.

            return directionAngle;
        }


        #endregion

        #region Reusable Methods
        protected Vector3 GetMovementDirection()
        {
            return new Vector3(stateMachine.ReusableData.MovementInput.x, 0f, stateMachine.ReusableData.MovementInput.y);
        }
        protected float GetMovementSpeed()
        {
            return movementData.BaseSpeed * stateMachine.ReusableData.MovementSpeedModifer * stateMachine.ReusableData.MovementOnSlopeSpeedModifer;
        }
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

            playerHorizontalVelocity.y = 0f;

            return playerHorizontalVelocity;
        }

        protected Vector3 GetPlayerVerticalVelocity()
        {
            return new Vector3(0f, stateMachine.Player.Rigidbody.velocity.y, 0f);
        }

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            // 현재 player의 회전 값

            if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }
            // 현재 player의 회전 값과, 목표 회전 값이 같으면 return

            float smoothYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, 
            ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

            // 현재 player의 회전값에서 목표 회전값까지 smoothdampangle을 사용하여 부드럽게 회전 시킨다.

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);
            // 유저 입력값의 방향을 Atan2를 사용해 구한 값

            if (shouldConsiderCameraRotation)
            {
                directionAngle = AddCameraRotationtoAngle(directionAngle);
                // 카메라 y축 회전값을 더해준다.
            }

            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            // 최종적으로 구한 회전값을 업데이트 해준다.

            return directionAngle;
        }

        private Vector3 GetTargetRotationDirection(float targetAngle)
        {
            return Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        protected void ResetVelocity()
        {
            stateMachine.Player.Rigidbody.velocity = Vector3.zero;
        }

        protected virtual void AddInputActionsCallbacks()
        {
            stateMachine.Player.Input.playerActions.WalkToggle.started += OnWalkToggleStarted;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            stateMachine.Player.Input.playerActions.WalkToggle.started -= OnWalkToggleStarted;
        }
        #endregion

        #region Input Methods
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            stateMachine.ReusableData.ShouldWalk = !stateMachine.ReusableData.ShouldWalk;
        }
        #endregion
    }
}
