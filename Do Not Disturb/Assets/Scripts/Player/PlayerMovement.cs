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
    public float friction = 0.1f;

    private Animator anim;
    private Camera cam;
    private CharacterController controller;
    private GameManager gameManager;

    public bool toggleCameraRotation;
    public bool isRun;
    public bool isJump;

    private bool jDown;
    private float currentSpeed;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private Vector3 yVelocity;

    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();

        if (anim == null) Debug.LogError("Animator component is missing on " + gameObject.name);
        if (controller == null) Debug.LogError("CharacterController component is missing on " + gameObject.name);
        if (gameManager == null) Debug.LogError("GameManager component is missing in the scene.");
        HP = 100;
    }

    void FixedUpdate()
    {
        jDown = Input.GetButton("Jump");
    }

    void Update()
    {
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

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = forward * verticalInput + right * horizontalInput;
        targetVelocity = moveDirection.normalized * finalSpeed;

        if (targetVelocity.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * (1.0f / accelerationTime));
        }
        else
        {
            // 현재 속도에 비례한 감속 속도 계산
            float decelerationFactor = Mathf.Max(friction, 1.0f / decelerationTime);
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * decelerationFactor * currentVelocity.magnitude);
        }


        currentVelocity *= (1 - friction * Time.deltaTime);

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

        //if(isRun)
        //{
        //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Run);
        //}
        //else
        //{
        //    AudioManager.instance.PlaySfx(AudioManager.Sfx.Walk);
        //}

        anim.SetFloat("Horizontal", horizontalInput);
        anim.SetFloat("Vertical", verticalInput);

        // Set Blend Tree blend based on speed
        float blend = Mathf.Clamp(currentSpeed / runSpeed, 0f, 1f);
        anim.SetFloat("Blend", blend);

        if (currentSpeed > animationThreshold)
        {
            toggleCameraRotation = false;
        }
        else
        {
            toggleCameraRotation = true;
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
