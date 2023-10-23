using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static System.Action OnPlayerPieceWin;
    public static System.Action OnPlayerPlatformWin;

    public static System.Action<Piece> OnSpawnPiece;
    [SerializeField] GameObject[] piecePrefabs;
    [SerializeField] Vector2 spawPiecePosition;
   
    public Vector2 playerPlatformSpawnPosition;
    Piece lastPiece;

    [SerializeField] GameObject arenaPartObj;

    [SerializeField] Vector2Int arenaSize;

    [SerializeField] bool [,] pieceMatrix;


    bool GetTileState(Vector2Int index) => pieceMatrix[index.x,index.y];
    void SetTileState(Vector2Int index, bool value) => pieceMatrix[index.x, index.y] = value;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Time.timeScale = 1;

        pieceMatrix = new bool[arenaSize.x , arenaSize.y];

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if(y == 0 || x == 0 || x == (arenaSize.x - 1))
                    pieceMatrix[x, y] = true;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x-1))
                    Gizmos.DrawWireCube(new(x, y), new(1, 1));
    }

    public void PlayerPiecesWin()
    {
        OnPlayerPieceWin?.Invoke();
        Time.timeScale = 0;
    }
    public void PlayerPlatformWin()
    {
        Time.timeScale = 0;
        OnPlayerPlatformWin?.Invoke();
    }
    public void GoToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
