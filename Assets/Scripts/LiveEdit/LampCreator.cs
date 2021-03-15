using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampCreator : MonoBehaviour
{

    public GameObject ListView;
    public GameObject ListViewContent;

    public GameObject ButtonPrefab;
    public GameObject TextfeldPrefab;

    private float UISize = 200;
    // Start is called before the first frame update
    void Start()
    {
        UISize = 0.25f * Screen.width;
        ListView.GetComponent<RectTransform>().sizeDelta = new Vector2(UISize, 0);


        FillList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillList()
    {
        float spawnY = 10;

        //newPOsition
        Vector3 pos = new Vector3(10, -spawnY, 0);
        //instatntiate Item
        GameObject SpawnedItem = Instantiate(TextfeldPrefab, pos,new Quaternion());
        SpawnedItem.transform.SetParent(ListViewContent.transform, false);
        SpawnedItem.GetComponentInChildren<Text>().text = "firstlol";
        spawnY += 20;
        pos = new Vector3(10, -spawnY, 0);
        SpawnedItem = Instantiate(TextfeldPrefab, pos, new Quaternion());
        SpawnedItem.transform.SetParent(ListViewContent.transform, false);
        spawnY += 30;
        pos = new Vector3(10, -spawnY, 0);
        SpawnedItem = Instantiate(ButtonPrefab, pos, new Quaternion());
        SpawnedItem.transform.SetParent(ListViewContent.transform, false);

    }

    public void SaveIntoJson(LampDetails x)
    {
        string l = JsonUtility.ToJson(x);
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/" + x.name + ".json", l);
    }

}
