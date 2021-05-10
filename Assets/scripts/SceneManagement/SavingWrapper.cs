﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Saving;
namespace RPG.SceneManagement
{


    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float FadeInTime = 0.2f;
        private bool isRunning = false;

        private IEnumerator LoadLastScene()
        {
            isRunning = true;
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(FadeInTime);
            isRunning = false;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadFromLastSave();
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }
        public void LoadFromLastSave()
        {
            if(isRunning == false)
                StartCoroutine(LoadLastScene());
        }
        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}
