using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test
{
    public void Func()
    {
        Debug.Log("Hello World");
    }

    public void Rotate()
    {
        GameObject go = null;
        go = GameObject.Find("LoadCSharpSkript");
        go.transform.Rotate(Vector3.up * Time.deltaTime * 20);
    }
	
	public bool ReturnTypeFunc()
	{
		Debug.Log("ReturnTypeFunc executed");
		return true;
	}
}
