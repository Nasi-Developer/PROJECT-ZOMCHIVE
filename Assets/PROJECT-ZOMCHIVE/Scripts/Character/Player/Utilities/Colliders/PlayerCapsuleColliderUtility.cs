using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZOMCHIVE
{
    [Serializable]
    public class PlayerCapsuleColliderUtility : CapsuleColliderUtility // Player ���� ĸ�� �ݶ��̴� ��ƿ��Ƽ
    {
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }
    }
}
