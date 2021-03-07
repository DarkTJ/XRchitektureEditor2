using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylLight : DMXDevice
{


    public override int NumChannels { get { return 5; } }
    

    [Header("rotateProps")]
    public float heightMovement = 1.5f;
    public float heightTarget;
    public float liftSpeed = 1.0f;

    public float height;


    public GameObject cylinderLight;
    //Channels
    // 1. Height
    // 2. Dimmer
    // 3. Red
    // 4. Green
    // 5. Blue


    // Channel immer minus 1 da hier alles bei 0 Startet!!!!!!!!!!!!!!!!

    void Start()
    {
        
    }

    public override void SetData(byte[] dmxData)
    {
        base.SetData(dmxData);
        SetHeight(dmxData[0]);
        //SetTilt(dmxData[2], 0);

        SetColor(dmxData[2], dmxData[3], dmxData[4], dmxData[1]);

    }

    void SetHeight(byte heightdata)
    {
        heightTarget = (heightdata) * heightMovement / 256f;
    }

    void SetColor(byte r, byte g, byte b, byte a)
    {
        /*if (r == 0 && g == 0 && b == 0)
        {

            cylinderLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Vector4(1, 1, 1, (a/265f)* 15 ));
          //myBeam.color = new Vector4(1, 1, 1, 3 * a / 265f);
        }
        else
        {*/
            cylinderLight.GetComponent<Renderer>().material.SetColor("_EmissionColor", new Vector4(r / 265f, g / 265f, b / 265f, (a / 265f) * 15));
           
        //}

        
    }

    // Update is called once per frame
    void Update()
    {
        UpdataHeight();
    }

    void UpdataHeight()
    {
        float dheight = (heightTarget - height);

        if (dheight != 0)
        {
            dheight = Mathf.Min(Mathf.Abs(dheight), Time.deltaTime * liftSpeed) * Mathf.Sign(dheight);
            height += dheight;
            cylinderLight.transform.Translate(new Vector3(0, -dheight,0),Space.World);
        }
       
    }

}
