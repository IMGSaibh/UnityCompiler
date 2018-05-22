using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;


public class LoadCSharpScript : MonoBehaviour
{
    PluginLoader loader = new PluginLoader();
    Assembly assembly = null;
    object instance = null;
    public string LoadScript()
    {
        string quellcode = File.ReadAllText(Application.dataPath + "/Test.cs");
        Debug.Log(quellcode);
        if (quellcode != null )
            return quellcode;

        else
            return null;
    }

    void Start()
    {
        string source = File.ReadAllText(Application.dataPath + "/StreamingAssets/Test.cs");
        assembly = loader.CompileScript(source);
        Debug.Log(assembly.FullName);
        instance = loader.CreateInstance(assembly);

        loader.ExecPluginFunction(assembly, instance, "Func");
        bool returned = (bool)loader.ExecPluginFunction(assembly, instance, "ReturnTypeFunc");
        Debug.Log("Returned value: " + returned);
    }

    private void Update()
    {
        loader.ExecPluginFunction(assembly, instance, "Rotate");
    }
}
