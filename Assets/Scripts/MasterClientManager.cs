using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MasterClientManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView view;
    
    public Dictionary<Player, bool> playersReady = new Dictionary<Player, bool>();
    public Dictionary<Player, PlayerType> playersType= new Dictionary<Player, PlayerType>();

    [SerializeField] Color[] playerColors;
   
    public static System.Action<bool> OnPlayersReady;
    public static System.Action<bool> OnSetMasterClient;

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

    public bool hasSpaceToCharacters;

    public static int GetPlayerId(int playerNumber)
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (playerNumber == PhotonNetwork.CurrentRoom.Players[i].ActorNumber)
                return i - 1;
        }
        return 0;
    }

    public Color GetPlayerColor(int playerID)
    {
        return playerColors[playerID];
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
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerIconManager.Instance.AddPlayer(newPlayer);

        if (IsMaster)
        {
            playersReady.Add(newPlayer, false);
            playersType.Add(newPlayer, PlayerType.None);
        }

        UpdateIsReady();
        UpdatePlayersTeam();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (IsMaster)
        {
            playersReady.Remove(otherPlayer);
            playersReady.TrimExcess();
            UpdateIsReady();
        }

        PlayerIconManager.Instance.RemovePlayer(otherPlayer);
    }
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
                playersType.Add((Player)newPlayersType[0], (PlayerType)newPlayersType[1]);
            }
        }

        int pieceCount = 0;
        int characterCount = 0;

        foreach (var playersType in playersType)
        {
            if (playersType.Value == PlayerType.Piece)
                pieceCount++;
            else if (playersType.Value == PlayerType.Character)
                characterCount++;
        }

        hasPiecePlayer = pieceCount == 0;
        hasSpaceToCharacters = true;
            
            //characterCount < PhotonNetwork.CurrentRoom.PlayerCount - 1 || PhotonNetwork.CurrentRoom.PlayerCount == 1;

        OnPlayerSetTeam?.Invoke();
    }
    public void ClientSetTeam(PlayerType playerType)
    {
        object[] data = new object[2];

        data[0] = MyPlayer;
        data[1] = playersType;

        view.RPC(nameof(SetTeam), RpcTarget.MasterClient, data);
    }
    void SetTeam(object[] data)
    {
        Player playerToSet = (Player)data[0];
        playersType[playerToSet] = (PlayerType)data[1];

        UpdatePlayersTeam();
    }
    public void StartMatch()
    {
        if(IsAllPlayersReady)
            MatchManager.Instance.StartCounter();
    }
}
