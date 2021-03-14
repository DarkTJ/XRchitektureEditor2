using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;
using System.IO;
using VLB;

public class LevelDataLoader : MonoBehaviour
{

    GameObject loadedObject;
    Transform spawnPoint;
    public GameObject lampPreset;
    public GameObject copyLightPreset;
    public void LoadLevelfromFile(string location)
    {
        //Backwards lol

        //     GameSaving gameSaving = JsonUtility.FromJson<GameSaving>( File.ReadAllText( Application.persistentDataPath + "/saveload.json" ) );

        LevelDataSaver.SaveObject sO;


        sO = JsonUtility.FromJson<LevelDataSaver.SaveObject>(File.ReadAllText(location));
        //Lamps first:
        foreach (LampSave s in sO.lampSaves)
        {
            GameObject g = Instantiate<GameObject>(lampPreset);
            g.transform.position = s.position;
            g.transform.rotation = s.rotation;
            DMXDevice gD = g.GetComponent<DMXDevice>();

            gD.startChannel = s.startChannel;
            gD.startChannelSET = true;



            if (s.lType == LampSave.LampType.VLB)
            {
                VolumetricLightBeam gV = g.GetComponent<VolumetricLightBeam>();

                //TODO set all VLB informations lel
            }

        }
        
        //Copylighst second

        foreach (CopyLightSave s in sO.copyLights)
        {
            GameObject g = Instantiate<GameObject>(copyLightPreset);

            g.transform.position = s.position;

            AmbientCopyLight gA = g.GetComponent<AmbientCopyLight>();
            Light gL = g.GetComponent<Light>();

            gL.intensity = s.intensity;
            gA.range = s.range;

        }


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
        }
        loadedObject.transform.SetParent(spawnPoint);

    }
}
