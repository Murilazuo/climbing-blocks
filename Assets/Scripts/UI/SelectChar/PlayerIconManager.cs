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

        playerIcon.SetColor(MasterClientManager.Instance.GetPlayerColor(player.ActorNumber));
        playerIcons.Add(player, playerIcon);

        playerIcon.SetMasterClient(player.IsMasterClient);
    }
    public void RemovePlayer(Player player)
    {
        print("Remove Player");
        Destroy(playerIcons[player].gameObject);
        playerIcons.Remove(player);
        playerIcons.TrimExcess();
    }

    void UpdatePlayerReady(bool allPlayersReady)
    {
        foreach(var playerReady in MasterClientManager.Instance.playersReady)
        {
            if (playerReady.Key != null)
                playerIcons[playerReady.Key].SetReady(playerReady.Value ? readyButton : notReadyButton);
        }
    }

    void UpdatePlayerTeam()
    {
        foreach (var playerType in MasterClientManager.Instance.playersType)
        {

            if(playerType.Key != null)
                playerIcons[playerType.Key].SetTeam(teamsSprites[(int)playerType.Value]);
        }
    }

    void EndGame(int endId, Vector2 position)
    {
        foreach(var icon in playerIcons)
        {
            icon.Value.SetReady(notReadyButton);
            icon.Value.SetTeam(teamsSprites[2]);
        }
    }
    public void SetMasterClient(Player masterClient)
    {
        playerIcons[masterClient].SetMasterClient(true);
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
