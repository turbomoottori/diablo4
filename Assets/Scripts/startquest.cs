using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class startquest : MonoBehaviour {

    public questType type;
    public Quest quest;
    public FetchQuest fQuest;
    public DeliveryQuest dQuest;

    public void CheckQuest()
    {
        switch (type)
        {
            case questType.regular:
                if (quests.questList != null)
                {
                    Quest q = quests.questList.FirstOrDefault(i => i.questName == quest.questName);
                    if (q == null)
                        quests.questList.Add(quest);
                }
                else
                    quests.questList.Add(quest);

                print(quest.questName + "started");
                break;
            case questType.fetch:
                if (quests.questList != null)
                {
                    Quest q = quests.questList.FirstOrDefault(i => i.questName == fQuest.questName);
                    if (q == null)
                        quests.questList.Add(fQuest);
                }
                else
                    quests.questList.Add(fQuest);
                break;
            case questType.delivery:
                if (quests.questList != null)
                {
                    Quest q = quests.questList.FirstOrDefault(i => i.questName == dQuest.questName);
                    if (q == null)
                        quests.questList.Add(dQuest);
                }
                else
                    quests.questList.Add(dQuest);

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
                break;
        }
    }

    public enum questType
    {
        regular,
        fetch,
        delivery
    }
}
