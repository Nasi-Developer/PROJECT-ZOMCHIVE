using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerLightStoppingState : PlayerStoppingState
    {
        public PlayerLightStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();
            stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.LightDecelerationForce;
        }

        #endregion
    }
}
