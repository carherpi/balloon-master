using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ability_PointExtra : Ability
{
    public GameObject scoreboard;
    private string whoami;
    private PhotonView pV;

    #region OverrideMethods
    protected override void ChildrenAwake() {
        this.isStartup = false;
        scoreboard = GameObject.Find("HUD/Canvas/Scoreboard");

        pV = GameObject.Find("Abilities").GetComponent<PhotonView>();
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
        // Update my scoreboard
        scoreboard.GetComponent<Scoreboard>().SubtractPointToText(whoami);

        // Update opponent scoreboard
        pV.RPC("SetOpponentScore", RpcTarget.Others, whoami); 
    }
    #endregion

    #region Attributes

    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game)
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(1, 0, 0); // 1 hour (longer than the game) // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    #endregion
}
