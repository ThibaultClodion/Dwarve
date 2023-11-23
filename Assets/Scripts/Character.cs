using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Character : MonoBehaviour
{
    private TrailRenderer trail;
    private CharacterController controller;
    private Animator animator;

    //Default Data
    [SerializeField] GameObject hand;
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    private bool canAttack = true;
    private Vector3 direction;
    private Vector3 nonNullDirection;  //Allow the player to perform dash while not moving
    private bool canDash = true;

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
        //Player Movement
        Gravity();
        Rotation();
        Move();
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
        if (direction.magnitude <= 1f) return;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
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

    void OnDash()
    {
        if (canDash)
        {
            canDash = false;
            StartCoroutine(DashCoroutine(0.25f));
        }
    }

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


        //End the trail at this precise coolddown to look good
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
