using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVariable", menuName = "Variables/Int")]
public class IntVariable : ScriptableObject
{
    [SerializeField] int value;
    public int Value { get => value; }
}
