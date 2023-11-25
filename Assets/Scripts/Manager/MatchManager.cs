using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType { Piece, Character, None}
public class MatchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView view;
    static MatchManager instance;

    public static MatchManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<MatchManager>();
            return instance;
        }
    }

    public static System.Action OnStarGame;
    public static System.Action OnStarCounter;
    public static System.Action<int> OnEndGame;
    public static System.Action OnUpdatePlayerSelect;
    public static System.Action OnPlayAgain;
    public static System.Action<PlayerType> OnSelectPlayerType;

    public bool HasSpaceToCharacters { get
        {
            return FindObjectsOfType<CharacterSelect>().Length < PhotonNetwork.CurrentRoom.PlayerCount-1 || PhotonNetwork.CurrentRoom.PlayerCount == 1;
        } }
    public bool HasPiecePlayer { get => FindObjectsOfType<PieceSelect>().Length == 1; }
    PlayerSelect currentSelect;
    [SerializeField] GameObject selectCharacter;
    [SerializeField] GameObject selectPiece;
    private void Awake()
    {
        instance = this;

        print("Is Master " + PhotonNetwork.IsMasterClient);
    }

    public const byte PIECE_COLIDE_WITH_PLATFORM_EVENT = 3;

    public void PlayerDrowned()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PLATFORM_DROWNED_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PLATFORM_DROWNED_EVENT);
    }
    public void PlatformReachTop()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PLATFORM_REACH_TOP_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PLATFORM_REACH_TOP_EVENT);
    }
    public void PieceCollideWithPieceReachTop()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PIECE_COLIDE_WITH_PLATFORM_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_COLIDE_WITH_PLATFORM_EVENT);
    }
    public void PieceReachTop()
    {
        OnEndGame?.Invoke(NetworkEventSystem.PIECE_REACH_TOP_EVENT);
        NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_REACH_TOP_EVENT);
    }

    bool matchStarted = false;
    public void StartMatch()
    {
        if (matchStarted) return;

        matchStarted = true;
        OnStarGame?.Invoke();
        NetworkEventSystem.CallEvent(NetworkEventSystem.START_MATCH_EVENT);
    }
    public void StartCounter()
    {
        OnStarCounter?.Invoke();
        NetworkEventSystem.CallEvent(NetworkEventSystem.START_COUNTER_EVENT);
    }
    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
    public void PlayAgain()
    {
        OnPlayAgain?.Invoke();
        matchStarted = false;
    }

    public static System.Action<Vector2> OnDestroyBlock;
    public void DestroyBlock(int x, int y)
    {
        Vector2 blockPosition = new Vector2(x, y);
        OnDestroyBlock?.Invoke(blockPosition);
    }
    PlayerReady playerReady;
    [SerializeField] GameObject playerReadyPrefab;
    public void SerIsReady(bool isReady)
    {
        if(isReady)
        {
            playerReady = PhotonNetwork.Instantiate(playerReadyPrefab.name, Vector2.zero, Quaternion.identity).GetComponent<PlayerReady>();
        }
        else
        {
            if (playerReady)
            {
                if (playerReady.IsMine)
                {
                    PhotonNetwork.Destroy(playerReady.gameObject);
                    playerReady = null;
                }
            }
        }

        if(PhotonNetwork.CurrentRoom.PlayerCount > 1 && FindObjectsOfType<PlayerReady>().Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            StartCounter();
        }
    }

    public void SelectPlayerType(PlayerType newPlayerType)
    {
        switch (newPlayerType)
        {
            case PlayerType.Piece:
                if (!HasPiecePlayer)
                {
                    currentSelect = PhotonNetwork.Instantiate(selectPiece.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerSelect>();
                    OnSelectPlayerType?.Invoke(PlayerType.Piece);
                    NetworkEventSystem.CallEvent(NetworkEventSystem.UPDATE_PLAYERS_SELECT_EVENT);
                }
                break;
            case PlayerType.Character:
                if (HasSpaceToCharacters)
                {
                    currentSelect = PhotonNetwork.Instantiate(selectCharacter.name, Vector3.zero, Quaternion.identity).GetComponent<PlayerSelect>();
                    OnSelectPlayerType?.Invoke(PlayerType.Character);
                    NetworkEventSystem.CallEvent(NetworkEventSystem.UPDATE_PLAYERS_SELECT_EVENT);
                }
                break;
            case PlayerType.None:
                if (currentSelect)
                {
                    if (currentSelect.IsMine)
                    {
                        PhotonNetwork.Destroy(currentSelect.gameObject);
                    }
                }
                OnSelectPlayerType?.Invoke(PlayerType.None);
                NetworkEventSystem.CallEvent(NetworkEventSystem.UPDATE_PLAYERS_SELECT_EVENT);
                break;
        }
    }
    

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEndGameEvent;
    }
    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEndGameEvent;
    }
    void OnEndGameEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case NetworkEventSystem.PIECE_COLIDE_WITH_PLATFORM_EVENT:
            case NetworkEventSystem.PLATFORM_REACH_TOP_EVENT:
            case NetworkEventSystem.PIECE_REACH_TOP_EVENT:
            case NetworkEventSystem.PLATFORM_DROWNED_EVENT:
                OnEndGame?.Invoke(eventData.Code);
                break;
            case NetworkEventSystem.START_MATCH_EVENT:
                if (!matchStarted)
                {
                    matchStarted = true;
                    OnStarGame?.Invoke();
                }
                break;
            case NetworkEventSystem.START_COUNTER_EVENT:
                OnStarCounter?.Invoke();
                break;
        }
    }
}
