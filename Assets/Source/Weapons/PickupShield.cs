using UnityEngine;
using System.Collections;

/// <summary>
/// Activates the shield on a ship.
/// </summary>
public class PickupShield : PickupBase {

    public override void OnInit(float dmg, ShipRefs r)
    {
        base.OnInit(dmg, r);
    }

    public override void OnUse()
    {
        // activate shield for 5 seconds
        PU_SHIP.ActivateShield(5.0f);

        base.OnUse();
    }
}
