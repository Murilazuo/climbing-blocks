using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterClientManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView view;
    
    public Dictionary<Player, bool> playersReady = new Dictionary<Player, bool>();
    public Dictionary<Player, PlayerType> playersType= new Dictionary<Player, PlayerType>();
    public PlayerType lastSelectType;
    [SerializeField] Color[] playerColors;
   
    public static System.Action<bool> OnPlayersReady;
    public static System.Action<bool> OnSetMasterClient;
    public static System.Action<bool> OnSetMatchIsStart;

    public static System.Action OnPlayerSetTeam;

    bool IsAllPlayersReady
    {
        get
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                return false;

            foreach (var isReady in playersReady)
            {
                if(isReady.Value == false)
                    return false;
            }
            return true;
        }
    }

    public static MasterClientManager Instance;
    bool IsMaster { get => PhotonNetwork.IsMasterClient; }
    Player MyPlayer { get => PhotonNetwork.LocalPlayer; }
    int PlayerCount { get => PhotonNetwork.CurrentRoom.PlayerCount; }

    public bool hasPiecePlayer;

    public bool characterIsFull;

    bool matchIsStart;
    int colorId;
    public Color GetPlayerColor(int playerID)
    {
        int id = 0;
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            print(player.Value.ActorNumber);
            if (playerID > player.Value.ActorNumber)
                id++;
        }

        return playerColors[id];
    }

    private void Awake()
    {
        Instance = this;    
    }
    private void Start()
    {
        OnSetMasterClient?.Invoke(IsMaster);
        if (IsMaster)
        {
            PlayerIconManager.Instance.AddPlayer(MyPlayer);
        }
        else
        {
            foreach(var player in PhotonNetwork.CurrentRoom.Players)
            {
                PlayerIconManager.Instance.AddPlayer(player.Value);
            }
        }

        
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        PlayerIconManager.Instance.SetMasterClient(newMasterClient);
        OnSetMasterClient?.Invoke(newMasterClient.IsLocal);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {

        PlayerIconManager.Instance.AddPlayer(newPlayer);

        playersReady.Add(newPlayer, false);
        playersType.Add(newPlayer, PlayerType.None);

        OnPlayersReady?.Invoke(false);

        CheckPlayerTeam();

        OnPlayerSetTeam?.Invoke();

        view.RPC(nameof(SetMatchStart), RpcTarget.All ,false);

    }
    [PunRPC]
    void SetMatchStart(bool isStart)
    {
        print("Set MatchStart " + isStart);
        matchIsStart = isStart;
        OnSetMatchIsStart?.Invoke(matchIsStart);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerIconManager.Instance.RemovePlayer(otherPlayer);

        if (IsMaster)
        {
            playersReady.Remove(otherPlayer);
            playersReady.TrimExcess();
            playersType.Remove(otherPlayer);
            playersType.TrimExcess();
            UpdateIsReady();
        }

        CheckPlayerTeam();
    }
    public void StartMatch()
    {
        MatchManager.Instance.StartCounter();
    }

    #region Ready
    void UpdateIsReady()
    {
        List<object> data = new List<object>();

        foreach(var isReady in playersReady)
        {
            data.Add(isReady.Key);
            data.Add(isReady.Value);
        }

        view.RPC(nameof(UpdateIsReadyOnClientes),RpcTarget.All,data.ToArray());
    }

    [PunRPC]
    void UpdateIsReadyOnClientes(object[] newDictionary)
    {
        if (!IsMaster)
        {
            playersReady.Clear();
            for(int i = 0; i < newDictionary.Length;i+= 2)
            {
                playersReady.Add((Player)newDictionary[i], (bool)newDictionary[i+1]);
            }
        }

        OnPlayersReady?.Invoke(IsAllPlayersReady);
    }

    public void ClienteSetReady(bool isReady)
    {
        object[] data = new object[2];

        data[0] = MyPlayer;
        data[1] = isReady;

        view.RPC(nameof(SetReady), RpcTarget.MasterClient, data);
    }

    [PunRPC]
    void SetReady(object[] data)
    {
        Player playerToSet = (Player)data[0];
        playersReady[playerToSet] = (bool)data[1];

        UpdateIsReady();
    }
    #endregion

    #region Team
    void UpdatePlayersTeam()
    {
        List<object> data = new List<object>();

        foreach (var playerTeam in playersType)
        {
            data.Add(playerTeam.Key);
            data.Add(playerTeam.Value);
        }

        view.RPC(nameof(UpdatePlayersTeamInClients), RpcTarget.All, data.ToArray());
    }
    [PunRPC]
    void UpdatePlayersTeamInClients(object[] newPlayersType)
    {
        if (!IsMaster)
        {
            playersType.Clear();
            for (int i = 0; i < newPlayersType.Length; i += 2)
            {
                Player player = (Player)newPlayersType[i];

                if (!player.IsInactive && player != null)
                    playersType.Add(player, (PlayerType)newPlayersType[i+1]);
            }
        }

        CheckPlayerTeam();

        OnPlayerSetTeam?.Invoke();
    }
    void CheckPlayerTeam()
    {
        int pieceCount = 0;
        int characterCount = 0;

        foreach (var playersType in playersType)
        {
            if (playersType.Value == PlayerType.Piece)
                pieceCount++;
            else if (playersType.Value == PlayerType.Character)
                characterCount++;
        }

        hasPiecePlayer = pieceCount >= 1;

        characterIsFull = characterCount == PhotonNetwork.CurrentRoom.PlayerCount-1 && PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }
    public void ClientSetTeam(PlayerType playerType)
    {
        object[] data = new object[2];

        data[0] = MyPlayer;
        data[1] = playerType;

        lastSelectType = playerType;

        view.RPC(nameof(SetTeam), RpcTarget.MasterClient, data);
    }
    [PunRPC]
    void SetTeam(object[] data)
    {
        Player playerToSet = (Player)data[0];
        playersType[playerToSet] = (PlayerType)data[1];

        UpdatePlayersTeam();
    }

    #endregion

    void EndGame(int endId, Vector2 position)
    {
        if (IsMaster)
        {
            foreach(var player in PhotonNetwork.CurrentRoom.Players)
            {
                playersType[player.Value] = PlayerType.None;
                playersReady[player.Value] = false;
            }

            UpdateIsReady();
            UpdatePlayersTeam();
            view.RPC(nameof(SetMatchStart), RpcTarget.All, false);
        }
    }
    private void StarGame()
    {
        if (IsMaster)
        {
           view.RPC(nameof(SetMatchStart), RpcTarget.All ,true);
        }
    }
    public override void OnEnable()
    {
        base.OnEnable();
        
        MatchManager.OnEndGame += EndGame;
        MatchManager.OnStarGame += StarGame;
    }
    public override void OnDisable()
    {
        base.OnDisable();

        MatchManager.OnEndGame -= EndGame;
        MatchManager.OnStarGame -= StarGame;
    }
}
