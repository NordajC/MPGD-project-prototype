using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCraftingItems : MonoBehaviour
{
    public GameObject craftingItemPrefab;
    public ItemTemplate[] items;
    public int rowSize = 4;
    public int offset_y = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Dynamically adds all the crafting items to the tab using the items list.
        for(int i = 0; i < items.Length; i++)
        {
            if(i % 4 == 0) //If a set of 4 has been added, then the y offset is incremented to start a new row.
            {
                offset_y += 75;
            }

            var spawnedTemplate = Instantiate(craftingItemPrefab, transform);
            spawnedTemplate.GetComponent<RectTransform>().localPosition = new Vector2(-115 + ((i % 4)*75), 190 - (offset_y));

            spawnedTemplate.GetComponent<SetCraftableInfo>().setInfo(items[i]);

        }
    }
}
