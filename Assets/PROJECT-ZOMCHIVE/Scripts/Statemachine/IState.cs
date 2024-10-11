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

        // 플레이어의 현재 상태를 관리하는 PlayerStateMachine에 상속할 메소드 들.
    }
}
