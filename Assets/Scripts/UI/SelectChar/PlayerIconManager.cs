using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PlayerIconManager : MonoBehaviour
{
    [SerializeField] GameObject playerIconPrefab;
    Dictionary<Player, PlayersIcon> playerIcons = new Dictionary<Player, PlayersIcon>();

    [SerializeField] Sprite readyButton, notReadyButton;
    [SerializeField] Sprite[] teamsSprites;

    public static PlayerIconManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayer(Player player)
    {
        PlayersIcon playerIcon = Instantiate(playerIconPrefab,transform).GetComponent<PlayersIcon>();

        playerIcon.SetColor(MasterClientManager.Instance.GetPlayerColor(player.ActorNumber-1));
        print("Is Mine " + (player == PhotonNetwork.LocalPlayer));
        print("Player actor num " + player.ActorNumber);
        print("Player Id " + MasterClientManager.GetPlayerId(player.ActorNumber));
        playerIcons.Add(player, playerIcon);
    }
    public void RemovePlayer(Player player)
    {
        Destroy(playerIcons[player].gameObject);

        playerIcons.Remove(player);
    }

    void UpdatePlayerReady(bool allPlayersReady)
    {
        foreach(var playerReady in MasterClientManager.Instance.playersReady)
            playerIcons[playerReady.Key].SetReady(playerReady.Value ? readyButton : notReadyButton);
    }

    void UpdatePlayerTeam()
    {
        foreach (var playerType in MasterClientManager.Instance.playersType)
            playerIcons[playerType.Key].SetTeam(teamsSprites[(int)playerType.Value]);
    }

    private void OnEnable()
    {
        MasterClientManager.OnPlayersReady += UpdatePlayerReady;
        MasterClientManager.OnPlayerSetTeam += UpdatePlayerTeam;
    }
    private void OnDisable()
    {
        MasterClientManager.OnPlayersReady -= UpdatePlayerReady;
        MasterClientManager.OnPlayerSetTeam += UpdatePlayerTeam;
    }
}