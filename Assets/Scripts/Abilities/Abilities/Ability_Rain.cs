using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* This Ability increases the movement of the balloon for a certain amount of time which is set in activeTimeTotal */
public class Ability_Rain : Ability
{
    #region OverrideMethods
    protected override void ChildrenAwake()
    {
        this.isActive = false;
        pV = GameObject.Find("Abilities").GetComponent<PhotonView>();

        this.balloonMovement = GameObject.FindObjectOfType<BalloonMovement>();
        if (this.balloonMovement == null)
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
        if (rain != null)
        {
            Debug.Log(rain.transform.position);
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
        return this.initialCooldownTotal;
    }
    /** Enables the effect of the ability */
    protected override void EnableEffect()
    {
        this.isActive = true;
        balloonMovement.EnableAbilityRain(this.downforceIncrease);
        rain = Instantiate(Resources.Load("Abilities/Rain")) as GameObject;
        rain.transform.parent = GameObject.Find("CenterOfMap").transform;
        rain.transform.position = GameObject.Find("CenterOfMap").transform.position;

        // Update rain for opponent
        pV.RPC("SetOpponentRain", RpcTarget.Others, 15); //+1 minute
    }
    #endregion

    #region Attributes
    private BalloonMovement balloonMovement;
    private GameObject rain;

    private PhotonView pV;

    private float downforceIncrease = 1.75f;
    private System.TimeSpan initialActiveTimeTotal = new System.TimeSpan(0, 0, 15); // 15 seconds
    private System.TimeSpan initialCooldownTotal = new System.TimeSpan(0, 1, 0); // one minute // only use for GetCoolDownTotal; otherwise use cooldownTotal
    #endregion

    #region RegularFunctions
    private void DisableAbility()
    {
        this.isActive = false;
        balloonMovement.DisableAbilityRain();
        Destroy(rain);
    }
    #endregion
}
