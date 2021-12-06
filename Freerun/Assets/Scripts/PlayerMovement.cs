using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController CC;
    float speed;
    float walkspeed = 7f;
    float sprintspeed = 14f;
    public float gravity = -9.81f;
    public float jump = 10f;
    public float flight = 1f;
    Vector3 velocity;
    public Transform groundCheck;
    float groundDistance = 0.2f;
    public LayerMask groundMask; //What layer the object is on
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        float x = Input.GetAxis("Horizontal"); // + or - based on what key is being pressed (between a and d)
        float z = Input.GetAxis("Vertical"); // + or - between w and s)

        if (z != 0f && x != 0f) //If moving diagonal, reduce speed so that you dont go faster diagonally 
        {
            z = z / 1.25f;
            x = x / 1.25f;
        }
        
        if (Input.GetKey("left shift")) //While left shift is being pressed, speed is increased (sprinting)
        {
            speed = sprintspeed;
        }
        else
        {
            speed = walkspeed;
        }
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //If touching ground, returns true
        if (isGrounded)
        {
            flight = 1;
            if (Input.GetButtonDown("Jump")) //If on ground and jump //ButtonDown only activated the frame that button is pressd
            {
                velocity.y = jump;
            }
        }
        /*if (Input.GetButton("Jump") && !isGrounded && flight > 0) //If space held while mid air  //GetButton activated every frame while space is pressed
        {
            flight -= 5f * Time.deltaTime; //Every frame you are flying, you can fly less
            velocity.y = 5f; 
        }*/

        Vector3 move = transform.right * x + transform.forward * z; // Increase move right by x amount and move forwards by z amount. If s is pressed, then z would be negtive, moving you backwards
        CC.Move(move * speed * Time.deltaTime); //Apply move to the Character controller, * speed (to determine the speed) and Time.deltaTime
        //Time.deltaTime is the time elapsed between frames. EG: At 20 fps, it is 0.05 and at ps it is 0.1. This means that movement is consistent no matter what your fps

        velocity.y += gravity * Time.deltaTime; //Adds gravity
        CC.Move(velocity * Time.deltaTime); //Applies gravity
    }
}
