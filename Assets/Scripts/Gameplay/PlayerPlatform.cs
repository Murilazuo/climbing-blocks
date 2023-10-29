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

    [Header("Component")]
    [SerializeField] PhotonView view;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] SpriteRenderer playerRenderer;

    bool InGrounded
    {
        get =>
        Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask);
    }
    void Update()
    {
        if (!view.IsMine) return;


        if (Input.GetButtonDown("Jump"))
        {
            if (InGrounded)
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
        {
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Punch(new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        }
    }
    void SetJumpVelocity()
    {
        Vector2 velocity = rig.velocity;
        velocity.y = jumpForce;
        rig.velocity = velocity;
    }
    private void FixedUpdate()
    {
        playerRenderer.flipX = !(rig.velocity.x > 0);

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
        punchPivot.gameObject.SetActive(true);
        
        punchPivot.eulerAngles = new(0,0, Vector2.Angle(Vector2.zero, direction));
        LeanTween.delayedCall(.5f, () => punchPivot.gameObject.SetActive(false));
    }
}
