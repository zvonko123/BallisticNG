using UnityEngine;
using System.Collections;

/// <summary>
/// Activates the shield on a ship.
/// </summary>
public class PickupShield : PickupBase {

    private bool timerStarted = false;

    public override void OnUse()
    {
        // activate shield for 5 seconds
        if (!timerStarted)
        {
            PU_SHIP.ActivateShield(5.0f);
            timerStarted = true;
        }
    }

    public override void OnDrop()
    {
        if (PU_SHIP.shieldTimer > 0)
            PU_SHIP.shieldTimer = 0;

        base.OnDrop();
    }

    public override void OnUpdate()
    {
        if (!PU_SHIP.shieldActivate && timerStarted)
            OnDrop();

        base.OnUpdate();
    }
}
