using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCommunication : MonoBehaviour
{
    private static GameCommunication _instance;
    public static GameCommunication Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The GameCommunication is null");
            }
            return _instance;
        }
    }

    private async void OnDestroy()
    {
        if (connection.IsConnectionOpen())
        {
            await connection.Disconnect();
        }
    }

    public ClientConnection connection;

    private void Awake()
    {
        _instance = this;
    }
}
