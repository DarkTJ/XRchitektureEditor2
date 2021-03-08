using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImportLamps : MonoBehaviour
{
    public GameObject Menue;

    
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OpenLampSelection);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OpenLampSelection()
    {
        if (Menue.active == false)
        {
            Menue.SetActive(true);
        }else
        {
            Menue.SetActive(false);
        }
        
    }
}
