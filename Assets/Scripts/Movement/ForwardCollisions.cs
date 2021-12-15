using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardCollisions : MonoBehaviour
{
    public SimpleSampleCharacterControl sSCC;

    public void SetSimpleSampleCharacterControl(SimpleSampleCharacterControl sSCC)
    {
        this.sSCC = sSCC;
    }
    public void ReleaseSimpleSampleCharacterControl()
    {
        this.sSCC = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (sSCC == null)
        {
            return;
        }
        sSCC.OnChararcterCollisionEnter(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (sSCC == null)
        {
            return;
        }

        sSCC.OnChararcterCollisionStay(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (sSCC == null)
        {
            return;
        }

        sSCC.OnChararcterCollisionExit(collision);
    }
}
