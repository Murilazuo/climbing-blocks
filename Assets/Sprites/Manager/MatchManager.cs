using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    readonly SyncList<bool> pieceMatrix = new SyncList<bool>();
    public static int Get2Dto1DIndex(Vector2Int index) => index.x + (index.y * Instance.arenaSize.x);
    bool GetTileState(Vector2Int index) => pieceMatrix[Get2Dto1DIndex(index)];
    void SetTileState(Vector2Int index, bool value) => pieceMatrix[Get2Dto1DIndex(index)] = value;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                pieceMatrix.Add(y == 0 || x == 0 || x == (arenaSize.x - 1));
    }

    public void SetPiece(Vector2Int[] toSet)
    {
        foreach (var set in toSet)
        {
            SetTileState(set, true);
        }
    }
    public bool HasPiece(Vector2Int[] toCheck)
    {
        foreach (var pos in toCheck)
        {
            if (GetTileState(pos)) return true;
        }
        return false;
    }

    [Server]    
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
#if UNITY_EDITOR
    [SerializeField] bool[] gridDebug;
    private void FixedUpdate()
    {
        gridDebug = pieceMatrix.ToArray();
    }
#endif
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
