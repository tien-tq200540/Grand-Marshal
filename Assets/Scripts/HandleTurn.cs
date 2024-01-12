using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string attacker; // name of attacker
    public string type;
    public GameObject attackGameObject; // who attacks
    public GameObject attackerTarget; // who is going to be attacked

    //which attack is performed
    public BaseAttack choosenAttack;
}
