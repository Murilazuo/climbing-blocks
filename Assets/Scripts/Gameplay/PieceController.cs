using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    Piece currentPiece;
    [SerializeField] float speed;
    private void FixedUpdate()
    {
        if (currentPiece)
        {
        }
    }
    
    void OnSpawnPiece(Piece piece)
    {
        currentPiece = piece;
    }
    private void OnEnable()
    {
        MatchManager.OnSpawnPiece += OnSpawnPiece;
    }
    private void OnDisable()
    {
        MatchManager.OnSpawnPiece -= OnSpawnPiece;
    }
}
