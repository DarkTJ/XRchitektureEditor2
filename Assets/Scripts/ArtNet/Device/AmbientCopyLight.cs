using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class AmbientCopyLight : MonoBehaviour
{

    public float range = 5f;

    private Light lamp;
    private float defaultIntensity;
    public Color color;


    public GameObject[] objectToSample;
    
    // Start is called before the first frame update
    void Start()
    {
        lamp = this.GetComponent<Light>();
        defaultIntensity = lamp.intensity;
        objectToSample = FindGameObjectsInsideRange(this.transform.position, range);
    }

    // Update is called once per frame
    void Update()
    {
        lamp.intensity = defaultIntensity;
        color = Color.black;
        for (int i = 0;i < objectToSample.Length; i++)
        {

            if (objectToSample[i].TryGetComponent<VolumetricLightBeam>(out VolumetricLightBeam vlb))
            {
                color = color + 2*vlb.color.a*vlb.color;

            } else if (objectToSample[i].GetComponentInParent< CylLight>() != null)
            {
                //color = Color.white - (Color.white - color) * (Color.white - objectToSample[i].GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor"));
                //color = color + ((Vector3.Distance(this.transform.position,objectToSample[i].GetComponentInChildren<Renderer>().transform.position)/5f + 0.2f) * objectToSample[i].GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor"));
                color = color + (objectToSample[i].GetComponent<Renderer>().material.GetColor("_EmissionColor"));
            } else if (objectToSample[i].GetComponentInParent<FlameThrower>() != null) 
            {
                FlameThrower f = objectToSample[i].GetComponentInParent<FlameThrower>();
                if (f.flameVideo.isPlaying == true)
                {
                    
                    if (f.endingFlames == false)
                    {
                        lamp.intensity = 2 * defaultIntensity;
                        color += 10 * new Color(1, 0.33f, 0, 1);
                    }
                    else
                    {
                        int frame =  Mathf.RoundToInt(30 - f.flameVideo.frame);
                        if (Mathf.Sign(frame) == -1) { frame = 0; }
                        color += frame*0.31f * new Color(1, 0.33f, 0, 1);
                    }
                    
                   
                }
            }
                
               
            //Debug.Log( Vector3.Distance(this.transform.position, objectToSample[i].GetComponentInChildren<Renderer>().transform.position) / 5f + 0.2f);
        }

        color = color / objectToSample.Length;
        lamp.color = color;

    }





    //mybe this works ??

    GameObject[] FindGameObjectsInsideRange(Vector3 center,float radius) {

    Collider[] cols  = Physics.OverlapSphere(center, radius);
    var q = cols.Length; // q = how many colliders were found
    //coppy gameobject into array
    GameObject[] gos = new GameObject[q];
        // copy the game objects inside range to it:
    for (int i = 0; i < q; i++)
    {
         gos[i] = cols[i].gameObject;
    }


    return gos; // return the GameObject array
 }
}
