using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Dummiesman;
using SimpleFileBrowser;

public class ImportOBJButton : MonoBehaviour
{
    string objPath = string.Empty;
    string error = string.Empty;
    GameObject loadedObject;
    public bool showImporter = false;
    public GameObject uIEventSystem;
    public Transform SpawnPoint;
    public LevelDataSaver LevelDataSaver;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OpenFileBrowser);
        


        //FileBrowser Setup
        FileBrowser.SetDefaultFilter(".obj");
    }


    public void OpenFileBrowser()
    {
        uIEventSystem.SetActive(false);
        FileBrowser.ShowLoadDialog((paths) => { objPath = paths[0]; uIEventSystem.SetActive(true); ImportObj(); },()  => { Debug.Log("Error"); }, FileBrowser.PickMode.Files,false,null,null,"Select .obj","Load");
    }
    
    void ImportObj()
    {
        if (!File.Exists(objPath))
        {
            error = "File doesn't exist.";
            showImporter = false;
        }
        else
        {
            if (loadedObject != null)
                Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(objPath);
            loadedObject.transform.SetParent(SpawnPoint);
            error = string.Empty;
            showImporter = false;

            //send whats loaded to the LevelDataSaver
            LevelDataSaver.loadedOBJPath = objPath;
            LevelDataSaver.loadedObj = loadedObject;

            //TODO FIX !!! NOT MTL IS PACKED AT THE MOMENT!
            LevelDataSaver.loadedmtl = false;
        }
    }

    void OnGUI()
    {

        if (showImporter == true)
        {


            objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);

            GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
            if (GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
            {
                //file path
                
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                GUI.color = Color.red;
                GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
                GUI.color = Color.white;
                showImporter = false;
            }
        }
    }
}
