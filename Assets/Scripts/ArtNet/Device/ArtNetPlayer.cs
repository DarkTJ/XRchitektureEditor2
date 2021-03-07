using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ArtNet.Packets;
//using FMODUnity;



public class ArtNetPlayer : MonoBehaviour
{


    public float paketeProSekunde = 20f; //10 pakete pro sekunde //TODO ddas in der json speichern und mit auslesen :D
    //3 Text-Dateien, die den artnet code in djson verpackt haben.
    public TextAsset save1;
    public TextAsset save2;
    public TextAsset save3;

    //die sound player
    //public AudioSource[] track;
    //
    //public StudioEventEmitter[] track;

    public DmxController dmxcontroller;

    public bool playingMusic = false;
    public bool playing = false;
    private int playingTrackID;


    //playback variables
    private int length;
    private int progres;

    [System.Serializable]
    public class DMXDataPacketRecorder
    {
        public List<ArtNetDmxPacket> m_data = new List<ArtNetDmxPacket>();
    }

    //Speicherort der ausgepackten json dateien
    DMXDataPacketRecorder rec1 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder rec2 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder rec3 = new DMXDataPacketRecorder();
    DMXDataPacketRecorder playback = new DMXDataPacketRecorder();


    void Start()
    {
        //dateien öffnen und in die Packetrecorder speichern
        rec1 = JsonUtility.FromJson<DMXDataPacketRecorder>(save1.text);
        rec2 = JsonUtility.FromJson<DMXDataPacketRecorder>(save2.text);
        rec3 = JsonUtility.FromJson<DMXDataPacketRecorder>(save3.text);

        

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPlayback(int id)
    {

    if (id == 1)
        {
            length = rec1.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec1;
            progres = 0;
            playingTrackID = 1;
        }
        if (id == 2)
        {
            length = rec2.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec2;
            progres = 220;
            playingTrackID = 2;
        }
        if (id == 3)
        {
            length = rec3.m_data.Count;  //anzahl pakete, die gespeichert wurden.
            playback = rec3;
            progres = 0;
            playingTrackID = 3;
        }

        
        InvokeRepeating("PlayArtNet", 2.0f, 1.0f / paketeProSekunde);   
        //erst dann die musik starten, wegen synch näh
        
    }


    public void StopPlayback()
    {
        /*track[0].Stop();
        track[1].Stop();
        track[2].Stop();*/
        playingMusic = false;
        if (IsInvoking("PlayArtNet")) {
            CancelInvoke("PlayArtNet");
        }
        StopCoroutine("PlayArtNetCorotine");

    }




    void PlayArtNet()
    {

        if (progres < length)
        {
            dmxcontroller.RecivefromLocalRecorder(playback.m_data[progres]);
            progres += 1;
            //Debug.Log("data send: " + progres);
            if (playingMusic == false)
            {
                //track[playingTrackID - 1].Play();
                playingMusic = true;
            }
        }
        else if (progres >= length)

        {
            StopPlayback();
            Debug.Log("panik now pls");

            CancelInvoke("PlayArtNet");
            StartPlayback(2);
        }




    }

    IEnumerator PlayArtNetCorotine()
    {
        if (playingMusic == false)
        {
            //track[playingTrackID - 1].Play();
            playingMusic = true;
        }
        while (progres < length)
        {
            Debug.Log(Time.time);
            dmxcontroller.RecivefromLocalRecorder(playback.m_data[progres]);
            progres += 1;

            //Debug.Log("data send: " + progres);
            Debug.Log(Time.time);
            yield return new WaitForSeconds(1.0f / paketeProSekunde);

        }
        if (progres >= length)

        {
            StopPlayback();
            Debug.Log("panik now pls");
            StopCoroutine("PlayArtNetCorotine");
            StartPlayback(2);
        }


    }
}
