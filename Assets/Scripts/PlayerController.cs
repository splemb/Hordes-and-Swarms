using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Referenced Components
    [Header("Referenced Components")]
    public Rigidbody rb;
    public CapsuleCollider collision;
    public GameObject model;
    public Collider standCollider;
    public Collider crouchCollider;
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioClip[] audioClips;
    public Camera playerCamera;
    public Animator animator;

    //Input Variables
    [Header("Input Variables")]
    public Vector2 axisInput;
    public Vector2 mouseInput;
    public bool tryJump;

    //Movement Variables
    [Header("Movement Variables")]
    public Vector3 hVelocity;
    public float vVelocity;
    public bool isCrouching;
    public bool isJumping;
    public bool isStrafing;
    public float currentGravityScale;


    //Player Parameters
    [Header("Player Parameters")]
    public float gravityScale;
    public float speed;
    public float crouchSpeedMultiplier;
    public float rotationSpeed;
    public float jumpForce;
    public float jumpGravityScaleModifier;
    [SerializeField] public LayerMask groundMask;

    bool controlling = true;

    private void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        collision = GetComponent<CapsuleCollider>();

        currentGravityScale = gravityScale;
    }

    private void Update()
    {
        if (controlling)
        {
            //Record Inputs
            axisInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            if (Input.GetButtonDown("Jump")) tryJump = true;
            if (Input.GetButtonDown("Crouch")) isCrouching = !isCrouching;

            
            if (hVelocity.magnitude > 0 && CheckGrounded(0.5f) && !audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
            else if (hVelocity.magnitude == 0 || !CheckGrounded(0.5f))
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    private void FixedUpdate()
    {
        if (controlling)
        {
            //transform.Rotate(transform.up * mouseInput.x * rotationSpeed * Time.deltaTime);

            //Calculate horizontal components of velocity
            hVelocity = Vector3.ClampMagnitude(new Vector3(axisInput.x * speed * Time.deltaTime, 0f, axisInput.y * speed * Time.deltaTime), speed * Time.deltaTime);

            //Calculate vertical velocity
            vVelocity = rb.velocity.y;

            if (CheckGrounded() && rb.velocity.y < 0f) vVelocity = 0f;
            else if (!CheckGrounded())
            {
                vVelocity += currentGravityScale * Physics.gravity.y * Time.deltaTime;
            }

            //Ignore vertical velocity for jumping
            if (tryJump)
            {
                if (CheckGrounded(0.3f)) { vVelocity = jumpForce; isJumping = true; audioSource.Stop(); audioSource2.PlayOneShot(audioClips[1]); tryJump = false; }
            }
            else
            {
                if (CheckGrounded()) { isJumping = false; }
            }

            tryJump = false;

            // Better jumping
            if (isJumping)
            {
                currentGravityScale = gravityScale * jumpGravityScaleModifier;
                if (!Input.GetButton("Jump")) isJumping = false;
            }
            else { currentGravityScale = gravityScale; }

            //Crouching
            if (isCrouching)
            {
                hVelocity *= crouchSpeedMultiplier;
                standCollider.enabled = false;
                crouchCollider.enabled = true;
            }
            else
            {
                standCollider.enabled = true;
                crouchCollider.enabled = false;
            }

            //Convert from local vectors to worldspace vectors, then apply
            transform.Rotate(new Vector3(0f, playerCamera.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0f));
            rb.velocity = transform.TransformDirection(new Vector3(hVelocity.x, vVelocity, hVelocity.z));
        }

        animator.SetFloat("VelocityX", hVelocity.x);
        animator.SetFloat("VelocityZ", hVelocity.z);
        animator.SetBool("Grounded", CheckGrounded(0.5f));
    }

    private bool CheckGrounded(float reach = 0.001f)
    {
        //return if the player is on the ground
        //reach variable (default 0.05) is how far the 
        if (Physics.BoxCast(transform.position + (transform.up * 0.5f * transform.localScale.y), new Vector3(0.25f, 0.1f, 0.25f), -transform.up, Quaternion.identity, (0.5f + reach) * transform.localScale.y, groundMask)) return true;
        else return false;
    }

}
