using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joysticks : MonoBehaviour
{
    public float MoveSpeed = 10f;

    private CapsuleCollider2D _collider;
    private Rigidbody2D _rigidBody;
    private Animator _animator;

    private float _verticalMove = 0;
    private float _horizontalMove = 0;

    void Awake()
    {
        _collider = this.GetComponent<CapsuleCollider2D>();
        _rigidBody = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        var h = Input.GetAxisRaw("Horizontal");
        var v = Input.GetAxisRaw("Vertical");

        _horizontalMove = h * MoveSpeed;
        _verticalMove = v * MoveSpeed;

        if (h != 0f || v != 0f)
        {
            _animator.SetBool("Moving", true);
        } 
        else
        {
            _animator.SetBool("Moving", false);
        }

        if (_verticalMove > 0f)
        {
            _animator.SetInteger("Direction", 2);
        }
        if (_verticalMove < 0f)
        {
            _animator.SetInteger("Direction", 0);
        }
        if (_horizontalMove > 0f)
        {
            _animator.SetInteger("Direction", 3);
        }
        if (_horizontalMove < 0f)
        {
            _animator.SetInteger("Direction", 1);
        }
    }

    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;

        move.x = _horizontalMove;
        move.y = _verticalMove;

        _rigidBody.velocity = move;
    }
}
