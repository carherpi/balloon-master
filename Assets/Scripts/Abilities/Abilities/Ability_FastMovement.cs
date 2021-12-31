using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_FastMovement : Ability
{
    #region OverrideMethods
    /** Substitute for the Start method. */
    protected override void ChildrenStart()
    {
        this.isActive = false;
        this.movement = GameObject.FindObjectOfType<SimpleSampleCharacterControl>();
        if (this.movement == null)
        {
            Debug.LogError("No SimpleSampleCharacterControl could be found");
        }
        Debug.Log("StartChildren is run");
    }
    /** Substitute for the Update method. */
    protected override void ChildrenUpdate()
    {
        if (isActive && !this.IsAbilityActive(this.activeTimeTotal))
        {
            this.DisableAbility();
        }
    }
    /** The total cooldown time of this ability */
    protected override System.TimeSpan GetCoolDownTotal()
    {
        // one minute
        return this.initialCooldownTotal;
    }
    /** Enables the effect of the ability */
    protected override void EnableEffect()
    {
        this.isActive = true;
        movement.EnableAbilityFastMovement(this.speedIncrease);
    }
    #endregion

    #region Attributes
    private SimpleSampleCharacterControl movement;
    private float speedIncrease = 1.75f;
    private bool isActive;
    private System.TimeSpan activeTimeTotal = new System.TimeSpan(0, 0, 15); // 15 seconds
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(0, 1, 0); // one minute // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    private void DisableAbility()
    {
        this.isActive = false;
        movement.DisableAbilityFastMovement();
    }
    #endregion
}
