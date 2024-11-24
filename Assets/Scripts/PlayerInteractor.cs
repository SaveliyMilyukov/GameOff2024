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
        // Пуск луча для поиска объекта взаимодействия
        RaycastHit hitInfo;
        Physics.Raycast(rayPlace.position, rayPlace.forward, out hitInfo, interactDistance);
        if (hitInfo.collider != null) // Если луч попал куда-то
        {
            if (!dialoguePlayer.isPlayingDialogue) // Если игрок не в диалоге
            {
                if (hitInfo.collider.gameObject.GetComponent<DialogueCharacter>()) // Если луч попал в диалогового персонажа
                {
                    haveInteractbleObject = true;
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueCharacter dc = hitInfo.collider.gameObject.GetComponent<DialogueCharacter>();
                        dc.StartDialogue();
                    }
                }
                else if (hitInfo.collider.gameObject.GetComponent<SteamSystemPart>()) // Если луч попал в кусок паровой трубы
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

        // Переключаем курсор
        interactCursor.SetActive(haveInteractbleObject);
    }
}
