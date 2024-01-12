using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public BattleStateMachine BSM;
    public BaseEnemy enemy;
    public BanditAnimCtrl enemyAnim;
    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEATH
    }

    public TurnState currentState;

    private float curCoolDown = 0f;
    private float maxCoolDown = 10f;

    private Vector3 startPosition;
    [SerializeField] private GameObject selector;

    private bool actionStarted = false;
    public GameObject heroToAttack;
    private float animSpeed = 10f;

    private bool alive = true;
    void Start()
    {
        enemyAnim = GetComponent<BanditAnimCtrl>();
        selector = transform.Find("Selector").gameObject;
        selector.SetActive(false);

        this.currentState = TurnState.PROCESSING;
        this.BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = this.transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                this.UpgradeProgressBar();
                break;

            case (TurnState.CHOOSEACTION):
                this.ChooseAction();
                this.currentState = TurnState.WAITING;
                break;

            case (TurnState.WAITING):

                break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEATH):
                if (!alive) return;
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadEnemy";
                    //not attackable by enemy
                    BSM.enemiesInBattle.Remove(this.gameObject);
                    //deactive selector
                    selector.SetActive(false);
                    //remove all heroInput in performList
                    if (BSM.enemiesInBattle.Count > 0 )
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if (BSM.performList[i].attackGameObject == this.gameObject)
                            {
                                BSM.performList.Remove(BSM.performList[i]);
                            } else if (BSM.performList[i].attackerTarget == this.gameObject)
                            {
                                BSM.performList[i].attackerTarget = BSM.enemiesInBattle[Random.Range(0, BSM.enemiesInBattle.Count)];
                            }
                        }
                    }
                   
                    //play Death Animation
                    this.enemyAnim.SetAnimDeath();
                    //Invoke("SetDeactive", 1.5f);
                    //reset enemy button
                    BSM.EnemyButtons();
                    //check state
                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
                    alive = false;
                }
                break;
        }
    }

    public void UpgradeProgressBar()
    {
        curCoolDown += Time.deltaTime;

        if (this.curCoolDown >= this.maxCoolDown)
        {
            this.currentState = TurnState.CHOOSEACTION;
        }
    }

    public void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.attacker = this.enemy.characterName;
        myAttack.type = "Enemy";
        myAttack.attackGameObject = this.gameObject;
        myAttack.attackerTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)];//BSM.herosInBattle[0];
        myAttack.choosenAttack = enemy.attackTypeList[Random.Range(0, enemy.attackTypeList.Count)];
        
        BSM.CollectActions(myAttack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 heroPosition = new Vector3 (heroToAttack.transform.position.x + 1.5f,
                                            heroToAttack.transform.position.y, heroToAttack.transform.position.z);
        while (this.MoveTowardsTarget(heroPosition))
        {
            enemyAnim.SetAnimRun();
            yield return null;
        }

        //wait abit
        enemyAnim.SetAnimIdle();
        yield return new WaitForSeconds(0.5f);
        //do damage
        enemyAnim.SetAnimAttack();
        yield return new WaitForSeconds(0.5f);
        DoDamage();
        //animate back to start position
        Vector3 firstPosition = this.startPosition;
        while (this.MoveTowardsStart(firstPosition))
        {
            enemyAnim.SetAnimRun();
            yield return null;
        }

        //remove this performer from the list in BSM
        BSM.performList.RemoveAt(0);
        //reset BSM -> WAIT
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        enemyAnim.SetAnimIdle();
        //reset this enemy state
        curCoolDown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardsTarget(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void DoDamage()
    {
        float calc_damage = enemy.curATK + BSM.performList[0].choosenAttack.attackDamage;
        heroToAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
    }

    public void TakeDamage(float damage)
    {
        enemy.curHP -= damage;
        if (enemy.curHP <= 0f)
        {
            enemy.curHP = 0f;
            this.currentState = TurnState.DEATH;
        }
    }
}
