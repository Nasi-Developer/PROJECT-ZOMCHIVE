using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("References")]
        [field:SerializeField] public PlayerSO Data {  get; private set; }
        public PlayerMovementStateMachine movementStateMachine;

        public Rigidbody Rigidbody {  get; private set; }
        public PlayerInput Input {  get; private set; }
        
        public Transform MainCameraTransform { get; private set; }

        private void Awake()
        {
            movementStateMachine = new PlayerMovementStateMachine(this);
            Input = GetComponent<PlayerInput>();
            Rigidbody = GetComponent<Rigidbody>();

            MainCameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            movementStateMachine.ChangeState(movementStateMachine.IdleState);
            
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
