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

    public Button playButton;
    public Button stopButton;
    public TextMeshProUGUI musicText;
    public AudioSource MusicPlayer;
    public float musicMaxLength;

    // Start is called before the first frame update
    void Start()
    {
        saveToFileButton.onClick.AddListener(SaveToFilePress);
        loadFromFileButton.onClick.AddListener(LoadFromFilePress);
        connectToServerButton.onClick.AddListener(connectToServerFromButtonpress);
        playButton.onClick.AddListener(playMusicButton);
        stopButton.onClick.AddListener(StopMusicButton);

        musicMaxLength = MusicPlayer.clip.length;
        
    }

    // Update is called once per frame
    void Update()
    {
        //MUSIC Updates 4 Server:
        
        //music UI Update:
        musicText.text = MusicPlayer.time.ToString() + " / " + musicMaxLength;
    }

    void SaveToFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowSaveDialog((paths) => { uIEventSystem.SetActive(true); saver.SaveLevelTo(paths[0]); }, () => { uIEventSystem.SetActive(true); Debug.Log("Error"); }, FileBrowser.PickMode.Folders, false, null, null, "Select .obj", "Save");
        //fuck shit up 


    }

    void LoadFromFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowLoadDialog((paths) => { uIEventSystem.SetActive(true); loader.LoadLevelfromFile(paths[0]); }, () => { uIEventSystem.SetActive(true); Debug.Log("Error"); }, FileBrowser.PickMode.Files, false, null, null, "Select .obj", "Load");
    }

    private void connectToServerFromButtonpress()
    {
        sendingClient.connectToArtNetServer(ipInput.text);
    }

    void playMusicButton()
    {
        MusicPlayer.Play();
    }

    void StopMusicButton()
    {
        MusicPlayer.Stop();
    }
    
}
