using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : MonoBehaviour
{
    // Variables
    [SerializeField] private bool isGrounded; // is the player on the ground?
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;

    private Vector3 velocity;

    public GameAutomaton GameAutomatonScript;

    private void Awake()
    {
        GameAutomatonScript = FindObjectOfType<GameAutomaton>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        
        
    }
}
