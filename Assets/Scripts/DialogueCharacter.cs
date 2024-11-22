using System.Collections.Generic;
using UnityEngine;

public class DialogueCharacter : MonoBehaviour
{
    public string characterName = "";
    public Sprite characterPortrait;
    public DialogueContainer dialogue;
    [Header("Other")]
    public bool isPlayerNearby = false;
    public GameObject useTip;

    void Update()
    {
        if(useTip != null) useTip.SetActive(isPlayerNearby);
    }

    public void StartDialogue()
    {
        if (DialoguePlayer.instance.isPlayingDialogue) return;
        DialoguePlayer p = DialoguePlayer.instance;
        p.characterName.text = characterName;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        // Показ портрета
        /*if (characterPortrait != null)
        {
            p.characterPortrait.color = Color.white;
            p.characterPortrait.sprite = characterPortrait;
        }
        else
        {
            p.characterPortrait.color = Color.clear;
        }*/

        NodeLinkData firstLink = null;
        for (int i = 0; i < dialogue.nodeLinks.Count; i++)
        {
            if (dialogue.nodeLinks[i].portName == "Next")
            {
                firstLink = dialogue.nodeLinks[i];
                break;
            }
        }
        if (firstLink == null) return;
        DialogueNodeData firstReplic = null;
        for (int i = 0; i < dialogue.dialogueNodeDatas.Count; i++)
        {
            if (dialogue.dialogueNodeDatas[i].GUID == firstLink.targetNodeGUID)
            {
                firstReplic = dialogue.dialogueNodeDatas[i];
                break;
            }
        }

        p.isPlayingDialogue = true;
        SetReplicToPlayer(firstReplic);
    }

    void SetReplicToPlayer(DialogueNodeData replic_)
    {
        DialoguePlayer p = DialoguePlayer.instance;

        p.characterReplic.text = replic_.dialogueText;

        // Ищем ответы, которые принадлежат реплике
        List<string> answers = new List<string>();
        for (int i = 0; i < dialogue.nodeLinks.Count; i++)
        {
            if (dialogue.nodeLinks[i].baseNodeGUID == replic_.GUID)
            {
                answers.Add(dialogue.nodeLinks[i].portName);
            }
        }

        p.ReloadAnswersInDialogue(answers.ToArray(), this);
    }

    public void Answer(string answer_)
    {
        // Ищем ответ, который дал игрок
        NodeLinkData answer = null;
        for (int i = 0; i < dialogue.nodeLinks.Count; i++)
        {
            if (dialogue.nodeLinks[i].portName == answer_)
            {
                answer = dialogue.nodeLinks[i];
                break;
            }
        }

        if (answer == null) // Если не найден - выходим из функции
        {
            Debug.Log("answer == null");
            return;
        }

        // Ищем следующую реплику после ответа
        DialogueNodeData nextReplic = null;
        for (int i = 0; i < dialogue.dialogueNodeDatas.Count; i++)
        {
            if (dialogue.dialogueNodeDatas[i].GUID == answer.targetNodeGUID)
            {
                nextReplic = dialogue.dialogueNodeDatas[i];
                break;
            }
        }

        if (nextReplic != null)
        {
            if (nextReplic.dialogueText != "End")
                SetReplicToPlayer(nextReplic);
            else
            {
                DialoguePlayer.instance.isPlayingDialogue = false;

                DialoguePlayer.instance.plMove.isBlockingLook = false;
                DialoguePlayer.instance.plMove.isBlockingMove = false;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerNearby = true;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerNearby = false;
        }
    }
}
