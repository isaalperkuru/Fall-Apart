using RPG.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.UI
{
    public class LoadButton : MonoBehaviour
    {
        public void LoadFromLastSave()
        {
            FindObjectOfType<SavingWrapper>().LoadFromLastSave();
        }

        public void NewGame()
        {
            FindObjectOfType<SavingWrapper>().Delete();
            SceneManager.LoadScene(1);
        }

        public void QuitGame()
        {
            Debug.Log("Quit");
            Application.Quit();
        }
    }
}

