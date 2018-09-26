using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class newQuest : MonoBehaviour {

    public Quest questToStart;

    //checks if quest is already started
    public void CheckQuest()
    {
        if (quests.questList != null)
        {
            Quest q = quests.questList.FirstOrDefault(i => i.questName == questToStart.questName);

            if (q == null)
                quests.questList.Add(questToStart);
        }
        else
        {
            quests.questList.Add(questToStart);
        }
    }
}
