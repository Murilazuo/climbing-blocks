using Mirror;
using System.Collections;
using UnityEngine;
public class PlayerPlataform : NetworkBehaviour
{
    [Header("Move")]
    [SerializeField] bool canMove;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Rigidbody2D rig;
    public Vector2 Velocity { get => rig.velocity; }
    public bool owner;
    public void Init()
    {
        owner = true;
    }
    private void Update()
    {
        if (owner) return;

        if (Input.GetButtonDown("Jump") && canMove)
            Jump();
    }
    private void FixedUpdate()
    {
        if (owner) return;

        Vector3 velocity = rig.velocity;
        velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
        rig.velocity = velocity;
    }
    
    void Jump()
    {
        if (IsGround())
            rig.AddForce(jumpForce * Vector2.up);
    }
    bool IsGround()
    {
        return Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance,layerMask);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(rig.position , Vector3.down * groundCheckDistance);
    }
}
