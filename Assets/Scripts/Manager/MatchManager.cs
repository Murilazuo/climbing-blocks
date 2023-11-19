using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    
    private void Awake()
    {
        instance = this;
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
    public static System.Action<Vector2> OnDestroyBlock;
    public void DestroyBlock(int x, int y)
    {
        Vector2 blockPosition = new Vector2(x, y);
        OnDestroyBlock?.Invoke(blockPosition);
    }
    bool[] playersReady = new bool[2];
    int count = 0;
    public void SerIsReady(bool isReady, int playerId)
    {
        view.RPC(nameof(RPCSetIsReady),RpcTarget.All, isReady, playerId);
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
        playersReady[playerId-1] = isReady;
        print($"Player {playerId} isRead {isReady}");
        print($"Player 0 isRead {playersReady[0]}");
        print($"Player 1 isRead {playersReady[1]}");
            
        if (playersReady[0] && playersReady[1])
            StartCounter();
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
