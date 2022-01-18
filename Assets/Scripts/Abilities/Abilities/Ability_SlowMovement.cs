using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ability_SlowMovement : Ability
{
    private PhotonView pV;

    #region OverrideMethods
    protected override void ChildrenAwake()
    {
        this.isStartup = false;      
        pV = GameObject.Find("Abilities").GetComponent<PhotonView>();

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
    [PunRPC]
    protected override void EnableEffect()
    {
        pV.RPC("SetOpponentMovement", RpcTarget.Others, speedDecrease); //0.5f parameter speedDescrease for function SetOpponentMovement() on AbilitiesLoad
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
        movement.DisableAbilitySlowMovement();
    }
        
    #endregion
}
