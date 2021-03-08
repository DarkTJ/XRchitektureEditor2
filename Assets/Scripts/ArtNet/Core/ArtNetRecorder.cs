using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArtNet.Packets;
//using FMODUnity;

public class ArtNetRecorder : MonoBehaviour
{
    public ArtNetPlayer player;
    public Button startButton;

    public Button save1Button;
    public Button save2Button;
    public Button save3Button;

    //public StudioEventEmitter[] track;

    public Button[] playbackbuttons;



    public int RecordRate = 10;

    int saveNumber = 1;

    public string[] savePath = { "/save1.json", "/save2.json", "/save3.json" };

    public Text statusText;

    bool recording = false;

    [SerializeField] ArtNetDmxPacket recievedDMX;

    [System.Serializable]
    public class DMXDataPacketRecorder
    {
        public List<ArtNetDmxPacket> m_data = new List<ArtNetDmxPacket>();
    }

    private ArtNetDmxPacket latestUniverse0Paket;
    private ArtNetDmxPacket latestUniverse1Paket;

    DMXDataPacketRecorder rec = new DMXDataPacketRecorder();

    // Start is called before the first frame update
    void Start()
    {
        save1Button.onClick.AddListener(() => SwitchSave(1));
        save2Button.onClick.AddListener(() => SwitchSave(2));
        save3Button.onClick.AddListener(() => SwitchSave(3));
        startButton.onClick.AddListener(RecordButton);



        startButton.image.color = Color.green;
        save1Button.image.color = Color.gray;
        Debug.Log(Application.persistentDataPath);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecordButton()
    {
        if (recording == true)
        {       // stop Recording
            recording = false;
            startButton.image.color = Color.green;

            lockUnlockButtons(false);
            //save m_data to File
            Debug.Log(rec);

            //start invoke to save at the definded rate
            if (IsInvoking("saveLatestToFile"))
            {
                CancelInvoke("saveLatestToFile");
            }
            //track[0].Stop();
            //track[1].Stop();
           // track[2].Stop();

            //save it to the right file
            string potion = JsonUtility.ToJson(rec);
            Debug.Log(potion);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePath[saveNumber - 1], potion);



            // TODO CLEAR DATA 
            // nö, mach ich jetzt nicht, sollte auch so gehn, wenn am ende vom recording alle slider auf null sind, sodass einige nullpakete ankommen!!

        }
        else  //start Recording
        {
            recording = true;
            startButton.image.color = Color.red;

            lockUnlockButtons(true);

            InvokeRepeating("saveLatestToFile", 2.0f, 1.0f / RecordRate);   //1/50 ist paketanzahl.
            //track[saveNumber - 1].Play();
        }
    }


    void SwitchSave(int saveNr)
    {
        Button[] saveButtons = { save1Button, save2Button, save3Button, playbackbuttons[0], playbackbuttons[1], playbackbuttons[2], playbackbuttons[3] };

        saveNumber = saveNr;

        for (int i = 0; i < 7; i++)
        {
            saveButtons[i].image.color = Color.white;
        }
        saveButtons[saveNr - 1].image.color = Color.gray;

    }


    public void DatatoRecord(ArtNetDmxPacket dmxPacket)
    {

        if (recording == true)
        {
            if (dmxPacket.Universe == 0)
            {
                latestUniverse0Paket = dmxPacket;
            }
            else if (dmxPacket.Universe == 1)
            {
                latestUniverse1Paket = dmxPacket;
            }

            recievedDMX = dmxPacket;

        }



    }

    //inovke this funktion to save the latest dmx to file.
    //can be invoked as often as you like, playback speed shoould match save speed *2, because 2 univeres are saved, but playback just grabs 1 packet, diregarding if its universe 0 or 1

    private void saveLatestToFile()
    {
        rec.m_data.Add(latestUniverse0Paket);
        rec.m_data.Add(latestUniverse1Paket);
    }

    public void lockUnlockButtons(bool l)
    {
        Button[] saveButtons = { save1Button, save2Button, save3Button, playbackbuttons[3], playbackbuttons[1], playbackbuttons[2] };

        if (l == true)
        {
            //lock all buttons
            //save buttons sperren
            for (int i = 0; i < 6; i++)
            {
                saveButtons[i].interactable = false;
            }

        }
        else
        {
            //unlock all buttons
            //save buttons wieder entsperren
            for (int i = 0; i < 6; i++)
            {
                saveButtons[i].interactable = true;
            }
        }
    }

}