using System.Collections;
using UnityEngine;
public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] bool canMove;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Rigidbody2D rig;
    public Vector2 Velocity { get => rig.velocity; }

    bool lookRight;

    public static Player instance;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetButtonDown("Jump") && canMove)
            Jump();
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 velocity = rig.velocity;
            velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
            rig.velocity = velocity;

            if (rig.velocity.x != 0)
                lookRight = rig.velocity.x > 0;

        }
    }
    
    public void OnPlayerTrigger(Collider2D collision)
    {

    }
    
    void Jump()
    {
        if (IsGround())
        {
            rig.AddForce(jumpForce * Vector2.up);
        }
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
