using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
	public static List<string> allMessages = new List<string>();
    public static Text TextOutput;

    public static void AddMessage(string messageText)
	{
        //allMessages.Add(messageText);
        //List<string> buffer = allMessages.GetRange(-1, -30);
        //string output = "";
        //foreach(string msg in buffer)
        //{
        //    output += listMember.ToString() + "\n";
        //}
        //TextOutput.text = output;


    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//[System.Serializable]
//public struct Message
//{
//	public string text;
//    public int hour;
//}
