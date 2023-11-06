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
    [SerializeField] float groundCheckUpDistance;
    [SerializeField] float groundCheckUpDistanceBtweenRays;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask layerMask;
    [SerializeField] FloatVariable startJumpTime;
    [SerializeField] FloatVariable coyoteJumpTime;
    float coyoteJumpTimer;
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
    [SerializeField] Transform rendererTransform;
    bool lookRigh;
    bool isMove;
    readonly int isIdleId = Animator.StringToHash("IsIdle");
    readonly int lookRightId = Animator.StringToHash("LookRight");

    Vector3 GroundCheckPosition
    {
        get
        {
            Vector3 pos = transform.position;
            pos.y -= groundCheckDistance;
            return pos;
        }
    }
    bool lastInGround;
    bool InGrounded
    {
        get
        {
            lastInGround = Physics2D.OverlapCircle(GroundCheckPosition, groundCheckRadius, layerMask);
            return lastInGround;
        }
    }
            
    bool HasGroundAbove
    {
        get
        {
            bool ray1, ray2;
            Vector3 pos;
            pos = transform.position;
            pos.x -= groundCheckUpDistanceBtweenRays;
            ray1 = Physics2D.Raycast(pos, Vector2.up, groundCheckUpDistance, layerMask);

            pos = transform.position;
            pos.x += groundCheckUpDistanceBtweenRays;
            ray2 = Physics2D.Raycast(pos, Vector2.up, groundCheckUpDistance, layerMask);
            return ray1 || ray2;
        }
    }
    float lastInputX;
    bool canMove;
    private void Start()
    {
        canMove = Application.isEditor;
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
    bool lastLookRight;
    void AnimationUpdate()
    {
        isMove = Input.GetButton("Horizontal");

        if (isMove)
            lastInputX = Input.GetAxisRaw("Horizontal");

        lookRigh = lastInputX > 0;

        if (lastLookRight != lookRigh)
        {
            lastLookRight = lookRigh;
            anim.SetBool(lookRightId, lookRigh);
            print("Test");
        }

        anim.SetBool(isIdleId, !isMove);
    }
    void JumpUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if ((coyoteJumpTimer <= coyoteJumpTime.Value || InGrounded) && !HasGroundAbove)
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
                isJumping = false;
        }

        if (Input.GetButtonUp("Jump"))
            isJumping = false;


        if (!InGrounded)
            coyoteJumpTimer += Time.deltaTime;
        else
            coyoteJumpTimer = 0;
    }
    void PlayerAttackUpdate()
    {
        if (Input.GetButtonDown("Fire1"))
            Punch(new (rendererTransform.localScale.x, Input.GetAxisRaw("Vertical")));
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

        if (direction.y > 0)
            eulerX = 90;
        else if (direction.y < 0)
            eulerX = 270;
        else if (direction.x > 0)
        {
            direction.y = 0;
            eulerX = 0;
        }
        else if (direction.x < 0)
        {
            direction.y = 0;
            eulerX = 180;
        }

        punchPivot.eulerAngles = new(0, 0, eulerX);
        LeanTween.delayedCall(timeToDisablePunch, () => punchPivot.gameObject.SetActive(false));

    }
    [SerializeField] float collideGizmo;
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        pos.y -= groundCheckDistance;
        Gizmos.DrawWireSphere(GroundCheckPosition, groundCheckRadius);

        pos = transform.position;
        pos.x -= groundCheckUpDistanceBtweenRays;
        Gizmos.DrawRay(pos, Vector2.up * groundCheckUpDistance);
        pos = transform.position;
        pos.x += groundCheckUpDistanceBtweenRays;
        Gizmos.DrawRay(pos, Vector2.up * groundCheckUpDistance);

        Gizmos.DrawWireCube(transform.position, Vector3.one * collideGizmo);
        Gizmos.DrawWireSphere(transform.position, 0.5f * collideGizmo);
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
