using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    //Component
    private TrailRenderer trail;
    private CharacterController controller;
    private Animator animator;
    private PlayerInput input;

    //Default Data
    [SerializeField] GameObject hand;
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    private bool canAttack = true;
    public Vector3 direction;
    private Vector3 nonNullDirection;  //Allow the player to perform dash while not moving
    public Vector3 rotation;
    private bool canDash = true;
    public bool isPlaying = false;  //Game Manager need to modify this bool

    //Data of sword
    [SerializeField] private HiltData hiltData;

    //Character Data
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float turnSmoothTime = 0.05f;
    [SerializeField] private float turnSmoothVelocity;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float dashCooldown = 3f;


    private void Awake()
    {
        //Initialize variables
        trail = GetComponent<TrailRenderer>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Allow the player to dash without having perform any mouvement
        nonNullDirection = transform.forward.normalized;

        //Initially there is no trail
        trail.widthMultiplier = 0.0f;

        //Init the sword
        hiltData.Init(hand);
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaying) 
        {
            //Player Movement
            Gravity();
            Rotation();
            Move();
        }
    }

    //Apply gravity
    private void Gravity()
    {
        if(controller.isGrounded && velocity < 0.0f)
        {
            velocity = -1f;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        direction.y = velocity;
    }

    //Rotate the Character to where is going
    private void Rotation()
    {
        //if (rotation.magnitude <= 1f) return;

        float targetAngle = Mathf.Atan2(rotation.x, rotation.y) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    //Move the Character
    private void Move() 
    {
        controller.Move(direction * walkSpeed * Time.deltaTime);
    }

    // Called when the Character Move
    void OnMove(InputValue pos)
    {
        //Define the position that the player want to reach
        Vector2 moveInput = pos.Get<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if(direction.magnitude >= 1f) 
        {
            nonNullDirection = direction;
        }
    }

    void OnRotate(InputValue pos)
    {
        //Define the position that the player want to reach
        Vector2 rotateInput = pos.Get<Vector2>();

        if(input.currentControlScheme == "Keyboard&Mouse")
        {
            Debug.Log("Yes");

            //Formula for transform the mouse screen position to [-1;1]
            rotateInput.y = (rotateInput.y / (Screen.height / 2)) - 1;
            rotateInput.x = (rotateInput.x / (Screen.width / 2)) - 1;

            rotation = new Vector3(rotateInput.x, rotateInput.y, 0f).normalized;
        }
        else
        {
            //Keep in Mind previous rotation if gamepad is not moving
            if (rotateInput.magnitude >= 1f)
            {
                Debug.Log("Yes");
                rotation = new Vector3(rotateInput.x, rotateInput.y, 0f).normalized;
            }
        }
    }

    //Called when the Character attack
    void OnFire() 
    {
        if(canAttack)
        {
            canAttack = false;
            StartCoroutine(AttackCooldown());
            animator.SetTrigger("Attack");
        }
    }

    //Called when the Chracter perform a Dash
    void OnDash()
    {
        if (canDash)
        {
            canDash = false;

            //Make the dash
            StartCoroutine(DashCoroutine(0.25f));
        }
    }

    //Make a dash with dashTime duration
    private IEnumerator DashCoroutine(float dashTime)
    {
        float startTime = Time.time;

        //Begin the trail behond the player
        trail.widthMultiplier = 1f;

        while (Time.time < startTime + dashTime)
        {
            controller.Move(nonNullDirection * Time.deltaTime * walkSpeed * 2);
            yield return null;
        }


        //End the trail at this precise cooldown to look good I don't find precise formula
        yield return new WaitForSeconds(dashCooldown - 1.75f);
        trail.widthMultiplier = 0f;

        yield return new WaitForSeconds(1.75f);
        canDash = true;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void Hit()
    {
        Debug.Log(gameObject.name + " Has been attacked");
        Destroy(gameObject);
    }
}
