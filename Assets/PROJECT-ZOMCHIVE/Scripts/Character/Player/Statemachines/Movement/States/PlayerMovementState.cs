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
            // ���� ��ǲ�� ���ų�(speedModifer�� 0�� ���), ĳ���Ͱ� �������� �ʵ��� �Ѵ�.

            Vector3 movementDirection = GetMovementDirection();
            // Vector2 ������ ���� �Է� ���� Vector3�� ��ȯ�Ͽ�, ĳ������ �̵� ������ ���Ѵ�.

            float targetRotationYAngle = Rotate(movementDirection);
            // ���� �Է� ������ ������� ĳ���Ͱ� ȸ���ؾ� �� ������ ����ϰ�, ĳ���͸� �ش� �������� ȸ����Ų��.

            Vector3 targetRotaitonDirection = GetTargetRotationDirection(targetRotationYAngle);
            // ��ǥ ȸ�� ������ ������� ĳ���Ͱ� ���ؾ� �� ���� ���� ���͸� ���Ѵ�.

            float movementSpeed = GetMovementSpeed();
            // �̵� �ӵ��� ���Ѵ�. �⺻ �ӵ�(baseSpeed)�� �ӵ� ������(speedModifer)�� ���Ͽ� ���� �̵� �ӵ��� ���.

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();
            // �÷��̾��� ���� ���� �ӵ��� �����´�. y �� �ӵ��� ���ܵȴ� (������� ��ȣ�ۿ��� �����ϱ� ����).

            stateMachine.Player.Rigidbody.AddForce(targetRotaitonDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
            // �÷��̾��� ���� �ӵ��� ������ �� ��ǥ �������� ���� ���Ѵ�.
            // ForceMode.VelocityChange�� �ﰢ���� �ӵ� ��ȭ(������ ���� ����)�� �ǹ��Ѵ�.
        }

        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);
            // ���� ��ǲ ���� + ī�޶� ������ ���� ���� ��

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

            // Atan2�� ����� ���� ���� ��ǲ�� ���� ������ ī�޶� �������� ����(0~360)�� ���Ѵ�.

            return directionAngle;
        }

        private static float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            // ���� �Է� ���� Vector3�� ���� direction�� Atan2�� ����Ͽ� ������ ��� ������ ���ϰ� ������ ������ ����
            // Atan2�� -180 ~ +180�� ��ȯ�ϹǷ�, ���� ������ ���� ���� ����Ͽ� +360f�� ���ǿ� ���� �����ش�.

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
            // ���� player�� ȸ�� ��

            if (currentYAngle == stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }
            // ���� player�� ȸ�� ����, ��ǥ ȸ�� ���� ������ return

            float smoothYAngle = Mathf.SmoothDampAngle(currentYAngle, stateMachine.ReusableData.CurrentTargetRotation.y, 
            ref stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, stateMachine.ReusableData.TimeToReachTargetRotation.y - stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

            // ���� player�� ȸ�������� ��ǥ ȸ�������� smoothdampangle�� ����Ͽ� �ε巴�� ȸ�� ��Ų��.

            stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothYAngle, 0f);

            stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);
            // ���� �Է°��� ������ Atan2�� ����� ���� ��

            if (shouldConsiderCameraRotation)
            {
                directionAngle = AddCameraRotationtoAngle(directionAngle);
                // ī�޶� y�� ȸ������ �����ش�.
            }

            if (directionAngle != stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            // ���������� ���� ȸ������ ������Ʈ ���ش�.

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
