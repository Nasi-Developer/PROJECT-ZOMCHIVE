using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        public PlayerMovementStateMachine movementStateMachine;
        public PlayerInput Input {  get; private set; }

        private void Awake()
        {
            movementStateMachine = new PlayerMovementStateMachine();
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdleState);
            Input = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }
    }
}
