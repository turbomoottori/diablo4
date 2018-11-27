using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class deliveryQuest : MonoBehaviour {

    public DeliveryQuest questToStart;

    public void Start()
    {
        questToStart.itemToDeliver.stackable = true;
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
                for(int i = 0; i < questToStart.levelforDelivery.Length; i++)
                    items.ownedItems.Add(questToStart.itemToDeliver);
            }
        }
        else
        {
            quests.questList.Add(questToStart);
        }

        Quest[] qq = quests.questList.FindAll(q => q is DeliveryQuest && q.completed == false).ToArray();
        if (qq.Length != 0)
        {
            foreach (DeliveryQuest d in qq)
            {
                for (int i = 0; i < d.levelforDelivery.Length; i++)
                {
                    if (SceneManager.GetActiveScene().name == d.levelforDelivery[i])
                    {
                        GameObject location = GameObject.Find(d.whereToDeliver[i]);
                        if (!d.delivered[i])
                        {
                            if (location.gameObject.GetComponent<interactable>() == null)
                                location.gameObject.AddComponent<interactable>();

                            location.GetComponent<interactable>().type = interactable.Type.deliveryLocation;
                            location.GetComponent<interactable>().deliveryQuest = d.questName;
                        }
                    }
                }
            }
        }
    }
}
