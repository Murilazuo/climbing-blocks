using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

public class PlayerPlatform : MonoBehaviour
{
    [SerializeField] Vector2 spawnPlatformPosition;

    [SerializeField] PlatformSettings settings;

    [Header("Move")]
    
    [Header("Jump")]
    [SerializeField] float groundCheckDistance;
    [SerializeField] float groundCheckUpDistance;
    [SerializeField] float groundCheckUpDistanceBtweenRays;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask layerMask;
    float coyoteJumpTimer;
    float jumpTime;
    bool isJumping;

    [Header("Attack 1")]
    [SerializeField] Transform punchPivot;
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float timeToDisablePunch;
    Ray attackRayCast;
    float attackTimer;
    public static System.Action<float> OnSetAttackTimer;
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

    [SerializeField] float timeToDieInDanger;
    float timeInDanger;
    bool inDanger;

    PostProcessVolumeController waterVolumeController;

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
            rig.gravityScale = settings.GravityScale;
            OnSpawnPlayerPlatform?.Invoke();
        }
        else
        {
            rig.simulated = false;
        }

        waterVolumeController = null;
        foreach (var volume in FindObjectsOfType<PostProcessVolumeController>())
        {
            waterVolumeController = volume.GetVolumeByTag(PostProcessTag.WaterDamage);
            if(waterVolumeController != null)
                break;
        }
    }
    void Update()
    {
        if (!view.IsMine || !canMove) return;
        AnimationUpdate();
        JumpUpdate();
        PlayerAttackUpdate();

        if (!inDanger && timeInDanger > 0)
            timeInDanger -= Time.deltaTime;

        float inDangerPercentage = timeInDanger / timeToDieInDanger;

        waterVolumeController.SetWeight(inDangerPercentage);
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
        }

        anim.SetBool(isIdleId, !isMove);
    }
    void JumpUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if ((coyoteJumpTimer <=  settings.CoyoteJumpTime || InGrounded) && !HasGroundAbove)
            {
                isJumping = true;
                jumpTime = settings.StartJumpTime;
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
        if (Input.GetButtonDown("Fire1") && attackTimer >= settings.AttackDelay)
        {
            attackTimer = 0;
            Punch(new (rendererTransform.localScale.x, Input.GetAxisRaw("Vertical")));
        }
        else
        {
            attackTimer += Time.deltaTime;
            if (attackTimer > settings.AttackDelay) attackTimer = settings.AttackDelay;
            OnSetAttackTimer?.Invoke(attackTimer / settings.AttackDelay);
        }
        
    }
   
    void SetJumpVelocity()
    {
        Vector2 velocity = rig.velocity;
        velocity.y = settings.JumpForce;
        rig.velocity = velocity;
    }
    private void FixedUpdate()
    {
        if (!view.IsMine || !canMove) return;
        
        Vector3 velocity = rig.velocity;
        velocity.x = settings.Speed * Input.GetAxisRaw("Horizontal");
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
        switch (collision.tag)
        {
            case "End":
                MatchManager.Instance.PlayerPlatformWin();
                break;
            case "Danger":
                inDanger = true;
                rig.gravityScale = settings.GravityScale;
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Danger":
                rig.gravityScale = settings.GravityScale;
                inDanger = false;
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Danger":
                timeInDanger += Time.deltaTime;
                if(timeInDanger > timeToDieInDanger)
                    MatchManager.Instance.PlayerPiecesWin();
                break;
        }
    }
    void Punch(Vector2 direction)
    {
        attackRayCast.origin = transform.position;
        attackRayCast.direction = direction;
        
        RaycastHit2D hit = Physics2D.Raycast(attackRayCast.origin,attackRayCast.direction,rayDistance,groundLayer);

        if (hit.collider && hit.transform.gameObject.CompareTag(Piece.STOPED_PIECE_TAG))
            view.RPC(nameof(DestroyBlock), RpcTarget.All, (int)hit.transform.position.x, (int)hit.transform.position.y);
            
        if (view.IsMine)
            view.RPC(nameof(PunchRenderer), RpcTarget.All, direction.x,direction.y);
    }
    [PunRPC]
    void DestroyBlock(int x, int y)
    {
        print("DestroyBlock - player platform");
        MatchManager.Instance.DestroyBlock(x,y);
    }
    [PunRPC]
    void PunchRenderer(float x, float y)
    {
        Vector2 direction = new(x, y);

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
