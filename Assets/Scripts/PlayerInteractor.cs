using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    private DialoguePlayer dialoguePlayer;
    private PlayerMovement playerMovement;

    [Header("Ray")]
    [SerializeField] private Transform rayPlace;
    [SerializeField] private float interactDistance = 2.5f;
    [Header("UI")]
    [SerializeField] private GameObject interactCursor;


    private bool haveInteractbleObject = false;


    private void Start()
    {
        if (dialoguePlayer == null) dialoguePlayer = GetComponent<DialoguePlayer>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        // ���� ���� ��� ������ ������� ��������������
        RaycastHit hitInfo;
        Physics.Raycast(rayPlace.position, rayPlace.forward, out hitInfo, interactDistance);
        if (hitInfo.collider != null) // ���� ��� ����� ����-��
        {
            if (!dialoguePlayer.isPlayingDialogue) // ���� ����� �� � �������
            {
                if (hitInfo.collider.gameObject.GetComponent<DialogueCharacter>()) // ���� ��� ����� � ����������� ���������
                {
                    haveInteractbleObject = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueCharacter dc = hitInfo.collider.gameObject.GetComponent<DialogueCharacter>();
                        dc.StartDialogue();
                    }
                }
                else if (hitInfo.collider.gameObject.GetComponent<SteamSystemPart>()) // ���� ��� ����� � ����� ������� �����
                {
                    haveInteractbleObject = true;

                    SteamSystemPart pipe = hitInfo.collider.gameObject.GetComponent<SteamSystemPart>();
                    pipe.outlineTimer = 0.1f;
                    if (Input.GetKeyDown(KeyCode.R)) pipe.RotateRight();
                    else if (Input.GetKeyDown(KeyCode.F)) pipe.RotateLeft();
                    else if (Input.GetKeyDown(KeyCode.E)) pipe.StartSteamSystem();
                }
                else haveInteractbleObject = false;
            }
            else haveInteractbleObject = false;
        }
        else haveInteractbleObject = false;

        // ����������� ������
        interactCursor.SetActive(haveInteractbleObject);
    }
}
