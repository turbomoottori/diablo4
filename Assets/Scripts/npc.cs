using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class npc : MonoBehaviour {

    public Dialogue[] dialogue;
    public bool cycleText;
    int currentDialogue;

	// Use this for initialization
	void Start () {
		
	}
	
	public void Interact()
    {
        if (cycleText)
        {
            currentDialogue += 1;
            if (currentDialogue > dialogue.Length)
                currentDialogue = 1;
        }
    }
}

[System.Serializable]
public class Dialogue
{
    public who who;
    public npcDialogue npc;
    public playerDialogue[] player;
}

[System.Serializable]
public class npcDialogue
{
    [TextArea]
    public string talk;
}

[System.Serializable]
public class playerDialogue
{
    public string answer;
    public string reactionToAnswer;
    public UnityEvent consequenceToAnswer;
}

public enum who
{
    npc,
    player
}
