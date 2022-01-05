using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
	public GameObject[] characters;
	public int selectedCharacter = 0;

    public void Awake()
    {
		
	}


    public void Start()
	{
		foreach (GameObject gameObject in characters)
		{
			gameObject.SetActive(false);
		}
		characters[0].SetActive(true);
	}

	public void NextCharacter()
	{
		characters[selectedCharacter].SetActive(false);
		selectedCharacter = (selectedCharacter + 1) % characters.Length;
		characters[selectedCharacter].SetActive(true);
	}

	public void PreviousCharacter()
	{
		characters[selectedCharacter].SetActive(false);
		selectedCharacter--;
		if (selectedCharacter < 0)
		{
			selectedCharacter += characters.Length;
		}
		characters[selectedCharacter].SetActive(true);
	}

	public void StartGame()
	{
		//PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
		//SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	
	void OnDestroy()
	{
		PlayerPrefs.SetString("CharacterName", characters[selectedCharacter].name);
		PlayerPrefs.Save();
	}
}
