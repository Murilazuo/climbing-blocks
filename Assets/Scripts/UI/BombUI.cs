using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] GameObject keyObject;
    [SerializeField] CanvasGroup canvasGroup;
    void OnSetBombTimer(float time)
    {
        image.fillAmount = time;
        if(time >= 1) keyObject.SetActive(true);
    }
    void DisableKey()
    {
        keyObject.SetActive(false);
    }
    void Init() => canvasGroup.alpha = 1f;
    private void OnEnable()
    {
        PlayerPlatform.OnSetAttackTimer += OnSetBombTimer;
        PlayerPlatform.OnSpawnPlayerPlatform += Init;
        Bomb.OnBombExplode += DisableKey;
    }
    private void OnDisable()
    {
        PlayerPlatform.OnSetAttackTimer -= OnSetBombTimer;
        PlayerPlatform.OnSpawnPlayerPlatform -= Init;
        Bomb.OnBombExplode -= DisableKey;
    }

}
