using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BalloonMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private bool isGrounded; // is the player on the ground?
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameAutomaton gameAutomaton;
    [SerializeField] private GameLogic gameLogic;

    Vector3 desiredPosition;
    ConstantForce desiredForce;

    private Vector3 initPosition;
    private Quaternion initRotation;

    public float maxSpeed;
    public float maxHeight;

    private float speed;
    private float offset;
    private float n;
    private float slowDownSpeed;
    private float rotDirection;

    private float desiredRot;
    public float rotSpeed = 5;
    public float damping = 10;
    public Quaternion desiredRotQ;

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
        // save initial position and rotation
        initPosition = this.gameObject.transform.position;
        initRotation = this.gameObject.transform.rotation;


        desiredForce = this.gameObject.GetComponent<ConstantForce>();
        desiredForce.enabled = false;     



        // rot
        desiredRot = transform.eulerAngles.z;
        desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);

        ResetBalloon();
    }
    public void ResetBalloon()
    {
        this.gameObject.transform.position = initPosition;
        this.gameObject.transform.rotation = initRotation;

        desiredPosition = initPosition;
        desiredPosition.y = 0;

        balloonState = BalloonStates.GoingDown;

        speed = maxSpeed;
        slowDownSpeed = 0.1f;

        // TODO reset the movement
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if ((int)gameAutomaton.GetGameState() != 3) // if GameRunning
        {
            return;
        }

        transform.Rotate(new Vector3(rotDirection * 0.1f, rotDirection *  0.1f, rotDirection * 0.3f));


        if (balloonState == BalloonStates.GoingUpFast || balloonState == BalloonStates.GoingUpSlow)
        {
            goUp();
            //Shake();
        } 

        if (balloonState == BalloonStates.Floating)
        {
            goFloating();
        }

        if (balloonState == BalloonStates.GoingDown)
        {
            goDown();
        }
        
        // activate force
        //desiredForce.enabled = true;
    }


    void OnCollisionEnter(Collision collision)
    {

        n = Random.Range(-1.0f, 1.0f);
        if (n > 0)
        {
            offset = Random.Range(-1.5f, -0.5f);
            rotDirection = -1.0f;
        }
        else
        {
            offset = Random.Range(0.5f, 1.5f);
            rotDirection = 1.0f;
        }

        if (collision.gameObject.name.Contains("Boy"))
        {
            FastBounce();

        } else // touch floor or anything else
        {

            SlowBounce();            
        }
        Debug.Log("Collision with Balloon and " + collision.gameObject.name);
        if (collision.gameObject.name.Equals(ground.name))
        {
            gameLogic.BalloonHitGround();
        }
    }


    public void FastBounce()
    {

        balloonState = BalloonStates.GoingUpFast;

        // update desired position & speed
        desiredPosition.y = maxHeight;

        Debug.Log("Offset!");
        Debug.Log(offset);

        desiredPosition.x = transform.position.x + offset;
        desiredPosition.z = transform.position.z + offset;
        speed = 3.0f;        
    }

    public void SlowBounce()
    {

        balloonState = BalloonStates.GoingUpSlow;

        // update desired position & speed
        desiredPosition.y = 2;
        speed = 0.3f;        
    }

    public void goUp()
    {
        
        // check if it has arrived desired position
        if (transform.position.y < maxHeight & speed >= 0) {
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);
            speed -= slowDownSpeed; // slows down the balloon rapidly
            //slowDownSpeed += 0.1f;
        } else
        {
            balloonState = BalloonStates.Floating;
            speed = 0.0f;
        }
    }

    public void goFloating()
    {
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);
        
        if (n > 0)
        {
            desiredPosition.x += 0.1f;
            desiredPosition.z += 0.1f;
        } else
        {
            desiredPosition.x -= 0.1f;
            desiredPosition.z -= 0.1f;
        }
        
        speed += 0.01f;

        if (speed >= maxSpeed)
        {
            balloonState = BalloonStates.GoingDown;
            desiredPosition.y = 0;

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
            
        }
    }

    public void goDown()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * speed);        

    }

    public void Shake()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * damping);
    }
}
