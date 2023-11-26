using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersIcon : MonoBehaviour
{
    [SerializeField] Image teamIcon;
    [SerializeField] Image readyImage;
    public void SetReady(Sprite spriteReady)
    {
        readyImage.sprite = spriteReady;
    }
    public void SetTeam(Sprite spriteTeam)
    {
        teamIcon.sprite = spriteTeam;
    }
    public void SetColor(Color color)
    {
        teamIcon.color = color;
    }
}
