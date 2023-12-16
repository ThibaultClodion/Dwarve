using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{
    //Component
    private TrailRenderer trail;
    private CharacterController controller;
    private Animator animator;
    private PlayerInput input;
    private GameManager gameManager;

    [Header("Default Data")]
    //Default Data
    [SerializeField] GameObject hand;
    private float gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    private bool canAttack = true;
    private Vector3 direction;
    private Vector3 nonNullDirection;  //Allow the player to perform dash while not moving
    private Vector3 rotation;
    private bool canDash = true;
    public bool isPlaying = false;  //Game Manager need to modify this bool

    [Header("Sword Data")]
    //Data of his sword
    [SerializeField] private HiltData hiltData;

    [Header("Weapon Canvas")]
    //Weapon Canvas and data associated
    [SerializeField] private GameObject weaponCanvas;
    private bool weaponReady;
    [SerializeField] private GameObject[] weaponReadyDisapear;


    //Character Data
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

    public void getGameManager()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    #endregion

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

    #region UI

    public void MoveWeaponCanvas()
    {
        GameObject layout = GameObject.Find("GroupLayout");

        //If we are in the seleciton menu than move the canvas to group all the canvas
        if (layout != null)
        {
            weaponCanvas.SetActive(true);
            weaponCanvas.transform.SetParent(layout.transform, false);
        }
        else
        {
            weaponCanvas.SetActive(false);
        }
    }

    public void WeaponReady()
    {
        if (!weaponReady)
        {
            gameManager.nbWeaponPlayersReady++;
            weaponReady = true;

            for (int i = 0; i < weaponReadyDisapear.Length; i++)
            {
                weaponReadyDisapear[i].SetActive(false);
            }
        }

        if (gameManager.nbWeaponPlayersReady == gameManager.players.Length && weaponReady)
        {
            gameManager.StartRound();
        }
    }

    public void WeaponNotReady()
    {
        if (weaponReady)
        {
            gameManager.nbWeaponPlayersReady--;
            weaponReady = false;

            for (int i = 0; i < weaponReadyDisapear.Length; i++)
            {
                weaponReadyDisapear[i].SetActive(true);
            }
        }
    }

    void OnCancel()
    {
        //Otherwise the script is call during the menu
        if(SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (weaponReady)
            {
                WeaponNotReady();
            }
            else
            {
                WeaponReady();
            }
        }
    }
    #endregion

    #region Movement

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

    #region ActionInput
    //Called when the Character attack
    void OnFire() 
    {
        if(canAttack && isPlaying)
        {
            canAttack = false;
            StartCoroutine(AttackCooldown());
            animator.SetTrigger("Attack");
        }
    }

    //Called when the Chracter perform a Dash
    void OnDash()
    {
        if (canDash && isPlaying)
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
        isPlaying = false;
        gameManager.nbPlayersAlive--;

        //If there is only one survivant
        if (gameManager.nbPlayersAlive == 1)
        {
            gameManager.Victory();
        }
        else
        {
            //The else is necessary or the player could be disable at unexpected time
            gameObject.SetActive(false);
        }
    }
}
