using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    float moveDelayTimer;
    float moveSpeedDelayTimer;
    float moveHoldTimer;

    public PieceSettings settings;
    
    [SerializeField] GameObject piecePrefab;
    [SerializeField] Vector2 spawPiecePosition;
    Piece lastPiece;
    [SerializeField] Vector2Int arenaSize;
    [SerializeField] bool[,] pieceMatrix;

    bool canMove;

    public static System.Action OnNextPiece;

    public static PieceController Instance;
    bool GetTileState(Vector2Int index) => pieceMatrix[index.x, index.y];
    void SetTileState(Vector2Int index, bool value) => pieceMatrix[index.x, index.y] = value;
    public void SetPiece(Vector2Int[] toSet)
    {
        foreach (var set in toSet)
            SetTileState(set, true);
    }
    public bool HasPiece(Vector2Int[] toCheck)
    {
        foreach (var pos in toCheck)
            if (GetTileState(pos)) return true;
        return false;
    }
    private void Awake()
    {
        pieceMatrix = new bool[arenaSize.x, arenaSize.y];

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    pieceMatrix[x, y] = true;

        Instance = this;
        canMove = true;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    void PieceGravity()
    {
        if (Piece.currentPiece)
        {
            MoveDown();
            Invoke(nameof(PieceGravity), settings.PieceGravity);
        } 
    }
    private void Update()
    {
        if (Piece.currentPiece && canMove)
        {
            moveDelayTimer += Time.deltaTime;
            
            if(Input.GetButton("Horizontal") || Input.GetButton("MoveDown"))
            {
                moveHoldTimer += Time.deltaTime;
            }

            if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("MoveDown"))
            {
                moveHoldTimer = 0;
                moveSpeedDelayTimer = 0;
            }

            if (Input.GetButtonDown("Horizontal"))
            {
                Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));
            }
            if (Input.GetButtonDown("MoveDown"))
            {
                MoveDown();
            }

            bool canMove = moveDelayTimer >= settings.MoveDelay;
            bool isSpeedMoving = moveHoldTimer >= settings.HoldTimeToMove;

            if (isSpeedMoving)
            {
                moveSpeedDelayTimer += Time.deltaTime;

                if(moveSpeedDelayTimer >= settings.TimeToMoveInSpeed)
                {
                    moveSpeedDelayTimer = 0;
                    if (Input.GetButton("Horizontal"))
                        Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));

                    if (Input.GetButton("MoveDown"))
                        MoveDown();
                }
            }
            else
            {
                /*
                if (Input.GetButtonDown("Horizontal"))
                {
                    moveDelayTimer = 0;
                    Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));

                }

                if (Input.GetButtonDown("MoveDown"))
                {
                    moveDelayTimer = 0;
                    MoveDown();
                } 
                 */
            }
        }
    }
    void MoveDown()
    {
        Piece.currentPiece.MoveDown();
        CancelInvoke(nameof(PieceGravity));
        Invoke(nameof(PieceGravity), settings.PieceGravity);
    }
    public void StartMatch()
    {
        NextPiece();
        Invoke(nameof(PieceGravity), settings.PieceGravity);
    }
    void NextPiece()
    {
        if (lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = PhotonNetwork.Instantiate(piecePrefab.name, spawPiecePosition, Quaternion.identity).GetComponent<Piece>();
        piece.OnPieceStop += NextPiece;

        lastPiece = piece;
    }
    private void EndGame(int i)
    {
        canMove = false;
        CancelInvoke(nameof(PieceGravity));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    Gizmos.DrawWireCube(new(x, y), new(1, 1));
    }
    void OnOpenEndGamePanel()
    {
        Destroy(gameObject);
    }
    public void OnEnable()
    {
        MatchManager.OnStarGame += StartMatch;
        MatchManager.OnDestroyBlock += OnDestroyBlock;
        MatchManager.OnEndGame += EndGame;
        EndGamePanel.OnOpenEndGamePanel += OnOpenEndGamePanel;
    }


    public void OnDisable()
    {
        MatchManager.OnStarGame -= StartMatch;
        MatchManager.OnDestroyBlock -= OnDestroyBlock;
        MatchManager.OnEndGame -= EndGame;
        EndGamePanel.OnOpenEndGamePanel -= OnOpenEndGamePanel;
    }

    private void OnDestroyBlock(Vector2 blockPosition)
    {
        print("DestroyBlock - piece controller");

        SetTileState(new((int)blockPosition.x, (int)blockPosition.y), false);
    }
}
