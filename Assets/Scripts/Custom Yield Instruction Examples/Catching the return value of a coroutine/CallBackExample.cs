﻿using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Callback is a an elegant way for quite a lot of problems, however it hurts readibility a little bit and 
/// more importantly delegates introduce one more indirection which might make them slower then calling a
/// regular method, however first measure then optimize. Method call optimalization is a micro optimalization
/// Solution: Dedicated coroutines for method calls
/// </summary>
public class CallBackExample : MonoBehaviour
{
    /// <summary>
    /// Tick this in the editor to simulate a server request to get the cubes actual color
    /// </summary>
    [SerializeField]
    bool GetColor = false;

    void Update()
    {
        if (GetColor)
        {
            StartCoroutine(CGetCubeColorFromServer(ChangeGameObjectColor));
            GetColor = false;
        }
    }

    // Changes the GameObjects color
    void ChangeGameObjectColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    #region Coroutines

    // The coroutine which handles the return value, in a non blocking way
    IEnumerator CGetCubeColorFromServer(Action<Color> callBackMethod)
    {
        float rand = Random.Range(0.0f, 1.0f);
        var colorTask = Task.Run(() => GetColorFromServer(rand));
        yield return new WaitUntil(() => colorTask.IsCompleted);

        // Null conditional operator => ?. only evaluates the method call if the left hand not evaluates to null
        callBackMethod?.Invoke(colorTask.Result);
    }

    #endregion

    #region Server Queries

    // The mocked server call with 3 seconds simulated waiting
    Task<Color> GetColorFromServer(float rand)
    {
        Thread.Sleep(3000);
        return Task.FromResult(Color.Lerp(Color.red, Color.blue, rand));
    }

    #endregion
}