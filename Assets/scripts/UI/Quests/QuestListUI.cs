using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] Quest[] tempQuests;
        [SerializeField] QuestItemUI questPrefab;

        private void Start()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Quest quest in tempQuests)
            {
                QuestItemUI uiINstance = Instantiate<QuestItemUI>(questPrefab, transform);
                uiINstance.Setup(quest);
            }
        }
    }
}

