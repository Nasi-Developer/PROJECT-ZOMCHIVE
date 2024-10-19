using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ZOMCHIVE
{
    public interface IState
    {
        public void StateEnter();
        public void StateExit();

        public void HandleInput();
        public void Update();
        public void PhysicsUpdate();

        public void OnAnimationEnterEvent(); // �ñر� ���� ������ ���� ���¸� �����ϱ� ���� ����.
        public void OnAnimationExitEvent(); // ���� ����.
        public void OnAnimationTransitionEvent();
        public void OnTriggerEnter(Collider collider);
        public void OnTriggerExit(Collider collider);

        // �÷��̾��� ���� ���¸� �����ϴ� PlayerStateMachine�� ����� �޼ҵ� ��.
    }
}
