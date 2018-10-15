using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class quests : MonoBehaviour {

    public static List<Quest> questList = new List<Quest>();

    public static void QuestCompleted(string qName)
    {
        Quest q = questList.FirstOrDefault(i => i.questName == qName);
        q.completed = true;
        switch (q.reward)
        {
            case Reward.item:
                gameControl.invItems.Add(q.rewardItem);
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
                gameControl.invItems.Add(q.rewardWeapon);
                break;

            case Reward.gun:
                gameControl.invItems.Add(q.rewardGun);
                break;
        }
    }

    public static bool CheckIfCompleted(string qName)
    {
        Quest q = questList.FirstOrDefault(i => i.questName == qName);
        if(q is FetchQuest)
        {
            FetchQuest fq = q as FetchQuest;
            List<Item> tempList = gameControl.invItems.FindAll(i => i.name.Equals(fq.what));
            if (tempList.Count >= fq.howMany)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    gameControl.invItems.Remove(tempList[i]);
                }

                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class Quest
{
    public string questName;
    [TextArea]
    public string questDesc;
    public bool completed;
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
    public QuestItem[] itemsToDeliver;
    public GameObject[] whereToDeliver;
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