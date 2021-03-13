using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;
using System.IO;

public class LevelDataSaver : MonoBehaviour
{

    public string loadedOBJPath;
    public string loadedmtlPath;
    public GameObject loadedObj;




    [System.Serializable]
    public class SaveObject
    {
        
        public int version = 1;
        public string saveName = "Default_Name";
        public string saveUntertitel = "Default Untertitel";
        //Arrays and Lists that get Saved:

        public List<CopyLightSave> copyLights;
        //public List<> objs;
        public Vector3 objPosition;
        public Quaternion objRotation;
        public Vector3 objScale;

        public LampSave[] lampSaves;
        //image? public Image 
    }

    // Update is called once per frame
    public void SaveLevelTo(string location)
    {

        List<LampSave> lampSavesList = new List<LampSave>(); ;
        SaveObject sO = new SaveObject();

        

        //Dynamic stuff:
        GameObject[] dmxDevicesSearch;
        GameObject[] copyLightSearch;
        


        dmxDevicesSearch = GameObject.FindGameObjectsWithTag("DMX");

        foreach (GameObject g in dmxDevicesSearch)
        {
            LampSave s = new LampSave();
            //LampDetails gDetail = g.GetComponent<LampDetails>();
            DMXDevice gD = g.GetComponent<DMXDevice>();

            //s.lType = gDetail.lType;

            if (s.lType == LampSave.LampType.VLB)
            {
                VolumetricLightBeam gV = g.GetComponent<VolumetricLightBeam>();

                //TODO get all the VLB Lamp infomations lel
            }

            s.position = g.transform.position;
            s.rotation = g.transform.rotation;
            s.startChannel = gD.startChannel;

            
            
            lampSavesList.Add(s);

        }
        //convert list to aray
        sO.lampSaves = lampSavesList.ToArray();

        //CopyLights
        copyLightSearch = GameObject.FindGameObjectsWithTag("CopyLight");
        
        foreach( GameObject g in copyLightSearch)
        {
            CopyLightSave s = new CopyLightSave();

            AmbientCopyLight gA = g.GetComponent<AmbientCopyLight>();
            Light gL = g.GetComponent<Light>();

            s.position = g.transform.position;
            s.intensity = gL.intensity;
            s.range = gA.range;

            if (sO.copyLights ==null ) { sO.copyLights = new List<CopyLightSave>(); }
            sO.copyLights.Add(s);
        }



        //save Obj

        //postition,rotation,scale
        sO.objPosition = loadedObj.transform.position;
        sO.objRotation = loadedObj.transform.rotation;
        sO.objScale = loadedObj.transform.localScale;

        //move to path and rename
        File.Copy(loadedOBJPath, location + "/toBeLoaded.obj", true);
        if (loadedmtlPath != "0")
        {
            File.Copy(loadedmtlPath, location + "/toBeLoaded.mtl", true);
        }


        //save as Json to FilePath
        if (location == null) { location = Application.persistentDataPath + "/" + sO.saveName + ".json"; } else { location = location + "/" + sO.saveName + ".json"; }

        string json = JsonUtility.ToJson(sO);
        File.WriteAllText(location, json);

    }
}
