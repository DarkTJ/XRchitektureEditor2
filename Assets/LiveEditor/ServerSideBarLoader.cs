using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using TMPro;

public class ServerSideBarLoader : MonoBehaviour
{
    public Button connectToServerButton;
    public Button disconnectFromTCPServer;
    public TMP_InputField ipInput;
    public TCPTestClient sendingClient;
    public Button saveToFileButton;
    public Button loadFromFileButton;
    public GameObject uIEventSystem;
    public LevelDataLoader loader;
    public LevelDataSaver saver;

    public Button playButton;
    public Button stopButton;
    public TextMeshProUGUI musicText;
    public AudioSource MusicPlayer;
    public string musicMaxLength;

    public Button sendControlButton;
    public TMP_InputField RateInputFiled;

    // Start is called before the first frame update
    void Start()
    {
        saveToFileButton.onClick.AddListener(SaveToFilePress);
        loadFromFileButton.onClick.AddListener(LoadFromFilePress);
        connectToServerButton.onClick.AddListener(recconect);
        disconnectFromTCPServer.onClick.AddListener(diconnectFromServer);
        playButton.onClick.AddListener(playMusicButton);
        stopButton.onClick.AddListener(StopMusicButton);
        sendControlButton.onClick.AddListener(sendTeleFromButton);

        musicMaxLength = Mathf.Floor(MusicPlayer.clip.length / 60) + ":" + Mathf.Floor(MusicPlayer.clip.length) % 60 + ":" + Mathf.Floor((MusicPlayer.clip.length % 1) * 1000);
        
    }

    // Update is called once per frame
    void Update()
    {
        //MUSIC Updates 4 Server:
        //sendingClient.getMusicStatus(MusicPlayer.time);        //music UI Update:
        musicText.text = Mathf.Floor(MusicPlayer.time / 60) + ":" + Mathf.Floor(MusicPlayer.time) % 60 + ":" + Mathf.Floor((MusicPlayer.time % 1)*1000) + " / " + musicMaxLength;
    }

    void sendTeleFromButton()
    {
        Debug.LogWarning(System.Convert.ToUInt32(RateInputFiled.text));
        sendingClient.SendTele(System.Convert.ToUInt32(RateInputFiled.text), true);
    }

    void SaveToFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowSaveDialog((paths) => { uIEventSystem.SetActive(true); saver.SaveLevelTo(paths[0]); }, () => { uIEventSystem.SetActive(true); Debug.Log("Error"); }, FileBrowser.PickMode.Folders, false, null, null, "Select .obj", "Save");
        //fuck shit up 


    }

    void recconect()
    {
        sendingClient.recconecttoServer();
    }

    void LoadFromFilePress()
    {
        //open file explorer in folder select#+
        uIEventSystem.SetActive(false);
        FileBrowser.ShowLoadDialog((paths) => { uIEventSystem.SetActive(true); loader.LoadLevelfromFile(paths[0]); }, () => { uIEventSystem.SetActive(true); Debug.Log("Error"); }, FileBrowser.PickMode.Files, false, null, null, "Select .obj", "Load");
    }

    private void connectToServerFromButtonpress()
    {
        //sendingClient.connectToArtNetServer(ipInput.text);
    }

    private void diconnectFromServer() 
    {
        //sendingClient.disconnectFromServer();
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
