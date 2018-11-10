using UnityEngine;
using System;

public class PartyLeaderController : CharacterController
{
    Vector2 velocity;
    void Update ()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Rigidbody2D my2DRigidbody = GetComponent<Rigidbody2D>();
        velocity.x = moveHorizontal * moveSpeed;
        velocity.y = moveVertical * moveSpeed;
        my2DRigidbody.velocity = velocity;
	}
}
