using UnityEngine;
using BnG.TrackData;
using System.Collections;

public class PickupTripLaser : PickupBase
{
    // track data
    private TrSection currentSection;

    public override void OnInit(float dmg, ShipRefs r, E_WEAPONS wClass)
    {
        base.OnInit(dmg, r, wClass);

        // get current track section
        currentSection = r.initialSection;
    }
}
