using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM;
    public BaseHero hero;
    public BanditAnimCtrl heroAnim;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEATH
    }

    public TurnState currentState;

    private float curCoolDown = 0f;
    private float maxCoolDown = 5f;

    public Image progressBar;
    [SerializeField] private GameObject selector;

    public GameObject enemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;

    private bool alive = true;

    //hero panel
    private HeroPanelState heroPanelState;
    public GameObject heroPanel;
    private Transform heroPanelSpacer;
    void Start()
    {
        //find spacer
        heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").Find("HeroPanelSpacer");
        //create panel, fill in info
        CreateHeroPanel();
        
        selector = transform.Find("Selector").gameObject;
        selector.SetActive(false);

        heroAnim = GetComponent<BanditAnimCtrl>();
        startPosition = transform.position;
        curCoolDown = Random.Range(0f, 2.5f);
        this.hero.characterName = this.gameObject.name;
        this.currentState = TurnState.PROCESSING;
        this.BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    void Update()
    {
        switch(currentState)
        {
            case (TurnState.PROCESSING):
                this.UpgradeProgressBar();
                break;

            case (TurnState.ADDTOLIST):
                BSM.herosToManage.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;

            case (TurnState.WAITING):
                //idle / waiting for action
                break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEATH):
                if (!alive) return;
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";
                    //not attackable by enemy
                    BSM.herosInBattle.Remove(this.gameObject);
                    //not managable
                    BSM.herosToManage.Remove(this.gameObject);
                    //deactive selector
                    selector.SetActive(false);
                    //reset GUI
                    BSM.attackPanel.SetActive(false);
                    BSM.enemySelectPanel.SetActive(false);
                    //remove item form performList
                    if (BSM.herosInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if (BSM.performList[i].attackGameObject == this.gameObject)
                            {
                                BSM.performList.Remove(BSM.performList[i]);
                            } else if (BSM.performList[i].attackerTarget == this.gameObject)
                            {
                                BSM.performList[i].attackerTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)];
                            }
                        }
                    }
                
                    //play Death Animation
                    this.heroAnim.SetAnimDeath();
                    //Invoke("SetDeactive", 1.5f);
                    //reset
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
    }

    public void UpgradeProgressBar()
    {
        curCoolDown += Time.deltaTime;

        float calcCoolDown = curCoolDown/ maxCoolDown;
        this.progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCoolDown, 0, 1),
                                                            progressBar.transform.localScale.y,
                                                            progressBar.transform.localScale.z);

        if (this.curCoolDown >= this.maxCoolDown)
        {
            this.currentState = TurnState.ADDTOLIST;
        }
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x - 1.5f,
                                            enemyToAttack.transform.position.y, enemyToAttack.transform.position.z);
        while (this.MoveTowardsTarget(enemyPosition))
        {
            heroAnim.SetAnimRun();
            yield return null;
        }

        //wait abit
        heroAnim.SetAnimIdle();
        yield return new WaitForSeconds(0.5f);
        //do damage
        heroAnim.SetAnimAttack();
        yield return new WaitForSeconds(0.5f);
        this.DoDamage();
        //animate back to start position
        Vector3 firstPosition = this.startPosition;
        while (this.MoveTowardsStart(firstPosition)) 
        {
            heroAnim.SetAnimRun();
            yield return null;
        }

        //remove this performer from the list in BSM
        BSM.performList.RemoveAt(0);
        //reset BSM -> WAIT
        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
            //reset this hero state
            curCoolDown = 0f;
            currentState = TurnState.PROCESSING;
        } else
        {
            currentState = TurnState.WAITING;
        }
        heroAnim.SetAnimIdle();
        //end coroutine
        actionStarted = false;
    }

    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float damage)
    {
        hero.curHP -= damage;
        if (hero.curHP <= 0f)
        {
            hero.curHP = 0f;
            currentState = TurnState.DEATH;
        }
        this.UpdateHeroPanel();
    }

    public void DoDamage()
    {
        float calc_damage = hero.curATK + BSM.performList[0].choosenAttack.attackDamage;
        hero.curMP -= BSM.performList[0].choosenAttack.attackCost;
        enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    public void CreateHeroPanel()
    {
        heroPanel = Instantiate(heroPanel) as GameObject;
        heroPanelState = heroPanel.GetComponent<HeroPanelState>();

        heroPanelState.heroName.text = hero.characterName;
        heroPanelState.heroHP.text = "HP: " + hero.curHP + "/" + hero.baseHP;
        heroPanelState.heroMP.text = "MP: " + hero.curMP + "/" + hero.baseMP;

        progressBar = heroPanelState.progressBar;
        heroPanelState.transform.SetParent(heroPanelSpacer, false);
    }

    public void UpdateHeroPanel()
    {
        heroPanelState.heroHP.text = "HP: " + hero.curHP + "/" + hero.baseHP;
        heroPanelState.heroMP.text = "MP: " + hero.curMP + "/" + hero.baseMP;
    }

    public void SetDeactive()
    {
        this.gameObject.SetActive(false);
    }
}
