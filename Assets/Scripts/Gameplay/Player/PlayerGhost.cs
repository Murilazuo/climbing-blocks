using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerDeathId { Drow, Smash}
public class PlayerGhost : MonoBehaviour
{
    [SerializeField] float toMove;
    [SerializeField] float timeMove;
    [SerializeField] float secondMove;
    [SerializeField] float timeToDisable;
    [SerializeField] PhotonView photonView;

    bool IsLastCharcter { get => FindObjectsOfType<PlayerPlatform>().Length <= 1; }

    public void Init(PlayerDeathId deathId)
    {
        LeanTween.delayedCall(toMove / 2, () => CheckDeath(deathId));
        LeanTween.moveY(gameObject, transform.position.y + toMove, timeMove).setOnComplete(() =>
        {
            CheckDeath(deathId);
            LeanTween.moveY(gameObject, transform.position.y + secondMove, timeToDisable).setOnComplete(() =>
            {
                if(photonView.IsMine) 
                    PhotonNetwork.Destroy(gameObject);
            });
        });
    }
    bool isLast = false;
    void CheckDeath(PlayerDeathId deathId)
    {
        if (IsLastCharcter && !isLast)
        {
            isLast = true;
            switch (deathId)
            {
                case PlayerDeathId.Drow:
                    MatchManager.Instance.PlayerDrowned(transform.position);
                    break;
                case PlayerDeathId.Smash:
                    MatchManager.Instance.PieceCollideWithPieceReachTop(transform.position);
                    break;
            }
        }
    }

}
