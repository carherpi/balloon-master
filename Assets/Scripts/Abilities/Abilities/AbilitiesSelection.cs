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
    }
  

    public void DisplayAbilityInfo()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        string buttonName = button.name;
        string abilityName = button.GetComponent<Image>().sprite.name;

        // Move red frame
        frame.SetActive(true);
        frame.transform.position = button.transform.position;

        if (abilities.Count > 3)
        {
            abilities.RemoveAt(0);            
        }        

        if (!abilities.Contains(abilityName))
        {
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
        PlayerPrefs.SetString("Ability" + i, abilities[i]);
        
        PlayerPrefs.Save();
    }
}
