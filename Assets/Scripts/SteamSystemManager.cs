using UnityEngine;
using System.Collections.Generic;

public class SteamSystemManager : MonoBehaviour
{
    public SteamSystemPart start;
    public SteamSystemPart end;
    public List<SteamSystemPart> passedPipes;
    public bool pathIsExists;
    public int dampersPassed = 0;

    private void Start()
    {
        
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            CheckSteamPath();
        }
    }

    public void CheckSteamPath()
    {
        Debug.Log("Checking steam path...");

        pathIsExists = false;
        ClearPassedPipes();
        dampersPassed = 0;
        start.LetOffSteam(false);

        Invoke(nameof(ClearPassedPipes), 1f);
    }

    public void ClearPassedPipes()
    {
        passedPipes.Clear();
    }
}
