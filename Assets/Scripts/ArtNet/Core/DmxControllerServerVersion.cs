using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;

using ArtNet.Sockets;
using ArtNet.Packets;

public class DmxControllerServerVersion : MonoBehaviour
{
    [Header("send dmx")]
    public bool useBroadcast;
    public string remoteIP = "localhost";
    IPEndPoint remote;

    [Header("dmx devices")]
    public UniverseDevices[] universes;
    public bool isServer;

    ArtNetSocket artnet;
    public TCPSendingClient server;
    public ArtNetRecorder recorder;
    [Header("send/recieved DMX data for debug")]
    [SerializeField] ArtNetDmxPacket latestReceivedDMX;
    [SerializeField] ArtNetDmxPacket dmxToSend;

    byte[] _dmxData;

    Dictionary<int, byte[]> dmxDataMap;

    [ContextMenu("send DMX")]
    public void Send()
    {
        if (useBroadcast && isServer)
            artnet.Send(dmxToSend);
        else
            artnet.Send(dmxToSend, remote);
    }
    public void Send(short universe, byte[] dmxData)
    {
        dmxToSend.Universe = universe;
        System.Buffer.BlockCopy(dmxData, 0, dmxToSend.DmxData, 0, dmxData.Length);

        if (useBroadcast && isServer)
            artnet.Send(dmxToSend);
        else
            artnet.Send(dmxToSend, remote);


    }

    private void OnValidate()
    {
        foreach (var u in universes)
            u.Initialize();
    }

    public void ValidateAllDMXDAta()
    {
        foreach (var u in universes)
            u.Initialize();
    }


    public bool newPacket;
    void Start()
    {

        artnet = new ArtNetSocket();

        if (isServer)
        {
            artnet.Open(FindFromHostName(remoteIP), null);
            Debug.Log("Server Startet on: " + remoteIP);
        }
        //サブネットマスクを設定すると、自分に送らないアドレスを設定してくれる（便利！）
        //なのだが、デバッグがめんどくさくなる
        dmxToSend.DmxData = new byte[512];

        artnet.NewPacket += (object sender, NewPacketEventArgs<ArtNetPacket> e) =>
        {

            newPacket = true;
            if (e.Packet.OpCode == ArtNet.Enums.ArtNetOpCodes.Dmx)
            {
                var packet = latestReceivedDMX = e.Packet as ArtNetDmxPacket;


                //Recorder,sendet alle Artnepakete an die aufnahem
                //recorder.DatatoRecord(packet);

                //send to tcp server
                server.ArtNetDatatoSend(packet);
                server.countrecievedPackages +=1;

                if (packet.DmxData != _dmxData)
                    _dmxData = packet.DmxData;

                var universe = packet.Universe;
                if (dmxDataMap.ContainsKey(universe))
                    dmxDataMap[universe] = packet.DmxData;
                else
                    dmxDataMap.Add(universe, packet.DmxData);
            }
        };

        if (!useBroadcast || !isServer)
        {
            remote = new IPEndPoint(FindFromHostName(remoteIP), ArtNetSocket.Port);
            Debug.Log(ArtNetSocket.Port + "port genutzt, kein server oder kein boreadcast");

        }

        dmxDataMap = new Dictionary<int, byte[]>();
    }

    public void RecivefromLocalRecorder(ArtNetDmxPacket e)
    {
        newPacket = true;
        if (e.OpCode == ArtNet.Enums.ArtNetOpCodes.Dmx)
        {

            var packet = latestReceivedDMX = e as ArtNetDmxPacket;
            recorder.DatatoRecord(packet);
            if (packet.DmxData != _dmxData)
                _dmxData = packet.DmxData;

            var universe = packet.Universe;
            if (dmxDataMap.ContainsKey(universe))
                dmxDataMap[universe] = packet.DmxData;
            else
                dmxDataMap.Add(universe, packet.DmxData);
        }
    }

    private void OnDestroy()
    {
        artnet.Close();
    }

    private void Update()
    {
        var keys = dmxDataMap.Keys.ToArray();

        for (var i = 0; i < keys.Length; i++)
        {
            var universe = keys[i];
            var dmxData = dmxDataMap[universe];
            if (dmxData == null)
                continue;

            var universeDevices = universes.Where(u => u.universe == universe).FirstOrDefault();
            if (universeDevices != null)
                foreach (DMXDevice d in universeDevices.devices)
                    d.SetData(dmxData.Skip(d.startChannel).Take(d.NumChannels).ToArray());

            dmxDataMap[universe] = null;
        }
    }

    static IPAddress FindFromHostName(string hostname)
    {
        var address = IPAddress.None;
        try
        {
            if (IPAddress.TryParse(hostname, out address))
                return address;

            var addresses = Dns.GetHostAddresses(hostname);
            for (var i = 0; i < addresses.Length; i++)
            {
                if (addresses[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    address = addresses[i];
                    break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogErrorFormat(
                "Failed to find IP for :\n host name = {0}\n exception={1}",
                hostname, e);
        }
        return address;
    }

    /*[System.Serializable]
   public class UniverseDevices
   {
       public string universeName;
       public int universe;
       public DMXDevice[] devices;

       public void Initialize()
       {
           var startChannel = 0;
           foreach (var d in devices)
               if (d != null)
               {
                   d.startChannel = startChannel;
                   startChannel += d.NumChannels;
                   d.name = string.Format("{0}:({1},{2:d3}-{3:d3})", d.GetType().ToString(), universe, d.startChannel, startChannel - 1);
               }
           if (512 < startChannel)
               Debug.LogErrorFormat("The number({0}) of channels of the universe {1} exceeds the upper limit(512 channels)!", startChannel, universe);
       }
   }*/

    //AN VALIDATE FUNKTION DENKEN

    [System.Serializable]
    public class UniverseDevices
    {
        public string universeName;
        public int universe;
        public DMXDevice[] devices;
        public GameObject[] dmxDevicesSearch;

        public void Initialize()
        {
            dmxDevicesSearch = GameObject.FindGameObjectsWithTag("DMX");
            int leangeDevices = 0;
            //count devices
            foreach (GameObject g in dmxDevicesSearch)
            {
                DMXDevice d = g.GetComponent<DMXDevice>();
                if (d.universe == universe)
                {
                    leangeDevices += 1;
                }
            }
                devices = new DMXDevice[leangeDevices];
            //devices = new ArrayList();
            int x = 0;
            var startChannel = 0;
            //check for desired dmx channel !
            foreach (GameObject g in dmxDevicesSearch)
            {
                
                DMXDevice d = g.GetComponent<DMXDevice>();
                if (d.universe == universe)
                {
                    devices[x] = d;
                    if (d.startChannelSET == false)
                    {
                        d.startChannel = startChannel;
                    }
                    else
                    {
                        if (startChannel < d.startChannel)
                        {
                            startChannel = d.startChannel;
                        }
                    }
                    startChannel += d.NumChannels;
                    d.name = string.Format("{0}:({1},{2:d3}-{3:d3})", d.GetType().ToString(), universe, d.startChannel, startChannel - 1);
                    x++;
                }
                
            }
        }

        
    }
}