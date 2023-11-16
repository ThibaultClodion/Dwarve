using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Character : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    private Vector3 direction;

    //Default Data
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    private bool canAttack = true;

    //Character Data
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float turnSmoothTime = 0.05f;
    [SerializeField] private float turnSmoothVelocity;
    [SerializeField] private float attackCooldown = 2f;


    private void Awake()
    {
        //Initialize variables
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void Hit()
    {
        Debug.Log(gameObject.name + " Has been attacked");
    }
}
