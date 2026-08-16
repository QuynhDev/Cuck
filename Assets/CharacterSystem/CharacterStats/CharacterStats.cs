using System;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour 
{
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    public float CurrentHealth { get; private set; }
    public float WalkingSpeed { get; private set; }
    public float RunningSpeed { get; private set; }
}
