using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurorialPlayerAnim : MonoBehaviour
{
    [SerializeField] Animator animator;
    
    readonly int idleId = Animator.StringToHash("IsIdle");
    private void Start()
    {
        animator.SetBool(idleId, false);
    }
    private void OnEnable()
    {
        LeanTween.delayedCall(2, () =>
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1; 
            transform.localScale = localScale;
        }).setLoopPingPong();
    }
    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }
}
