using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Text mapName;

    public void setMapName()
    {
        int mapNum = GameManager.Instance.GetCurMap();
        mapName.text = "Map " + (mapNum + 1);
    }
}
