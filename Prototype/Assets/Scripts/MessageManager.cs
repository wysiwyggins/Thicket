using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    public static List<string> allMessages = new List<string>();
    static TMP_Text TextOutput;

    public static void AddMessage(string messageText)
	{
        allMessages.Insert(0, messageText);
        
        string output = "";
        int length = Mathf.Min(30, allMessages.Count);
        for (int i = length - 1; i >= 0; i--)
            {
                output += allMessages[i] + "\n";
            }
        if (TextOutput != null)
            TextOutput.text = output;


    }
    // The message list is static, so it would otherwise carry over when the scene is reloaded.
    public static void ClearMessages()
    {
        allMessages.Clear();
        if (TextOutput != null)
            TextOutput.text = "";
    }

    // Awake (not Start) so the text exists before PieceManager.Start posts the first turn message
    void Awake()
    {
        Instance = this;
        GameObject textObject = GameObject.Find("Text");
        if (textObject != null)
            TextOutput = textObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
