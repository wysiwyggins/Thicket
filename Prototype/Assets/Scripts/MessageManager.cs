using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    public static List<string> allMessages = new List<string>();
    static Text TextOutput;

    public static void AddMessage(string messageText)
	{
        allMessages.Insert(0, messageText);
        
        string output = "";
        Debug.Log("adding a message!");
        int length = Mathf.Min(30, allMessages.Count);
        for (int i = length - 1; i >= 0; i--)
            {
                output += allMessages[i] + "\n";
                Debug.Log("doin thangs!");
            }
        TextOutput.text = output;
        Debug.Log("outputted");


    }
    // Start is called before the first frame update
    void Start()
    {
        TextOutput = GameObject.Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
