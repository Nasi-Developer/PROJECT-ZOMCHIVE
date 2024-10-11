using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [Serializable]
    public class PlayerWalkData
    {
        [field: SerializeField][field: Range(0f, 1f)] public float SpeedModifer { get; private set; } = 0.225f;
    }
}
