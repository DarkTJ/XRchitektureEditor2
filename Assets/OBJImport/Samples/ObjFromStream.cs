using Dummiesman;
using System.IO;
using System.Text;
using UnityEngine;

public class ObjFromStream : MonoBehaviour {
    void Start() {
        Debug.Log("HI!");
    }

    public void LoadObject()
    {
        //make www
        Debug.Log("request starting");
        var www = new WWW("https://people.sc.fsu.edu/~jburkardt/data/obj/lamp.obj");
        Debug.Log("request made. Loading");
        while (!www.isDone)
            System.Threading.Thread.Sleep(1);

        //create stream and load
        Debug.Log("loading to memory");
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
        Debug.Log("loading object");
        var loadedObj = new OBJLoader().Load(textStream);
        Debug.Log("loading done");
    }
}
