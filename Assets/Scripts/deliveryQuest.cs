using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class deliveryQuest : MonoBehaviour {

    public DeliveryQuest questToStart;
    int howMany = 0;

    private void Start()
    {
        questToStart.itemToDeliver.stackable = true;

        foreach(string locationName in questToStart.whereToDeliver)
        {
            GameObject location = GameObject.Find(locationName);
            if (location.gameObject.GetComponent<interactable>() == null)
                location.gameObject.AddComponent<interactable>();

            location.GetComponent<interactable>().type = interactable.Type.deliveryLocation;
            location.GetComponent<interactable>().deliveryQuest = questToStart.questName;
            howMany += 1;
        }
    }

    //checks if quest is already started
    public void CheckQuest()
    {
        if (quests.questList != null)
        {
            Quest q = quests.questList.FirstOrDefault(i => i.questName == questToStart.questName);

            if (q == null)
            {
                quests.questList.Add(questToStart);
                for(int i = 0; i < howMany; i++)
                    items.ownedItems.Add(questToStart.itemToDeliver);
            }
        }
        else
        {
            quests.questList.Add(questToStart);
        }
    }
}
