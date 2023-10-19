using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    static MatchManager instance;
    public static MatchManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<MatchManager>();
            return instance;
        }
    }
    public static System.Action<Piece> OnSpawnPiece;
    [SerializeField] GameObject[] piecePrefabs;
    [SerializeField] Vector2 spawPiecePosition;
   
    public Vector2 playerPlatformSpawnPosition;
    Piece lastPiece;

    [SerializeField] GameObject arenaPartObj;

    [SerializeField] Vector2Int arenaSize;

    bool[,] pieceMatrix;
    private void Start()
    {
        pieceMatrix = new bool[arenaSize.x, arenaSize.y];

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    pieceMatrix[x, y] = true;

        PrintGrid();
    }
    [ContextMenu("SpawnGrid")]
    void SpawnGrid()
    {
        for (int x = 0; x < arenaSize.x; x++)
        {
            for (int y = 0; y < arenaSize.y; y++)
            {
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                {
                    //pieceMatrix[x, y] = true;
                    Instantiate(arenaPartObj, new Vector3(x, y, 0), Quaternion.identity).transform.SetParent(transform);
                }
            }
        }
    }

    [ContextMenu("Print Grid")]
    void PrintGrid()
    {
        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                print($"x: {x} y: {y} = {pieceMatrix[x, y]}");
    }
    public void SetPiece(Vector2Int[] toSet)
    {
        foreach (var set in toSet)
        {
            pieceMatrix[set.x,set.y] = true;
        }
    }
    public bool HasPiece(Vector2Int[] toCheck)
    {
        foreach (var set in toCheck)
        {
            if (pieceMatrix[set.x, set.y]) return true;
        }
        return false;
    }

    private void Awake()
    {
        instance = this;
    }
    public void StartMatch()
    {
        NextPiece();
    }

    void NextPiece()
    {
        if(lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)], spawPiecePosition,Quaternion.identity).GetComponent<Piece>();

        NetworkServer.Spawn(piece.gameObject);

        piece.OnPieceStop += NextPiece;

        OnSpawnPiece?.Invoke(piece);

        lastPiece = piece;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);


        for (int x = 0; x < arenaSize.x; x++)
        {
            for (int y = 0; y < arenaSize.y; y++)
            {
                if (y == 0 || x == 0 || x == (arenaSize.x-1))
                {
                    Gizmos.DrawWireCube(new(x, y), new(1, 1));
                }
            }
            
        }
    }
}
