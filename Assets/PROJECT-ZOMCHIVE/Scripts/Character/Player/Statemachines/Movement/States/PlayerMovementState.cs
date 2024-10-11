using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine stateMachine;
        protected Vector2 movementInput;
        protected float baseSpeed = 5f;
        protected float speedModifer = 1f;

        protected Vector3 currentTargetRotation;
        protected Vector3 timeToReachTargetRotation;
        protected Vector3 dampedTargetRotaitonCurrentVelocity;
        protected Vector3 dampedTargetRotationPassedTime;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            stateMachine = playerMovementStateMachine;

            InitializeData();
        }

        private void InitializeData()
        {
            timeToReachTargetRotation.y = 0.14f;
        }

        #region IState Interface
        public virtual void StateEnter()
        {
            Debug.Log("State : " + GetType().Name); 
        }

        public virtual void StateExit()
        {
            
        }
        public virtual void HandleInput()
        {
            ReadMovementInput();
        }

        public virtual void PhysicsUpdate()
        {
            Move();
        }

        public void Update()
        {
            
        }

        #endregion

        #region Main Methods
        private void ReadMovementInput()
        {
            movementInput = stateMachine.Player.Input.playerActions.Movement.ReadValue<Vector2>();
        }

        private void Move()
        {
            if (movementInput == Vector2.zero || speedModifer == 0f)
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
            currentTargetRotation.y = targetAngle;

            dampedTargetRotationPassedTime.y = 0f;
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

        #region Reuseable Methods
        protected Vector3 GetMovementDirection()
        {
            return new Vector3(movementInput.x, 0f, movementInput.y);
        }
        protected float GetMovementSpeed()
        {
            return baseSpeed * speedModifer;
        }
        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = stateMachine.Player.Rigidbody.velocity;

            playerHorizontalVelocity.y = 0f;

            return playerHorizontalVelocity;
        }

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = stateMachine.Player.Rigidbody.rotation.eulerAngles.y;
            // ���� player�� ȸ�� ��

            if (currentYAngle == currentTargetRotation.y)
            {
                return;
            }
            // ���� player�� ȸ�� ����, ��ǥ ȸ�� ���� ������ return

            float smoothYAngle = Mathf.SmoothDampAngle(currentYAngle, currentTargetRotation.y, 
            ref dampedTargetRotaitonCurrentVelocity.y, timeToReachTargetRotation.y - dampedTargetRotationPassedTime.y);

            // ���� player�� ȸ�������� ��ǥ ȸ�������� smoothdampangle�� ����Ͽ� �ε巴�� ȸ�� ��Ų��.

            dampedTargetRotationPassedTime.y += Time.deltaTime;

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

            if (directionAngle != currentTargetRotation.y)
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
        #endregion
    }
}
