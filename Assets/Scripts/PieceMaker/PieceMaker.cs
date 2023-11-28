using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMaker : MonoBehaviour
{
    [SerializeField] List<Piece.PieceData> piecesData;

    [SerializeField]PieceMakerPart[] piecePart;

    private void Awake()
    {
        piecePart = FindObjectsOfType<PieceMakerPart>();
    }

    public void Create()
    {
        Piece.PieceData data = new();
        List<Vector2> position = new List<Vector2>();

        foreach (var piece in piecePart)
        {
            if(piece.active)
            {
                position.Add(piece.position);
            }
        }

        //data.positions = position.ToArray();

        piecesData.Add(data);
    }
}
