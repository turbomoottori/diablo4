using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class consequences : MonoBehaviour {

    string questName;
    int tempAmount;

	public void LoseMoney(int moneyToLose)
    {
        if (gameControl.control.money >= moneyToLose)
            gameControl.control.money -= moneyToLose;
        else
            gameControl.control.money = 0;
    }

    public void GainMoney(int moneyToGain)
    {
        gameControl.control.money += moneyToGain;
    }

    public void TurnHostile(GameObject who)
    {
        if (who.GetComponent<civilian>() != null)
            who.GetComponent<civilian>().TurnHostile();
    }

    public void FinishQuest(newQuest q)
    {
        quests.QuestCompleted(q.questToStart.questName);
    }

    public void BlacksmithMerchant(GameObject blacksmith)
    {
        Destroy(blacksmith.GetComponent<npc>());
        blacksmith.AddComponent<merchant>();
        List<Item> listToAdd = new List<Item>();
        listToAdd.Add(new Weapon() { name = "cool sword", damage = 2, id = 1, baseValue = 20, speed = 0.15f, weight = 2, stackable = false, questItem = false });
        blacksmith.GetComponent<merchant>().mitems = listToAdd;
        blacksmith.GetComponent<merchant>().priceMultiplier = 2;
        blacksmith.GetComponent<interactable>().type = interactable.Type.merchant;
    }

    public void ChangeAmount(int amount)
    {
        tempAmount = amount;
    }

    public void GiveMoney(GameObject self)
    {
        if (gameControl.control.money < tempAmount)
        {
            self.GetComponent<npc>().dialogues[self.GetComponent<npc>().currentDialogue].dialogue[ui.currentConvo.Length-1].npc.talk = "NOT ENOUGH MONEY!!!! DIE";
            ui.currentConvo[ui.currentConvo.Length-1].npc.talk="NOT ENOUGH MONEY";
            TurnHostile(self);
        }
        else
        {
            self.GetComponent<npc>().dialogues[self.GetComponent<npc>().currentDialogue].dialogue[self.GetComponent<npc>().dialogues[self.GetComponent<npc>().currentDialogue].dialogue.Length - 1].npc.talk = "THANKS";
            gameControl.control.money -= tempAmount;
            items.ownedItems.Add(new Item() {
                name = "METAL THING",
                baseValue = 20,
                id = 0,
                questItem = true,
                stackable = false,
                weight = 10
            });

            Destroy(self.GetComponent<dropItem>());
            self.GetComponent<npc>().NextConvo();
        }
    }

    public void StartRobotMinigameQuest()
    {
        Quest q = quests.questList.FirstOrDefault(quest => quest.questName == "Fix robot");
        if (q == null)
            quests.questList.Add(new Quest() {
                questName = "Fix robot",
                questDesc = "help watchmaker fix the robot",
                completed = false,
                reward = Reward.item,
                rewardItem = new Item() {
                    name = "part",
                    stackable = true,
                    questItem = true
                }
            });
    }

    //QUEST PROGRESS STUFF

    public void QuestName(string name)
    {
        questName = name;
    }

    public void QuestCompleted(string qName)
    {
        Quest q = quests.questList.FirstOrDefault(i => i.questName == qName);
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

    /*public void QuestMarkVisited(int markNumber)
    {
        foreach(Quest q in quests.questList)
        {
            if (q.questName == questName)
                q.requirements.marks[markNumber] = true;

            if (AllMarks(q.requirements.marks))
                q.completed = true;
        }
    }

    private bool AllMarks(bool[] q)
    {
        for(int i=0; i < q.Length; i++)
        {
            if (q[i] == false)
                return false;
        }

        return true;
    }*/
}
