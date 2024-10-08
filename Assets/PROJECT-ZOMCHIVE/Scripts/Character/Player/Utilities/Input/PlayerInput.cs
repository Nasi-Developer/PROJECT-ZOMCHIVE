using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerInput : MonoBehaviour
    {
        public PlayerInputActions InputActions {  get; private set; }
        public PlayerInputActions.PlayerActions playerActions;

        private void Awake()
        {
            InputActions = new PlayerInputActions();
            playerActions = InputActions.Player;
        }

        private void OnEnable()
        {
            playerActions.Enable();
        }

        private void OnDisable()
        {
            playerActions.Disable();
        }
    }
}
