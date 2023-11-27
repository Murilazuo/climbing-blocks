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

    [SerializeField] Transform otherPlayersHolder;
    [SerializeField] Transform myPlayersHolder;

    public static PlayerIconManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    bool meWasAdd;

    public void AddPlayer(Player player)
    {
        Transform holder = meWasAdd ? otherPlayersHolder : myPlayersHolder;
        meWasAdd = true;

        PlayersIcon playerIcon = Instantiate(playerIconPrefab,holder).GetComponent<PlayersIcon>();

        playerIcon.SetColor(MasterClientManager.Instance.GetPlayerColor(player.ActorNumber-1));
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

    void EndGame(int endId)
    {
        foreach(var icon in playerIcons)
        {
            icon.Value.SetReady(notReadyButton);
            icon.Value.SetTeam(teamsSprites[2]);
        }
    }

    private void OnEnable()
    {
        MasterClientManager.OnPlayersReady += UpdatePlayerReady;
        MasterClientManager.OnPlayerSetTeam += UpdatePlayerTeam;
        MatchManager.OnEndGame += EndGame;
    }
    private void OnDisable()
    {
        MasterClientManager.OnPlayersReady -= UpdatePlayerReady;
        MasterClientManager.OnPlayerSetTeam -= UpdatePlayerTeam;
        MatchManager.OnEndGame -= EndGame;
    }
}
