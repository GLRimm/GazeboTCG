using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ReactBridgeController : MonoBehaviour
{
    // Singleton instance for easy access
    public static ReactBridgeController Instance { get; private set; }

    // Dictionary to store message handlers by message type
    private Dictionary<string, Action<string>> messageHandlers = new Dictionary<string, Action<string>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Subscribe to specific message types
    public void Subscribe(string messageType, Action<string> handler)
    {
        if (!messageHandlers.ContainsKey(messageType))
        {
            messageHandlers[messageType] = handler;
        }
        else
        {
            messageHandlers[messageType] += handler;
        }
    }

    // Unsubscribe from specific message types
    public void Unsubscribe(string messageType, Action<string> handler)
    {
        if (messageHandlers.ContainsKey(messageType))
        {
            messageHandlers[messageType] -= handler;
            
            // Remove the key if no handlers left
            if (messageHandlers[messageType] == null)
            {
                messageHandlers.Remove(messageType);
            }
        }
    }

    // Entry point for messages from React Native
    public void OnMessageFromReact(string jsonMessage)
    {
        try
        {
            // Parse the incoming message
            // Expected format: {"type": "messageType", "payload": "messageData"}
            var message = JsonUtility.FromJson<BridgeMessage>(jsonMessage);
            
            if (messageHandlers.ContainsKey(message.type))
            {
                messageHandlers[message.type]?.Invoke(message.payload);
            }
            else
            {
                Debug.LogWarning($"No handler registered for message type: {message.type}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error processing message from React: {e.Message}");
        }
    }

    // Send message to React Native
    public void SendMessageToReact(string messageType, string payload)
    {
        var message = new BridgeMessage
        {
            type = messageType,
            payload = payload
        };
        
        string jsonMessage = JsonUtility.ToJson(message);
        
        #if UNITY_IOS && !UNITY_EDITOR
        sendMessageToReactNative(jsonMessage);
        #elif UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                currentActivity.Call("sendMessageToReact", jsonMessage);
            }
        }
        #else
        Debug.Log($"Message to React: {jsonMessage}");
        #endif
    }

    // Message structure
    [Serializable]
    private class BridgeMessage
    {
        public string type;
        public string payload;
    }

    // Native bridge methods
    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void sendMessageToReactNative(string message);
    #endif
}