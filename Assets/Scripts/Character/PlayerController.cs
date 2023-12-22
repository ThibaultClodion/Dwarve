using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Component
    private TrailRenderer trail;
    private CharacterController controller;
    private Animator animator;

    [Header("Default Data")]
    //Default Data
    [SerializeField] GameObject hand;
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity = 0f;
    private bool canAttack = true;
    private Vector3 direction;
    private Vector3 nonNullDirection;  //Allow the player to perform dash while not moving
    private Vector3 rotation;
    private bool canDash = true;

    [Header("Sword Data")]
    //Data of his sword
    [SerializeField] private HiltData hiltData;

    //Player Data
    private float walkSpeed = 10f;
    private float turnSmoothTime = 0.05f;
    private float turnSmoothVelocity;
    private float attackCooldown = 2f;
    private float dashCooldown = 3f;

    #region Initialisation
    private void Awake()
    {
        //Initialize variables
        trail = GetComponent<TrailRenderer>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        //Allow the player to dash without having perform any mouvement
        nonNullDirection = transform.forward.normalized;

        //Initially there is no trail
        trail.widthMultiplier = 0.0f;

        //Init the sword
        hiltData.Init(hand);
    }

    #endregion


    #region Movement

    //Apply gravity
    private void Gravity()
    {
        if (controller.isGrounded && velocity < 0.0f)
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

    public void Movement()
    {
        //Apply all the movement
        Gravity();
        Rotation();
        Move();
    }

    // Change the Movement
    public void ChangeMove(InputValue pos)
    {
        //Define the position that the player want to reach
        Vector2 moveInput = pos.Get<Vector2>();
        direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (direction.magnitude >= 1f)
        {
            nonNullDirection = direction;
        }
    }

    // Change the Rotation
    public void ChangeRotate(InputValue pos, bool useKeyboard)
    {
        //Define the position that the player want to reach
        Vector2 rotateInput = pos.Get<Vector2>();

        if (useKeyboard)
        {
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
                rotation = new Vector3(rotateInput.x, rotateInput.y, 0f).normalized;
            }
        }
    }
    #endregion

    #region Actions
    public void Attack()
    {
        if (canAttack)
        {
            canAttack = false;
            StartCoroutine(AttackCooldown());
            animator.SetTrigger("Attack");
        }
    }

    public void Dash()
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
    #endregion

    public void Hit()
    {
        Debug.Log("is Hit");
    }
}
