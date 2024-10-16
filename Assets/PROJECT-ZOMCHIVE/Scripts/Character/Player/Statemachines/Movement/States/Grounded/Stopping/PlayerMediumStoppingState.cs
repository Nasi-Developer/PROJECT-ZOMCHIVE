using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerMediumStoppingState : PlayerStoppingState
    {
        public PlayerMediumStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void StateEnter()
        {
            base.StateEnter();
            stateMachine.ReusableData.MovementDecelerationForce = movementData.StopData.MediumDecelerationForce;
        }

        #endregion
    }
}
