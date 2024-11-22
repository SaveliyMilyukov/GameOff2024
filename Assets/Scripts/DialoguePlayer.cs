using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialoguePlayer : MonoBehaviour
{
    public PlayerMovement plMove;
    public static DialoguePlayer instance;

    public bool isPlayingDialogue = false;
    [Header("Showing")]
    public GameObject dialoguePanel;
    public Text characterName;
    public Text characterReplic;
    public DialogueLinkBtn answerButtonPrefab;
    public List<DialogueLinkBtn> nowAnswers;
    public Transform answersParent;

    private void Start()
    {
        instance = this;

        if (plMove == null) plMove = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        dialoguePanel.SetActive(isPlayingDialogue);

        if(isPlayingDialogue)
        {
            plMove.isBlockingLook = true;
            plMove.isBlockingMove = true;
        }
    }

    public void ReloadAnswersInDialogue(string[] answers_, DialogueCharacter character_)
    {
        for (int i = 0; i < nowAnswers.Count; i++)
        {
            Destroy(nowAnswers[i].gameObject);
        }
        nowAnswers.Clear();

        for (int i = 0; i < answers_.Length; i++)
        {
            DialogueLinkBtn button = Instantiate(answerButtonPrefab, answersParent).GetComponent<DialogueLinkBtn>();

            button.transform.GetChild(0).GetComponent<Text>().text = answers_[i];
            button.answerText = answers_[i];
            button.character = character_;

            nowAnswers.Add(button);
        }
    }
}
