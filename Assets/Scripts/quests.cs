using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class quests : MonoBehaviour {

    public static List<Quest> questList = new List<Quest>();
    public static int workbenchStage = 0;
    public static List<Minigame> minigames = new List<Minigame>();

    private void Start()
    {
        string mainQuestName = "Main quest";
        string mainQuestDesc = "win lol";

        if (questList.FirstOrDefault(q => q.questName == mainQuestName) == null)
        {
            questList.Add(new Quest() {
                questName = mainQuestName,
                isMainQuest = true,
                completed = false,
                questDesc = mainQuestDesc
            });
        }

        if (minigames == null)
            minigames = new List<Minigame>();

        Quest[] qq = questList.FindAll(q => q is DeliveryQuest && q.completed == false).ToArray();
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

    public static void workbenchUse()
    {
        Item[] questItems = items.ownedItems.FindAll(i => i.name == "part").ToArray();
        if (questItems.Length == 3 && !gameControl.control.knowsDash)
            workbenchStage = 1;
        if (gameControl.control.knowsDash)
            workbenchStage = 2;
    }

    public static void QuestCompleted(string qName)
    {
        Quest q = questList.FirstOrDefault(i => i.questName == qName);
        q.completed = true;
        switch (q.reward)
        {
            case Reward.item:
                items.ownedItems.Add(q.rewardItem);
                break;

            case Reward.ability:
                switch (q.rewardAbility)
                {
                    case Ability.dash:
                        gameControl.control.knowsDash = true;
                        playerMovement.dashState = playerMovement.DashState.Ready;
                        break;
                    case Ability.dJump:
                        gameControl.control.knowsDoubleJump = true;
                        break;
                    case Ability.slowTime:
                        gameControl.control.knowsSlowTime = true;
                        break;
                }
                break;

            case Reward.money:
                gameControl.control.money += q.rewardMoney;
                break;

            case Reward.weapon:
                items.ownedItems.Add(q.rewardWeapon);
                break;

            case Reward.gun:
                items.ownedItems.Add(q.rewardGun);
                break;
        }
    }

    public static bool CheckIfCompleted(string qName)
    {
        Quest q = questList.FirstOrDefault(i => i.questName == qName);
        if(q is FetchQuest)
        {
            FetchQuest fq = q as FetchQuest;
            List<Item> tempList = items.ownedItems.FindAll(i => i.name.Equals(fq.what));
            if (tempList.Count >= fq.howMany)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    items.ownedItems.Remove(tempList[i]);
                }

                return true;
            }
        }
        else if(q is DeliveryQuest)
        {
            DeliveryQuest dq = q as DeliveryQuest;
            if (AllDelivered(dq))
                return true;
        }
        return false;
    }

    public static bool AllDelivered(DeliveryQuest dq)
    {
        for(int i = 0; i < dq.whereToDeliver.Length; i++)
        {
            if (dq.delivered[i] == false)
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    [TextArea]
    public string questDesc;
    public bool completed;
    public bool isMainQuest;
    public Reward reward;
    public Ability rewardAbility;
    public int rewardMoney;
    public Item rewardItem;
    public Weapon rewardWeapon;
    public Gun rewardGun;
}

[System.Serializable]
public class FetchQuest: Quest
{
    public int howMany;
    public string what;
}

[System.Serializable]
public class DeliveryQuest : Quest
{
    public Item itemToDeliver;
    [Tooltip("must be same size as the rest")]
    public string[] levelforDelivery;
    [Tooltip("location must have collider and have unique name")]
    public string[] whereToDeliver;
    [Tooltip("must be same size as 'where to deliver'")]
    public bool[] delivered;
}

public enum Reward
{
    money,
    item,
    weapon,
    gun,
    ability
}

public enum Ability
{
    none,
    dJump,
    dash,
    slowTime
}