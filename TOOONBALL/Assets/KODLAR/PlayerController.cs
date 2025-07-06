using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 8f;
    public float jumpForce = 6f;
    public float gravity = -9.81f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float verticalVelocity = 0f;

    private float inputX;
    private float inputZ;
    private bool jumpPressed;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            HandleInput();
            Move();
        }
        else
        {
            // Smooth movement/update for network players
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }

    private void HandleInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        jumpPressed = Input.GetButtonDown("Jump");
    }

    private void Move()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        Vector3 moveDir = new Vector3(inputX, 0, inputZ);
        moveDir = Vector3.ClampMagnitude(moveDir, 1f);

        // Rotate player smoothly towards move direction relative to camera
        if (moveDir.magnitude > 0.1f)
        {
            // Get camera forward and right on horizontal plane
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 desiredMoveDirection = camForward * moveDir.z + camRight * moveDir.x;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), Time.deltaTime * rotationSpeed);

            // Check if run key pressed (left shift)
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            Vector3 movement = desiredMoveDirection * speed;

            controller.Move(movement * Time.deltaTime);
        }

        // Jump goofy mechanic: jump with bounce height that oscillates
        if (jumpPressed && isGrounded)
        {
            verticalVelocity = jumpForce * (1f + Mathf.Sin(Time.time * 5f) * 0.3f);  // oscillate jump height for goofiness
        }

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0);
        controller.Move(verticalMove * Time.deltaTime);
    }

    // Photon sync of position and rotation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}