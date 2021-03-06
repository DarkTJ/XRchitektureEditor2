using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using VLB;

public class LevelDataLoader : MonoBehaviour
{

    GameObject loadedObject;
    public Transform spawnPoint;
    public Transform lampSpawnpoint;
    public GameObject lampPreset;
    public GameObject flamePreset;
    public GameObject clyPreset;
    public GameObject copyLightPreset;

    public DmxControllerServerVersion dmxController;


    public void LoadLevelfromFile(string location)
    {
        LevelDataSaver.SaveObject sO;
        sO = new LevelDataSaver.SaveObject();

        //unpack the File:
        var filePackageReader = new FileManager.FilePackageReader(location);
        var filenameFileContentDictionary = filePackageReader.GetFilenameFileContentDictionary();
        
        foreach (var keyValuePair in filenameFileContentDictionary)
        {
            if (keyValuePair.Key == "save.json")
            {
                sO = JsonUtility.FromJson<LevelDataSaver.SaveObject>(keyValuePair.Value);
            }
            else if (keyValuePair.Key == "toBeLoaded.obj")
            {
                //OBJ LOADER
                if (loadedObject != null)
                {
                    Destroy(loadedObject);
                }
                using (var stream = GenerateStreamFromString(keyValuePair.Value))
                {
                    loadedObject = new OBJLoader().Load(stream);
                }

                    
            }
        }

        //Backwards lol

        //     GameSaving gameSaving = JsonUtility.FromJson<GameSaving>( File.ReadAllText( Application.persistentDataPath + "/saveload.json" ) );

        


        
        //Lamps first:
        foreach (LampSave s in sO.lampSaves)
        {
            GameObject g;
            if (s.lType == LampSave.LampType.VLB)
            {
                g = Instantiate(lampPreset);
                VolumetricLightBeam gV = g.GetComponent<VolumetricLightBeam>();
                
                //TODO set all VLB informations lel
            }else if (s.lType == LampSave.LampType.FLAME)
            {
                g = Instantiate(flamePreset);
            }
            else if (s.lType == LampSave.LampType.CYL)
            {
                g = Instantiate(clyPreset);
            }
            else
            {

                //this is an error
                Debug.LogError("cant find Lamp Type!");
                g = Instantiate(lampPreset);
            }


            g.transform.SetParent(lampSpawnpoint);
            g.transform.position = s.position;
            g.transform.rotation = s.rotation;
            DMXDevice gD = g.GetComponent<DMXDevice>();

            gD.startChannel = s.startChannel;
            gD.startChannelSET = true;



            

        }
        
        //Copylighst second

        foreach (CopyLightSave s in sO.copyLights)
        {
            GameObject g = Instantiate<GameObject>(copyLightPreset);
            g.transform.SetParent(lampSpawnpoint);
            g.transform.position = s.position;

            AmbientCopyLight gA = g.GetComponent<AmbientCopyLight>();
            Light gL = g.GetComponent<Light>();

            gL.intensity = s.intensity;
            gA.range = s.range;

        }

        /*
        string mtlocation = location + "/toBeLoaded.mtl";

        //OBJ LOADER
        if (loadedObject != null)
        {
            Destroy(loadedObject);
        }
          
        if (File.Exists(mtlocation)) {
            loadedObject = new OBJLoader().Load(location + "/toBeLoaded.obj", mtlocation);
        } else { 
            loadedObject = new OBJLoader().Load(location + "/toBeLoaded.obj"); 
        }*/
        loadedObject.transform.SetParent(spawnPoint);
        loadedObject.transform.position = sO.objPosition;
        loadedObject.transform.rotation = sO.objRotation;
        loadedObject.transform.localScale = sO.objScale;


        dmxController.ValidateAllDMXDAta();
    }

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}
