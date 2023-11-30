using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchUI : MonoBehaviour
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
    void Disable(int eventId, Vector2 position) => canvasGroup.alpha = 0f;
    private void OnEnable()
    {
        MatchManager.OnEndGame += Disable;
        PlayerPlatform.OnSetAttackTimer += OnSetBombTimer;
        PlayerPlatform.OnSpawnPlayerPlatform += Init;
        Bomb.OnBombExplode += DisableKey;
    }
    private void OnDisable()
    {
        MatchManager.OnEndGame -= Disable;
        PlayerPlatform.OnSetAttackTimer -= OnSetBombTimer;
        PlayerPlatform.OnSpawnPlayerPlatform -= Init;
        Bomb.OnBombExplode -= DisableKey;
    }

}
