using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    [SerializeField]
    int requiredLen = -1;
    [SerializeField]
    int requiredApples = -1;

    static int _requiredLen = -1;
    static int _requiredApples = -1;

    public static int LenPoints {  get { return _lenPoints; } set { if (_lenPoints != -1) _lenPoints = value; CheckWin(); } }
    public static int ApplePoints {  get { return _applePoints; } set { if (_applePoints != -1) _applePoints = value; CheckWin(); } }
    static int _lenPoints = -1;
    static int _applePoints = -1;

    private void Awake()
    {
        _requiredApples = requiredApples;
        _requiredLen = requiredLen;
        _lenPoints = requiredLen == -1 ? -1 : 0;
        _applePoints = requiredApples == -1 ? -1 : 0;
    }

    public static bool CheckWin()
    {
        if ((ApplePoints > _requiredApples) || (LenPoints > _requiredLen))
        {
            GameOver(false);
        }
        if(((ApplePoints == -1) || (ApplePoints == _requiredApples)) && ((LenPoints == -1) || (LenPoints == _requiredLen)))
        {
            GameOver(true);
        }

        return ((ApplePoints == -1) || (ApplePoints == _requiredApples)) && ((LenPoints == -1) || (LenPoints == _requiredLen));
    }

    public static void GameOver(bool isWin)
    {
        if (isWin)
            Debug.Log("You Win!");
        else
            Debug.Log("You Lose");
        Debug.Break();
    }
}
