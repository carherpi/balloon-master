using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicVars : MonoBehaviour
{
    #region GameObjects

    [SerializeField] public List<GameObject> gameObjects;

    public void SetGameObject(GameObject newGameObject)
    {
        gameObjects.Add(newGameObject);
    }
    public bool HasGameObject(string name)
    {
        return null != GetGameObject(name);
    }
    public GameObject GetGameObject(string name)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.name.Equals(name))
            {
                return gameObject;
            }
        }
        return null;
    }
    #endregion

    #region Constants
    public readonly string player1Name = "Boy";
    public readonly string player2Name = "BoyClient";
    #endregion

}
