using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    public CharacterController CC;
    float speed;
    float walkspeed = 7f;
    float sprintspeed = 14f;
    public float gravity = -9.81f;
    public float jump = 10f;
    public float flight = 10f;
    Vector3 velocity;
    public Transform groundCheck;
    public Transform flyCheck;
    float groundDistance = 0.2f;
    public LayerMask groundMask; //What layer the object is on
    public Image flyBar;
    bool canFly;
    // Start is called before the  first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        #region Movement
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

        #region Flight
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //If touching ground, returns true
        bool ballGrounded = Physics.CheckSphere(flyCheck.position, 0.5f, groundMask); //Checks a region under the player to see if there is ground there.
        //canFly is necessary to allow you to jump. Otherwise, as soon space is pressed, velocity.y is overwritten by flight 
            
        if (!ballGrounded)
        {
            canFly = true; //No idea why this works, but allows you to be close to ground and start flying again
        }
        if (isGrounded)
        {
            canFly = false;
            if (flight < 30)
            {
                flight += 20f * Time.deltaTime; //For some reason
            }
            if (Input.GetButtonDown("Jump")) //If on ground and jump //ButtonDown only activated the frame that button is pressd
            {
                velocity.y = jump;
            }
        }
        if (Input.GetButton("Jump") && canFly && flight > 0) //If space held while far enough from ground  //GetButton activated every frame while space is pressed 
        {
            flight -= 50f * Time.deltaTime; //Every frame you are flying, you can fly less
            velocity.y = 5f;
        }
        flyBar.fillAmount = flight / 30; //bar fill is the percentage of flight left
        #endregion

        Vector3 move = transform.right * x + transform.forward * z; // Increase move right by x amount and move forwards by z amount. If s is pressed, then z would be negtive, moving you backwards
        CC.Move(move * speed * Time.deltaTime); //Apply move to the Character controller, * speed (to determine the speed) and Time.deltaTime
        //Time.deltaTime is the time elapsed between frames. EG: At 20 fps, it is 0.05 and at ps it is 0.1. This means that movement is consistent no matter what your fps

        velocity.y += gravity * Time.deltaTime; //Adds gravity
        CC.Move(velocity * Time.deltaTime); //Applies upwards movement. If you are not moving, then velocity is constantly being reduced by gravity

        #endregion
    }
}
