using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float notWalkablePart;                          // Доля клеток, недоступных для ходьбы
    [SerializeField]
    private int width;                                      // Ширина поля
    [SerializeField]
    private int height;                                     // Высота поля
    [SerializeField]
    private int numberOfUnits;                              // Количество юнитов в команде
 
    private DatabaseLoader databaseLoader;

    public BaseGrid searchGrid;                             // Сетка поля

    [SerializeField]
    private Transform notWalkableNodePrefab;

    [SerializeField]
    private GameObject closeCombatUnitPrefab;
    [SerializeField]
    private GameObject distantCombatUnitPrefab;

    // массивы юнитов
    public GameObject[] men;
    public GameObject[] zombies;
    [SerializeField]
    private Material zombieMaterial;


    private void OnEnable()                                 
    {
        databaseLoader = GameObject.FindGameObjectWithTag("DatabaseLoader").GetComponent<DatabaseLoader>();
    }
    void Start()
    {   //загрузить параметры из конфигурационного файла
        notWalkablePart = databaseLoader.configurableParams[0].notWalkablePart;
        width = databaseLoader.configurableParams[0].width;
        height = databaseLoader.configurableParams[0].height;
        numberOfUnits = databaseLoader.configurableParams[0].numberOfUnits;

        searchGrid = CreateSearchGrid();

        men = new GameObject [numberOfUnits];
        zombies = new GameObject [numberOfUnits];

        SpawnAllUnits();
    }

    private BaseGrid CreateSearchGrid()                    // Создание сетки поля
    {
        // матрица прходимости
        bool[][] movableMatrix = new bool[width][];
        for (int widthTrav = 0; widthTrav < width; widthTrav++)
        {
            movableMatrix[widthTrav] = new bool[height];
            for (int heightTrav = 0; heightTrav < height; heightTrav++)
            {
                movableMatrix[widthTrav][heightTrav] = Random.Range(0f, 1f) > notWalkablePart;
            }
        }

        // тут изначально стоят юниты
        for (int i = 0; i < numberOfUnits; i++)
        {
            movableMatrix[i][0] = true;
            movableMatrix[width - 1 - i][width - 1] = true;
        }

        // визуализация непроходимых узлов
        for (int widthTrav = 0; widthTrav < width; widthTrav++)
        {
            for (int heightTrav = 0; heightTrav < height; heightTrav++)
            {
                if (!movableMatrix[widthTrav][heightTrav])
                {
                    Transform notWalkableNodeInstance;
                    notWalkableNodeInstance = Instantiate(notWalkableNodePrefab, new Vector3(widthTrav, 0f, heightTrav), transform.rotation.normalized) as Transform;
                }
            }
        }

        return new StaticGrid(width, height, movableMatrix);
    }

    private void SpawnAllUnits()                            // Порождение юнитов: четные - лучники, нечетные - щитники
    {
        for (int i = 1; i <= numberOfUnits; i++)
        {
            bool distantOrClose;
            GameObject closeOrDistantPrefab;
            GameObject instance;
            Unit unit;

            distantOrClose = i % 2 == 0;
            closeOrDistantPrefab = distantOrClose ? distantCombatUnitPrefab : closeCombatUnitPrefab;
            // люди
            instance = Instantiate(closeOrDistantPrefab, new Vector3(i - 1, 0f, 0f), transform.rotation.normalized);
            unit = instance.GetComponent<Unit>();
            unit.amINotZombie = true;
            if (distantOrClose) SetUnitConfigFromFileDistant(unit); else SetUnitConfigFromFileClose(unit);
            instance.name += i.ToString();
            men[i - 1] = instance;
            // зомби
            instance = Instantiate(closeOrDistantPrefab, new Vector3(width - i, 0f, width - 1), transform.rotation.normalized);
            unit = instance.GetComponent<Unit>();
            unit.amINotZombie = false;
            if (distantOrClose) SetUnitConfigFromFileDistant(unit); else SetUnitConfigFromFileClose(unit);
            instance.GetComponentInChildren<MeshRenderer>().material = zombieMaterial;
            instance.name += i.ToString();
            instance.name += " Zombie";
            zombies[i - 1] = instance;
        }
    }

    private void SetUnitConfigFromFileClose(Unit unit)         // Считать базовые характеристики юнита из файла
    {
        unit.DamageBaseMin = databaseLoader.configurableParams[0].closeCombatUnitDamageBaseMin;
        unit.DamageBaseMax = databaseLoader.configurableParams[0].closeCombatUnitDamageBaseMax;
        unit.AttackRange = databaseLoader.configurableParams[0].closeCombatUnitAttackRange;
        unit.AttackTime = databaseLoader.configurableParams[0].closeCombatUnitAttackTime;
        unit.MovementSpeed = databaseLoader.configurableParams[0].closeCombatUnitMovementSpeed;
        unit.StartingHealth = databaseLoader.configurableParams[0].closeCombatUnitStartingHealth;
        unit.bulletVelocity = databaseLoader.configurableParams[0].bulletVelocity;
    }

    private void SetUnitConfigFromFileDistant(Unit unit)         // Считать базовые характеристики юнита из файла
    {
        unit.DamageBaseMin = databaseLoader.configurableParams[0].distantCombatUnitDamageBaseMin;
        unit.DamageBaseMax = databaseLoader.configurableParams[0].distantCombatUnitDamageBaseMax;
        unit.AttackRange = databaseLoader.configurableParams[0].distantCombatUnitAttackRange;
        unit.AttackTime = databaseLoader.configurableParams[0].distantCombatUnitAttackTime;
        unit.MovementSpeed = databaseLoader.configurableParams[0].distantCombatUnitMovementSpeed;
        unit.StartingHealth = databaseLoader.configurableParams[0].distantCombatUnitStartingHealth;
        unit.bulletVelocity = databaseLoader.configurableParams[0].bulletVelocity;
    }

    public void CheckForGameOver()
    {
        int menLives = (from num in men where (num != null) select num).Count();
        int zombieLives = (from num in zombies where (num != null) select num).Count();

        if (menLives == 0)
        {
            if (zombieLives == 0) Debug.Log("DRAW!"); else Debug.Log("Zombies WIN!");
            //Time.timeScale = 0;
        }
        if ((zombieLives == 0)&&(menLives != 0))
        {
            Debug.Log("men WIN!");
            //Time.timeScale = 0;
        }
    }
}

