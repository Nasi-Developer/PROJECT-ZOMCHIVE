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

        public PlayerMovementStateMachine(Player player)
        {
            Player = player;
            ReusableData = new PlayerStateReusableData();
            IdleState = new PlayerIdlingState(this);
            WalkingState = new PlayerWalkingState(this);
            RunningState = new PlayerRunningState(this);
            SprintingState = new PlayerSprintingState(this);
            DashingState = new PlayerDashingState(this);
        }
    }
}
