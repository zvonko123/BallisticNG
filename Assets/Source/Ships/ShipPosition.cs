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

    void Start()
    {
        // set initial respawn transform
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
    }

    void FixedUpdate()
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
        // try to find track section using a raycast
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 1000.0f, 1 << LayerMask.NameToLayer("TrackFloor")))
        {
            int tri = hit.triangleIndex;
            TrTile tile = TrackDataHelper.TileFromTriangleIndex(hit.triangleIndex, E_TRACKMESH.FLOOR, RaceSettings.trackData.TRACK_DATA);
            TrSection section = tile.TILE_SECTION;

            bool canUpdate = true;
            if (section.SECTION_TYPE == E_SECTIONTYPE.JUMP_START)
            {
                onJump = true;
                expectedLandSection = section.SECTION_NEXT;
            }

            if (section.SECTION_TYPE == E_SECTIONTYPE.JUMP_END)
            {
                onJump = false;
                if (section != expectedLandSection)
                    canUpdate = false;
            }

            if (canUpdate)
            {
                r.initialSection = section;
                iSectionIndex = section.SECTION_INDEX;
                r.currentSection = r.initialSection;
                cSectionIndex = section.SECTION_INDEX;
            }
            iSectionFound = true;
        } else
        {
            iSectionFound = false;
        }
        */
        float distance = Mathf.Infinity;
        for (int i = 0; i < RaceSettings.trackData.TRACK_DATA.SECTIONS.Count; i++)
        {
            float newDistance = Vector3.Distance(transform.position, RaceSettings.trackData.TRACK_DATA.SECTIONS[i].SECTION_POSITION);
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
