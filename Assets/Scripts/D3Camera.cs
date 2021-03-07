using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D3Camera : MonoBehaviour
{

    public Transform CameraBase;
    public Transform Camera;

    public int MaxTilt = 85;
    public int MinTilt = 10;
    Vector3 prev_Mousepostition = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(2))
        {
            
            prev_Mousepostition = Input.mousePosition;
            
        }
       
        //middle Mouse rotate/+shift drag
        if (Input.GetMouseButton(2))
        {

            if (Input.GetKey(KeyCode.LeftShift))
            { //drag
                Quaternion rotationCorrection = Quaternion.Euler(CameraBase.rotation.eulerAngles);

                CameraBase.position -= Quaternion.AngleAxis(rotationCorrection.eulerAngles[1],Vector3.up)*(0.1f*new Vector3(Input.mousePosition[0] - prev_Mousepostition[0],0, Input.mousePosition[1] - prev_Mousepostition[1])) ;

            }else
            {//rotate

                Quaternion currentRotaiaon;
                Quaternion goalRotation;
                currentRotaiaon = CameraBase.rotation;
                goalRotation = currentRotaiaon;
                Vector3 toRotate = new Vector3(-(Input.mousePosition[1] - prev_Mousepostition[1]), Input.mousePosition[0] - prev_Mousepostition[0], 0);


                goalRotation.eulerAngles += toRotate;
                if (goalRotation.eulerAngles[0] > MaxTilt)
                {
                    toRotate[0] = 0;

                } else if (goalRotation.eulerAngles[0] < MinTilt)
                {
                    toRotate[0] = 0;
                }
                currentRotaiaon.eulerAngles += toRotate;
                CameraBase.rotation = currentRotaiaon;
            }

            prev_Mousepostition = Input.mousePosition;

            
        }


        //Zoom
        if (Input.mouseScrollDelta != new Vector2 (0,0))
        {
            Vector3 oldPosition = Camera.localPosition;
            oldPosition[2] += Input.mouseScrollDelta[1];
            Camera.localPosition = oldPosition;

        }
    }


}
