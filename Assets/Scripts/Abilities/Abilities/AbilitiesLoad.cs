using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

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
        abilityLeft = PlayerPrefs.GetString("Ability0");
        abilityMid = PlayerPrefs.GetString("Ability1");
        abilityRight = PlayerPrefs.GetString("Ability2");

        // Find and Load Sprites from disk
        leftAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityLeft);
        midAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityMid);
        rightAbilitySprite = Resources.Load<Sprite>("Sprites/" + abilityRight);

        // Assign them to buttons
        ButtonLeftAbility.GetComponent<Image>().sprite = leftAbilitySprite;
        ButtonMidAbility.GetComponent<Image>().sprite = midAbilitySprite;
        ButtonRightAbility.GetComponent<Image>().sprite = rightAbilitySprite;


        // Load scripts dynamically
        ButtonLeftAbility.AddComponent(Type.GetType(abilityLeft)).GetComponent<Button>().onClick.AddListener(() => {
            ButtonLeftAbility.GetComponentInParent<Ability>().UseAbility();
        });
        ButtonMidAbility.AddComponent(Type.GetType(abilityMid)).GetComponent<Button>().onClick.AddListener(() => {
            ButtonMidAbility.GetComponentInParent<Ability>().UseAbility();
        });
        ButtonRightAbility.AddComponent(Type.GetType(abilityRight)).GetComponent<Button>().onClick.AddListener(() => {
            ButtonRightAbility.GetComponentInParent<Ability>().UseAbility();
        });

    }

    [PunRPC]       
    private void SetOpponentMovement(float speed)
    {
        if (speed < 1f)
        {
            GameObject.FindObjectOfType<SimpleSampleCharacterControl>().EnableAbilitySlowMovement(speed);
        } else
        {
            GameObject.FindObjectOfType<SimpleSampleCharacterControl>().DisableAbilitySlowMovement();
        }
        
    }

    [PunRPC]
    private void SetOpponentClock(int time)
    {
        GameObject.Find("HUD/Canvas/Clock").GetComponent<GameClock>().totalTimeMinutes += time;

    }

    [PunRPC]
    private void SetOpponentScore(string opponent)
    {
        GameObject scoreboard = GameObject.Find("HUD/Canvas/Scoreboard");

        if (opponent == "PlayerOne")
        {
            scoreboard.GetComponent<Scoreboard>().SubtractPointToText("PlayerTwo");
        } else
        {
            scoreboard.GetComponent<Scoreboard>().SubtractPointToText("PlayerOne");
        }
    }
}
