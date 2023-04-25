using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerAgent))]
public class Movement : MonoBehaviour
{
    public float WalkingSpeed = 5;
    public float RunningSpeed = 10;
    [SerializeField] float JumpHeight = 3;
    [SerializeField] float Gravity = -10;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheckTransform;
    [SerializeField] private float _groundCheckRadius = .15f;

    private Rigidbody2D _rigidbody2D;
    private PlayerAgent _player;
    private Vector2 _velocity;
    private SpriteRenderer _spriteRenderer;
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    private void Awake()
    {
        //Character Controller setup
        _rigidbody2D = GetComponent<Rigidbody2D>();

        //Player agent setup
        _player = GetComponent<PlayerAgent>();

        //Get ref for sprite
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public bool IsGrounded => Physics2D.OverlapCircle(_groundCheckTransform.position, _groundCheckRadius, _groundLayer);

    public Vector2 Velocity => _velocity;

    public void Move(float speed)
    {
        if (_player.MovementInput != 0)
            _spriteRenderer.flipX = _player.MovementInput < 0;
        _player.Animator.SetFloat(MoveSpeed, Math.Abs(
            _velocity.x));
        _velocity.x = _player.MovementInput * speed;
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(-Gravity * 2 * JumpHeight);
    }

    private void FixedUpdate()
    {
        _player.Animator.SetBool(Grounded, IsGrounded);
        GravityApply();
        _rigidbody2D.velocity = _velocity;
    }

    private void GravityApply()
    {
        _velocity.y += Gravity * Time.deltaTime;

        if (_velocity.y <= -2f && IsGrounded) _velocity.y = -2f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (IsGrounded) Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_groundCheckTransform.position, _groundCheckRadius);
    }
}