using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DangerArea : MonoBehaviour
{
    [SerializeField] DangerAreaSettings settings;
    int floor = 0;
    int spawnedPieces = 0;
    bool start = false;
    void NextPiece()
    {
        if (floor < settings.MaxFloor)
        {
            spawnedPieces++;
            if (!start)
            {
                if (spawnedPieces >= settings.PiecesToStart)
                {
                    NextFloor();
                    start = true;
                }
            }
            else
            {
                if (spawnedPieces >= settings.PiecesToMove)
                    NextFloor();
            }
        }
    }
    void NextFloor()
    {
        spawnedPieces = 0;
        floor++;
        LeanTween.scaleY(gameObject, floor, settings.TimeToMoveUp);
    }

    public void OnEnable()
    {
        Piece.OnStopPiece += NextPiece;
    }
    public void OnDisable()
    {
        Piece.OnStopPiece -= NextPiece;
    }
}



