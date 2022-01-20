using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;

public class SimpleSampleCharacterControl : MonoBehaviourPun
{
    [SerializeField] private FollowObject jumpSFX;
    //[SerializeField] private FollowObject runningSFX;
    private enum ControlMode
    {
        /// <summary>
        /// Up moves the character forward, left and right turn the character gradually and down moves the character backwards
        /// </summary>
        Tank,
        /// <summary>
        /// Character freely moves in the chosen direction from the perspective of the camera
        /// </summary>
        Direct
    }

    [SerializeField] private float initial_moveSpeed = 2;
    [SerializeField] private float initial_turnSpeed = 200;
    private float m_moveSpeed;
    private float m_turnSpeed;
    private float ability_FastMovementFactor = 1;
    private float ability_SlowMovementFactor = 1;
    [SerializeField] private float m_jumpForce = 4;

    private bool jumpToBalloon = false;
    private float percentageBalloonJump = 1;
    private Vector2 moveToBalloon = Vector2.zero;

    [SerializeField] private Collider balloonCollider;

    [SerializeField] private Transform character;
    [SerializeField] private Transform balloon;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    public Vector3 spawnServePlayerPos = new Vector3(13f, 1f, 4f);
    public readonly Quaternion spawnServePlayerRot = Quaternion.identity;
    public Vector3 spawnWaitPlayerPos = new Vector3(12f, 5f, 0f);
    public readonly Quaternion spawnWaitPlayerRot = Quaternion.identity;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;
    private bool m_jumpInput = false;

    private bool m_isGrounded = true;


    private List<Collider> m_collisions = new List<Collider>();


    public GameAutomaton GameAutomatonScript;

    private Vector2 inputMovement;
    private bool inputButton;

    private void Start()
    {
        m_moveSpeed = initial_moveSpeed;
        m_turnSpeed = initial_turnSpeed;
    }

    private void Awake()
    {
        character.GetComponent<ForwardCollisions>().SetSimpleSampleCharacterControl(this);

        GameAutomatonScript = FindObjectOfType<GameAutomaton>();

        if (!m_animator) { gameObject.GetComponent<Animator>(); }
        if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }

        Debug.Log("Awake received");

        //runningSFX.Mute(true);
    }

    public void OnChararcterCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    public void OnChararcterCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if (validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        }
        else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    public void OnChararcterCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

    private void Update()
    {
        // use this for increasing the speed of the person
        m_moveSpeed = initial_moveSpeed * ability_FastMovementFactor * ability_SlowMovementFactor;
        m_turnSpeed = initial_turnSpeed * ability_FastMovementFactor * ability_SlowMovementFactor;

        this.inputMovement = GetComponent<GyroscopeHandler>().GetPlayerInputMovement();
        //Debug.Log("SSCC inputMovement: " + this.inputMovement);
        // A critical aspect of user control over the network is that the same prefab will be instantiated for all players, but only one of them represents the user actually playing in front of the computer, all other instances represents other users, playing on other computers.
        //if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        //{
        //    return;
        //}

        if (GameAutomatonScript.GetGameState() == GameAutomaton.GameStates.GameRunning) // if GameRunning
        {
            if (!m_jumpInput && inputButton) //Input.GetKey(KeyCode.Space))
            {
                m_jumpInput = true;
                jumpSFX.PlaySound();
            }
        }
        inputButton = false;
    }

    private void FixedUpdate()
    {
        if (this.jumpToBalloon)
        {
            this.UpdateJumpToBalloonMovement();
        }

        m_animator.SetBool("Grounded", m_isGrounded);

        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }

        m_wasGrounded = m_isGrounded;
        m_jumpInput = false;
    }

    private void TankUpdate()
    {
        float v;
        float h;
        if (this.jumpToBalloon)
        {
            // if we are jumping to balloon
            v = this.moveToBalloon.y;
            h = this.moveToBalloon.x;
        }
        else
        {
            // normal mode
            v = inputMovement.y;
            h = inputMovement.x;
        }

        bool walk = false; // Input.GetKey(KeyCode.LeftShift);

        if (v < 0)
        {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        }
        else if (walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        character.position += character.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        character.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);

        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
        //if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        //{
        //    return;
        //}

        if (GameAutomatonScript.GetGameState() == GameAutomaton.GameStates.GameRunning) // if GameRunning
        {
            float v;
            float h;
            if (this.jumpToBalloon)
            {
                // if we are jumping to balloon
                v = this.moveToBalloon.y;
                h = this.moveToBalloon.x;
            }
            else
            {
                // normal mode
                v = inputMovement.y;
                h = inputMovement.x;
            }

            // soundFX if running
            //runningSFX.Mute(!(m_isGrounded && (v!=0 || h !=0)));

            Transform camera = Camera.main.transform;

            // we don't use LeftShift
            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    v *= m_walkScale;
            //    h *= m_walkScale;
            //}

            m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
            m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

            Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

            float directionLength = direction.magnitude;
            direction.y = 0;
            direction = direction.normalized * directionLength;

            if (direction != Vector3.zero)
            {
                m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

                character.rotation = Quaternion.LookRotation(m_currentDirection);
                character.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

                m_animator.SetFloat("MoveSpeed", direction.magnitude);
            }

            JumpingAndLanding();
        }
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;        

        if (jumpCooldownOver && m_isGrounded && m_jumpInput)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce * this.percentageBalloonJump, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
            this.percentageBalloonJump = 1; // reset value
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

    // update input of w,a,s,d and joystick
    public void OnMovement(InputValue value)
    {
        /*
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        */
        //if (photonView.IsMine)
        //{
//Gyro instead            inputMovement = value.Get<Vector2>();
        //}
    }

    // update input of space and multiFuncButton
    public void OnMultiFuncButton(InputValue value)
    {
        inputButton = value.isPressed;
    }

    public void JumpToBalloon()
    {
        this.UpdateJumpToBalloonMovement();
        this.jumpToBalloon = true;
        inputButton = true; // activate jump
        //Debug.Log("moveToBalloon" + moveToBalloon);
        //Debug.Log("percentageJump" + percentageBalloonJump);
    }

    private void UpdateJumpToBalloonMovement()
    {
        //Debug.Log("Position Balloon" + this.balloon.position);
        //Debug.Log("Position Charact" + character.position);
        // calculate new vector to reach the balloon
        Vector3 diffVector = this.balloon.position - character.position;
        Vector2 newMove = new Vector2(-diffVector.x, - diffVector.z); // z is to the back
        if (newMove.magnitude > 1)
        {
            newMove.Normalize();
        }
        this.moveToBalloon = newMove;
        float curJumpForce = Math.Min(1, diffVector.y / 2); // y is upwards
        this.percentageBalloonJump = Math.Max(0.3f, curJumpForce); // y is upwards
        //Debug.Log("percentageJump" + this.percentageBalloonJump);
    }

    public void EndJumpToBalloon()
    {
        this.jumpToBalloon = false;
    }
    public void ResetPlayer(bool isServing)
    {
        if (isServing)
        {
            character.position = spawnServePlayerPos;
            character.rotation = spawnServePlayerRot;
        }
        else
        {
            character.position = spawnWaitPlayerPos;
            character.rotation = spawnWaitPlayerRot;
        }
    }

    public void SetPlayer(GameObject activePlayer)
    {
        Debug.Log("Set SSCC");
        character.GetComponent<ForwardCollisions>().ReleaseSimpleSampleCharacterControl();
        activePlayer.GetComponent<ForwardCollisions>().SetSimpleSampleCharacterControl(this);

        // set character for movement
        character = activePlayer.GetComponent<Transform>();
        m_animator = activePlayer.GetComponent<Animator>();
        m_rigidBody = activePlayer.GetComponent<Rigidbody>();
    }

    public void SetBalloonCollisionActive(GameLogic.Players activePlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Physics.IgnoreCollision(character.GetComponent<Collider>(), balloonCollider, activePlayer != GameLogic.Players.PlayerOne);
            Debug.Log("IgnoreCollision for master = " + (activePlayer != GameLogic.Players.PlayerOne));
        }
        else
        {
            Physics.IgnoreCollision(character.GetComponent<Collider>(), balloonCollider, activePlayer != GameLogic.Players.PlayerTwo);
            Debug.Log("IgnoreCollision for client = " + (activePlayer != GameLogic.Players.PlayerTwo));
        }
    }

    /** speedIncrease is a percentage of the current speed.
     *  Examples: The input 1 would not change the speed. 1.5 would set the speed the speed to speed * 1.5
     */
    public void EnableAbilityFastMovement(float speedIncrease)
    {
        this.ability_FastMovementFactor = speedIncrease;
    }
    
    public void DisableAbilityFastMovement()
    {
        this.ability_FastMovementFactor = 1;
    }

    /* Decreased speel */
    public void EnableAbilitySlowMovement(float speedDecrease)
    {
        this.ability_SlowMovementFactor = speedDecrease;
    }
    
    public void DisableAbilitySlowMovement()
    {
        this.ability_SlowMovementFactor = 1;
    }
}