using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] FloatVariable explosionSize;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] PhotonView view;
    [SerializeField] TMP_Text text;
    [SerializeField] IntVariable secondsToExplode;
    int curretSecondsToExplode;


    public static Action OnBombExplode;
    private void Start()
    {
        LeanTween.delayedCall(1, PassOneSecond);
        curretSecondsToExplode = secondsToExplode.Value;
        text.text = curretSecondsToExplode.ToString();
    }

    void PassOneSecond()
    {
        curretSecondsToExplode--;

        if (curretSecondsToExplode == 0)
            LeanTween.delayedCall(.5f, Explode);
        else
            LeanTween.delayedCall(1, PassOneSecond);

        text.text = curretSecondsToExplode.ToString();
    }
    void Explode()
    {
        OnBombExplode?.Invoke();


        foreach(var coll in Physics2D.OverlapCircleAll(transform.position, explosionSize.Value))
            MatchManager.Instance.DestroyBlock(coll.gameObject);

        if(view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionSize.Value);
    }
}
