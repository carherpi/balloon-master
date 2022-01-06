using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability_PointExtra : Ability
{
    #region OverrideMethods
    protected override void ChildrenAwake() {
        this.isStartup = true;
    }

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
        // one minute
        return this.initialCooldownTotal;
    }

    protected override void EnableEffect()
    {
        // if I am player 1 { Scoreboard.scorePlayer2 - 1 }
        // else if im player 2 { Scoreboard.scorePlayer2 - 1 }
    }
    #endregion

    #region Attributes

    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game)
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game) // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    #endregion
}
