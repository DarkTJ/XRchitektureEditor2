using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListCreator : MonoBehaviour
{

    [SerializeField]
    private Transform SpawnPoint = null;

    [SerializeField]
    private GameObject lamp = null;

    [SerializeField]
    private RectTransform content = null;

    [SerializeField]
    private int numberOfItems = 3;

    public string[] lampNames = null;
    public Sprite[] lampImages = null;

    // Use this for initialization
    void Start()
    {

        //setContent Holder Height;
        content.sizeDelta = new Vector2(0, numberOfItems * 60);
        int spawnRow = -1;

        for (int i = 0; i < numberOfItems; i++)
        {
            if (i % 3 == 0) { spawnRow += 1; }
            // 60 width of item
            float spawnY = spawnRow * 200;
            float spawnX = i % 3 * 200;
            //newSpawn Position
            Vector3 pos = new Vector3(SpawnPoint.position.x + spawnX, -spawnY, SpawnPoint.position.z);
            
            //instantiate item
            GameObject SpawnedItem = Instantiate(lamp, pos, SpawnPoint.rotation);
            //setParent
            SpawnedItem.transform.SetParent(SpawnPoint, false);
            //get ItemDetails Component
            LampDetails itemDetails = SpawnedItem.GetComponent<LampDetails>();
            //set name
            itemDetails.text.text = lampNames[i];
            //set image
            if (lampImages[i] != null)
            {
                itemDetails.image.sprite = lampImages[i];
            }
            

            


        }

        this.transform.gameObject.SetActive(false);
    }
}
