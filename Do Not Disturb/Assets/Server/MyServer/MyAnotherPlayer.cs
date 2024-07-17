using UnityEngine;

public class MyAnotherPlayer : MonoBehaviour
{
    public string playerName;
    public float moveSpeed = 5.0f;
    private Vector3 targetPosition;
    private Vector3 targetDirection;
    private Animator anim;
    private State state = State.IDLE;

    void Start()
    {
        anim = GetComponent<Animator>();
        targetPosition = transform.position;
        targetDirection = transform.forward;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, moveSpeed * Time.deltaTime);
        PlayAnimation();
    }

    public void UpdatePosition(Vector3 newPosition, Vector3 newDirection)
    {
        targetPosition = newPosition;
        targetDirection = newDirection;
    }

    public void UpdateState(State newState)
    {
        if (state != newState)
        {
            state = newState;
            PlayAnimation();
        }
    }

    void PlayAnimation()
    {
        anim.SetBool("Death", state == State.DEATH);
        anim.SetBool("Attack", state == State.ATTACK);
        anim.SetBool("isWalk", state == State.WALK);
        anim.SetBool("isJump", state == State.JUMP);

        switch (state)
        {
            case State.DEATH:
                anim.SetBool("Death", true); break;
            case State.ATTACK:
                anim.SetBool("Attack", true); break;
            case State.WALK:
                anim.SetBool("isWalk", true); break;
            case State.JUMP:
                anim.SetBool("isJump", true); break;
            case State.IDLE:
                anim.SetBool("Death", false);
                anim.SetBool("Attack", false);
                anim.SetBool("isWalk", false);
                anim.SetBool("isJump", false);
                break;
            default: break;
        }
    }
}
