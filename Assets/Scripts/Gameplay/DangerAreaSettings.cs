using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Danger Area")]
public class DangerAreaSettings : ScriptableObject
{
    public int PiecesToStart { get => piecesToStart; }
    [SerializeField] int piecesToStart;
    public int PiecesToMove { get => piecesToMove; }
    [SerializeField] int piecesToMove;
    public float TimeToMoveUp { get => timeToMoveUp; }
    [SerializeField] float timeToMoveUp;
    public float MaxFloor { get => maxFloor; }
    [SerializeField] int maxFloor;
}

