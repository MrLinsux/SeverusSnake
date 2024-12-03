using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField]
    int requiredLen = -1;
    [SerializeField]
    int requiredApples = -1;
    [SerializeField]
    TMP_Text lenCounter;
    [SerializeField]
    TMP_Text applesCounter;

    int _requiredLen = -1;
    int _requiredApples = -1;

    public int LenPoints
    {
        get { return _lenPoints; }
        private set
        {
            if (_lenPoints != -1)
            {
                _lenPoints = value;
                lenCounter.text = (_requiredLen - value).ToString();
                CheckWin();
            }
        }
    }
    public int ApplePoints
    {
        get { return _applePoints; }
        private set
        {
            if (_applePoints != -1)
            {
                _applePoints = value;
                lenCounter.text = (_requiredApples - value).ToString();
                CheckWin();
            }
        }
    }
    int _lenPoints = -1;
    int _applePoints = -1;
    public void AppleEaten()
    {
        ApplePoints++;
    }

    public void SetApplesPoints(int points)
    {
        ApplePoints = points;
    }

    public void LenIncreased()
    {
        LenPoints++;
    }

    public void SetLengthPoints(int points)
    {
        LenPoints = points;
    }

    public static GameController CurrentController;
    public static Railway CurrentRailway;

    private void Awake()
    {
        CurrentController = this;
        CurrentRailway = new Railway();

        _requiredApples = requiredApples;
        _requiredLen = requiredLen;
        _lenPoints = requiredLen == -1 ? -1 : 0;
        _applePoints = requiredApples == -1 ? -1 : 0;
        lenCounter.gameObject.SetActive(_requiredLen != -1);
        applesCounter.gameObject.SetActive(_requiredApples != -1);
    }

    public bool CheckWin()
    {
        if ((ApplePoints > _requiredApples) || (LenPoints > _requiredLen))
        {
            Camera.allCameras[0].GetComponent<AudioController>().PlayLoseSound();
            GameOver(false);
        }
        if(((ApplePoints == -1) || (ApplePoints == _requiredApples)) && ((LenPoints == -1) || (LenPoints == _requiredLen)))
        {
            GameOver(true);
        }

        return ((ApplePoints == -1) || (ApplePoints == _requiredApples)) && ((LenPoints == -1) || (LenPoints == _requiredLen));
    }

    public void GameOver(bool isWin)
    {
        if (isWin)
        {
            Debug.Log("You Win!");

        }
        else
        {
            Debug.Log("You Lose");
        }
        //Debug.Break();
        SceneManager.LoadScene("MainMenu");
    }
}
