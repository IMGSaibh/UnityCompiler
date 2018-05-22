using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Diagnostics;
using System;
using System.Reflection;

using Mono.CSharp;
using System.CodeDom.Compiler;
using System.Reflection.Emit;

public class PluginLoader : MonoBehaviour
{

	private string className = ""; 
	private Assembly assembly = null;




	public Assembly CompileScript(string source)
	{
		if (source == null)
		{
			return null;
		}
			
		try 
		{
			assembly = Compiler.CompileCSharpScript(source);
			return assembly;

		} 
		catch (Exception ex)
		{
			return null;
		}

	}

	public object CreateInstance(Assembly assembly)
	{
		if (assembly != null) 
		{
			className = Path.GetFileNameWithoutExtension("Test");
			object instance = null;
			System.Type script = assembly.GetType (className);
			instance = Activator.CreateInstance(script);
			return instance;
		} 
		else
		{
			return null;
		}

	}

	public object ExecPluginFunction(Assembly assembly,object instance, string function)
	{
		MethodInfo method = null; 
		if (assembly != null) 
		{
			className = Path.GetFileNameWithoutExtension("Test");
			System.Type script = assembly.GetType (className);
			method = script.GetMethod(function, BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
			object returnedValue = method.Invoke (instance, null);
			return returnedValue; 
		}
		else 
		{
			return null;
		}
	}

}
