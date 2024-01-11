using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    [SerializeField]
    string loadLevelName = "TestLevel";
    public void LoadLevel()
    {
        SceneManager.LoadScene(loadLevelName);
    }
}
