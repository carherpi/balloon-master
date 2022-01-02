using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This Ability reduces the cooldown of all other abilities.
 * It is active at all time. It cannot be called explicitely.
 */
public class Ability_CooldownReduction : Ability
{
    #region OverrideMethods
    protected override void ChildrenAwake() {
        this.isStartup = true;
    }
    /** Enable cooldownreduction effect before the game started. */
    protected override void ChildrenStart()
    {
        this.UseAbility();
    }
    protected override void ChildrenUpdate() { }

    protected override System.TimeSpan GetActiveTimeTotal()
    {
        return this.initialActiveTimeTotal;
    }

    protected override System.TimeSpan GetCoolDownTotal()
    {
        return this.initialCooldownTotal;
    }
    protected override void EnableEffect()
    {
        foreach (string buttonName in new string[] {"Button-LeftAbility", "Button-MidAbility", "Button-RightAbility"})
        {
            // find other abilities
            Ability ability = GameObject.Find(buttonName).GetComponent<Ability>();
            // change their cooldown time
            ability.ChangeCooldownTime(cooldownPercentage);
        }
    }
    #endregion

    #region Attributes
    private double cooldownPercentage = 0.75; // percentage of the total amount of cooldown time (cooldownTime = cooldownTime * cooldownPercentage)
    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game)
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game) // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    #endregion
}
