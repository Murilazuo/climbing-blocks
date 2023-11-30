using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    [SerializeField] Vector2 spawnPlatformPosition;

    [SerializeField] PlatformSettings settings;

    [Header("Move")]
    [SerializeField] float rayMoveDistance;
    [Header("Jump")]
    [SerializeField] Vector2 groundCheckSize;
    [SerializeField] float groundCheckDistance;
    [Header("Roof")]
    [SerializeField] float groundCheckUpDistance;
    [SerializeField] float groundCheckUpDistanceBtweenRays;
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
    [SerializeField] GameObject[] outlineObjects;
    [SerializeField] GameObject playerGhostPrefab;
    bool lookRigh;
    bool isMove;
    readonly int isIdleId = Animator.StringToHash("IsIdle");
    readonly int lookRightId = Animator.StringToHash("LookRight");

    [SerializeField] SpriteRenderer[] spriteRenderers;
    
    bool IsLastCharcter { get => FindObjectsOfType<PlayerPlatform>().Length <= 1; }

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

    bool groundCheckDelay = false;
    bool InGrounded
    {
        get
        {
            lastInGround = Physics2D.BoxCast(GroundCheckPosition, groundCheckSize, 0, Vector2.zero, 1, layerMask);

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
    bool lookVertical;
    private void Start()
    {
        if (view.IsMine)
        {
            rig.gravityScale = settings.GravityScale;
            OnSpawnPlayerPlatform?.Invoke();
            view.RPC(nameof(SetColor), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber-1);
            rig.simulated = true;
            rig.isKinematic = false;

            foreach(var outline in outlineObjects)
                outline.SetActive(true);
        }
        else
        {
            foreach (var outline in outlineObjects)
                Destroy(outline);

            foreach (var spr in spriteRenderers)
                spr.sortingOrder = spr.sortingOrder - 1 - view.OwnerActorNr;

            rig.isKinematic = true;
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
    [PunRPC]
    void SetColor(int playerId)
    {
        Color color = MasterClientManager.Instance.GetPlayerColor(playerId);
        foreach(var spr in spriteRenderers)
        {
            spr.color = color;
        }
    }
    void Update()
    {
        if (!view.IsMine || !canMove) return;
        AnimationUpdate();
        JumpUpdate();
        PlayerAttackUpdate();


        if (Input.GetButtonDown("Horizontal"))
        {
            LeanTween.cancel(gameObject);
            lookVertical = false;
        }
        if (Input.GetButtonDown("Vertical"))
            lookVertical = true;

        if (Input.GetButtonUp("Horizontal"))
        {
            if (Input.GetButton("Vertical"))
                lookVertical = true;

            if (!Input.GetButton("Horizontal"))
            {
                Snap();
            }
        }

        if (Input.GetButtonUp("Vertical"))
            if (Input.GetButtonDown("Horizontal"))
                lookVertical = false;

        if (!inDanger && timeInDanger > 0)
            timeInDanger -= Time.deltaTime;

        float inDangerPercentage = timeInDanger / settings.TimeToDieInDangerZone;

        waterVolumeController.SetWeight(inDangerPercentage);
    }
    bool lastLookRight;

    void Snap()
    {
        LeanTween.moveX(gameObject, Mathf.Round(transform.position.x), settings.TimeToSnap);
    }
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
    [PunRPC]
    void PlaySound(int soundTypeId)
    {
        SoundManager.Instance.PlaySound((SoundType)soundTypeId);
    }
    bool callDealayedCall;

    void Jump()
    {
        if (!groundCheckDelay)
        {
            groundCheckDelay = true;
            LeanTween.delayedCall(settings.JumpDelay, () =>
            {
                groundCheckDelay = false;
            });



            print("Jump");
            view.RPC(nameof(PlaySound), RpcTarget.All, (int)SoundType.Jump);
            isJumping = true;
            jumpTime = settings.StartJumpTime;
            Snap();
            SetJumpVelocity();
        }
    }

    LTDescr jumpDelayDelayCall;

    void CallDelayedJump()
    {
        print("End delay time");
        callDealayedCall = false;
    }
    void JumpUpdate()
    {
       
        if (Input.GetButtonDown("Jump"))
        {


            if ((coyoteJumpTimer <= settings.CoyoteJumpTime || InGrounded) && !HasGroundAbove && !callDealayedCall)
            {
                print("Jump button");
                Jump();
            }
            else if (!HasGroundAbove && !InGrounded)
            {
                callDealayedCall = true;
                print("call delay jump");

                Invoke(nameof(CallDelayedJump), settings.DelayedJumpTime);
            }
        }

        if (InGrounded && callDealayedCall)
        {
            CancelInvoke(nameof(CallDelayedJump));
            print("delay jump");

            callDealayedCall = false;
            Jump();
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
            if (colideWithPiece) return;
                colideWithPiece = true;

            PlayerDie(PlayerDeathId.Smash);
        }
    }
    bool colideWithPiece = false;
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Piece.MOVE_PIECE_TAG))
        {
            if (colideWithPiece) return;
                colideWithPiece = true;

            PlayerDie(PlayerDeathId.Smash);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "End":
                MatchManager.Instance.PlatformReachTop(transform.position);
                break;
            case "Danger":
                SoundManager.Instance.PlaySound(SoundType.Bubbles);
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
                SoundManager.Instance.Stop(SoundType.Bubbles);
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
                
                if (timeInDanger > settings.TimeToDieInDangerZone)
                {
                    PlayerDie(PlayerDeathId.Drow);
                }
                break;
        }
    }
    public bool sendLastPlayerDieEvent = false;
    void PlayerDie(PlayerDeathId playerDeathId)
    {
        if (view.IsMine)
        {
            PhotonNetwork.Instantiate(playerGhostPrefab.name, transform.position, Quaternion.identity).GetComponent<PlayerGhost>().Init(playerDeathId);

            waterVolumeController.SetWeight(0);

            PhotonNetwork.Destroy(gameObject);
        }

        StopPlayerMove();
    }
    void StopPlayerMove()
    {
        canMove = false;
        rig.isKinematic = true;
        anim.speed = 0;
        rig.simulated = false;
    }
    void Punch(Vector2 direction)
    {
        if(lookVertical && direction.y != 0)
            direction.x = 0;
        else
        {
            direction.y = 0;
        }

        attackRayCast.origin = transform.position;
        attackRayCast.direction = direction;
        
        RaycastHit2D hit = Physics2D.Raycast(attackRayCast.origin,attackRayCast.direction,rayDistance,groundLayer);

        view.RPC(nameof(PlaySound), RpcTarget.All, (int)SoundType.Punch);

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
            eulerX = 0;
        else if (direction.x < 0)
            eulerX = 180;

        punchPivot.eulerAngles = new(0, 0, eulerX);
        LeanTween.delayedCall(timeToDisablePunch, () => punchPivot.gameObject.SetActive(false));
    }
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        pos.y -= groundCheckDistance;

        Gizmos.DrawWireCube(GroundCheckPosition, groundCheckSize);

        pos = transform.position;
        pos.x -= groundCheckUpDistanceBtweenRays;
        Gizmos.DrawRay(pos, Vector2.up * groundCheckUpDistance);
        pos = transform.position;
        pos.x += groundCheckUpDistanceBtweenRays;
        Gizmos.DrawRay(pos, Vector2.up * groundCheckUpDistance);

        Gizmos.DrawRay(transform.position, rendererTransform.localScale.x * rayMoveDistance * transform.right);
    }
   
    private void OnEnable()
    {
        LeanTween.cancel(gameObject);
        if(view.IsMine)
        {
            MatchManager.OnStarGame += OnStartMatch;
            MatchManager.OnEndGame += EndGame;
        }
    }
    private void OnDisable()
    {
        if(view.IsMine)
        {
            MatchManager.OnStarGame -= OnStartMatch;
            MatchManager.OnEndGame -= EndGame;
        }
    }
    void OnStartMatch()
    {
        canMove = true;
        sendLastPlayerDieEvent = false;
    }
    private void EndGame(int obj, Vector2 position)
    {
        waterVolumeController.SetWeight(0);

        

    }
}
