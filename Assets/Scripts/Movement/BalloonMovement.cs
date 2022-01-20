using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class BalloonMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private bool isGrounded; // is the player on the ground?
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameAutomaton gameAutomaton;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private SimpleSampleCharacterControl sscc;

    Vector3 desiredPosition;
    ConstantForce desiredForce;

    private Vector3 initPosition;
    private Quaternion initRotation;

    public float maxSpeed;
    public float maxHeight;

    private float speed; // actual speed of the balloon (including abilities)
    private float baseSpeed; // speed of balloon without abilities
    private float slowDownSpeed; // actual slowDownSpeed of the balloon (including abilities)
    private float baseSlowDownSpeed; // slowDownSpeed of balloon without abilities
    private float downforce; // actual downforce that is applied to the balloon (including abilities)
    private float gravity; // force that weighs the balloon down while falling (without abilities)
    
    private float ability_PowerHitFactor;
    private float ability_RainFactor;

    // Accelerometer & Balloon rotation
    private AccelerometerHandler acc;
    private float minMovement = 0.3f;
    Vector3 newDirection;
    Vector3 rotDirection;

    public enum BalloonStates
    {
        GoingUpFast,
        GoingUpSlow,
        Floating,
        GoingDown,
    }

    private BalloonStates balloonState;

    // Start is called before the first frame update
    void Start()
    {
        this.ability_PowerHitFactor = 1;
        this.ability_RainFactor = 1;

        // save initial position and rotation
        initPosition = this.gameObject.transform.position;
        initRotation = this.gameObject.transform.rotation;


        desiredForce = this.gameObject.GetComponent<ConstantForce>();
        desiredForce.enabled = false;


        // rot
        rotDirection = new Vector3(0f, 0.3f, 0f);

        ResetBalloon();

        acc = gameObject.GetComponent<AccelerometerHandler>();
    }
    public void ResetBalloon()
    {
        this.gameObject.transform.position = initPosition;
        this.gameObject.transform.rotation = initRotation;

        desiredPosition = initPosition;
        desiredPosition.y = 0;

        balloonState = BalloonStates.GoingDown;

        this.baseSpeed = maxSpeed;
        this.speed = this.baseSpeed * this.ability_PowerHitFactor;
        this.baseSlowDownSpeed = 0.07f;
        this.gravity = 0.01f;

        // TODO reset the movement
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        this.speed = this.baseSpeed * this.ability_PowerHitFactor;
        this.slowDownSpeed = this.baseSlowDownSpeed * this.ability_RainFactor;
        this.downforce = this.gravity * this.ability_RainFactor;

        if (gameAutomaton.GetGameState() != GameAutomaton.GameStates.GameRunning) // if GameRunning
        {
            return;
        }

        
        goRotate();

        if (this.transform.position.y > maxHeight)
        {
            balloonState = BalloonStates.GoingDown;
            goDown();
        }

        if (balloonState == BalloonStates.GoingUpFast || balloonState == BalloonStates.GoingUpSlow)
        {
            goUp();
        } 

        if (balloonState == BalloonStates.Floating)
        {
            goFloating();
        }

        if (balloonState == BalloonStates.GoingDown)
        {
            goDown();
        }

        //Debug.Log(balloonState);
        //Debug.Log(rotDirection);

        // activate force
        //desiredForce.enabled = true;
    }


    void OnCollisionEnter(Collision collision)
    {

        changeRotation();

        bool isCharacter = collision.gameObject.name.Contains("Boy") || collision.gameObject.name.Contains("Ninja") || collision.gameObject.name.Contains("Girl");

        if (isCharacter)
        {
            this.newDirection = acc.CalculateDirection();
            Debug.Log("NewDirection" + newDirection);   
            sscc.EndJumpToBalloon();
            acc.StopSavingBiggestMovement();
            FastBounce();

        } else // touch floor or anything else
        {
            SlowBounce();            
        }
        //Debug.Log("Collision with Balloon and " + collision.gameObject.name);
        if (collision.gameObject.name.Equals(ground.name))
        {
            gameLogic.BroadcastBalloonHitGround();
        }
        else if (isCharacter && PhotonNetwork.IsMasterClient && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            gameLogic.BroadcastBalloonHitBy(GameLogic.Players.PlayerOne);
        }
        else if (isCharacter && !PhotonNetwork.IsMasterClient && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            gameLogic.BroadcastBalloonHitBy(GameLogic.Players.PlayerTwo);
        }
    }
    void OnTriggerStay(Collider collision)
    {
        // when character enters area to hit the balloon
        bool isCharacter = collision.gameObject.name.Contains("Boy") || collision.gameObject.name.Contains("Ninja") || collision.gameObject.name.Contains("Girl");
        if (isCharacter)
        {
            if ((PhotonNetwork.IsMasterClient && gameLogic.getPlayerToHitBalloon() == GameLogic.Players.PlayerOne)
                || (!PhotonNetwork.IsMasterClient && gameLogic.getPlayerToHitBalloon() != GameLogic.Players.PlayerOne))
            {
                //Debug.Log("Trigger Balloon");
                if (acc.CalculateDirection().magnitude > this.minMovement)
                {
                    //Debug.Log("Trigger move big enough");
                    acc.StartSavingBiggestMovement();
                    sscc.JumpToBalloon();
                }
            }
        }
    }

    public void FastBounce()
    {

        balloonState = BalloonStates.GoingUpFast;

        // update desired position & speed
        desiredPosition.y = maxHeight;
        
        desiredPosition.x = transform.position.x + -newDirection.x * 5; // * SCALAR for testing. We have to think how to estimate the desired force...
        desiredPosition.z = transform.position.z + -newDirection.y * 5;
        baseSpeed = 3.0f;        
    }

    public void SlowBounce()
    {

        balloonState = BalloonStates.GoingUpSlow;

        // update desired position & speed
        desiredPosition.y = 2;
        baseSpeed = 0.3f;        
    }

    public void goUp()
    {
        
        // check if it has arrived desired position
        if (transform.position.y < maxHeight && baseSpeed >= 0) {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);
            baseSpeed -= slowDownSpeed; // slows down the balloon rapidly
            //slowDownSpeed += 0.1f;
        } else
        {
            balloonState = BalloonStates.Floating;
            baseSpeed = 0.0f;
        }
    }

    public void goFloating()
    {
        desiredPosition.y = 0;
        Debug.Log("Floating" + speed);
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);

        /*
        if (n > 0)
        {
            desiredPosition.x += 0.1f;
            desiredPosition.z += 0.1f;
        } else
        {
            desiredPosition.x -= 0.1f;
            desiredPosition.z -= 0.1f;
        }
        */

        baseSpeed += this.downforce;

        if (baseSpeed >= maxSpeed)
        {
            //goDown
            balloonState = BalloonStates.GoingDown;
            desiredPosition.y = 0;
            baseSpeed = maxSpeed;

            /*
            if (n > 0)
            {
                
                desiredPosition.x = transform.position.x + 1f;
                desiredPosition.z = transform.position.z + 1f;
            }
            else
            {
                desiredPosition.x = transform.position.x - 1f;
                desiredPosition.z = transform.position.z - 1f;
            }
            */

        }
    }

    public void goDown()
    {
        desiredPosition.y = 0;
        baseSpeed = maxSpeed;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);        

    }

    public void goRotate()
    {
        // rotation
        Vector3 targetDirection = desiredPosition - transform.position;
        float singleStep = speed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);        
        transform.rotation = Quaternion.LookRotation(newDirection); 
    }
    public void changeRotation()
    {        
        //rotDirection = newDirection.normalized;        
    }

    /** speedIncrease is a percentage of the current speed.
     *  Examples: The input 1 would not change the speed. 1.5 would set the speed to speed * 1.5
     */
    public void EnableAbilityPowerHit(float speedIncrease)
    {
        this.ability_PowerHitFactor = speedIncrease;
    }
    public void DisableAbilityPowerHit()
    {
        this.ability_PowerHitFactor = 1;
    }

    /** downforceIncrease is a percentage of the current downforce.
     *  Examples: The input 1 would not change the downforce. 1.5 would set the downforce = downforce * 1.5
     */
    public void EnableAbilityRain(float speedIncrease)
    {
        this.ability_RainFactor = speedIncrease;
    }
    public void DisableAbilityRain()
    {
        this.ability_RainFactor = 1;
    }

}
