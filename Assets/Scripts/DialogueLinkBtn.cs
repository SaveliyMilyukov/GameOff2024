using UnityEngine;

public class DialogueLinkBtn : MonoBehaviour
{
    public string answerText;
    public DialogueCharacter character;
    public void Answer()
    {
        character.Answer(answerText);
    }
}
