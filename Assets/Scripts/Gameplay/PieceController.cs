using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    Piece currentPiece;
    [SerializeField] float speed;
    private void FixedUpdate()
    {
        if (currentPiece)
        {
            Rigidbody2D rig = currentPiece.rig;

            Vector3 velocity = rig.velocity;
            velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal2");
            rig.velocity = velocity;
        }
    }
    
    void OnSpawnPiece(Piece piece)
    {
        currentPiece = piece;
    }
    private void OnEnable()
    {
        MatchManager.OnSpawnPiece += OnSpawnPiece;
    }
    private void OnDisable()
    {
        MatchManager.OnSpawnPiece -= OnSpawnPiece;
    }
}
