using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public PlayerStateReusableData ReusableData { get; }
        public Player Player { get; }
        public PlayerIdlingState IdleState { get; }
        public PlayerWalkingState WalkingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }
        public PlayerDashingState DashingState { get; }
        public PlayerLightStoppingState LightStoppingState { get; }
        public PlayerMediumStoppingState MediumStoppingState { get; }
        public PlayerHardStoppingState HardStoppingState { get; }
        public PlayerJumpingState JumpingState { get; }
        public PlayerFallingState FallingState { get; }
        public PlayerLightLandingState LightLandingState { get; }
        public PlayerRollingState RollingState { get; }
        public PlayerHardLandingState HardLandingState { get; }

        public PlayerMovementStateMachine(Player player)
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();
            IdleState = new PlayerIdlingState(this);

            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);

            DashingState = new PlayerDashingState(this);

            LightStoppingState = new PlayerLightStoppingState(this);
            MediumStoppingState = new PlayerMediumStoppingState(this);
            HardStoppingState = new PlayerHardStoppingState(this);

            JumpingState = new PlayerJumpingState(this);
            FallingState = new PlayerFallingState(this);

            LightLandingState = new PlayerLightLandingState(this);
            RollingState = new PlayerRollingState(this);
            HardLandingState = new PlayerHardLandingState(this);
        }
    }
}
