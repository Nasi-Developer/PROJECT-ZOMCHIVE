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

        public void OnAnimationEnterEvent(); // 궁극기 사용시 무적과 같은 상태를 설정하기 위해 만듬.
        public void OnAnimationExitEvent(); // 위와 같음.
        public void OnAnimationTransitionEvent();
        public void OnTriggerEnter(Collider collider);
        public void OnTriggerExit(Collider collider);

        // 플레이어의 현재 상태를 관리하는 PlayerStateMachine에 상속할 메소드 들.
    }
}
