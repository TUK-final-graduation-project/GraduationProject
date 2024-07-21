using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float HP = 100;
    public float maxHP = 100;
    public float attackDamage = 100;
    public float speed = 5.0f;
    public float runSpeed = 8.0f;
    public float smoothness = 10.0f;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.5f;
    public float jumpForce = 5.0f;
    public float gravity = -9.8f;
    public float animationThreshold = 1f;
    public int coinCount = 0;
    public State state;

    private Animator anim;
    private Camera cam;
    private CharacterController controller;

    public bool toggleCameraRotation;
    public bool isRun;
    public bool isJump;

    private bool jDown;
    private float currentSpeed;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private Vector3 yVelocity;

    //private MyAnotherPlayer anotherPlayer;

    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        //anotherPlayer = GetComponent<MyAnotherPlayer>();

        if (anim == null) Debug.LogError("Animator component is missing on " + gameObject.name);
        if (controller == null) Debug.LogError("CharacterController component is missing on " + gameObject.name);
        HP = 100; 
        //if (anotherPlayer == null) Debug.LogError("AnotherPlayer component is missing on " + gameObject.name);
    }

    void FixedUpdate()
    {
        jDown = Input.GetButton("Jump");
    }

    void Update()
    {
        toggleCameraRotation = Input.GetKey(KeyCode.LeftAlt);
        isRun = Input.GetKey(KeyCode.LeftShift);

        InputMovement();
    }

    void LateUpdate()
    {
        if (!toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {
        if (controller == null || anim == null) return;

        float finalSpeed = isRun ? runSpeed : speed;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        targetVelocity = moveDirection.normalized * finalSpeed;

        if (targetVelocity.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * (1.0f / accelerationTime));
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * (1.0f / decelerationTime));
        }

        if (jDown)
        {
            if (!isJump)
            {
                yVelocity.y = jumpForce;
                isJump = true;
                anim.SetTrigger("Jump");
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
            }
        }
        else
        {
            yVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalMove = currentVelocity * Time.deltaTime;
        finalMove.y = yVelocity.y * Time.deltaTime;

        controller.Move(finalMove);

        currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > animationThreshold)
        {
            float percent = ((isRun) ? 1 : 0.5f) * (currentSpeed / finalSpeed);
            anim.SetFloat("Blend", percent, 0.5f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Blend", 0, 0.5f, Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isJump = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (controller.isGrounded)
        {
            isJump = false;
        }
    }

    public void SetHP(float hp)
    {
        HP = hp;
    }
    public void SetDamage(float damage)
    {
        attackDamage = damage;
    }
     public void SetSpeed(float speed_)
    {
        speed = speed_;
    }
     public void SetRunSpeed(float run_speed)
    {
        runSpeed = run_speed;
    }

    public void ApplyServerData(Vector3 position, Vector3 direction, State newState)
    {
        //anotherPlayer.UpdatePosition(position, direction);
        //anotherPlayer.UpdateState(newState);
    }
}
