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
    }
}
