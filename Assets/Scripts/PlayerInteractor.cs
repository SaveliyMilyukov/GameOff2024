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

    private void Start()
    {
        if (dialoguePlayer == null) dialoguePlayer = GetComponent<DialoguePlayer>();
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
    }

    
    void Update()
    {
        bool haveInteractbleObject = false;

        RaycastHit hitInfo;
        Physics.Raycast(rayPlace.position, rayPlace.forward, out hitInfo, interactDistance);
        if (hitInfo.collider != null)
        {
            if(!dialoguePlayer.isPlayingDialogue)
            {
                if(hitInfo.collider.gameObject.GetComponent<DialogueCharacter>())
                {
                    haveInteractbleObject = true;
                    if(Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueCharacter dc = hitInfo.collider.gameObject.GetComponent<DialogueCharacter>();
                        dc.StartDialogue();
                    }
                }
            }
        }

        interactCursor.SetActive(haveInteractbleObject);
    }
}
