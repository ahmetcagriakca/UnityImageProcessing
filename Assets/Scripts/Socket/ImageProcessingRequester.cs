using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AsyncIO;
using JetBrains.Annotations;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

public class ImageProcessingRequester : BaseCoroutine
{
    protected override IEnumerator Run(string message, Action<string> action)
    {
        yield return null;
        ForceDotNet.Force();
        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");
            client.SendFrame(message);
            string socketMessage = null;
            bool messageReceived = false;
            while (Running)
            {
                messageReceived = client.TryReceiveFrameString(out socketMessage);
                if (messageReceived) break;
            }

            if (messageReceived)
            {
                Debug.Log("Received " + socketMessage);
                action(socketMessage);
            }
        }

        NetMQConfig.Cleanup();
    }
}