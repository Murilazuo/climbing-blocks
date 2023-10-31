using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    [SerializeField] Vector2 spawnPlatformPosition;
    
    [Header("Move")]
    [SerializeField] float speed;
    
    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float startJumpTime;
    float jumpTime;
    bool isJumping;

    [Header("Attack")]
    [SerializeField] Transform punchPivot;
    [SerializeField] float rayDistance;
    [SerializeField] LayerMask groundLayer;
    Ray attackRayCast;

    [Header("Component")]
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] SpriteRenderer playerRenderer;
    bool InGrounded
    {
        get =>
        Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask);
    }
    bool HasGroundAbove
    {
        get =>
        Physics2D.Raycast(rig.position, Vector2.up, groundCheckDistance, layerMask);
    }
    float lastInputX;
    private void Start()
    {
        if(!view.IsMine) rig.simulated = false;
    }
    void Update()
    {
        if (!view.IsMine) return;

        if (Input.GetButton("Horizontal"))
            lastInputX = Input.GetAxisRaw("Horizontal");

        if (lastInputX > 0) playerRenderer.flipX = false;
        else if (lastInputX < 0) playerRenderer.flipX = true;

        if (Input.GetButtonDown("Jump"))
        {
            if (InGrounded && !HasGroundAbove)
            {
                isJumping = true;
                jumpTime = startJumpTime;
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

        if (Input.GetKeyDown(KeyCode.Space))
            Punch(new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
    }
    void SetJumpVelocity()
    {
        Vector2 velocity = rig.velocity;
        velocity.y = jumpForce;
        rig.velocity = velocity;
    }
    private void FixedUpdate()
    {
        if (!view.IsMine) return;

        Vector3 velocity = rig.velocity;
        velocity.x = speed * Input.GetAxisRaw("Horizontal");
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
        
        if(hit.collider )
            if (hit.collider.gameObject.CompareTag(Piece.STOPED_PIECE_TAG))
            {
                object[] data =
                {
                    hit.collider.gameObject.GetComponentInParent<Piece>().pieceId,
                    hit.transform.GetSiblingIndex(),
                    hit.transform.position
                };

                hit.transform.gameObject.SetActive(false);

                NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_DESTROY_EVENT,data);
            }
        
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
        LeanTween.delayedCall(.1f, () => punchPivot.gameObject.SetActive(false));

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(attackRayCast.origin,attackRayCast.direction * rayDistance);
    }
}
