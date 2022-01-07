using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilitiesSelection : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI AbilityInformation;

    [SerializeField]
    public GameObject frame;

    [SerializeField]
    public GameObject SA1;
    public GameObject SA2;
    public GameObject SA3;

    // 
    private Sprite SA1Sprite;
    private Sprite SA2Sprite;
    private Sprite SA3Sprite;

    List<string> abilities = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetString("Ability0", "");
        PlayerPrefs.SetString("Ability1", "");
        PlayerPrefs.SetString("Ability2", "");
        PlayerPrefs.Save();

        AbilityInformation.text = "";

        frame.SetActive(false);
        SA1.SetActive(false);
        SA2.SetActive(false);
        SA3.SetActive(false);
    }
  

    public void DisplayAbilityInfo()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        string buttonName = button.name;
        string abilityName = button.GetComponent<Image>().sprite.name;

        // Move red frame
        frame.SetActive(true);
        frame.transform.position = button.transform.position;


        Debug.Log(abilities.Contains(abilityName));
        if (!abilities.Contains(abilityName))
        {
            if (abilities.Count >= 3)
            {
                abilities.RemoveAt(0);
            }

            abilities.Add(abilityName);
            int i = abilities.IndexOf(abilityName);
            SaveAbilities(i);
        }        

        switch (buttonName)
        {
            case "A1":
                AbilityInformation.text = "Increases your character's speed for a few seconds";
                break;

            case "A2":
                AbilityInformation.text = "Increases your character's hit power";
                break;

            case "A3":
                AbilityInformation.text = "Rain appears to your opponent and accelerates the movement of the balloon.";
                break;

            case "A4":
                AbilityInformation.text = "Slow down your opponent's speed for a few seconds";
                break;

            case "A5":
                AbilityInformation.text = "Your opponent will have to score an extra balloon";
                break;

            case "A6":
                AbilityInformation.text = "You will have extra time to score a balloon";
                break;

            default:                
                break;
                
        }
        
    }

    void SaveAbilities(int i)
    {
               
        PlayerPrefs.SetString("Ability0", PlayerPrefs.GetString("Ability1"));
        PlayerPrefs.SetString("Ability1", PlayerPrefs.GetString("Ability2"));
        PlayerPrefs.SetString("Ability2", abilities[i]);
        PlayerPrefs.Save();

        displaySelectedAbilities();

    }

    void displaySelectedAbilities()
    {
        /*
        Debug.Log(PlayerPrefs.GetString("Ability0"));
        Debug.Log(PlayerPrefs.GetString("Ability1"));
        Debug.Log(PlayerPrefs.GetString("Ability2"));
        */

        // Find and Load Sprites from disk
        SA1Sprite = Resources.Load<Sprite>("Sprites/" + PlayerPrefs.GetString("Ability0"));
        SA2Sprite = Resources.Load<Sprite>("Sprites/" + PlayerPrefs.GetString("Ability1"));
        SA3Sprite = Resources.Load<Sprite>("Sprites/" + PlayerPrefs.GetString("Ability2"));

        // Assign them to buttons
        SA1.GetComponent<Image>().sprite = SA1Sprite;
        SA2.GetComponent<Image>().sprite = SA2Sprite;
        SA3.GetComponent<Image>().sprite = SA3Sprite;

        SA1.SetActive(true);
        SA2.SetActive(true);
        SA3.SetActive(true);
    }
}
