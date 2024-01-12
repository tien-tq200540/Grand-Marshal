using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBattleOnclick : MonoBehaviour
{
    public GameObject button;
    private void Start()
    {
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(() => GameManager.Instance.StartBattle());
    }
}
