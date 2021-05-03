using RPG.Core;
using RPG.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Barrier : MonoBehaviour
    {
        [SerializeField] GameObject questGiver;
        [SerializeField] GameObject portal = null;
        [SerializeField] GameObject barrier;
        void Awake()
        {
            if(portal != null)
                portal.SetActive(false);
        }
        // Update is called once per frame
        void Update()
        {
            if (questGiver.GetComponent<QuestGiver>().DidQuestGive())
            {
                if (portal != null)
                    portal.SetActive(true);
                barrier.SetActive(false);
            }
        }
        
    }
}

