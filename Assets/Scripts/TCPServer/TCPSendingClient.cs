using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using ArtNet.Packets;
using UnityEngine.UI;
using TMPro;

public class TCPSendingClient : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private ArtNetDmxPacket dart;
    #endregion

    public string serverIP = "localhost";
    public int paketeProSekunde = 10;
    public string dmxZwischenspeicherUniverse0;
    public string dmxZwischenspeicherUniverse1;
    private byte[] lastdmxZwischenspeicherUniverse0;
    private byte[] lastdmxZwischenspeicherUniverse1;

    public DmxControllerServerVersion dmxcontroller;


    public string serverStatusString = "not connected";
    public Color serverStatusColor = Color.red;
    public string PacketCoutnString;
    public TextMeshProUGUI ServerStatus;
    public TextMeshProUGUI PacketCount;
    private long countsendPackages = 0;
    public long countrecievedPackages = 0;

    void ServerStatusTextSet(string e, bool c)
    {

        if (c == false)
        {
            //ServerStatus.fontSize = 25;
            serverStatusColor = Color.red;
            serverStatusString = "Error connecting: " + e;
        }
        else
        {
            //ServerStatus.fontSize = 36;
            serverStatusColor = Color.green;
            serverStatusString = "Connected:" + e;
        }

    }
    // Use this for initialization 	
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (ServerStatus.text != serverStatusString)
        {
            ServerStatus.text = serverStatusString;
            ServerStatus.color = serverStatusColor;
        }

        PacketCoutnString = countsendPackages + "/" + countrecievedPackages;
        if (PacketCount.text != PacketCoutnString)
        {
            PacketCount.text = PacketCoutnString;
        }

    }

    public void connectToArtNetServer(string IP)
    {
        serverIP = IP;
        ConnectToTcpServer();
    }

    public void disconnectFromServer()
    {
        CancelInvoke("SendArtNet");
        clientReceiveThread.Abort();
        ServerStatusTextSet("NoConnection", false);
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();

            InvokeRepeating("SendArtNet", 2.0f, 1.0f / paketeProSekunde);
        }
        catch (Exception e)
        {
            //Debug.Log("On client connect exception " + e);
            ServerStatusTextSet(e.ToString(), false);
        }
    }

    private void SendArtNet()
    {

        //only send when there is a connection


        //only Send if there is soemthing to send!
        if (socketConnection != null)
        {
            //TODO: Only send if there is a change !
            if (dmxZwischenspeicherUniverse0.Length != 0 && !dmxZwischenspeicherUniverse0.Equals(lastdmxZwischenspeicherUniverse0))
            {
                SendMessageTOServer(dmxZwischenspeicherUniverse0);
                //lastdmxZwischenspeicherUniverse0 = new byte[dmxZwischenspeicherUniverse0.Length];
                // Array.Copy(dmxZwischenspeicherUniverse0, lastdmxZwischenspeicherUniverse0, dmxZwischenspeicherUniverse0.Length);

                //dmxZwischenspeicherUniverse0.CopyTo(lastdmxZwischenspeicherUniverse0,0);
                countsendPackages += 1;
            }

            if (dmxZwischenspeicherUniverse1.Length != 0 && !dmxZwischenspeicherUniverse1.Equals(lastdmxZwischenspeicherUniverse1))
            {
                SendMessageTOServer(dmxZwischenspeicherUniverse1);
                //lastdmxZwischenspeicherUniverse1 = new byte[dmxZwischenspeicherUniverse1.Length];
                //dmxZwischenspeicherUniverse1.CopyTo(lastdmxZwischenspeicherUniverse1, 0);


                countsendPackages += 1;
            }
        }



    }
    /// <summary> 
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>   
    /// 
    //Server listens to data but ignores it for now
    private void ListenForData()
    {
        try
        {
            Debug.LogWarning(" ." + serverIP + ". .");
            socketConnection = new TcpClient(serverIP, 8886);
            Byte[] bytes = new Byte[2100];
            ServerStatusTextSet("Connected", true);
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {

                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);

                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        //Debug.Log(serverMessage);
                        //Debug.Log(serverMessage[serverMessage.Length - 1] + " " + serverMessage[0]);
                        //Debug.Log(serverMessage[serverMessage.Length - 1].ToString().Equals("}") + " " + serverMessage[0].ToString().Equals("{"));
                        /*if (serverMessage[serverMessage.Length -1].ToString().Equals("}") && serverMessage[0].ToString().Equals("{"))
                        {
                            dart = JsonUtility.FromJson<ArtNetDmxPacket>(serverMessage);
                            dmxcontroller.RecivefromLocalRecorder(dart);
                            Debug.Log(length);
                        } else
                        {
                            Debug.Log("broken Message");
                        }*/
                        Debug.Log("recieved message, but ignoring it LOL");
                        //change message to ArtNetOPacket and send it to DMX Controller


                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            ServerStatusTextSet(socketException.ToString(), false);
            //Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	


    private void SendMessageTOServer(string messagetoSend)
    {
        if (socketConnection == null)
        {
            Debug.Log("no connection, but tryid it ....");
            ServerStatusTextSet("lost connection", false);
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(messagetoSend);
                // Write byte array to socketConnection stream.   
                stream.Write(serverMessageAsByteArray, 0, messagetoSend.Length);


            }
        }
        catch (SocketException socketException)
        {
            ServerStatusTextSet(socketException.ToString(), false);
            Debug.Log("Socket exception: " + socketException);
        }
    }

    void OnApplicationQuit()
    {
        CancelInvoke("SendArtNet");
        clientReceiveThread.Abort();
    }

    public void ArtNetDatatoSend(ArtNetDmxPacket e)
    {

        if (e.Universe == 0)
        {
            string dat = JsonUtility.ToJson(e);
            //Debug.Log(e);
            dmxZwischenspeicherUniverse0 = dat;
        }
        else if (e.Universe == 1)
        {
            string dat = JsonUtility.ToJson(e);
            //Debug.Log(e);
            dmxZwischenspeicherUniverse1 = dat;
        }
        else
        {
            Debug.Log("this packet has univese: " + e.Universe + " and will not be send");
        }
        /*
        if (e.Universe == 0)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(stream, e);
                    try
                    {
                        byte[] dat = new byte[1024];
                        dat = stream.ToArray();
                        //Debug.LogWarning(dat[6].ToString());
                        dmxZwischenspeicherUniverse0 = new byte[dat.Length];
                        dmxZwischenspeicherUniverse0 = (byte[])dat.Clone();
                        //Array.Copy(dat, dmxZwischenspeicherUniverse0, dat.Length);
                        //dat.CopyTo(dmxZwischenspeicherUniverse0, 0);
                        //DeserializeandPrint(dat);
                        //Debug.Log(dmxZwischenspeicherUniverse0[6].ToString());
                        //Debug.Log(dat.Equals(dmxZwischenspeicherUniverse0));

                        ArtNetDmxPacket deserializedPacket = (ArtNetDmxPacket)formatter.Deserialize(stream);
                        Debug.Log(deserializedPacket.DmxData);
                    }
                    catch (Exception e1) { print(e1); }
                }
                catch (SerializationException hhe) { Debug.Log("Serialization Failed : " + hhe.Message); }
            }
        }
        else if (e.Universe == 1)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    formatter.Serialize(stream, e);
                    try
                    {
                        byte[] dat = stream.ToArray();
                        dmxZwischenspeicherUniverse1 = new byte[dat.Length];
                        dat.CopyTo(dmxZwischenspeicherUniverse1, 0);
                    }
                    catch (Exception hhhj) { print(hhhj); }
                }
                catch (SerializationException hhe) { Debug.Log("Serialization Failed : " + hhe.Message); }
            }
        }
        else
        {
            Debug.LogWarning("this packet has univese: " + e.Universe + " and will not be send");
        }*/
    }

    /*void DeserializeandPrint(byte[] bS)
    {
        
        using (MemoryStream stream = new MemoryStream())
        {
            Debug.LogWarning(bS[6]);
            int length;
            try
            {
                length = stream.Read(bS, 0, bS.Length);
            } catch (Exception e)
            {
                length = 0;
                Debug.LogError(e);
            }
            
            Debug.Log(length);
            // Read incomming stream into byte arrary. 					
            while (length != 0)
            {
                
                var incommingData = new byte[length];
                Array.Copy(bS, 0, incommingData, 0, length);
                BinaryFormatter formatter = new BinaryFormatter();
                try
                {
                    Debug.Log((ArtNetDmxPacket)formatter.Deserialize(stream));
                }
                catch (SerializationException e)
                {
                    Debug.LogError(e);
                }

                length = stream.Read(bS, 0, bS.Length);
            }
        }
    }*/
}
