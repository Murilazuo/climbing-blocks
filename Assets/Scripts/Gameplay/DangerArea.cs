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

    public static System.Action<int,int> OnSetDangerArea;
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
                OnSetDangerArea?.Invoke(spawnedPieces, settings.PiecesToStart);
            }
            else
            {
                if (spawnedPieces >= settings.PiecesToMove)
                    NextFloor();
                OnSetDangerArea?.Invoke(spawnedPieces, settings.PiecesToMove);
            }

        }
    }
    void NextFloor()
    {
        spawnedPieces = 0;
        floor++;
        LeanTween.scaleY(gameObject, floor, settings.TimeToMoveUp);
    }

    void ResetDanger(int endId)
    {
        floor = 0;
        spawnedPieces = 0;

        transform.localScale = new(transform.localScale.x,0);
    }
    public void OnEnable()
    {
        Piece.OnStopPiece += NextPiece;
        MatchManager.OnEndGame += ResetDanger;
    }
    public void OnDisable()
    {
        Piece.OnStopPiece -= NextPiece;
        MatchManager.OnEndGame -= ResetDanger;
    }
}



