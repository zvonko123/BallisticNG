using UnityEngine;
using BnG.TrackData;
using System.Collections;

public class ShipSim : ShipBase {

    // engine
    public float enginePower;
    public float engineThrust;
    public float engineAccel;
    public float engineHyper;

    // turning
    public float turnAmount;
    public float turnVelocity;
    public float prevTurnInput;

    public float pitchAmount;

    // airbrakes
    public float airbrakeAmount;
    public float airbrakeVelocity;
    public float prevAirbrakeInput;
    public bool isShipBraking;

    public float brakeDrag;
    public float brakeGain;
    public float brakeFalloff;

    // tilt
    public float tiltAmount;
    public float tiltVelocity;
    public float tiltGain;
    public float tiltFalloff;
    public bool tiltGainReset;
    private float tiltBounce;

    // gravity
    private float gravityForce;
    public bool isShipGrounded;
    private bool hitTrack;

    // grip
    private float airbrakeSlip;
    private float airbrakeSlide;

    // resistance
    private float airbrakeResistance;
    private float airResistance;
    private float pitchResistance;

    // collision
    public bool isShipScraping;
    private float collisionBounce;
    private float wantedCollisionBounce;

    void FixedUpdate()
    {
        if (r.isRespawning)
        {
            ShipRespawn();
        } else
        {
            ShipAcceleration();
            ShipRotation();
            ShipGravity();
            ShipDrag();
        }
    }

    private void ShipRespawn()
    {
        // lerp to respawn position
        r.body.velocity = Vector3.zero;
        transform.position = Vector3.MoveTowards(transform.position, r.position.respawnPosition, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, r.position.respawnRotation, Time.deltaTime * 0.2f);

        // if close enough to respawn position then stop respawning
        if (Vector3.Distance(transform.position, r.position.respawnPosition) < 0.3f)
            r.isRespawning = false;

        // reset engine
        engineThrust = 0;
    }

    private void ShipAcceleration()
    {
        // get speed class based values
        float maxSpeed = 0.0f;
        float acceleration = 0.0f;
        float powerGain = 0.0f;
        float powerFalloff = 0.0f;
        switch (RaceSettings.speedclass)
        {
            case E_SPEEDCLASS.SPARK:
                maxSpeed = r.settings.ENGINE_MAXSPEED_SPARK;
                acceleration = r.settings.ENGINE_ACCELERATION_SPARK;
                powerGain = r.settings.ENGINE_POWER_GAIN_SPARK;
                powerFalloff = r.settings.ENGINE_POWER_FALLOFF_SPARK;
                break;
            case E_SPEEDCLASS.TOXIC:
                maxSpeed = r.settings.ENGINE_MAXSPEED_TOXIC;
                acceleration = r.settings.ENGINE_ACCELERATION_TOXIC;
                powerGain = r.settings.ENGINE_POWER_GAIN_TOXIC;
                powerFalloff = r.settings.ENGINE_POWER_FALLOFF_TOXIC;
                break;
            case E_SPEEDCLASS.APEX:
                maxSpeed = r.settings.ENGINE_MAXSPEED_APEX;
                acceleration = r.settings.ENGINE_ACCELERATION_APEX;
                powerGain = r.settings.ENGINE_POWER_GAIN_APEX;
                powerFalloff = r.settings.ENGINE_POWER_FALLOFF_APEX;
                break;
            case E_SPEEDCLASS.HALBERD:
                maxSpeed = r.settings.ENGINE_MAXSPEED_HALBERD;
                acceleration = r.settings.ENGINE_ACCELERATION_HALBERD;
                powerGain = r.settings.ENGINE_POWER_GAIN_HALBERD;
                powerFalloff = r.settings.ENGINE_POWER_FALLOFF_HALBERD;
                break;

            case E_SPEEDCLASS.SPECTRE:
                maxSpeed = r.settings.ENGINE_MAXSPEED_SPECTRE;
                acceleration = r.settings.ENGINE_ACCELERATION_SPECTRE;
                powerGain = r.settings.ENGINE_POWER_GAIN_SPECTRE;
                powerFalloff = r.settings.ENGINE_POWER_FALLOFF_SPECTRE;
                break;
        }

        // braking Check
        if (r.input.AXIS_LEFTAIRBRAKE != 0 && r.input.AXIS_RIGHTAIRBRAKE != 0)
            isShipBraking = true;
        else
            isShipBraking = false;

        // engine Power Inc/Dec
        if (r.input.ACTION_THRUST)
        {
            enginePower = Mathf.MoveTowards(enginePower, 1.0f, Time.deltaTime * powerGain);
        }
        else
        {
            if (isShipBraking)
                enginePower = Mathf.MoveTowards(enginePower, 0.0f, Time.deltaTime * powerFalloff * 5);
            else
                enginePower = Mathf.MoveTowards(enginePower, 0.0f, Time.deltaTime * powerFalloff);
        }
        engineAccel = Mathf.MoveTowards(engineAccel, enginePower, Time.deltaTime * acceleration);

        // hyper thrust
        if (r.input.ACTION_SPECIAL && r.shield > 25)
        {
            engineHyper = 2f;
            r.shield -= Time.deltaTime * 20;
        }
        else
        {
            engineHyper = 1.0f;
        }

        // interpolate thrust to maxspeed and use enginepower as a multiplier
        if (!RaceSettings.shipsRestrained && !r.shipRestrained)
            engineThrust = Mathf.Lerp(engineThrust, ((maxSpeed * engineAccel) * engineHyper) + r.boostAccel, Time.deltaTime * (acceleration * engineHyper));

        // apply Force
        r.body.AddRelativeForce(Vector3.forward * engineThrust);
    }

    private void ShipRotation()
    {
        // calculate turn force
        float turnForce = r.settings.TURN_SPEED * Time.deltaTime * Mathf.Rad2Deg;
        float turnNormal = (Mathf.Abs(turnVelocity) / turnForce);
        float tiltNormal = (Mathf.Abs(tiltVelocity) / 55);
        turnNormal = Mathf.Clamp(turnNormal, r.settings.TURN_NORMAL_MIN, r.settings.TURN_NORMAL_MAX);
        tiltNormal = Mathf.Clamp(tiltNormal, 0.35f, 0.6f);

        // tilting gain reset
        if ((r.input.AXIS_STEER > 0 && tiltVelocity < 0) || (r.input.AXIS_STEER < 0 && tiltVelocity > 0))
        {
            if (!tiltGainReset)
            {
                tiltGainReset = true;
                //tiltGain = 0;
            }
        }
        else
        {
            tiltGainReset = false;
        }

        // update Turn and Tilt
        if (Mathf.Abs(r.input.AXIS_STEER) >= Mathf.Abs(prevTurnInput) && r.input.AXIS_STEER != 0)
        {
            if ((r.input.AXIS_STEER > 0 && turnVelocity < 0) || (r.input.AXIS_STEER < 0 && turnVelocity > 0))
            {
                turnNormal *= 2;
            }

            turnVelocity = Mathf.MoveTowards(turnVelocity, r.input.AXIS_STEER * turnForce, Time.deltaTime * (r.settings.TURN_GAIN * turnNormal));

            tiltFalloff = 0;
            tiltGain = Mathf.Lerp(tiltGain, r.settings.TILT_GAIN, Time.deltaTime * 5.0f);
            tiltVelocity = Mathf.Lerp(tiltVelocity, r.input.AXIS_STEER * -55.0f, Time.deltaTime * tiltGain * tiltNormal);
        }
        else
        {
            turnVelocity = Mathf.MoveTowards(turnVelocity, r.input.AXIS_STEER * turnForce, Time.deltaTime * r.settings.TURN_FALLOFF);

            tiltGain = 0;
            tiltFalloff = Mathf.Lerp(tiltFalloff, r.settings.TILT_FALLOFF, Time.deltaTime * 2.0f);
            tiltVelocity = Mathf.Lerp(tiltVelocity, r.input.AXIS_STEER * -55.0f, Time.deltaTime * tiltFalloff);
        }
        prevTurnInput = r.input.AXIS_STEER;
        tiltAmount = Mathf.Lerp(tiltAmount, tiltVelocity, Time.deltaTime * r.settings.TILT_GAIN);
        turnAmount = Mathf.MoveTowards(turnAmount, turnVelocity, Time.deltaTime * 5);

        // calculate Airbrake force
        float airbrakeForce = r.settings.AIRBRAKE_SPEED * Time.deltaTime * (Mathf.Rad2Deg * 0.1f);
        float airbrakeSpeed = ((transform.InverseTransformDirection(r.body.velocity).z * 10) * Time.deltaTime) * airbrakeForce;

        float airbrakeNormal = (Mathf.Abs(airbrakeVelocity) / airbrakeForce);
        airbrakeNormal = Mathf.Clamp(airbrakeNormal, r.settings.AIRBRAKE_NORMAL_MIN, r.settings.AIRBRAKE_NORMAL_MAX);

        if (Mathf.Abs(r.input.AXIS_BOTHAIRBRAKES) >= Mathf.Abs(prevAirbrakeInput) && r.input.AXIS_BOTHAIRBRAKES != 0)
        {
            if ((r.input.AXIS_BOTHAIRBRAKES > 0 && airbrakeVelocity < 0) || (r.input.AXIS_BOTHAIRBRAKES < 0 && airbrakeVelocity > 0))
            {
                airbrakeNormal *= 2;
            }

            airbrakeVelocity = Mathf.MoveTowards(airbrakeVelocity, r.input.AXIS_BOTHAIRBRAKES * airbrakeSpeed, Time.deltaTime * (r.settings.AIRBRAKE_GAIN * airbrakeNormal));
        }
        else
        {
            airbrakeVelocity = Mathf.MoveTowards(airbrakeVelocity, r.input.AXIS_BOTHAIRBRAKES * airbrakeSpeed, Time.deltaTime * r.settings.AIRBRAKE_FALLOFF);
        }
        prevAirbrakeInput = r.input.AXIS_BOTHAIRBRAKES;

        airbrakeAmount = Mathf.Lerp(airbrakeAmount, airbrakeVelocity, Time.deltaTime * 5);

        // apply final rotation
        transform.Rotate(Vector3.up * (turnAmount + airbrakeAmount));

        // apply tilt
        r.axis.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, tiltAmount + tiltBounce);

        // wall bounce
        wantedCollisionBounce = Mathf.Lerp(wantedCollisionBounce, 0.0f, Time.deltaTime * 10.0f);
        collisionBounce = Mathf.Lerp(collisionBounce, wantedCollisionBounce, Time.deltaTime * 50.0f);
        tiltBounce = Mathf.Lerp(tiltBounce, (-collisionBounce * 80), Time.deltaTime * 5);
        transform.Rotate(Vector3.up * collisionBounce);
    }

    private void ShipGravity()
    {
        // get position and normal of current section
        TrSection current = r.currentSection;
        Vector3 trackPosition = current.SECTION_POSITION;
        Vector3 trackNormal = current.SECTION_NORMAL;
        if (r.position.onJump)
            trackNormal = current.SECTION_NORMAL;
        float ground = trackPosition.y;

        // reset grounded
        isShipGrounded = false;

        // get raycast origin (defaults to ship position unless on a jump)
        Vector3 raypos = transform.position;
        if (current.SECTION_TYPE == E_SECTIONTYPE.JUMP_START && r.input.ACTION_THRUST)
        {
            Vector3 landPos = current.SECTION_NEXT.SECTION_POSITION;
            raypos.x = landPos.x;
            raypos.z = landPos.z;
        }

        // do raycast
        RaycastHit hit;
        if (Physics.Raycast(raypos, -transform.up, out hit, r.settings.AG_HOVER_HEIGHT, 1 << LayerMask.NameToLayer("TrackFloor")))
        {
            // return if not under hover height
            if (hit.distance > r.settings.AG_HOVER_HEIGHT) return;

            // set grounded
            isShipGrounded = true;

            // bottom out on track
            if (hit.distance < r.settings.AG_HOVER_HEIGHT * r.settings.AG_REBOUND_THRESHOLD && !hitTrack)
            {
                hitTrack = true;
                float hitForce = (Mathf.Abs(transform.InverseTransformDirection(r.body.velocity).y) * r.settings.AG_REBOUND_MULTIPLIER);
                if (hitForce >= 1)
                {
                    r.body.AddForce(transform.up * hitForce, ForceMode.Impulse);
                    engineAccel *= 0.8f;

                    r.PlayOneShot(r.settings.SFX_WALLHIT);
                }

                // Slow down ship very slightly
                Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
                lv.z *= 0.95f;
                Vector3 wv = transform.TransformDirection(lv);
                r.body.velocity = wv;

            }

            // calculate hoverforce
            float compression = 1 - (hit.distance / r.settings.AG_HOVER_HEIGHT);
            float hoverForce = r.settings.AG_HOVER_HEIGHT - hit.distance;

            Vector3 spring = raypos - hit.point;
            float length = spring.magnitude;
            float displacement = length - r.settings.AG_HOVER_HEIGHT;

            Vector3 springN = spring / length;
            Vector3 restoreForce = springN * (displacement * (hoverForce * (r.settings.AG_HOVER_FORCE)));
            float force = transform.InverseTransformDirection(restoreForce).y;
            force -= transform.InverseTransformDirection(r.body.velocity).y * r.settings.AG_HOVER_DAMP;

            // apply hoverforce
            r.body.AddForce(-transform.up * force, ForceMode.Acceleration);
        }

        if (isShipGrounded)
        {
            // Apply Gravity
            gravityForce = r.settings.GRAVITY_FORCE / 2;
            r.body.AddForce(-Vector3.up * gravityForce);
        }
        else
        {
            float gravity = r.settings.GRAVITY_FORCE;
            gravityForce = Mathf.Lerp(gravityForce, gravity, Time.deltaTime * r.settings.GRAVITY_WEIGHT);

            // Apply Gravity
            r.body.AddForce(-Vector3.up * gravityForce);
            hitTrack = false;
        }

        // Respawn if under track
        if (transform.position.y < ground - r.settings.AG_HOVER_HEIGHT * 10)
            r.isRespawning = true;

        if (transform.position.y > ground + (r.settings.AG_HOVER_HEIGHT * 8))
        {
            r.jumpHeight = true;
        }
        else
        {
            if (isShipGrounded)
                r.jumpHeight = false;
        }

        // Rotate Ship
        Quaternion wantedRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, trackNormal), trackNormal);

        float rotationSpeed = r.settings.AG_ROTATION_SPEED;

        transform.rotation = Quaternion.Lerp(transform.rotation, wantedRotation, Time.deltaTime * rotationSpeed);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0.0f);

        // Pitch
        pitchAmount = Mathf.Lerp(pitchAmount, r.input.AXIS_PITCH * r.settings.AG_PITCH_AMOUNT, Time.deltaTime * 3.0f);
        transform.Rotate(Vector3.right * (pitchAmount * Time.deltaTime));

    }

    private void ShipDrag()
    {
        // base grip
        float grip = r.settings.AG_GRIP;

        // airbrake slip/slide
        if (r.input.AXIS_LEFTAIRBRAKE != 0 || r.input.AXIS_RIGHTAIRBRAKE != 0)
        {
            airbrakeSlip = Mathf.Lerp(airbrakeSlip, Mathf.Abs(airbrakeAmount) * r.settings.AG_SLIP_AMOUNT, Time.deltaTime * r.settings.AG_SLIP_GAIN);
            airbrakeSlide = Mathf.Lerp(airbrakeSlide, airbrakeAmount * r.settings.AG_SLIDE_AMOUNT, Time.deltaTime * r.settings.AG_SLIDE_GAIN);

            float resistance = Mathf.Abs(r.input.AXIS_BOTHAIRBRAKES);
            resistance = Mathf.Clamp(resistance, 0.0f, 1.0f);
            airbrakeResistance = Mathf.Lerp(airbrakeResistance, Mathf.Abs(airbrakeAmount) * resistance, Time.deltaTime * (Mathf.Abs(
                    ((transform.InverseTransformDirection(r.body.velocity).x * 10) * Time.deltaTime)) * r.settings.AIRBRAKE_DRAG_MULT) * r.settings.AIRBRAKE_DRAG_GAIN);
        } else
        {
            airbrakeSlip = Mathf.Lerp(airbrakeSlip, 0.0f, Time.deltaTime * r.settings.AG_SLIP_FALLOFF);
            airbrakeSlide = Mathf.Lerp(airbrakeSlide, 0.0f, Time.deltaTime * r.settings.AG_SLIDE_FALLOFF);

            airbrakeResistance = Mathf.Lerp(airbrakeResistance, 0.0f, Time.deltaTime * r.settings.AIRBRAKE_DRAG_FALLOFF);
        }

        if (isShipGrounded)
        {
            airResistance = Mathf.Lerp(airResistance, 0.0f, Time.deltaTime * r.settings.RESISTANCE_FALLOFF);
        }
        else
        {
            if (r.jumpHeight)
                airResistance = Mathf.Lerp(airResistance, r.settings.GRAVITY_RESISTANCE * 0.1f, Time.deltaTime * r.settings.RESISTANCE_GAIN);
            else
                airResistance = Mathf.Lerp(airResistance, r.settings.GRAVITY_RESISTANCE, Time.deltaTime * r.settings.RESISTANCE_GAIN);
        }

        // brakes drag
        if (isShipBraking)
        {
            brakeFalloff = 0.0f;
            brakeGain = Mathf.Lerp(brakeGain, Time.deltaTime * 40.0f, Time.deltaTime);
            brakeDrag = Mathf.Lerp(brakeDrag, Time.deltaTime * 100.0f, Time.deltaTime * brakeGain);
        } else
        {
            brakeGain = 0.0f;
            brakeFalloff = Mathf.Lerp(brakeFalloff, 100.0f, Time.deltaTime);
            brakeDrag = Mathf.Lerp(brakeDrag, 0.0f, Time.deltaTime * brakeFalloff);
        }

        // pitch resistance
        float dot = Vector3.Dot(transform.forward, BnG.Helpers.TrackDataHelper.SectionGetRotation(r.currentSection) * Vector3.up);
        float resistmult = (dot > 0) ? 1.8f : 1;
        dot = Mathf.Abs(dot);
        dot = Mathf.Clamp(dot, 0.0f, 0.4f);

        if (dot > 0.2f * resistmult)
            pitchResistance = Mathf.Lerp(pitchResistance, dot * 0.08f, Time.deltaTime * 0.5f);
        else
            pitchResistance = Mathf.Lerp(pitchResistance, 0.0f, Time.deltaTime);

        // setup for applying drag
        float localZ = transform.InverseTransformDirection(r.body.velocity).z * 10.0f;
        float xDrag = (grip * Time.deltaTime) - (airbrakeSlip);
        xDrag = Mathf.Clamp(xDrag, 0.0f, 1.0f);

        // apply airbrake slide
        r.body.AddForce(transform.right * airbrakeSlide, ForceMode.Acceleration);

        // apply drag (convert world-space to local space, apply changes and convert back to world space)
        Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
        // horizontal grip
        lv.x *= 1 - xDrag;
        // vertical drag
        lv.y *= 1 - 0.001f;
        // air density
        lv.z *= 1 - (0.0015f + (Mathf.Abs(localZ) * Time.deltaTime) * 0.007f);
        // other resistances
        lv.z *= 1 - (airbrakeResistance + brakeDrag + airResistance + pitchResistance);
        Vector3 wv = transform.TransformDirection(lv);

        // finally apply drag
        r.body.velocity = wv;

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TrackWall"))
        {
            // Get Collision impact
            float impact = Vector3.Dot(other.contacts[0].normal, other.relativeVelocity);
            float hitDot = Vector3.Dot(other.contacts[0].normal, transform.forward);
            Vector3 impactDir = transform.InverseTransformPoint(other.contacts[0].point);

            if (Mathf.Abs(impact) > 1 && hitDot < 0.1f)
            {
                // Zero out relative Z velocity
                if (!r.shieldActivate)
                    r.PlayOneShot(r.settings.SFX_WALLHIT);

                Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
                lv.y = 0;
                lv.z = 0;
                Vector3 wv = transform.TransformDirection(lv);
                r.body.velocity = Vector3.zero;

                // Reduce engine power and thrust
                engineAccel *= 0.1f;
                engineThrust *= 0.2f;

                // Push ship away from wall
                Vector3 dir = other.contacts[0].normal;
                dir.y = 0;

                Vector3 pushForce = dir * Mathf.Abs(impact);
                pushForce = Vector3.ClampMagnitude(pushForce, 3.0f);
                r.body.AddForce(pushForce, ForceMode.Impulse);

                wantedCollisionBounce = (-impactDir.x * Mathf.Abs(impact));


                // Spawn hit particle
                if (!r.shieldActivate)
                {
                    GameObject particle = Instantiate(Resources.Load("Particles/CollisionHit") as GameObject) as GameObject;
                    particle.transform.position = other.contacts[0].point;
                    particle.transform.forward = -transform.forward;
                }

                // Ship take damage
                if (!r.shieldActivate)
                    r.TakeDamage(Mathf.Abs(impact * 1.5f));

                // change shield color
                r.ShieldDamage();

                r.perfectLap = false;

            }
        }

        if (other.gameObject.tag == "Ship")
        {

            //r.PlayOneShot(SHIP2SHIP SOUND);

            // Stop Bounce
            Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
            lv.y = 0;
            Vector3 wv = transform.TransformDirection(lv);
            if (!isShipGrounded)
                r.body.velocity = wv;

            // Slow ship down slightly
            engineThrust *= 0.8f;

            // Push away from other ship
            Vector3 dir = other.contacts[0].normal;
            dir.y = 0;
            r.body.AddForce(dir * 4, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("TrackWall"))
        {
            // get Impact Direction
            Vector3 impactDir = transform.InverseTransformDirection(other.contacts[0].point);

            // zero out any Y velocity
            if (transform.InverseTransformDirection(r.body.velocity).y > 0)
            {
                Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
                lv.y = 0;
                Vector3 wv = transform.TransformDirection(lv);
                r.body.velocity = wv;
            }


            // scrape Check
            float impact = transform.InverseTransformDirection(other.relativeVelocity).x;
            float hitDot = Vector3.Dot(other.contacts[0].normal, transform.forward);
            if ((Mathf.Abs(impact) < 0.5f && Mathf.Abs(impact) > 0.05f) || hitDot > 0.1f)
            {
                isShipScraping = true;
            }
            else
            {
                isShipScraping = false;
            }
        }
    }

    void OnCollisionExit(Collision other)
    {
        // ship is no longer scraping
        isShipScraping = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "TurbZone")
        {
            TrTurbZone tz = other.GetComponent<TrTurbZone>();
            r.body.AddForce(tz.windDirection * tz.windSpeed);
        }
    }
}
