using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero : BaseCharacter
{
    public int stamina;
    public int intellect;
    public int dexterity;
    public int agility;

    public List<BaseAttack> magicAtk = new List<BaseAttack>();
}
