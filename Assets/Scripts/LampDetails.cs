using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LampDetails : MonoBehaviour
{
    
    //Item Name
    public Text text = null;
    public Text Hersteller;
    //Item Image
    public Image image = null;

    public string type = "basic"; // basic, movignhead, laser,clyinder

    //Stuff to read from JSON
    public int numChannels;
    public string[] belegung;
    public Vector2[] MinMax;
    
    
    
}
