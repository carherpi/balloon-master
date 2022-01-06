using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesLoad : MonoBehaviour
{
    [SerializeField]
    public GameObject ButtonLeftAbility;

    [SerializeField]
    public GameObject ButtonMidAbility;

    [SerializeField]
    public GameObject ButtonRightAbility;

    private Sprite leftAbilitySprite;
    private Sprite midAbilitySprite;
    private Sprite rightAbilitySprite;

    private string abilityLeft;
    private string abilityMid;
    private string abilityRight;

    // Start is called before the first frame update
    void Start()
    {

        // Get PlayerPrefs
        abilityLeft = PlayerPrefs.GetString("Ability1");
        abilityMid = PlayerPrefs.GetString("Ability2");
        abilityRight = PlayerPrefs.GetString("Ability3");

        // Find and Load Sprites from disk
        leftAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityLeft);
        midAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityMid);
        rightAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityRight);

        // Assign them to buttons
        ButtonLeftAbility.GetComponent<Image>().sprite = leftAbilitySprite;
        ButtonMidAbility.GetComponent<Image>().sprite = midAbilitySprite;
        ButtonRightAbility.GetComponent<Image>().sprite = rightAbilitySprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
