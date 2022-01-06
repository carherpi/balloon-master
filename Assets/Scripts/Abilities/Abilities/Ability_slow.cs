using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_slow : Ability
{
    #region OverrideMethods
    protected override void ChildrenAwake()
    {
        this.isActive = false;
        this.movement = GameObject.FindObjectOfType<SimpleSampleCharacterControl>();
        if (this.movement == null)
        {
            Debug.LogError("No SimpleSampleCharacterControl could be found");
        }
    }
    protected override void ChildrenStart() { }
    protected override void ChildrenUpdate()
    {
        if (isActive && !this.IsAbilityActive(this.activeTimeTotal))
        {
            this.DisableAbility();
        }
    }
    /** The total active time of this ability */
    protected override System.TimeSpan GetActiveTimeTotal()
    {
        return this.initialActiveTimeTotal;
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
        movement.EnableAbilityLowMovement(this.speedDecrease);
    }
    #endregion

    #region Attributes
    private SimpleSampleCharacterControl movement;
    private float speedDecrease = 0.50f;
    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(0, 0, 15); // 15 seconds
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(0, 1, 0); // one minute // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    private void DisableAbility()
    {
        this.isActive = false;
        movement.DisableAbilityLowMovement();
    }
    #endregion
}
