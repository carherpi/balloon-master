using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] allScenes;
    [SerializeField]
    private GameObject[] allOptionalSceneObjects;
    [SerializeField]
    private GameObject startupScene, mainMenuScene;
    private GameObject activeScene;
    private List<GameObject> prevActiveScenes = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        // disable all optional scene objects
        foreach (GameObject obj in allOptionalSceneObjects)
        {
            obj.SetActive(false);
        }

        activeScene = startupScene;
        LoadScene(activeScene);
    }

    public void LoadScene(GameObject scene)
    {
        if (allScenes.Contains(scene))
        {
            // adjust vars
            if (activeScene != scene)
            {
                prevActiveScenes.Add(activeScene);
                activeScene = scene;
            }

            ActivateCorrectScene();
            scene.SetActive(true);
        }
        else
        {
            Debug.LogError("The scene " + scene.name + " is not known to the SceneManager.");
        }
    }

    public void LoadPrevScene()
    {
        if (prevActiveScenes.Count != 0)
        {
            // adjust vars
            activeScene = prevActiveScenes.Last();
            prevActiveScenes.RemoveAt(prevActiveScenes.Count - 1);
        }
        else
        {
            // in case there is no previous scene, go to main menu
            activeScene = mainMenuScene;
        }
        ActivateCorrectScene();
    }

    private void ActivateCorrectScene()
    {
        foreach (GameObject scene in allScenes)
        {
            scene.SetActive(false);
        }
        activeScene.SetActive(true);
    }
}
