using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joysticks : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public Text JoystickDisplay;

    private Rigidbody2D _rigidBody;
    private Animator _animator;

    private float _verticalMove = 0;
    private float _horizontalMove = 0;

    private Vector2 _joystickCenter;
    private float _radius = 100f;

    void Awake()
    {
        _rigidBody = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();

        _joystickCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    void Update()
    {
        _keyboardInputs();
        //_screenTouchInputs();
    }

    private void _keyboardInputs()
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

    private void _screenTouchInputs()
    {
        if (Input.touchCount == 0)
        {
            _animator.SetBool("Moving", false);
            _horizontalMove = 0f;
            _verticalMove = 0f;
        }
        else if (Input.touchCount == 1)
        {
            var firstTouch = Input.GetTouch(0).rawPosition;
            var distance = Vector2.Distance(firstTouch, _joystickCenter);

            if (distance > _radius)
            {
                return;
            }

            var currentPos = Input.GetTouch(0).position;
            float ratio = 300f;
            float xDiff = (firstTouch.x - currentPos.x) / ratio;
            float yDiff = (firstTouch.y - currentPos.y) / ratio;

            if (xDiff > 0.3f)
            {
                xDiff = 1f;
            }
            else if (xDiff < -0.3f)
            {
                xDiff = -1f;
            }
            else
            {
                xDiff = 0f;
            }

            if (yDiff > 0.3f)
            {
                yDiff = 1f;
            }
            else if (yDiff < -0.3f)
            {
                yDiff = -1f;
            }
            else
            {
                yDiff = 0f;
            }

            _horizontalMove = xDiff * -1f * MoveSpeed;
            _verticalMove = yDiff * -1f * MoveSpeed;

            JoystickDisplay.text = $"firstTouch: {firstTouch} \ncurrentPos: {currentPos} \nx: {_horizontalMove} y: {_verticalMove}";

            if (_horizontalMove != 0f || _verticalMove != 0f)
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
    }

    void FixedUpdate()
    {
        Vector2 move = Vector2.zero;

        move.x = _horizontalMove;
        move.y = _verticalMove;

        _rigidBody.velocity = move;
    }
}
