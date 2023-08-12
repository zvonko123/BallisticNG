using UnityEngine;
using BnG.TrackData;
using BnG.Helpers;
using System.Collections;

public class ShipPosition : ShipBase {

    // track data
    public int cSectionIndex;
    public int iSectionIndex;
    public bool iSectionFound;
    public bool onJump;
    private TrSection expectedLandSection;

    // respawn
    public Vector3 respawnPosition;
    public Quaternion respawnRotation;

    public override void OnInit()
    {
        // set initial respawn transform
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    public override void OnUpdate()
    {
        if (!r.isRespawning)
        {
            UpdateInitialSection();
            UpdateCurrentSection();
            UpdateDirection();
        }
    }

    private void UpdateInitialSection()
    {
        float distance = Mathf.Infinity;
        float newDistance = distance;
        int i = 0;
        int length = RaceSettings.trackData.TRACK_DATA.SECTIONS.Count;
        for (i = 0; i < length; ++i)
        {
            newDistance = Vector3.Distance(transform.position, RaceSettings.trackData.TRACK_DATA.SECTIONS[i].SECTION_POSITION);
            if (newDistance < distance)
            {
                distance = newDistance;
                iSectionFound = true;
                r.currentSection = RaceSettings.trackData.TRACK_DATA.SECTIONS[i];
            }
        }

    }

    private void UpdateCurrentSection()
    {
        if (!iSectionFound) return;

        // update respawn
        respawnPosition = r.currentSection.SECTION_NEXT.SECTION_POSITION;
        respawnPosition.y += r.settings.AG_HOVER_HEIGHT;
        respawnRotation = TrackDataHelper.SectionGetRotation(r.currentSection.SECTION_NEXT);
    }

    private void UpdateDirection()
    {
        Vector3 trackRot = TrackDataHelper.SectionGetRotation(r.currentSection) * Vector3.forward;

        float dot = Vector3.Dot(transform.forward, trackRot.normalized);
        if (dot > 0)
            r.facingFoward = true;
        else if (dot < 0)
            r.facingFoward = false;
    }

}
