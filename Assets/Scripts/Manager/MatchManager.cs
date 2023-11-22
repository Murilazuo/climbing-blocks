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

    [SerializeField] CanvasGroup endGameCanvasGroup;
    [SerializeField] GameObject tutorialPanel;
    [SerializeField] SelectTeamController selectTeamController;

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
    public static System.Action<PlayerType> OnSelectPlayerType;

    public bool HasSpaceToCharacters { get
        {
            return FindObjectsOfType<CharacterSelect>().Length < PhotonNetwork.CurrentRoom.PlayerCount-1 || PhotonNetwork.CurrentRoom.PlayerCount == 1;
        } }
    public bool HasPiecePlayer { get => FindObjectsOfType<PieceSelect>().Length == 1; }
    PlayerSelect currentSelect;
    [SerializeField] GameObject selectCharacter;
    [SerializeField] GameObject selectPiece;
    [SerializeField] GameObject playAgain;
   
    private void Awake()
    {
        instance = this;

        print("Players Count in room" + PhotonNetwork.CurrentRoom.PlayerCount);

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            playersReady.Add(false);
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
        /*
        tutorialPanel.SetActive(true);
        endGameCanvasGroup.blocksRaycasts = false;
        endGameCanvasGroup.alpha = 0;
        selectTeamController.ActivePanel();

        if (currentSelect)
            PhotonNetwork.Destroy(currentSelect.gameObject);
         */
        //PhotonNetwork.DestroyAll();



        PhotonNetwork.LoadLevel(2);
    }

    public static System.Action<Vector2> OnDestroyBlock;
    public void DestroyBlock(int x, int y)
    {
        Vector2 blockPosition = new Vector2(x, y);
        OnDestroyBlock?.Invoke(blockPosition);
    }
    List<bool> playersReady = new List<bool>();
    int count = 0;
    PlayerReady playerReady;
    [SerializeField] GameObject playerReadyPrefab;
    public void SerIsReady(bool isReady, int playerId)
    {
        if(isReady)
        {
            PhotonNetwork.Instantiate(playerReadyPrefab.name, Vector2.zero, Quaternion.identity);
        }
        else
        {
            if (playerReady)
            {
                PhotonNetwork.Destroy(playerReady.gameObject);
                playerReady = null;
            }
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount > 1 && FindObjectsOfType<PlayerReady>().Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            StartCounter();
        }

        //view.RPC(nameof(RPCSetIsReady),RpcTarget.All, isReady, playerId);
        if (Application.isEditor)
        {
            count++;
            if(count >= 5)
                StartCounter();
        }
    }
    [PunRPC]
    void RPCSetIsReady(bool isReady, int playerId)
    {
        

        if(playersReady.Count < PhotonNetwork.CurrentRoom.PlayerCount)
        {
            for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount - playersReady.Count; i++)
                playersReady.Add(false);
        }
            
        playersReady[playerId-1] = isReady;

        if (!(playersReady.Contains(false)))
            StartCounter();
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
                if(currentSelect)
                    PhotonNetwork.Destroy(currentSelect.gameObject);
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
