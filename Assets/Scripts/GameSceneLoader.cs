using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public void LoadSceneAfterBatlle()
    {
        GameManager.Instance.enemyToBattle.Clear();
        SceneManager.LoadScene("WorldMapScene", LoadSceneMode.Single);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
