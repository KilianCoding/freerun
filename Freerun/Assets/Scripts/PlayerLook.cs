using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    #region Variables
    public float mouseSens = 100f;
    public Transform playerBody; //When we move the mouse on the x axis, we want the whole player object (graphic and camera) to follow suit
    float xRotation = 0f;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; //Locks the cursor to the centre of the screen
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime; //Gets the x value of how much the mouse has moved * mouse sensitivity and time delta time
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime; //Time.deltaTime prevents faster framerate = higher sens

        //Uncomment below to see how Input.GetAxis works
        //Debug.Log(Input.GetAxis("Mouse X"));

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX); //Rotate the player gameobject on the Y axis by one * the x value of the mouse
    }
    #endregion
}
    