using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] LayerMask blockLayer;
    [SerializeField] PhotonView view;
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject explosionObject;
    int curretSecondsToExplode;


    public static Action OnBombExplode;
    private void Start()
    {
        LeanTween.delayedCall(1, PassOneSecond);
        curretSecondsToExplode = 3;
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


        foreach(var coll in Physics2D.OverlapCircleAll(transform.position,3))
            MatchManager.Instance.DestroyBlock((int)coll.gameObject.transform.position.x, (int)coll.gameObject.transform.position.y);

        
        Instantiate(explosionObject,transform.position,Quaternion.identity);
        if(view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3);
    }
}
