using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction;

    //Character Data
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float turnSmoothTime = 0.05f;
    [SerializeField] private float turnSmoothVelocity;

    private void Awake()
    {
        //Initialize variables
        controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    //Make the character Moving
    private void Move() 
    {
        //Avoid the fact that de player move if he not really wanna do it
        if (direction.magnitude >= 0.1f)
        {
            //Rotate the player
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //Move the player
            controller.Move(direction * walkSpeed * Time.deltaTime);
        }
    }

    // Called when the Character Move
    void OnMove(InputValue pos)
    {
        //Define the position that the player want to reach
        Vector2 moveInput = pos.Get<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
    }
}
