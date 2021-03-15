using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using ArtNet.Packets;

public class TCPTestClient : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private ArtNetDmxPacket dart;
    #endregion

    public string serverIP = "localhost";

    public DmxController dmxcontroller;
    public ArtNetPlayer artNetPlayer;
    public int trackToPlay = 0;
    private int randomTrack;

    private bool startSinglePlayerbool = false;
    private bool singlePlayerRunning = false;
    private bool serverConnetion = false;
    private bool recievedaDMXfromTheWWW = false;

    private string messagesave = "";



    // Use this for initialization 	
    void Start()
    {
        randomTrack = UnityEngine.Random.Range(1, 3);
        ConnectToTcpServer();
    }
    // Update is called once per frame
    void Update()
    {
        //we need to get info from thread and call methods beecause the thread itself cant and/or should not!

        //1.Posibilty the server cant connect 
        //is something playing already ??


        //server connected
        //new packet recieved
        if (serverConnetion)
        {
            if (recievedaDMXfromTheWWW)
            {
                dmxcontroller.RecivefromLocalRecorder(dart);
                //Debug.Log("send data to dmx");
                recievedaDMXfromTheWWW = false;
                
            }
        }


        if (startSinglePlayerbool)
        {
            if (!singlePlayerRunning)
            {
                startSinglePlayer();
                //TODO start connection retry
                singlePlayerRunning = true;
            }
            startSinglePlayerbool = false;
        }


        //saveThe Message
        
        if (messagesave.StartsWith("{"))
        {
            if (messagesave.IndexOf("}") != -1) //starts with {, look for }
            {
                var d = messagesave.Substring(0, messagesave.IndexOf("}") + 1);
                try
                {
                    dart = JsonUtility.FromJson<ArtNetDmxPacket>(d);
                    recievedaDMXfromTheWWW = true;

                }
                catch (ArgumentException argumentExeption)
                {
                    Debug.Log("cathced JSON EXEPTION AGAIN *facepalm* AND THIS ONE WAS CORRECTED : " + argumentExeption);
                }
                //Debug.Log("lol" + messagesave);
                messagesave = messagesave.Remove(0, messagesave.IndexOf("}") + 1);
                //Debug.Log("lol2" + messagesave);
                //Debug.Log("recoverd broken message: " + d);
            }

        }
        else if (messagesave.IndexOf("}") != -1) //if message contains a } delete everything up to that point
        {
            messagesave.Remove(0, messagesave.IndexOf("}") + 1);
        }
        else
        {
            if (messagesave != "")
            {
                Debug.Log("broken Message: " + messagesave);
                messagesave = "";
            }
            
        }
        
       



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
        }
        catch (Exception e)
        {

            //this should not be happening but just in case we just start the single player!
            Debug.Log("On client connect exception " + e);

            startSinglePlayerbool = true;
            Debug.Log("Starting Singleplayer, no Server found");
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient(serverIP, 8886);
            Byte[] bytes = new Byte[887];
            //conection hat geklappt;
            serverConnetion = true;
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
                        /*BinaryFormatter formatter = new BinaryFormatter();
                        try
                        {
                            dart = (ArtNetDmxPacket)formatter.Deserialize(stream);
                        }
                        catch (SerializationException e)
                        {
                            Debug.LogError(e);
                        }*/
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        ////Debug.Log(serverMessage);
                        ////Debug.Log(serverMessage[serverMessage.Length - 1] + " " + serverMessage[0]);
                        ////Debug.Log(serverMessage[serverMessage.Length - 1].ToString().Equals("}") + " " + serverMessage[0].ToString().Equals("{"));
                        if (serverMessage[serverMessage.Length -1].ToString().Equals("}") && serverMessage[0].ToString().Equals("{"))
                        {
                            try
                            {
                                dart = JsonUtility.FromJson<ArtNetDmxPacket>(serverMessage);
                                recievedaDMXfromTheWWW = true;
                                //Debug.Log("recieved message: " + length);
                            }
                            catch (ArgumentException argumentExeption)
                            {
                                Debug.Log("cathced JSON EXEPTION AGAIN *facepalm* : " + argumentExeption);
                            }

                            messagesave = "";
                           
                            
                            

                        } else
                        {
                            //try to fix the message!!
                            messagesave = messagesave.Insert(messagesave.Length, serverMessage.ToString() );
                            
                            //Debug.Log("messagesave: " + messagesave + " " + messagesave.StartsWith("{") + " " + messagesave.IndexOf("}"));
                        }
                        //change message to ArtNetOPacket and send it to DMX Controller
                        
                        
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
            startSinglePlayerbool = true ;
            serverConnetion = false;
            Debug.Log("ServerConnection False");
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                //Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    void OnApplicationQuit()
    {
        clientReceiveThread.Abort();
    }

    void startSinglePlayer()
    {
        if (trackToPlay == 0 || trackToPlay >3 )
        {
            //start random playback
            artNetPlayer.StartPlayback(randomTrack);
        }else
        {
            artNetPlayer.StartPlayback(trackToPlay);
        }
        
    }


}