using UnityEngine;
using System.Collections.Generic;

public class SteamSystemManager : MonoBehaviour
{
    public SteamSystemPart start;
    public SteamSystemPart end;
    public List<SteamSystemPart> passedPipes;
    public bool pathIsExists;
    public bool haveEmptyExit = false; // Есть ли пустой выход (то есть есть ли труба, которая ведет пар в никуда)
    public int dampersPassed = 0;
    public int maxDampersCanBeOpened = 1;
    [Space(5)]
    public Animator finishAnimator;

    public void StartSteam()
    {
        start.FindNeigboursForWholeSystem();
        Invoke(nameof(CheckSteamPath), 0.5f);
    }

    public void CheckSteamPath()
    {
        Debug.Log("Checking steam path...");

        haveEmptyExit = false;
        pathIsExists = false;
        ClearPassedPipes();
        dampersPassed = 0;
        start.LetOffSteam(false);

        Invoke(nameof(ShowFinish), 0.75f);
        //Invoke(nameof(ClearPassedPipes), 1f);
    }

    public void ShowFinish()
    {
        bool isRight = false;
        if (dampersPassed <= maxDampersCanBeOpened && pathIsExists && !haveEmptyExit) isRight = true;

        finishAnimator.SetBool("HavePath", pathIsExists);
        finishAnimator.SetBool("IsRight", isRight);
        finishAnimator.SetTrigger("Try");
    }

    public void ClearPassedPipes()
    {
        passedPipes.Clear();
    }
}
