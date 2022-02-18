using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController CC;
    [SerializeField] float speed;
    float crouchSpeed = 3f;
    float walkSpeed = 7f;
    float sprintSpeed = 14f;
    [SerializeField, Range(0, 10)] float slideSlow;
    [SerializeField] bool sliding;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jump = 10f;
    [SerializeField] float flight = 10f;
    [SerializeField] float crouchRadius = 0.2f;
    Vector3 velocity;
    [SerializeField] Transform groundCheck, flyCheck, crouchCheck;
    [SerializeField] float groundDistance = 0.6f;
    [SerializeField] LayerMask groundMask; //What layer the object is on
    [SerializeField] Image flyBar;
    bool canFly;
    bool isGrounded;
    // Start is called before the  first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        CC.stepOffset = 0f; //When the character is in the air, the slope limit is set to 0. This prevents the character from spazzing when when jumping onto something
        
        #region Movement
        float x = Input.GetAxis("Horizontal"); // + or - based on what key is being pressed (between a and d)
        float z = Input.GetAxis("Vertical"); // + or - between w and s)

        if (z != 0f && x != 0f) //If moving diagonal, reduce speed so that you dont go faster diagonally 
        {
            z = z / 1.25f;
            x = x / 1.25f;
        }
        
        if (Input.GetKey("left shift") && !Input.GetKey("c")) //While left shift is being pressed, speed is increased (sprinting)
        {
            speed = sprintSpeed;
        }
        else if (!Input.GetKey("left shift") && !sliding && !Input.GetKey("c")) //When you aren't running or crouching or sliding then you are set to walk speed
        {
            speed = walkSpeed;
        }
        #endregion

        #region Crouching and Sliding
        bool crouchSphere = Physics.CheckSphere(crouchCheck.position, crouchRadius, groundMask); //Checks a region above the player to see if there is ground there.
        sliding = false; //Allows you to be slowed to crouch speed

        if (speed > crouchSpeed && isGrounded) //If you are walking and on the ground
        {
            if (speed > walkSpeed && Input.GetKey("c")) //If you are running and press c 
            {
                sliding = true; //You cannot be instantly slowed to crouch speed while sliding
                speed = speed - (slideSlow * Time.deltaTime); //Gradually slow down
            }
            else if (speed < walkSpeed && Input.GetKey("c")) //If you are slower and you hold c then you will continue to slow down
            {
                sliding = true;
                speed = speed - (slideSlow * Time.deltaTime);
            }
            //These if statements means that you can only start sliding when you are sprinting but can continue to slide even if you are slower than walk speed, you will keep sliding until you reach crouch speed
        }


        if (Input.GetKey("c"))
        {
            if (transform.localScale.y > 0.5f) //Prevents you from becoming flat
            { transform.localScale = transform.localScale - new Vector3(0f, 10f, 0f) * Time.deltaTime; } //Used so that you gradually crouch //Can specify player.transform... but because this script is on the player it is assumed that you are acting on the player

            if (isGrounded && !sliding) 
            { speed = crouchSpeed; } //You instantly slow down, unless in the air or you are sliding
        }
        else
        {
            if (transform.localScale.y < 1f && !crouchSphere) //If you are not pressing ctrl, not underneeth anything and you are not already full size, return to normal size
            { transform.localScale = transform.localScale + new Vector3(0f, 10f, 0f) * Time.deltaTime; } //Used to not instantly uncrouch

            else if (transform.localScale.y < 1f && !sliding) //If you are smaller than normal and not sliding, go to crouch speed (basically if you are under something but not holding c)
            { speed = crouchSpeed; }
        }
        #endregion

        #region Flight
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //If touching ground, returns true
        bool ballGrounded = Physics.CheckSphere(flyCheck.position, 0.5f, groundMask); //Checks a region under the player to see if there is ground there.
        //canFly is necessary to allow you to jump. Otherwise, as soon space is pressed, velocity.y is overwritten by flight 
            
        if (isGrounded && velocity.y < 0)
        {velocity.y = -6f;}

        if (!ballGrounded)
        {canFly = true;} //No idea why this works, but allows you to be close to ground and start flying again
        
        if (isGrounded)
        {
            CC.stepOffset = 0.7f; //Earler, we set step offset to 0 when in the air. This makes slope limit .7 on the ground, allowing you to climb stairs and slopes (not sure if this is necessary, but probably good practice to have this reset once you reach the ground)
            canFly = false; //This prevents jump being overwritten by flight (basically, you can't fly until you are a certain height above the ground, allowing you to jump and then fly)
            if (flight < 30)
            { flight += 30f * Time.deltaTime; } //For some reason
            
            if (Input.GetButtonDown("Jump")) //If on ground and jump //ButtonDown only activated the frame that button is pressd
            {velocity.y = jump;}
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
    }
}
