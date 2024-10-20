using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ZOMCHIVE
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [field:Header("References")]
        [field: SerializeField] public PlayerSO Data {  get; private set; }
        [field: Header("Collisions")]
        [field: SerializeField] public PlayerCapsuleColliderUtility ColliderUtility { get; private set; }
        [field: SerializeField] public PlayerLayerData LayerData { get; private set; }
        [field: Header("Camera")]
        [field: SerializeField] public PlayerCameraUtility CameraUtility { get; private set; }
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
            CameraUtility.Initialized();

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

        private void OnTriggerEnter(Collider collider)
        {
            movementStateMachine.OnTriggerEnter(collider);
        }
        private void OnTriggerExit(Collider collider)
        {
            movementStateMachine.OnTriggerExit(collider);
        }

        private void Update()
        {
            movementStateMachine.HandleInput();

            movementStateMachine.Update();

            // Debug.Log(Rigidbody.velocity);
        }

        private void FixedUpdate()
        {
            movementStateMachine.PhysicsUpdate();
        }
    }
}
