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
    public string musicMaxLength;


    public TMP_InputField ArtNetIP;
    public Button startArtNetService;
    public Button NextIPAdresse;
    public DmxControllerServerVersion dmxController;
    public System.Net.IPAddress[] adressen;
    private int Adresse = 0;

    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        saveToFileButton.onClick.AddListener(SaveToFilePress);
        loadFromFileButton.onClick.AddListener(LoadFromFilePress);
        connectToServerButton.onClick.AddListener(connectToServerFromButtonpress);
        disconnectFromTCPServer.onClick.AddListener(diconnectFromServer);
        playButton.onClick.AddListener(playMusicButton);
        stopButton.onClick.AddListener(StopMusicButton);
        startArtNetService.onClick.AddListener(StartArtNetPressed);
        NextIPAdresse.onClick.AddListener(nextIpAdressen);

        ArtNetIP.text = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0].ToString();
        adressen = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
        musicMaxLength = Mathf.Floor((MusicPlayer.clip.length + 4.173f) / 60) + ":" + Mathf.Floor((MusicPlayer.clip.length + 4.173f)) % 60 + ":" + Mathf.Floor(((MusicPlayer.clip.length + 4.173f) % 1) * 1000);
        

    }

    // Update is called once per frame
    void Update()
    {
        //MUSIC Updates 4 Server:
        float mU;
        mU = (MusicPlayer.time - 4.173f);
        if (mU < 0) { mU = 0; }
        sendingClient.getMusicStatus(mU);        //music UI Update:
        musicText.text = Mathf.Floor(MusicPlayer.time / 60) + ":" + Mathf.Floor(MusicPlayer.time) % 60 + ":" + Mathf.Floor((MusicPlayer.time % 1)*1000) + " / " + musicMaxLength;


        if (MusicPlayer.isPlaying == false)
        {
            if(Input.GetKey("left ctrl") == true && Input.GetKey("h") == true)
            {
                playMusicButton();
            }
        }
    }
    void nextIpAdressen()
    {
        Adresse += 1;
        if (Adresse == adressen.Length)
        {
            Adresse = 0;
        }
        ArtNetIP.text = adressen[Adresse].ToString();
    }
    void StartArtNetPressed()
    {
        dmxController.StartArtNet(ArtNetIP.text);
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

    private void diconnectFromServer() 
    {
        sendingClient.disconnectFromServer();
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
