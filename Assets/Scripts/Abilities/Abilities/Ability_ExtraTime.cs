using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ability_ExtraTime : Ability
{
    public GameObject clock;
    private PhotonView pV;

    #region OverrideMethods
    protected override void ChildrenAwake() {
        this.isStartup = false;
        pV = GameObject.Find("Abilities").GetComponent<PhotonView>();

        clock = GameObject.Find("HUD/Canvas/Clock");
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
        // Update our Clock
        clock.GetComponent<GameClock>().totalTimeMinutes += 1;

        // Update opponent clock
        pV.RPC("SetOpponentClock", RpcTarget.Others, 1); //+1 minute
    }
    #endregion

    #region Attributes

    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game)
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game) // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    #endregion
}
