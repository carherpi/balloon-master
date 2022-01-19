using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** This class is the parent class of all abilities.
 *  
 *  When implementing this class, do not use built-in Unity functions. Instead implement the function here and call a children function.
 *  For example, in Start() we call ChildrenStart(). If you want to add something like OnCollisionTrigger(), please use the same schema just to be safe.
 *  Note that you should use the override keyword for abstract methods, and the new keyword for overriding regular methods.
 *  
 *  Note that private attributes and functions are not visible in children classes. You may want to use the protected keyword.
 */
public abstract class Ability : MonoBehaviour
{
    #region ChildrenMethods
    /** Substitute for the Start method. */
    protected abstract void ChildrenStart();
    /** Substitute for the Update method. */
    protected abstract void ChildrenUpdate();
    /** Substitute for the Awake method. */
    protected abstract void ChildrenAwake();

    /** The total active time of this ability */
    protected abstract System.TimeSpan GetActiveTimeTotal();
    /** The total cooldown time of this ability */
    protected abstract System.TimeSpan GetCoolDownTotal();
    /** Enables the effect of the ability */
    protected abstract void EnableEffect();
    #endregion

    #region Attributes
    protected GameAutomaton gameAutomaton;

    protected bool isStartup; // set it to true in ChildrenAwake() function if the ability should be activated before the game
    protected bool isAvailable; // true if ability is available; false if it is recharging
    protected bool isActive; // true if ability is active
    protected System.TimeSpan activeTimeTotal; // active time of ability
    protected System.TimeSpan cooldownTotal; // total time of the cooldown of this ability
    protected double percentageCooldown; // value between 0 (just used) and 1 (fully recharged). Holds the status of cooldown in percentage
    protected System.DateTime lastUsed; // the time when the ability was last used (does not make sense before the first usage)

    protected GameObject currentButton;

    #endregion

    #region PublicFunctions
    public void UseAbility()
    {
        Debug.Log("UseAbility()");
        // only activate if it is allowed
        if (!(this.gameAutomaton.GetGameState() == GameAutomaton.GameStates.GameRunning)
            && !this.isStartup)
        { return;}
        if (!this.isAvailable) { return; }
        this.EnableEffect();
        this.isAvailable = false;
        this.percentageCooldown = 0;
        this.lastUsed = System.DateTime.UtcNow;

        // 
        currentButton = EventSystem.current.currentSelectedGameObject;
        UpdateButtonState();
    }

    public void ChangeCooldownTime(double cooldownPercentage)
    {
        Debug.Log("Old cooldown time: " + this.cooldownTotal);
        long ticks = (long) (this.cooldownTotal.Ticks * cooldownPercentage);
        this.cooldownTotal = new System.TimeSpan(ticks);
        Debug.Log("New cooldown time: " + this.cooldownTotal);
    }
    #endregion

    #region RegularFunctions
    void Awake()
    {
        // init values
        this.gameAutomaton = GameObject.FindObjectOfType<GameAutomaton>();
        if (this.gameAutomaton == null)
        {
            Debug.LogError("No GameAutomaton could be found");
        }
        this.isStartup = false;
        this.isAvailable = true;
        this.isActive = false;
        this.lastUsed = System.DateTime.UtcNow;
        this.activeTimeTotal = this.GetActiveTimeTotal();
        this.cooldownTotal = this.GetCoolDownTotal();
        this.percentageCooldown = 1;
        this.lastUsed = System.DateTime.UtcNow;

        this.ChildrenAwake();
    }
    void Start()
    {
        this.ChildrenStart();
    }

    void Update()
    {
        this.UpdateAvailability();
        this.ChildrenUpdate();
    }


    /** Updates isAvailable and percentageCooldown */
    protected void UpdateAvailability()
    {
        if (!this.isAvailable)
        {
            System.TimeSpan timeElapsed = System.DateTime.UtcNow - lastUsed;
            if (timeElapsed >= this.cooldownTotal)
            {
                isAvailable = true;
                percentageCooldown = 1;
                UpdateButtonState();
            }
            else
            {
                percentageCooldown = this.cooldownTotal.TotalMilliseconds / timeElapsed.TotalMilliseconds;
            }
        }
    }

    /** The total time that the ability is active is passed. This function returns whether the function is still active */
    protected bool IsAbilityActive(System.TimeSpan abilityTimeTotal)
    {
        return (System.DateTime.UtcNow - lastUsed) < abilityTimeTotal;
    }

    protected void UpdateButtonState()
    {
        bool buttonState = currentButton.GetComponent<Button>().interactable;
        if (buttonState)
        {
            currentButton.GetComponent<Button>().interactable = false;
        } else
        {
            currentButton.GetComponent<Button>().interactable = true;
        }

    }
    #endregion
}
