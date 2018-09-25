using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class quests : MonoBehaviour {

    public static List<Quest> questList;

	// Use this for initialization
	void Start () {
		
	}

    void QuestCompleted(string qName)
    {
        Quest temp = questList.FirstOrDefault(i => i.questName == qName);
        temp.completed = true;
        switch (temp.reward)
        {
            case Reward.item:
                menus.invItems.Add(temp.rewardItem);
                break;

            case Reward.ability:
                switch (temp.rewardAbility)
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
                gameControl.control.money += temp.rewardMoney;
                break;

            case Reward.weapon:
                menus.invItems.Add(temp.rewardWeapon);
                break;

            case Reward.gun:
                menus.invItems.Add(temp.rewardGun);
                break;
        }
    }
}

[System.Serializable]
public class Quest
{
    public string questName, questDesc;
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
    dJump,
    dash,
    slowTime
}