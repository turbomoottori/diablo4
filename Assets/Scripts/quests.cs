using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class quests : MonoBehaviour {

    public static List<Quest> questList = new List<Quest>();

    void QuestCompleted(string qName)
    {
        Quest q = questList.FirstOrDefault(i => i.questName == qName);
        q.completed = true;
        switch (q.reward)
        {
            case Reward.item:
                menus.invItems.Add(q.rewardItem);
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
                menus.invItems.Add(q.rewardWeapon);
                break;

            case Reward.gun:
                menus.invItems.Add(q.rewardGun);
                break;
        }
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