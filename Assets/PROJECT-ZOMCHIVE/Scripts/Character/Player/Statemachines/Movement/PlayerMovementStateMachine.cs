using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    public class PlayerMovementStateMachine : StateMachine
    {
        public PlayerIdlingState IdleState { get; }
        public PlayerWalkingState WalingState { get; }
        public PlayerRunningState RunningState { get; }
        public PlayerSprintingState SprintingState { get; }

        public PlayerMovementStateMachine()
        {
            IdleState = new PlayerIdlingState();
            WalingState = new PlayerWalkingState();
            RunningState = new PlayerRunningState();
            SprintingState = new PlayerSprintingState();
        }
    }
}
