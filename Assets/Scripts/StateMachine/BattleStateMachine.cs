using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public PerformAction battleStates;

    public List<HandleTurn> performList = new List<HandleTurn>();
    public List<GameObject> herosInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();

    public enum HeroUI
    {
        ACTIVATE,
        WAITING,
        INPUT1, //for basic attack
        INPUT2, //for select enemy
        DONE
    }

    public HeroUI heroInput;
    public List<GameObject> herosToManage = new List<GameObject>();
    private HandleTurn heroChoice;

    public GameObject enemyButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject enemySelectPanel;
    public GameObject magicPanel;

    public Transform actionSpacer;
    public Transform magicSpacer;

    public GameObject actionButton;
    public GameObject magicButton;
    private List<GameObject> atkButtons = new List<GameObject>();

    private List<GameObject> enemyButtons = new List<GameObject>();

    public List<Transform> spawnPos = new List<Transform>();

    public GameObject winPanel;
    public GameObject losePanel;

    private void Awake()
    {
        for (int i = 0; i < GameManager.Instance.numOfEnemies; i++)
        {
            GameObject newEnemy = Instantiate(GameManager.Instance.enemyToBattle[i], spawnPos[i].position, Quaternion.identity) as GameObject;
            newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.characterName + "_" + i;
            newEnemy.GetComponent<EnemyStateMachine>().enemy.characterName = newEnemy.name;
            enemiesInBattle.Add(newEnemy);
        }
    }

    void Start()
    {
        battleStates = PerformAction.WAIT;
        
        //enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        heroInput = HeroUI.ACTIVATE;
        
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        magicPanel.SetActive(false);

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        this.EnemyButtons();
    }

    void Update()
    {
        switch (battleStates)
        {
            case (PerformAction.WAIT):
                if (this.performList.Count > 0)
                {
                    this.battleStates = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(this.performList[0].attacker);
                if (performList[0].type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i < herosInBattle.Count; i++)
                    {
                        if (performList[0].attackerTarget == herosInBattle[i])
                        {
                            ESM.heroToAttack = performList[0].attackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        } else
                        {
                            performList[0].attackerTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];
                            ESM.heroToAttack = performList[0].attackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                    
                }

                if (performList[0].type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.enemyToAttack = performList[0].attackerTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }

                battleStates = PerformAction.PERFORMACTION;
                break;

            case (PerformAction.PERFORMACTION):
                //idle
                break;

            case (PerformAction.CHECKALIVE):
                if (herosInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                } else if (enemiesInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                } else
                {
                    this.ClearAttackPanel();
                    this.heroInput = HeroUI.ACTIVATE;
                }
                break;

            case (PerformAction.WIN):
                for (int i = 0; i < herosInBattle.Count; i++)
                {
                    herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }
                StartCoroutine(CompleteFightWin());
                GameManager.Instance.enemyToBattle.Clear();
                GameManager.Instance.gameState = GameManager.GameState.WAIT;                
                break;

            case (PerformAction.LOSE):
                for (int i = 0; i < enemiesInBattle.Count; i++)
                {
                    enemiesInBattle[i].GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.WAITING;
                }
                StartCoroutine(CompleteFightLose());
                GameManager.Instance.enemyToBattle.Clear();
                GameManager.Instance.gameState = GameManager.GameState.WAIT;
                break;
        }

        switch (heroInput)
        {
            case (HeroUI.ACTIVATE):
                if (herosToManage.Count > 0)
                {
                    herosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroChoice = new HandleTurn();

                    attackPanel.SetActive(true);

                    this.CreateAttackButtons();

                    heroInput = HeroUI.WAITING;
                }
                break;

            case (HeroUI.WAITING):
                //idle
                break;

            case (HeroUI.DONE):
                HeroInputDone();
                break;
        }
    }

    public void CollectActions(HandleTurn input)
    {
        performList.Add(input);
    }

    public void EnemyButtons()
    {
        //clean up
        foreach (GameObject enemyBtn in enemyButtons)
        {
            Destroy(enemyBtn);
        }
        enemyButtons.Clear();

        //create button
        foreach(GameObject enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = cur_enemy.enemy.characterName;

            button.enemyPrefab = enemy;

            newButton.transform.SetParent(this.spacer, false);
            enemyButtons.Add(newButton);
        }
    }

    public void Input1() //attack button
    {
        heroChoice.attacker = herosToManage[0].name;
        heroChoice.attackGameObject = herosToManage[0];
        heroChoice.type = "Hero";
        heroChoice.choosenAttack = herosToManage[0].GetComponent<HeroStateMachine>().hero.attackTypeList[0];
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void Input2(GameObject enemyChoosen) //enemy select
    {
        heroChoice.attackerTarget = enemyChoosen;
        heroInput = HeroUI.DONE;
    }

    public void HeroInputDone()
    {
        performList.Add(heroChoice);
        this.ClearAttackPanel();

        herosToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        herosToManage.RemoveAt(0);
        heroInput = HeroUI.ACTIVATE;
    }

    public void ClearAttackPanel()
    {
        enemySelectPanel.SetActive(false);
        attackPanel.SetActive(false);
        magicPanel.SetActive(false);

        foreach (GameObject atkBtn in atkButtons)
        {
            Destroy(atkBtn);
        }
        atkButtons.Clear();
    }

    public void CreateAttackButtons()
    {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        TMP_Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        attackButtonText.text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        attackButton.transform.SetParent(actionSpacer, false);
        atkButtons.Add(attackButton);

        GameObject magicAtkButton = Instantiate(actionButton) as GameObject;
        TMP_Text magicAtkButtonText = magicAtkButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
        magicAtkButtonText.text = "Magic Attack";
        magicAtkButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        magicAtkButton.transform.SetParent(actionSpacer, false);
        atkButtons.Add(magicAtkButton);

        if (herosToManage[0].GetComponent<HeroStateMachine>().hero.magicAtk.Count > 0)
        {
            foreach (BaseAttack magicAttack in herosToManage[0].GetComponent<HeroStateMachine>().hero.magicAtk)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                TMP_Text magicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<TMP_Text>();
                magicButtonText.text = magicAttack.attackName;

                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.magicAttackToPerform = magicAttack;
                MagicButton.transform.SetParent(magicSpacer, false);
                atkButtons.Add(MagicButton);
            }
        } else
        {
            magicAtkButton.GetComponent<Button>().interactable = false;
        }
    }

    public void Input4(BaseAttack choosenMagic) // choosen magic attack
    {
        heroChoice.attacker = herosToManage[0].name;
        heroChoice.attackGameObject = herosToManage[0];
        heroChoice.type = "Hero";

        heroChoice.choosenAttack = choosenMagic;
        magicPanel.SetActive(false);
        enemySelectPanel.SetActive(true); 
    }

    public void Input3() //switching to magic panel
    {
        attackPanel.SetActive(false);
        magicPanel.SetActive(true);
    }

    IEnumerator CompleteFightWin()
    {
        yield return new WaitForSeconds(3.5f);
        winPanel.SetActive(true);
    }

    IEnumerator CompleteFightLose()
    {
        yield return new WaitForSeconds(3.5f);
        losePanel.SetActive(true);  
    }
}
