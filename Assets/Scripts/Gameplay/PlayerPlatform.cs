using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    [SerializeField] Vector2 spawnPlatformPosition;
    
    [Header("Move")]
    [SerializeField] FloatVariable speed;
    
    [Header("Jump")]
    [SerializeField] FloatVariable jumpForce;
    [SerializeField] FloatVariable gravityScale;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] FloatVariable startJumpTime;
    float jumpTime;
    bool isJumping;

    [Header("Attack 1")]
    [SerializeField] Transform punchPivot;
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float timeToDisablePunch;
    Ray attackRayCast;
    [Header("Attack 2")]
    [SerializeField] GameObject bombObject;
    [SerializeField] FloatVariable bombDelay;
    float bombTimer;
    public static System.Action<float> OnSetBomTimer;
    public static System.Action OnSpawnPlayerPlatform;

    [Header("Component")]
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] SpriteRenderer playerRenderer;
    
    [Header("Anim")]
    [SerializeField] Animator anim;
    readonly int lookRightId = Animator.StringToHash("LookRight");
    bool InGrounded { get => Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask);}
    bool HasGroundAbove { get => Physics2D.Raycast(rig.position, Vector2.up, groundCheckDistance, layerMask); }
    float lastInputX;
    bool canMove;
    private void Start()
    {
        canMove = false;
        if (view.IsMine)
        {
            rig.gravityScale = gravityScale.Value;
            OnSpawnPlayerPlatform?.Invoke();
        }
        else
        {
            rig.simulated = false;
        }
    }
    void Update()
    {
        if (!view.IsMine || !canMove) return;
        BombUpdate();
        AnimationUpdate();
        JumpUpdate();
        PlayerAttackUpdate();
    }
    void AnimationUpdate()
    {
        if (Input.GetButton("Horizontal"))
            lastInputX = Input.GetAxisRaw("Horizontal");

        if (lastInputX > 0) anim.SetBool(lookRightId, true);
        else if (lastInputX < 0) anim.SetBool(lookRightId, false);
    }
    void JumpUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (InGrounded && !HasGroundAbove)
            {
                isJumping = true;
                jumpTime = startJumpTime.Value;
                SetJumpVelocity();
            }
        }
        
        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTime > 0)
            {
                SetJumpVelocity();
                jumpTime -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
            isJumping = false;
    }
    void PlayerAttackUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
            Punch(new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }
    void BombUpdate()
    {
        if (Input.GetButtonDown("Fire2") && bombTimer >= bombDelay.Value)
        {
            bombTimer = 0;
            PhotonNetwork.Instantiate(bombObject.name,transform.position, Quaternion.identity);
        }else
        {
            bombTimer += Time.deltaTime;
            if (bombTimer > bombDelay.Value) bombTimer = bombDelay.Value;
            OnSetBomTimer?.Invoke(bombTimer / bombDelay.Value);
        }
    }
    void SetJumpVelocity()
    {
        Vector2 velocity = rig.velocity;
        velocity.y = jumpForce.Value;
        rig.velocity = velocity;
    }
    private void FixedUpdate()
    {
        if (!view.IsMine || !canMove) return;

        Vector3 velocity = rig.velocity;
        velocity.x = speed.Value * Input.GetAxisRaw("Horizontal");
        rig.velocity = velocity;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Piece.MOVE_PIECE_TAG))
        {
            MatchManager.Instance.PlayerPiecesWin();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            MatchManager.Instance.PlayerPlatformWin();
        }
    }
    void Punch(Vector2 direction)
    {
        attackRayCast.origin = transform.position;
        attackRayCast.direction = direction;
        
        RaycastHit2D hit = Physics2D.Raycast(attackRayCast.origin,attackRayCast.direction,rayDistance,groundLayer);

        if (hit.collider)
            MatchManager.Instance.DestroyBlock(hit.collider.gameObject);
        
        PunchRenderer(direction);
    }
    void PunchRenderer(Vector2 direction)
    {
        punchPivot.gameObject.SetActive(true);
        float eulerX = 0;

        if (direction.x > 0)
        {
            direction.y = 0;
            eulerX = 0;
        }
        else if (direction.x < 0)
        {
            direction.y = 0;
            eulerX = 180;
        }
        else if (direction.y > 0)
            eulerX = 90;
        else if (direction.y < 0)
            eulerX = 270;

        punchPivot.eulerAngles = new(0, 0, eulerX);
        LeanTween.delayedCall(timeToDisablePunch, () => punchPivot.gameObject.SetActive(false));

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(attackRayCast.origin,attackRayCast.direction * rayDistance);
    }

    private void OnEnable()
    {
        if(view.IsMine)
        {
            MatchManager.OnStarGame += OnStartMatch;
        }
    }
    private void OnDisable()
    {
        if(view.IsMine)
        {
            MatchManager.OnStarGame -= OnStartMatch;
        }
    }
    void OnStartMatch()
    {
        canMove = true;
    }
}
