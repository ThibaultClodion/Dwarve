using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Datas
    //Component
    private TrailRenderer trail;
    private CharacterController controller;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    public Character character;

    [Header("Default Data")]
    //Default Data
    public GameObject hand;
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity = 0f;
    public bool canAttack = true;
    private Vector3 direction;
    private Vector3 nonNullDirection;  //Allow the player to perform dash while not moving
    private Vector3 rotation;
    private bool canDash = true;
    private bool haveAttackAnimation = false;   //Tell if the player have an attack animation because make it on ResetData cause errors.

    //Player Data
    private float walkSpeed = 10f;
    private float turnSmoothTime = 0.05f;
    private float turnSmoothVelocity;
    private float attackCooldown = 0.5f;
    private float dashCooldown = 1f;

    #endregion

    #region Initialisation
    private void Awake()
    {
        //Initialize variables
        trail = GetComponent<TrailRenderer>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
    }


    void Start()
    {
        ResetData();
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

    //Rotate the Player to where is going
    private void Rotation()
    {
        //if (rotation.magnitude <= 1f) return;

        float targetAngle = Mathf.Atan2(rotation.x, rotation.y) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    //Move the Player
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

            //Rotate the player toward his movement
            rotation = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        }
    }

    // Change the Rotation with second joystick
    public void ChangeRotate(InputValue pos, bool useKeyboard)
    {
        /*
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
        */
    }
    #endregion

    #region Actions
    public void Attack()
    {
        if (canAttack)
        {
            //Set the attack animation link to the hilt
            if (!haveAttackAnimation)
            {
                animatorOverrideController["DefaultAttack"] = character.GetSword().GetHilt().attack;
                haveAttackAnimation = true;
            }

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

    #region CharacterCommunications
    public void ResetData()
    {
        canAttack = true;
        canDash = true;

        //Reset the animation
        animator.writeDefaultValuesOnDisable = true;
        haveAttackAnimation = false;
        

        //Allow the player to dash without having perform any mouvement
        nonNullDirection = transform.forward.normalized;

        //Initially there is no trail
        trail.widthMultiplier = 0.0f;

        //Reset Rotation and direction
        nonNullDirection = new Vector3(0, 0, 0);
        direction = new Vector3(0, 0, 0);
        rotation = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

    }

    public void Hit()
    {
        //Give the character info that he dies
        character.Die();
    }
    #endregion
}
