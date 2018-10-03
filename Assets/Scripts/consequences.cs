using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class consequences : MonoBehaviour {

    string questName;

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



    //QUEST PROGRESS STUFF

    public void QuestName(string name)
    {
        questName = name;
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
