using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FlameThrower : DMXDevice
{

    public override int NumChannels { get { return 2; } }
    public VideoPlayer flameVideo;
    public VideoClip flameStart;
    public VideoClip flameMID;
    public VideoClip flameEnd;

    public override LampSave.LampType lType { get { return LampSave.LampType.FLAME; } }

    public int playBackSpeedSLOW = 1;
    public int playBackSpeedFast = 2;

    bool firering = false;
    bool startingFlames = false;
    public bool endingFlames = false;
    
    // Start is called before the first frame update
    void Start()
    {
        flameVideo = GetComponentInChildren<VideoPlayer>();
        flameVideo.loopPointReached += loopPointFlame;
        flameVideo.clip = flameStart;
        flameVideo.playbackSpeed = playBackSpeedFast;
    }
    public override void SetData(byte[] dmxData)
    {
        base.SetData(dmxData);
        StartFlame(dmxData[1], 0);
    }

    void StartFlame(byte flamechannel, byte safetychannel)
    {
        if (flamechannel >= 255) {
            if (firering == false) {
                firering = true;
                if (endingFlames == true)
                {
                    endingFlames = false;
                    flameVideo.Stop();
                    flameVideo.clip = flameStart;
                    flameVideo.playbackSpeed = playBackSpeedFast;
                    //restart the fire
                }
                flameVideo.Play();
                startingFlames = true;
            }
            
        } else {
            if (firering == true)
            {
                firering = false;
                flameVideo.Stop();
                flameVideo.clip = flameEnd;
                flameVideo.playbackSpeed = playBackSpeedSLOW;
                flameVideo.Play();
                endingFlames = true;
            }
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        //check for changes while running
    }


    void loopPointFlame(VideoPlayer vp)
    {
        if (startingFlames == true)
        {
            vp.Stop();
            startingFlames = false;
            vp.clip = flameMID;
            flameVideo.playbackSpeed = playBackSpeedSLOW;
            vp.Play();
        }
        if (endingFlames == true)
        {
            endingFlames = false;
            vp.Stop();
            vp.clip = flameStart;
            flameVideo.playbackSpeed = playBackSpeedFast;
            vp.Prepare();
        }
    }
}
