using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;
    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(enemyPrefab); //save input enemy prefabs
    }

    public void ShowSelector()
    {
        enemyPrefab.transform.Find("Selector").gameObject.SetActive(true);
    }

    public void HideSelector()
    {
        enemyPrefab.transform.Find("Selector").gameObject.SetActive(false);
    }
}
