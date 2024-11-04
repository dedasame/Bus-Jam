using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void LevelIndex()
    {
        PlayerPrefs.SetInt("levelIndex",GameScript.Instance.levelIndex);
    }


}
