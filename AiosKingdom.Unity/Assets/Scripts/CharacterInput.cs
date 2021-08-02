using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
    public float TileSize = 1f;
    public float SwipeTreshold = 20f;

    private Vector2 _currentPosition;
    private Vector2 _nextPosition;
    private bool _moved = false;
    private bool _moving = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            _keyboardInputs();
            //_screenTouchInputs();
        }
        else
        {
            _screenTouchInputs();
        }
    }

    public void Move(JsonObjects.MovingState state)
    {
        var newPosition = new Vector2(state.CurrentCoordinateX * TileSize, state.CurrentCoordinateY * TileSize);
        _move(newPosition);
    }

    private void _keyboardInputs()
    {
        if (!AdventureUIManager.This.CanMove)
        {
            return;
        }

        if (!_moved && DatasManager.Instance.Adventure.MovingState.CurrentStamina > 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NetworkManager.This.Move(JsonObjects.Adventures.Movement.Up, WorldManager.This.StaminaConsumption);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NetworkManager.This.Move(JsonObjects.Adventures.Movement.Down, WorldManager.This.StaminaConsumption);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NetworkManager.This.Move(JsonObjects.Adventures.Movement.Right, WorldManager.This.StaminaConsumption);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                NetworkManager.This.Move(JsonObjects.Adventures.Movement.Left, WorldManager.This.StaminaConsumption);
            }
        }

        if (!Input.anyKey)
        {
            _moved = false;
        }
    }

    private void _screenTouchInputs()
    {
        if (!AdventureUIManager.This.CanMove)
        {
            return;
        }

        if (Input.touchCount == 1 && !_moved && DatasManager.Instance.Adventure.MovingState.CurrentStamina > 0)
        {
            var touch = Input.touches[0];
            var delta = touch.rawPosition - touch.position;

            if (Mathf.Abs(delta.x) > SwipeTreshold && Mathf.Abs(delta.y) < SwipeTreshold)
            {
                if (delta.x > 0f)
                {
                    NetworkManager.This.Move(JsonObjects.Adventures.Movement.Left, WorldManager.This.StaminaConsumption);
                }
                else
                {
                    NetworkManager.This.Move(JsonObjects.Adventures.Movement.Right, WorldManager.This.StaminaConsumption);
                }
            }
            else if (Mathf.Abs(delta.y) > SwipeTreshold && Mathf.Abs(delta.x) < SwipeTreshold)
            {
                if (delta.y > 0f)
                {
                    NetworkManager.This.Move(JsonObjects.Adventures.Movement.Up, WorldManager.This.StaminaConsumption);
                }
                else
                {
                    NetworkManager.This.Move(JsonObjects.Adventures.Movement.Down, WorldManager.This.StaminaConsumption);
                }
            }
        }

        if (Input.touchCount == 0)
        {
            _moved = false;
        }
    }

    private void _move(Vector2 newPosition)
    {
        var hits = Physics2D.OverlapCircleAll(newPosition, 0);
        bool canGo = true;
        foreach (var hit in hits)
        {
            if (hit.tag == "Wall")
            {
                canGo = false;
                break;
            }
        }

        if (canGo)
        {
            _nextPosition = newPosition;
            _moved = true;
            _moving = true;
        }
    }

    void FixedUpdate()
    {
        if (_moving)
        {
            float step = 0.2f;
            var diff = _nextPosition - _currentPosition;
            if (Mathf.Abs(diff.x) > step || Mathf.Abs(diff.y) > step)
            {
                _currentPosition = Vector2.MoveTowards(_currentPosition, _nextPosition, step);
            }
            else
            {
                _currentPosition = _nextPosition;
            }
            transform.position = new Vector3(_currentPosition.x, _currentPosition.y, transform.position.z);

            if (_currentPosition == _nextPosition)
            {
                _moving = false;
            }
        }
    }

    public void Spawn(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
        _currentPosition = position;
    }
}
