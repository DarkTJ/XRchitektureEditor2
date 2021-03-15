using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using TMPro;

public class ServerSideBarLoader : MonoBehaviour
{
    public Button connectToServerButton;
    public TMP_InputField ipInput;
    public TCPSendingClient sendingClient;
    public Button saveToFileButton;
    public Button loadFromFileButton;
    public GameObject uIEventSystem;
    public LevelDataLoader loader;
    public LevelDataSaver saver;

    
    // Start is called before the first frame update
    void Start()
    {
        saveToFileButton.onClick.AddListener(SaveToFilePress);
        loadFromFileButton.onClick.AddListener(LoadFromFilePress);
        connectToServerButton.onClick.AddListener(connectToServerFromButtonpress);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SaveToFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowSaveDialog((paths) => { uIEventSystem.SetActive(true); saver.SaveLevelTo(paths[0]); }, () => { Debug.Log("Error"); }, FileBrowser.PickMode.Folders, false, null, null, "Select .obj", "Save");
        //fuck shit up 


    }

    void LoadFromFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowLoadDialog((paths) => { uIEventSystem.SetActive(true); loader.LoadLevelfromFile(paths[0]); }, () => { Debug.Log("Error"); }, FileBrowser.PickMode.Files, false, null, null, "Select .obj", "Load");
    }

    private void connectToServerFromButtonpress()
    {
        sendingClient.connectToArtNetServer(ipInput.text);
    }
}
