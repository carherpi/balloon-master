using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    #region AbstractMethods
    /** Substitute for the Start method. */
    protected abstract void ChildrenStart();
    /** Substitute for the Update method. */
    protected abstract void ChildrenUpdate();
    /** The total cooldown time of this ability */
    protected abstract System.TimeSpan GetCoolDownTotal();
    /** Enables the effect of the ability */
    protected abstract void EnableEffect();
    #endregion

    #region Attributes
    private GameAutomaton gameAutomaton;

    private bool isAvailable; // true if ability is available; false if it is recharging
    private System.TimeSpan cooldownTotal; // total time of the cooldown of this ability
    private System.DateTime lastUsed; // the time when the ability was last used (does not make sense before the first usage)
    private double percentageCooldown; // value between 0 (just used) and 1 (fully recharged). Knows the status of cooldown in percentage

    #endregion

    #region PublicFunctions
    public void UseAbility()
    {
        // only activate if it is allowed
        if (!(this.gameAutomaton.GetGameState() == GameAutomaton.GameStates.GameRunning)
            || !isAvailable)
        {
            return;
        }
        this.EnableEffect();
        this.isAvailable = false;
        this.percentageCooldown = 0;
        this.lastUsed = System.DateTime.UtcNow;
    }
    #endregion

    #region RegularFunctions
    /* Do not use Start() in child-class! Please use ChildrenStart() which is called in this Start()
     * We try not to override the Start() method.
     */ 
    void Start()
    {
        // init values
        this.gameAutomaton = GameObject.FindObjectOfType<GameAutomaton>();
        if (this.gameAutomaton == null)
        {
            Debug.LogError("No GameAutomaton could be found");
        }
        isAvailable = true;
        lastUsed = System.DateTime.UtcNow;
        cooldownTotal = this.GetCoolDownTotal();

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
    #endregion
}
