using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Team { None, Piece, Platform}
public class NetworkPlayer : MonoBehaviourPun
{
    int playerId;
    Color color;
    [SerializeField]
    Color[] colors;
    [SerializeField] Image image;
    [SerializeField] Sprite pieceSprite, platformSprite, noneSprite;
    [SerializeField] PhotonView view;
    [SerializeField] float piecePosition, platformPosition;
    [SerializeField] float timeToMove;
    [SerializeField] float startPosition;
    [SerializeField] float spacing;

    float AnchoredPositionX { get => (transform as RectTransform).anchoredPosition.x;
        set
        {
            Vector2 pos = (transform as RectTransform).anchoredPosition;
            pos.x = value;
            (transform as RectTransform).anchoredPosition = pos;
        }
    }
    bool HasPiece
    {
        get
        {
            foreach (Transform t in transform.parent)
                if(t.transform.localPosition.x < 0)
                    return true;
            return false;
        }
    }
    public void Init(int playerId)
    {
        this.playerId = playerId;
        image.color = colors[playerId];
        transform.SetParent(GameObject.Find("Players").transform);

        AnchoredPositionX = 0;
        Vector2 pos = (transform as RectTransform).anchoredPosition;
        pos.y = startPosition - (spacing * playerId);
        (transform as RectTransform).anchoredPosition = pos;
    }

    private void Update()
    {
        if (!view.IsMine) return;

        if (Input.GetButtonDown("Horizontal"))
        {
            print("Horizontal");
            SelectTeam((int)Input.GetAxisRaw("Horizontal"));
        }
    }

    void SelectTeam(int dir)
    {
        print("Select Team " + dir);
        if (dir < 0 )
        {
            if(AnchoredPositionX > 0)
            {
                OnSelectTeam(Team.None);
                AnchoredPositionX = 0;
            }
            else
            {
                OnSelectTeam(Team.Piece);
                AnchoredPositionX = piecePosition;
            }
        }
        else if (dir > 0)
        {
            if (AnchoredPositionX < 0)
            {
                OnSelectTeam(Team.None);
                AnchoredPositionX = 0;
            }
            else
            {
                OnSelectTeam(Team.Platform);
                AnchoredPositionX = platformPosition;
            }
        }
    }
    void OnSelectTeam(Team team)
    {
        SelectTeam(team);
        object[] data = new object[]
        {
            team,
            playerId,
        };

        NetworkEventSystem.CallEvent(NetworkEventSystem.SELECT_TEAM_EVENT, data);
    }
    void ClientSelectTeam(EventData data)
    {
        if (NetworkEventSystem.SELECT_TEAM_EVENT == data.Code)
        {
            object[] datas = (object[])data.CustomData;
         
            if (playerId == (int)datas[1])
                SelectTeam((Team)datas[0]);
        }
    }
    void SelectTeam(Team team)
    {
        if(team == Team.Piece)
        {
            image.sprite = pieceSprite;

        }
        else if (team == Team.Platform)
        {
            image.sprite = platformSprite;
        }else
        {
            image.sprite = noneSprite;
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += ClientSelectTeam;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= ClientSelectTeam;
    }
}