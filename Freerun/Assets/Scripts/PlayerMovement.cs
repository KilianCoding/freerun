using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController CC;
    float speed;
    float crouchSpeed = 3f;
    float walkSpeed = 7f;
    float sprintSpeed = 14f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jump = 10f;
    [SerializeField] float flight = 10f;
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
        
        if (Input.GetKey("left shift")) //While left shift is being pressed, speed is increased (sprinting)
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
        #endregion

        #region Crouching
        bool crouchSphere = Physics.CheckSphere(crouchCheck.position, 0.2f, groundMask); //Checks a region under the player to see if there is ground there.
        
        if (Input.GetKey("left ctrl"))
        {
            if (transform.localScale.y > 0.5f) //Prevents you from becoming flat
            { transform.localScale = transform.localScale - new Vector3(0f, .05f, 0f); } //Used so that you gradually crouch //Can specify player.transform... but because this script is on the player it is assumed that you are acting on the player


            if (isGrounded)
            { speed = crouchSpeed; } //You slow down, unless in the air
        }
        else
        {
            if (transform.localScale.y < 1f && !crouchSphere) //If you are not pressing ctrl and you are not already full size, return to normal size
            { transform.localScale = transform.localScale + new Vector3(0f, .05f, 0f); } //Used to not instantly uncrouch
            else if (transform.localScale.y < 1f)
            { speed = crouchSpeed; }
        }
        #endregion

        #region Flight
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //If touching ground, returns true
        bool ballGrounded = Physics.CheckSphere(flyCheck.position, 0.5f, groundMask); //Checks a region under the player to see if there is ground there.
        //canFly is necessary to allow you to jump. Otherwise, as soon space is pressed, velocity.y is overwritten by flight 
            
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -6f;
        }

        if (!ballGrounded)
        {
            canFly = true; //No idea why this works, but allows you to be close to ground and start flying again
        }
        if (isGrounded)
        {
            CC.stepOffset = 0.7f; //Earler, we set step offset to 0 when in the air. This makes slope limit .7 on the ground, allowing you to climb stairs and slopes (not sure if this is necessary, but probably good practice to have this reset once you reach the ground)
            canFly = false; //This prevents jump being overwritten by flight (basically, you can't fly until you are a certain height above the ground, allowing you to jump and then fly)
            if (flight < 30)
            {
                flight += 30f * Time.deltaTime; //For some reason
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
    }
}
