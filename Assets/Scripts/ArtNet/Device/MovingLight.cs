using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

[ExecuteAlways]
public class MovingLight : DMXDevice
{

    public override int NumChannels{ get { return 13; } }
    public Transform panRotater;
    public Transform tiltRotater;

    [Header("rotateProps")]
    public float panMovement = 360f;
    public float tiltMovement = 270f;
    public float panTarget;
    public float tiltTarget;
    public float minRotSpeed = 1f;
    public float maxRotSpeed = 200f;

    

    public float pan;
    public float tilt;
    public float rotSpeed;

    public GameObject thisObject;
    public VolumetricLightBeam myBeam;
    public Color beamColor;
    //Channels kopiert von MH-360!
    // 1. Pan
    // 2. PanFine
    // 3. Tilt
    // 4. Tilt Fine
    // 5. Pan/Tilt Speed
    // 6. Special
    // 7. Dimmer
    // 8. Strobe
    // 9. Red
    // 10. Green
    // 11. Blue
    // 12. Colour / Chase / Fade
    // 13. Chase / Fade Speed

    // Channel immer minus 1 da hier alles bei 0 Startet!!!!!!!!!!!!!!!!

    private void Start()
    {
        myBeam = thisObject.GetComponent<VolumetricLightBeam>();
        rotSpeed = 100f;
        myBeam.noiseMode = VLB.NoiseMode.WorldSpace;
        myBeam.noiseIntensity = 0.75f;
        myBeam.noiseScaleUseGlobal = false;
        myBeam.noiseScaleLocal = 0.6f*Random.value;
        myBeam.fallOffStart = 11f;
        myBeam.fallOffEnd = myBeam.fallOffStart * 1.2f;
        myBeam.spotAngle = 32f;

        

    }

    public override void SetData(byte[] dmxData)
    {
        base.SetData(dmxData);
        SetPan(dmxData[0], 0);
        SetTilt(dmxData[2], 0);

        SetColor(dmxData[8], dmxData[9], dmxData[10], dmxData[6]);

       

    }
    void SetPan(byte panData, byte panFineData = 0)
    {
        panTarget = (panData + panFineData / 256f) * panMovement / 256f;
    }
    void SetTilt(byte tiltData, byte tiltFineData = 0)
    {
        tiltTarget = (tiltData + tiltFineData / 256f) * tiltMovement / 256f;
    }

    private void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        float dpan = (panTarget - pan);
        float dtilt = (tiltTarget - tilt);
        if(dpan != 0)
        {
            dpan = Mathf.Min(Mathf.Abs(dpan), Time.deltaTime * rotSpeed) * Mathf.Sign(dpan);
            pan += dpan;
            panRotater.Rotate(0, dpan, 0, Space.Self);
        }
        if(dtilt != 0)
        {
            dtilt = Mathf.Min(Mathf.Abs(dtilt), Time.deltaTime * rotSpeed) * Mathf.Sign(dtilt);
            tilt += dtilt;

            tiltRotater.Rotate(dtilt, 0, 0, Space.Self);
        }
    }




    void UpdateStrobo()
    {

    }
    void SetColor(byte r,byte g, byte b,byte a)
    {
        /*if(r==0 && g==0 && b == 0)
        {
            myBeam.color = new Vector4(1, 1, 1, 3 * a / 265f);
        }else
        {*/
            myBeam.color = new Vector4(r / 265f, g / 265f, b / 265f, 3*a/265f);
        //}

        beamColor = myBeam.color;
        myBeam.UpdateAfterManualPropertyChange();
    }
    float GetColor(byte val, byte fineVal = 0)
    {
        return val / 256f + fineVal / (256f * 256f);
    }
}
