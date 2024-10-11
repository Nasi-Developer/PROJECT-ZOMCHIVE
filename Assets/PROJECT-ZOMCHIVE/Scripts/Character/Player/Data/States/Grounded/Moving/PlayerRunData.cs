using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [Serializable]
    public class PlayerRunData 
    {
        [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifer { get; private set; } = 1f;
    }
}
