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

public class TCPSendingClient : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private ArtNetDmxPacket dart;
    #endregion

    public string serverIP = "localhost";
    public int paketeProSekunde = 10;
    public byte[] dmxZwischenspeicherUniverse0;
    public string dmxZwischenspeicherUniverse1;

    public DmxControllerServerVersion dmxcontroller;
    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {

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
            Debug.Log("On client connect exception " + e);
        }
    }

    private void SendArtNet()
    {
        //SendMessage(dmxZwischenspeicherUniverse0);
        // SendMessage(dmxZwischenspeicherUniverse1);

    }
    /// <summary> 
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(serverIP, 8886);
            Byte[] bytes = new Byte[2100];
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
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	


    new private void SendMessage(string messagetoSend)
    {
        if (socketConnection == null)
        {
            Debug.Log("no connection, but tryid it ....");
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                Debug.Log("jsonlengthLenth: " + messagetoSend.Length);
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(messagetoSend);
                // Write byte array to socketConnection stream.   
                Debug.Log("serverSendMessageLenth: " + serverMessageAsByteArray.Length);
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                Debug.Log("Server sent his message - should be received by client");
            }
        }
        catch (SocketException socketException)
        {
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

            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {

                try
                {
                    formatter.Serialize(stream, e);

                    try
                    {

                        byte[] dat = stream.ToArray();

                        dmxZwischenspeicherUniverse0 = new byte[dat.Length];
                        dat.CopyTo(dmxZwischenspeicherUniverse0, 0);

                    }
                    catch (Exception hhhj) { print(hhhj); }
                    //Debug.Log(e);


                }
                catch (SerializationException hhe)
                {
                    Debug.Log("Serialization Failed : " + hhe.Message);
                }
            }

        }
        else if (e.Universe == 1)
        {
            string dat = JsonUtility.ToJson(e);
            //Debug.Log(e);
            dmxZwischenspeicherUniverse1 = dat;
            Debug.LogWarning("stringArray: " + dat.Length);
        }
        else
        {
            Debug.Log("this packet has univese: " + e.Universe + " and will not be send");
        }


    }
}
