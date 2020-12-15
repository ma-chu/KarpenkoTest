using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

using System;
using UnityEngine.Events;
// Логика юнита
public class Unit : MonoBehaviour
{
    // Базовые характеристики юнита 
    [SerializeField]
    private float damageBaseMin;                        // минимальный базовый урон
    [SerializeField]
    private float damageBaseMax;                        // максимальный базовый урон
    [SerializeField]
    private float attackRange;                          // дальность атаки > корня из 2, чтоб бить по диагонали
    [SerializeField]
    private float attackTime;                           // время атаки
    [SerializeField]
    private float movementSpeed;                        // скорость перемещения
    [SerializeField]
    private float startingHealth;                       // начальное здоровье
    public float DamageBaseMin
    {
        get { return damageBaseMin; }
        set { damageBaseMin=value; }
    }
    public float DamageBaseMax
    {
        get { return damageBaseMax; }
        set { damageBaseMax = value; }
    }
    public float AttackRange
    {
        get { return attackRange; }
        set { attackRange = value; }
    }
    public float AttackTime
    {
        get { return attackTime; }
        set { attackTime = value; }
    }
    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = value; }
    }
    public float StartingHealth
    {
        get { return startingHealth; }
        set { startingHealth = value; }
    }

    public float bulletVelocity = 10f;


    private UnitHP unitHP;
    private UnitMovement unitMovement;
    private UnitAttack unitAttack;
    private GameManager gameManager;

    private Transform unitTransform;
    //private Animator anim;

    public bool amINotZombie;             // true - человек, false - зомби

    [SerializeField]
    private GameObject[] targets;         // Массив возможных целей для атаки
    [SerializeField]
    private GameObject target;            // Цель для атаки
    private Transform targetTransform;
    private Vector3 targetPosition;       // Вектор цели
    private Vector3 toTargetVector;       // Вектор от юнита до цели

    [SerializeField]
    private GridPos destination;          // Конечный узел сетки
    [SerializeField]
    private GridPos currentNode;          // Текущий узел
    private Vector3 currentNodeVector;    // Текущий узел в векторе
    [SerializeField]
    private GridPos nextNode;             // Следующий узел (к которому двигаемся сейчас)
    private Vector3 nextNodeVector;       // Следующий узел в векторе
    [SerializeField]
    private List<GridPos> resultPathList; // Путь до цели  

    [SerializeField]
    private bool movingToNode;            // идет перемещение на соседний узел - просьба не беспокоить
    [SerializeField]
    private bool attacking;               // дерусь - просьба не беспокоить

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        unitHP = GetComponent("UnitHP") as UnitHP;
        unitMovement = GetComponent("UnitMovement") as UnitMovement;
        unitAttack = GetComponent("UnitAttack") as UnitAttack;
        unitTransform = GetComponent<Transform>();

        //anim = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        UnitHP.IAmDead += OnTargetDead;
    }
    private void OnDisable()
    {
        UnitHP.IAmDead -= OnTargetDead;
    }
    public void OnTargetDead(System.Object sender, EventArgs e)
    {
        GameObject go = sender as GameObject;
        if (target == go)
        { 
            Retarget();
            Debug.Log(this.name + ": " + go.name + " Event Handled"); 
        }
    }

    private void Start()
    {
        currentNode = Vector3ToGridPos(unitTransform.position);
        currentNodeVector = unitTransform.position;

        unitHP.SetStartHealth(StartingHealth);

        if (amINotZombie) targets = gameManager.zombies;
        else targets = gameManager.men;

        Targeting();
    }

    private void Update()
    {
        if (!movingToNode) Desicion();
    }

    private void Desicion()             // Принятие решения: если возможно - атака, иначе - поиск цели и перемещение
    {
        if (target == null) Targeting();
        else
        {
            targetPosition = targetTransform.position;
            toTargetVector = targetPosition - unitTransform.position;
        }

        attacking = (toTargetVector.magnitude < AttackRange);
        if (attacking)
        {
            if (unitAttack.Attack(target, toTargetVector))
            {
                targets[System.Array.IndexOf(targets, target)] = null;
                Destroy(target, toTargetVector.magnitude / bulletVelocity);
                Retarget();
            }
        }
        else Targeting();
    }

    public virtual void Targeting()                                 // Актуализация цели атаки - ближайшая доступная
    {
        foreach (GameObject item in targets)
        {
            if (item != null)
            {
                if ((target == null) || (item.GetComponent<Transform>().position - unitTransform.position).magnitude < toTargetVector.magnitude)
                {
                    target = item;
                    targetTransform = target.GetComponent<Transform>();
                }
            }
        }

        if (target != null)
        {
            destination = Vector3ToGridPos(targetPosition);

            JumpPointParam jpParam = new JumpPointParam(gameManager.searchGrid, EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.OnlyWhenNoObstacles);
            jpParam.Reset(currentNode, destination);
            resultPathList = JumpPointFinder.FindPath(jpParam);

            if (resultPathList.Count > 1) nextNode = resultPathList[1];
            if (nextNode == null) return;
            nextNodeVector = GridPosToVector3(nextNode);            

            if ((currentNode != nextNode) && gameManager.searchGrid.IsWalkableAt(nextNode))   
            {
                gameManager.searchGrid.SetWalkableAt(nextNode, false);                         // захватить следующий узел
                gameManager.searchGrid.SetWalkableAt(currentNode, true);                       // отпустить текущий узел
                movingToNode = true;
            }
        }
        else                                                // Целей для атаки не осталось
        {
            attacking = false;
            gameManager.CheckForGameOver();
        }
    }

    private void Retarget()                                 // Противник убит - перенацелиться
    {
        target = null;
        attacking = false;
        Targeting();
    }

    private void FixedUpdate()
    {
        if (movingToNode)
        {
            Vector3 currentToNextNodeVector = nextNodeVector - unitTransform.position;
            if (currentToNextNodeVector.magnitude < 0.05f)                                      // currentNode reached
            {
                unitTransform.position = nextNodeVector;
                currentNode = nextNode;
                currentNodeVector = nextNodeVector;
                movingToNode = false;
            }
            else
            {
                unitMovement.Move(currentToNextNodeVector);
            }
            //Animating(lh, lv);
        }
    }

    public Vector3 GridPosToVector3(GridPos gridPos)       // Надо бы сделать метод расширения
    {
        return new Vector3(gridPos.x, 0f, gridPos.y);
    }
    public GridPos Vector3ToGridPos(Vector3 vector3)       // Надо бы сделать метод расширения
    {
        return new GridPos((int)vector3.x, (int)vector3.z);
    }
}
