using UnityEngine;
using System.Collections;

public class PickupEPack : PickupBase
{
    public override void OnUse()
    {
        // increase shield by 25%
        PU_SHIP.shield += PU_SHIP.shield * 0.25f;

        base.OnUse();
    }
}
