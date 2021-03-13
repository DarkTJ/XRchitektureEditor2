using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dummiesman;

public class LevelDataLoader : MonoBehaviour
{

    GameObject loadedObject;
    Transform spawnPoint;
    public void LoadLevelfromFile(string location,string mtlocation)
    {
        //Backwards lol

        //     GameSaving gameSaving = JsonUtility.FromJson<GameSaving>( File.ReadAllText( Application.persistentDataPath + "/saveload.json" ) );


        if (loadedObject != null)
            Destroy(loadedObject);
        loadedObject = new OBJLoader().Load(location,mtlocation);
        loadedObject.transform.SetParent(spawnPoint);


    }
}
