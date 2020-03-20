﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ExampleThreadedCoroutine : ThreadedCoroutine
{
    GameObject Fred;

    protected override IEnumerator WorkOnUnityThread()
    {
        // Doing work on Unitys Main Thread
        Debug.Log("<color=yellow>WorkOnUnityThread method Thread ID:</color> " + Thread.CurrentThread.ManagedThreadId);

        // We then request a gameObject called "Fred" (its a cube) and multiply its position value on the Unity Thread
        Fred = GameObject.Find("Fred");
        Fred.transform.position *= 2;
        EditorApplication.ExecuteMenuItem("Edit/Play");
        // Yield work to the coroutine thread
        yield return RequestThreadedCoroutineThread();

        // Now we change the gameobject color to blue indicating that we are finished with the process
        Fred.GetComponent<Renderer>().material.color = Color.green;

        yield return RequestThreadedCoroutineThread();
    }

    protected override void WorkOnCoroutineThread(CancellationToken cancellationToken)
    {
        Debug.Log("<color=#00FF00>WorkOnCoroutineThread method Thread ID:</color> " + Thread.CurrentThread.ManagedThreadId);
        // Emulating some hardwork on the Coroutines thread at start.
        Thread.Sleep(3500);

        Debug.Log("<color=#00FF00>Requesting main thread...</color> " + Thread.CurrentThread.ManagedThreadId);
        // Use request main thread to pause execution of the current thread and yield control to Unitys Main Thread
        RequestUnitysMainThread(cancellationToken);

        // After that another heavy workload takes place
        Thread.Sleep(2000);

        // Use request main thread to pause execution of the current thread and yield control to Unitys Main Thread
        RequestUnitysMainThread(cancellationToken);

        Debug.Log("<color=#00FF00>Finished execution of thread</color>" + Thread.CurrentThread.ManagedThreadId);
        Finish();
    }
}
