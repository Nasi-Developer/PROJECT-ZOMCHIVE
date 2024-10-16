using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerStoppingState : PlayerGroundedState
    {
        public PlayerStoppingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods

        public override void StateEnter()
        {
            base.StateEnter();

            stateMachine.ReusableData.MovementSpeedModifer = 0f;
        }

        #endregion
    }
}
