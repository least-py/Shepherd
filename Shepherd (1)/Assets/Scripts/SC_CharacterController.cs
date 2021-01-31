using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class SC_CharacterController : MonoBehaviour
{

    public enum PState { Idle, Running, Attacking, Shouting }
    public PState currentStates = PState.Idle;

    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public Animator animator;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    SpriteRenderer m_spriteRenderer;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void flipX(Vector3 movementDirection)
    {
        // get camera direction
        Vector3 cameraDirection = Camera.main.transform.forward;

        // compute orthogonal vector in xz plane
        Vector3 orthogonal = new Vector3(cameraDirection.z, 0, -cameraDirection.x);

        //multiply
        float product = Vector3.Dot(orthogonal, movementDirection);
        if (product < -0.1f)
            m_spriteRenderer.flipX = true;
        else if (product > 0.1f)
            m_spriteRenderer.flipX = false;
    }


    void Update()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = speed * Input.GetAxis("Vertical");
            float curSpeedY = speed * Input.GetAxis("Horizontal");
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            flipX(moveDirection);

            if (Input.GetMouseButtonDown(1))
            {
                if(currentStates != PState.Shouting)
                {
                    currentStates = PState.Shouting;
                }
            }
            else if (moveDirection != Vector3.zero)
            {
                currentStates = PState.Running;
            }
            else
            {
                currentStates = PState.Idle;
            }
            SwitchAnimationStates(currentStates);
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(23f, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }

    void SwitchAnimationStates(PState state)
    {
        //Animation control
        if (animator)
        {
            animator.SetBool("isAttacking", state == PState.Attacking);
            animator.SetBool("isRunning", state == PState.Running);
            animator.SetBool("isShouting", state == PState.Shouting);
        }
    }
}