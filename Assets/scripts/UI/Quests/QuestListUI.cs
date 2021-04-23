using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;
        QuestList questList;

        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI uiINstance = Instantiate<QuestItemUI>(questPrefab, transform);
                uiINstance.Setup(status);
            }
        }
    }
}

