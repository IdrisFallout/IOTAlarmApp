using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading;
using UnityEngine.Networking;

public class AlarmActivity : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    
    private Thread syncThread;
    private bool isSyncing = false;
    System.Diagnostics.Stopwatch stopwatch;

    private void Update()
    {
        textMeshPro.text = System.DateTime.Now.ToString("hh:mm:ss");
        CheckSyncStatus();
    }

    
    // run this method in a separate thread
    public void SyncToCloud()
    {
        Debug.Log("Syncing to cloud");
    }

    public void SendToCloud()
    {
        if (!isSyncing)
        {
            StartSyncThread();
        }
    }
    
    private void StartSyncThread()
    {
        // Create and start a new thread for the SyncToCloud function
        syncThread = new Thread(SyncToCloud);
        syncThread.Start();
        stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        isSyncing = true;
    }

    private void OnDestroy()
    {
        StopSyncThread();
    }
    
    private void StopSyncThread()
    {
        // Stop the syncThread if it's running
        if (syncThread != null && syncThread.IsAlive)
        {
            syncThread.Abort();
        }
    }

    public void CheckSyncStatus()
    {
        // Check if the thread is still running
        if (isSyncing && syncThread != null && !syncThread.IsAlive)
        {
            stopwatch.Stop();
            // Thread has finished, perform any necessary operations here
            Debug.Log("Syncing to cloud thread has finished. It took " + stopwatch.ElapsedMilliseconds + " milliseconds");
            isSyncing = false;
        }
    }
}
