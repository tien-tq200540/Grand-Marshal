using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter
{
    public string characterName;

    public float baseHP;
    public float curHP;

    public float baseMP;
    public float curMP;

    public float baseATK;
    public float curATK;

    public float baseDEF;
    public float curDEF;

    public List<BaseAttack> attackTypeList = new List<BaseAttack>();

    public enum Rarity
    {
        NORMAL,
        RARE,
        SUPER_RARE,
        ULTRA_RARE
    }

    public enum Type
    {
        METAL, PLANT, WATER, FIRE, EARTH
    }

    public Rarity rarity;
    public Type characterType;
}
