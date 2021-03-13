using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LampDetails : MonoBehaviour
{
    //Details that the Editor uses!


    //Item Name
    public Text text = null;
    public Text Hersteller;
    //Item Image
    public Image image = null;

    
    public LampSave.LampType lType;

    //Stuff to read from JSON
    public int numChannels;
    public List<DMXDevice.ChannelFunction> channelFunctions;
    public Vector2[] MinMax;
    
    
    
}
