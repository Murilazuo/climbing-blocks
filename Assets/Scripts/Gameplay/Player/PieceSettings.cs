using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPieceSettings", menuName = "Settings/Piece Settings")]
public class PieceSettings : ScriptableObject
{
    public float HoldTimeToMove { get => holdTimeToMove; }
    [SerializeField] float holdTimeToMove;
    public float MoveDelay { get => moveDalay; }
    [SerializeField] float moveDalay;
    public float PieceGravity { get => pieceGravity; }
    [SerializeField] float pieceGravity;
    public float TimeToMoveInSpeed { get => timeToMoveInSpeed; }
    [SerializeField] float timeToMoveInSpeed;
    public int BlockLife { get => blockLife; }
    [SerializeField] int blockLife;
}
