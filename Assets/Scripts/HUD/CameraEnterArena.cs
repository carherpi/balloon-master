using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEnterArena : MonoBehaviour
{
    [SerializeField]
    private GameAutomaton gameAutomaton;
    [SerializeField]
    private Camera[] cameras;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
//            cameras[1].SetActive();
//            waitForAnimationToEnd();
        }
//        mainCamera.SetActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
