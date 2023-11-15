using ExitGames.Client.Photon;
using Photon.Pun;
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
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    void PieceGravity()
    {
        if(Piece.currentPiece)
            Piece.currentPiece.MoveDown();
    }
    private void Update()
    {
        if (Piece.currentPiece)
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
                        Piece.currentPiece.MoveDown();
                }
            }
            else
            {
                if (Input.GetButtonDown("Horizontal") && canMove)
                {
                    moveDelayTimer = 0;
                    Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));

                }

                if (Input.GetButtonDown("MoveDown") && canMove)
                {
                    moveDelayTimer = 0;
                    Piece.currentPiece.MoveDown();
                } 
            }
        }
    }
    public void StartMatch()
    {
        print("Piece Controller Start Match");
        NextPiece();
        InvokeRepeating(nameof(PieceGravity), settings.PieceGravity, settings.PieceGravity);
    }
    void NextPiece()
    {
        if (lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = PhotonNetwork.Instantiate(piecePrefab.name, spawPiecePosition, Quaternion.identity).GetComponent<Piece>();
        piece.OnPieceStop += NextPiece;

        lastPiece = piece;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    Gizmos.DrawWireCube(new(x, y), new(1, 1));
    }

    public void OnEnable()
    {
        MatchManager.OnStarGame += StartMatch;
        MatchManager.OnDestroyBlock += OnDestroyBlock;
    }

    public void OnDisable()
    {
        MatchManager.OnStarGame -= StartMatch;
        MatchManager.OnDestroyBlock -= OnDestroyBlock;
    }

    private void OnDestroyBlock(Vector2 blockPosition)
    {
        print("DestroyBlock - piece controller");

        SetTileState(new((int)blockPosition.x, (int)blockPosition.y), false);
    }
}
