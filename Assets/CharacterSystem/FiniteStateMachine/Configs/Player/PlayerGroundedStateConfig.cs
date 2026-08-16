using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerGroundedStateConfig
{
    [field: SerializeField] [field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 5f;
    [field: SerializeField] [field: Range(0f, 25f)] public float SpeedChangeRate { get; private set; } = 10.0f;
}
