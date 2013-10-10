///-----------------------------------------------------------------------------------------------
/// author Ivan Murashko iclickable@gmail.com
///-----------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System;

public class DebugTest : MonoBehaviour
{
    void Start()
    {
        Test1();
    }

    void Update()
    {
        //Test1();
    }

    private void Test1()
    {
        Action action = CallAll;
        action();
    }

    private void CallAll()
    {
        LogMethod();
        WarningMethod();
        ErrorMethod();
        CheckMethod();
    }

    private void ErrorMethod()
    {
        Debug.LogError("Error");
    }

    private void WarningMethod()
    {
        Debug.LogWarning("Warning");
    }

    private void LogMethod()
    {
        Debug.Log("Log");
    }

    private void CheckMethod()
    {
        Debug.Check(false, "There is Check method");
    }
}