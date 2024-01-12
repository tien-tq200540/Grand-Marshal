using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneOnclick : MonoBehaviour
{
    public GameObject button;
    public int level;
    private void Start()
    {
        Button b = button.GetComponent<Button>();
        b.onClick.AddListener(() => GameManager.Instance.SetCurMap(level));
    }

}
