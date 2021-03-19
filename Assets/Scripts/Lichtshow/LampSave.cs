using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LampSave
{
    //Everything thats needed to save the Lamp

    //stuff thats Defined by the Lamp
    public enum LampType
    {
        VLB,
        CYL,
        WASH,
        FLAME
    }

    public LampType lType;

    int numChannels;
    List<DMXDevice.ChannelFunction> channelFunctions;

    int spotAngle;
    List<Vector2[]> minMaxRotation;

    //
    //Stuff thats defined by the Editor!
    public Vector3 position;
    public Quaternion rotation;

    //DMX Server Info
    public int universe;
    public int startChannel;
    //numchannels von oben


    



}
[System.Serializable]
public class CopyLightSave
{
    public Vector3 position;
    public float intensity;
    public float range;
}

