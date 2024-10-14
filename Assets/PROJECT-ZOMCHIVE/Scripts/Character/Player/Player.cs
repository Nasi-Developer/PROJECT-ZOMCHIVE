using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("References")]
        [field: SerializeField] public PlayerSO Data {  get; private set; }
        [field: Header("Collisions")]
        [field: SerializeField] public CapsuleColliderUtility ColliderUtility { get; private set; }
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }
        public PlayerMovementStateMachine movementStateMachine;

        public Rigidbody Rigidbody {  get; private set; }
        public PlayerInput Input {  get; private set; }
        
        public Transform MainCameraTransform { get; private set; }

        private void Awake()
        {
            movementStateMachine = new PlayerMovementStateMachine(this);
            Input = GetComponent<PlayerInput>();
            Rigidbody = GetComponent<Rigidbody>();
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimension();

            MainCameraTransform = Camera.main.transform;
        }

        private void OnValidate()
        {
            ColliderUtility.Initialize(gameObject);
            ColliderUtility.CalculateCapsuleColliderDimension();
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
