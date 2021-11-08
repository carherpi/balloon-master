using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAllObjects : MonoBehaviour
{
    [SerializeField]
    private GameObject[] gameObjects;
    
    // When the scene is set active, it must make its objects visible too
    void OnEnable()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(true);
        }
    }

    // When the scene is set inactive, all its objects must be hidden
    void OnDisable()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
