using UnityEngine;

public class SteamSystemPart : MonoBehaviour
{
    public SteamSystemManager mySystem;
    [Header("Headers")]
    public bool isSteamStart = false; // ������ �� ���� ����
    public bool isSteamEnd = false; // ����� �� ���� ����
    public bool isDamper = false; // �������� ��
    [Header("Directions")] // ������ ���� � �������
    public bool dForward = false; // z + 1
    public bool dBackward = false; // z - 1
    public bool dRight = false; // x + 1
    public bool dLeft = false; // x - 1
    [Header("Neigbours")]
    [Space(5)] // �������� ������� ����� (���������� �� �������� ����� �����)
    public SteamSystemPart nForward; 
    public SteamSystemPart nBackward; 
    public SteamSystemPart nRight; 
    public SteamSystemPart nLeft; 
    [Space(5)]
    [Header("Model")]
    public Transform modelTransform;
    [Header("Interact")]
    public float outlineTimer = 0; // ����� �� ���������� ���������
    public GameObject outline; // ���������

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

    public void RotateRight() // ��������� �� ������� �������
    {
        if (isDamper || isSteamStart || isSteamEnd) return;

        bool f, b, r, l;
        r = dForward; // �������� ������� �������
        b = dRight; // ������ ������� �����
        l = dBackward; // ������ ������� �����
        f = dLeft; // ����� ������� ������

        // ���������� ����� �������
        dForward = f;
        dBackward = b;
        dRight = r;
        dLeft = l;

        // ������� ������
        modelTransform.Rotate(Vector3.up, 90);
    }

    public void RotateLeft() // ��������� ������ ������� �������
    {
        if (isDamper || isSteamStart || isSteamEnd) return;

        bool f, b, r, l;
        l = dForward; // �������� ������� �����
        b = dLeft; // ����� ������� ����
        r = dBackward; // ������ ������� �������
        f = dRight; // ������ ������� �����

        // ���������� ����� �������
        dForward = f;
        dBackward = b;
        dRight = r;
        dLeft = l;

        // ������� ������
        modelTransform.Rotate(Vector3.up, -90);
    }

    // ������� ��� �� �������� ������
    public void LetOffSteam(bool checkInList_ = true)
    {
        if (checkInList_ && isSteamStart) return;

        if(checkInList_) // ���������, ��� �� � ������ ���������� ����
        {
            if (mySystem.passedPipes.Contains(this)) return;
        }
        if(mySystem == null) { Debug.LogError(gameObject.name + " {pipe}  haven't steam system!"); return; }
        mySystem.passedPipes.Add(this); // ��������� � ������ ���������� ����

        if (isDamper) mySystem.dampersPassed++; // ���� ����� �������� ��������� - ������� ���-�� ��������

        if(isSteamEnd) // ���� ��� ����� ���� - �� �������� ��, ��� ���� ����������
        {
            mySystem.pathIsExists = true;
            Debug.Log("<color=#FFFC02>STEAM PATH IS EXISTS!</color>");
            return;
        }

        if (dForward && nForward != null) 
            if(nForward.nBackward == this && nForward.dBackward) nForward?.LetOffSteam();

        if (dBackward && nBackward != null) 
            if (nBackward.nForward == this && nBackward.dForward) nBackward?.LetOffSteam();

        if (dRight && nRight != null) 
            if (nRight.nLeft == this && nRight.dLeft) nRight?.LetOffSteam();

        if (dLeft && nLeft != null)
            if (nLeft.nRight == this && nLeft.dRight) nLeft?.LetOffSteam();
    }

    // ����� � ���������
    public void OnDrawGizmosSelected()
    {
        // ���������� ���������
        if (isDamper) Gizmos.color = Color.blue;
        if (isSteamStart) Gizmos.color = Color.cyan;
        if (isSteamEnd) Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (dForward) // ���� ���� ����� �����
        {
            if (nForward != null) // ���� ���� �����
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nForward.transform.position);
            }
            else // ��� ������
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
            }
        }

        if (dBackward) // ���� ���� ����� �����
        {
            if (nBackward != null) // ���� ���� �����
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nBackward.transform.position);
            }
            else // ��� ������
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.back);
            }
        }

        if (dRight) // ���� ���� ����� �������
        {
            if (nRight != null) // ���� ���� �����
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nRight.transform.position);
            }
            else // ��� ������
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.right);
            }
        }

        if (dLeft) // ���� ���� ����� ������
        {
            if (nLeft != null) // ���� ���� �����
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nLeft.transform.position);
            }
            else // ��� ������
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.left);
            }
        }

        if(mySystem != null)
        {
            Gizmos.color = Color.yellow - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawLine(transform.position, mySystem.transform.position);
        }
    }
}
