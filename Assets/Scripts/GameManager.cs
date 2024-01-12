using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        WAIT, BATTLE
    }

    public GameState gameState;

    [System.Serializable]
    public class MapData
    {
        public string mapName;
        public int maxEnemies;
        public string battleScene;
        public List<GameObject> possibleEnemies = new List<GameObject>();
    }
    public List<MapData> map = new List<MapData>();

    public int numOfEnemies;
    public List<GameObject> enemyToBattle = new List<GameObject>();

    [SerializeField] private int curMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameState = GameState.WAIT;
    }

    public void SetCurMap(int curMap)
    {
        this.curMap = curMap;
    }

    public int GetCurMap()
    {
        return this.curMap;
    }

    public void StartBattle()
    {
        //get random amount of enemy
        this.numOfEnemies = map[curMap - 1].maxEnemies;

        //which enemy
        for (int i = 0; i < numOfEnemies; i++)
        {
            enemyToBattle.Add(map[curMap - 1].possibleEnemies[Random.Range(0, map[curMap - 1].possibleEnemies.Count)]);
        }

        //load scene
        gameState = GameState.BATTLE;
        SceneManager.LoadScene(map[curMap - 1].battleScene, LoadSceneMode.Single);
    }
}
