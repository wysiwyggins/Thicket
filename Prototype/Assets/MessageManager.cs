using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour
{
	public static List<Message> allMessages = new List<Message>();


	public static void AddMessage()
	{
		//...
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


[System.Serializable]
public struct Message
{
	public string text;
	public Color aColor;
	public Sprite drawing;
}
