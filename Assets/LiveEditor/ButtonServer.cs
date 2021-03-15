using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonServer : MonoBehaviour
{

    public GameObject ServerSidepanel;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OpenServerSidepanel);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenServerSidepanel()
    {
        if (ServerSidepanel.activeSelf == false)
        {
            ServerSidepanel.SetActive(true);
        }
        else
        {
            ServerSidepanel.SetActive(false);
        }

    }

}
