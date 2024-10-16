using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        public void DisableActionFor(InputAction action, float seconds)
        {
            StartCoroutine(DisableAction(action, seconds));
        }

        private IEnumerator DisableAction(InputAction action, float seconds)
        {
            action.Disable();
            yield return new WaitForSeconds(seconds);
            action.Enable();
        }
    }
}
