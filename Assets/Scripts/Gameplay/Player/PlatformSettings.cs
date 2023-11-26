using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Settings/Player Settings")]
public class PlatformSettings : ScriptableObject
{
    [Header("Move")]
    [SerializeField] float speed;
    public float Speed { get => speed; }
    public float TimeToSnap { get => TimeToSnap; }
    [SerializeField] float timeToSnap;
    [Header("Danger Area")]
    [SerializeField] float timeToDie;
    public float TimeToDieInDangerZone { get => timeToDie; }

    [Header("Jump")]
    [SerializeField] float coyoteJumpTime;
    public float CoyoteJumpTime { get => coyoteJumpTime; }
    public float GravityScale { get => gravityScale; }
    [SerializeField] float gravityScale;
    public float JumpForce { get => jumpForce; }
    [SerializeField] float jumpForce;
    public float StartJumpTime { get => startJumpTimer; }
    [SerializeField] float startJumpTimer;
 
    [Header("Attack")]
    [SerializeField] float attackDelay;
    public float AttackDelay { get => attackDelay; }
}
