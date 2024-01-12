using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int level;
    public Text textLevel;

    private void Start()
    {
        textLevel.text = level.ToString();
    }
}
