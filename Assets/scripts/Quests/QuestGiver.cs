using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest quest;
        public bool givedQuest = false;

        public void GiveQuest()
        {
            givedQuest = true;
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(quest);
        }

        public bool DidQuestGive()
        {
            return givedQuest;
        }
    }
}


