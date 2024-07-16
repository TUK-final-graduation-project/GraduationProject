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
        state = newState;
        PlayAnimation();
    }

    void PlayAnimation()
    {
        anim.SetBool("Dead", state == State.DEAD);
        anim.SetBool("Attack", state == State.ATTACK);
        anim.SetBool("Hit", state == State.HIT);
        anim.SetBool("Walk", state == State.WALK);
        anim.SetBool("Run", state == State.RUN);
        if (state == State.IDLE)
        {
            anim.SetBool("Dead", false);
            anim.SetBool("Attack", false);
            anim.SetBool("Hit", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }
    }
}
