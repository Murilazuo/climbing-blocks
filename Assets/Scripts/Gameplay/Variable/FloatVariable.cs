using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVariable", menuName ="Variables/Float")]
public class FloatVariable : ScriptableObject
{
    public float Value { get => value; }
    [SerializeField] float value;
}
