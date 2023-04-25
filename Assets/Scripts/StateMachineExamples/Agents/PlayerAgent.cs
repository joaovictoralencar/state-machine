using StateMachine;
using UnityEngine;

public class PlayerAgent : STM_Agent<PlayerAgent>
{
    [SerializeField] private Animator _animator;

    private bool _runPressed;
    private bool _jumpPressed;
    private float _movementInput;
    public bool RunPressed => _runPressed;

    public Animator Animator => _animator;

    public float MovementInput => _movementInput;
    
    public bool WantsToMove => _movementInput != 0;

    public bool JumpPressed => _jumpPressed;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        CreateStates();
    }

    private void CreateStates()
    {
        //Grounded
        GroundedState groundedState = new GroundedState();
        AddState(groundedState, true);
        //In Air
        InAirState inAirState = new InAirState();
        AddState(inAirState, true);
        //Jump
        JumpState jumpState = new JumpState();
        AddState(jumpState, true);
        //Idle
        IdleState idleState = new IdleState();
        AddState(idleState, false);
        //Run
        WalkingState walkingState = new WalkingState();
        AddState(walkingState, false);
        //Run
        RunningState runningState = new RunningState();
        AddState(runningState, false);

        //Sets first state
        InitializeAgent(groundedState);
    }

    private void Update()
    {
        _movementInput = Input.GetAxisRaw("Horizontal");
        _runPressed = Input.GetKey(KeyCode.LeftShift);
        _jumpPressed = Input.GetKey(KeyCode.Space);
    }
}