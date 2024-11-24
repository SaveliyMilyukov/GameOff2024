using UnityEngine;

public class SteamSystemPart : MonoBehaviour
{
    public SteamSystemManager mySystem;
    [Header("Headers")]
    public bool isSteamStart = false; // Начало ли пути пара
    public bool isSteamEnd = false; // Конец ли пути пара
    public bool isDamper = false; // Заслонка ли
    [Header("Directions")] // Выходы пара в системе
    public bool dForward = false; // z + 1
    public bool dBackward = false; // z - 1
    public bool dRight = false; // x + 1
    public bool dLeft = false; // x - 1
    [Header("Neigbours")]
    [Space(5)] // Соседние участки трубы (Независимо от поворота самой трубы)
    public SteamSystemPart nForward; 
    public SteamSystemPart nBackward; 
    public SteamSystemPart nRight; 
    public SteamSystemPart nLeft; 
    [Space(5)]
    [Header("Model")]
    public Transform modelTransform;
    [Header("Interact")]
    public float outlineTimer = 0; // Время до выключения подсветки
    public GameObject outline; // Подсветка

    private void Start()
    {
        
    }


    private void Update()
    {
        if(outlineTimer > 0)
        {
            outlineTimer -= Time.deltaTime;
        }
        outline.SetActive(outlineTimer > 0);
    }

    public void RotateRight() // Повернуть ПО часовой стрелке
    {
        if (isDamper || isSteamStart || isSteamEnd) return;

        bool f, b, r, l;
        r = dForward; // Переднюю сторону направо
        b = dRight; // Правую сторону назад
        l = dBackward; // Заднюю сторону влево
        f = dLeft; // Левую сторону вперед

        // Присвоение новых позиций
        dForward = f;
        dBackward = b;
        dRight = r;
        dLeft = l;

        // Поворот модели
        modelTransform.Rotate(Vector3.up, 90);
    }

    public void RotateLeft() // Повернуть ПРОТИВ часовой стрелки
    {
        if (isDamper || isSteamStart || isSteamEnd) return;

        bool f, b, r, l;
        l = dForward; // Переднюю сторону влево
        b = dLeft; // Левую сторону вниз
        r = dBackward; // Нижнюю сторону направо
        f = dRight; // Правую сторону вперёд

        // Присвоение новых позиций
        dForward = f;
        dBackward = b;
        dRight = r;
        dLeft = l;

        // Поворот модели
        modelTransform.Rotate(Vector3.up, -90);
    }

    public void StartSteamSystem()
    {
        if (!isSteamStart) return;
        if(mySystem == null) { Debug.LogError(gameObject.name + " pipe haven't system!"); return; }

        mySystem.StartSteam();
    }

    // Пустить пар по соседним трубам
    public void LetOffSteam(bool checkInList_ = true)
    {
        if (checkInList_ && isSteamStart) return;

        if(checkInList_) // Проверяем, нет ли в списке пройденный труб
        {
            if (mySystem.passedPipes.Contains(this)) return;
        }
        if(mySystem == null) { Debug.LogError(gameObject.name + " {pipe}  haven't steam system!"); return; }
        mySystem.passedPipes.Add(this); // Добавляем в список пройденных труб

        if (isDamper) mySystem.dampersPassed++; // Если труба является заслонкой - плюсуем кол-во заслонок

        if(isSteamEnd) // Если это конец пути - то отмечаем то, что путь существует
        {
            mySystem.pathIsExists = true;
            Debug.Log("<color=#FFFC02>STEAM PATH IS EXISTS!</color>");
            return;
        }

        // Проверка выходов в никуда
        if(!isSteamStart && !isSteamEnd)
        {
            if(dForward && nForward == null ||
                dForward && !nForward.dBackward)
            {
                mySystem.haveEmptyExit = true;
                Debug.DrawLine(transform.position, transform.position + Vector3.forward * 3, Color.red, 12f, false); Debug.Log("HAVE EMPTY EXIT!");
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 12f, false);
            }
            if (dBackward && nBackward == null ||
                dBackward && !nBackward.dForward)
            {
                mySystem.haveEmptyExit = true;
                Debug.DrawLine(transform.position, transform.position - Vector3.forward * 3, Color.red, 12f, false); Debug.Log("HAVE EMPTY EXIT!");
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 12f, false);
            }
            if (dRight && nRight == null ||
                dRight && !nRight.dLeft)
            {
                mySystem.haveEmptyExit = true;
                Debug.DrawLine(transform.position, transform.position + Vector3.right* 3, Color.red, 12f, false); Debug.Log("HAVE EMPTY EXIT!");
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 12f, false);
            }
            if (dLeft && nLeft == null||
                dLeft && !nLeft.dRight)
            {
                mySystem.haveEmptyExit = true;
                Debug.DrawLine(transform.position, transform.position - Vector3.left * 3, Color.red, 12f, false); Debug.Log("HAVE EMPTY EXIT!");
                Debug.DrawLine(transform.position, transform.position + Vector3.up * 2, Color.red, 12f, false);
            }
        }

        // Проверка соседей
        if (dForward && nForward != null)
            if (nForward.nBackward == this && nForward.dBackward) 
            { 
                Debug.DrawLine(transform.position, nForward.transform.position, Color.yellow, 7f, false); 
                nForward.LetOffSteam(); 
            }

        if (dBackward && nBackward != null) 
            if (nBackward.nForward == this && nBackward.dForward)
            {
                Debug.DrawLine(transform.position, nBackward.transform.position, Color.yellow, 7f, false);
                nBackward.LetOffSteam();
            }

        if (dRight && nRight != null) 
            if (nRight.nLeft == this && nRight.dLeft)
            {
                Debug.DrawLine(transform.position, nRight.transform.position, Color.yellow, 7f, false);
                nRight.LetOffSteam();
            }

        if (dLeft && nLeft != null)
            if (nLeft.nRight == this && nLeft.dRight)
            {
                Debug.DrawLine(transform.position, nLeft.transform.position, Color.yellow, 7f, false);
                nLeft.LetOffSteam();
            }
    }

    public void FindNeigbours()
    {
        //if (isDamper) return;
        
        SteamSystemPart[] pipes = FindObjectsOfType<SteamSystemPart>();
        //Debug.Log(gameObject.name + " is trying to find neigbours... pipes count: " + pipes.Length);

        int pipesLooked = 0;
        for(int i = 0; i < pipes.Length; i++)
        {
            if (pipes[i] == this) continue;
            if (pipes[i].mySystem != mySystem) continue;
            pipesLooked++;

            if (pipes[i].transform.position.x == transform.position.x &&
               pipes[i].transform.position.z == transform.position.z + 1) // Сосед впереди
            {
                nForward = pipes[i];
                Debug.DrawLine(transform.position, pipes[i].transform.position, Color.magenta, 7f, false);
            }
            else if (pipes[i].transform.position.x == transform.position.x &&
              pipes[i].transform.position.z == transform.position.z - 1) // Сосед сзади
            {
                nBackward = pipes[i];
                Debug.DrawLine(transform.position, pipes[i].transform.position, Color.magenta, 7f, false);
            }
            else if (pipes[i].transform.position.x == transform.position.x + 1 &&
              pipes[i].transform.position.z == transform.position.z) // Сосед справа
            {
                nRight = pipes[i];
                Debug.DrawLine(transform.position, pipes[i].transform.position, Color.magenta, 7f, false);
            }
            else if (pipes[i].transform.position.x == transform.position.x - 1 &&
              pipes[i].transform.position.z == transform.position.z) // Сосед слева
            {
                nLeft = pipes[i];
                Debug.DrawLine(transform.position, pipes[i].transform.position, Color.magenta, 7f, false);
            }
        }
    }

    public void FindNeigboursForWholeSystem()
    {
        SteamSystemPart[] pipes = FindObjectsOfType<SteamSystemPart>();

        for(int i = 0; i < pipes.Length; i++)
        {
            if(pipes[i].mySystem == mySystem)
            {
                pipes[i].FindNeigbours();
            }
        }
    }

    // Вывод в редакторе
    public void OnDrawGizmosSelected()
    {
        // Применение расцветки
        if (isDamper) Gizmos.color = Color.blue;
        if (isSteamStart) Gizmos.color = Color.cyan;
        if (isSteamEnd) Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);

        if (dForward) // Если есть выход вперёд
        {
            if (nForward != null) // Если есть сосед
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nForward.transform.position);
                Gizmos.DrawWireSphere(nForward.transform.position, 0.5f);
            }
            else // Нет соседа
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * 2.5f);
            }
        }

        if (dBackward) // Если есть выход назад
        {
            if (nBackward != null) // Если есть сосед
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nBackward.transform.position);
                Gizmos.DrawWireSphere(nBackward.transform.position, 0.5f);
            }
            else // Нет соседа
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.back * 2.5f);
            }
        }

        if (dRight) // Если есть выход направо
        {
            if (nRight != null) // Если есть сосед
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nRight.transform.position);
                Gizmos.DrawWireSphere(nRight.transform.position, 0.5f);
            }
            else // Нет соседа
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 2.5f);
            }
        }

        if (dLeft) // Если есть выход налево
        {
            if (nLeft != null) // Если есть сосед
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nLeft.transform.position);
                Gizmos.DrawWireSphere(nLeft.transform.position, 0.5f);
            }
            else // Нет соседа
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.left * 2.5f);
            }
        }

        if(mySystem != null)
        {
            Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawLine(transform.position, mySystem.transform.position);
        }
    }
}
