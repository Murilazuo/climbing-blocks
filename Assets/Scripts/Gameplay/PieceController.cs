using ExitGames.Client.Photon;
using Photon.Pun;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    float moveDelayTimer;
    float moveSpeedDelayTimer;
    float moveHoldTimer;

    public float moveDelay;
    public float moveHoldTimeToSpeedMove;
    public float moveTimeToMoveSpeed;
    public float timeToMoveDown;

    [SerializeField] GameObject[] piecePrefabs;
    [SerializeField] Vector2 spawPiecePosition;
    Piece lastPiece;
    [SerializeField] Vector2Int arenaSize;
    [SerializeField] bool[,] pieceMatrix;

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

            bool canMove = moveDelayTimer >= moveDelay;
            bool isSpeedMoving = moveHoldTimer >= moveHoldTimeToSpeedMove;

            if (isSpeedMoving)
            {
                moveSpeedDelayTimer += Time.deltaTime;

                if(moveSpeedDelayTimer >= moveTimeToMoveSpeed)
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
        InvokeRepeating(nameof(PieceGravity), timeToMoveDown, timeToMoveDown);
    }
    public void SetGravity(float gravity)
    {
        timeToMoveDown = gravity;
        CancelInvoke(nameof(PieceGravity));
        InvokeRepeating(nameof(PieceGravity), timeToMoveDown, timeToMoveDown);
    }
    void NextPiece()
    {
        if (lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = PhotonNetwork.Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)].name, spawPiecePosition, Quaternion.identity).GetComponent<Piece>();
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
        PhotonNetwork.NetworkingClient.EventReceived += NetworkCliente_RisedEvent;
    }

    public void OnDisable()
    {
        MatchManager.OnStarGame -= StartMatch;
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkCliente_RisedEvent;
    }

    private void NetworkCliente_RisedEvent(EventData eventData)
    {
        if(eventData.Code == NetworkEventSystem.PIECE_DESTROY_EVENT)
        {
            object[] data = (object[])eventData.CustomData;
            Vector3 blockPosition = (Vector3)data[0];
         
            SetTileState(new((int)blockPosition.x, (int)blockPosition.y), false);
        }
    }
}
