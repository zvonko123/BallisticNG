using UnityEngine;
using System.Collections;

public class PickupBase {

    // base variables
    public float PU_DMG;
    public ShipRefs PU_SHIP;

    /// <summary>
    /// Call to set variables.
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="r"></param>
    public virtual void OnInit(float dmg, ShipRefs r)
    {
        PU_DMG = dmg;
        PU_SHIP = r;
    }

    /// <summary>
    /// Called when the pickup is used.
    /// </summary>
    public virtual void OnUse()
    {
        PU_SHIP.pickup = null;
    }

    /// <summary>
    /// Called when the pickup is dropped.
    /// </summary>
    public virtual void OnDrop()
    {
        PU_SHIP.pickup = null;
    }

    /// <summary>
    /// Called to update the pickup.
    /// </summary>
    public virtual void OnUpdate()
    {
        if (Input.GetButtonDown("Use Pickup"))
            OnUse();

        if (Input.GetButtonDown("Drop Pickup"))
            OnDrop();
    }

}

public enum E_WEAPONS
{
    ROCKETS,
    MISSILES,
    MINES,
    TRIPLASER,
    PLASMABOLT,
    ENERGYWALL,
    BOMB,
    IMPULSE,
    CANNONS,
    SHIELD,
    AUTOPILOT,
    EPACK,
    FSHIELD,
    REFLECTOR,
    CLOAK
}

public enum E_WEAPONCLASSES
{
    AGRESSIVE,
    PACIFIST,
    MIXED
}