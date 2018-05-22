using Mono.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

public class Compiler
{
    static long assemblyCounter = 0;

	public static Assembly CompileCSharpScript(string source)
    {
        //MonoCSharp Compiler
        CompilerSettings settings = new CompilerSettings();
        CompilerParameters param = new CompilerParameters();
		CompilerResults compilerResults = null; 
		CompilerErrorPrinter compilerErrorPrinter = new CompilerErrorPrinter (compilerResults);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // holt die dll`s die zum Compilieren gebraucht werden.
            // Assembly die durch Reflection.Emit erzeugt werden überspringen (Laufzeit-Scripte)
            // Verhindert das Abstürtzen der Assemblies beim Level umladen
            if (!(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder))
            {
                settings.AssemblyReferences.Add(assembly.Location);
            }
        }

        //Compiler-Parameter spezifizieren Compiler-Settings 
        //Parameterliste
        //https://msdn.microsoft.com/en-us/library/system.codedom.compiler.compilerparameters(v=vs.110).aspx

        settings.Target = Target.Library;

        //Warnungen sollen nicht ins ErrorLog geschrieben werden 
        settings.WarningLevel = 0;

        if (string.IsNullOrEmpty(param.OutputAssembly))
        {
            param.OutputAssembly = settings.OutputFile = "DynamicAssembly_" + assemblyCounter + settings.TargetExt;
            assemblyCounter++;
        }
		
        // if it is not being outputted, we use this to set name of the dynamic assembly
        settings.OutputFile = param.OutputAssembly;

        //getStream holt das Script das Kompiliet werden soll. Ohne Parameter
        //getStream() wird an die SourceFile Klasse übergeben
        Func<Stream> getStream = () => { return new MemoryStream(Encoding.UTF8.GetBytes(source)); };
		SourceFile unit = new SourceFile("0","0",settings.SourceFiles.Count + 1, getStream); 
		settings.SourceFiles.Add (unit);

		DynamicCompilerDriver driver = new DynamicCompilerDriver(new CompilerContext(settings,compilerErrorPrinter));
		return driver.Compile(AppDomain.CurrentDomain, true);

//
//		try
//        {
//           
//        }
//        catch (Exception e)
//        {
//            compilerResults.Errors.Add(new CompilerError()
//            {
//                IsWarning = false,
//                ErrorText = e.Message,
//            });
//
//			Log.Debug (LogColor.red, "Errorcount " + compilerResults.Errors.Count);
//
//			return null;
//        }

    }


    //    public static void AddAssemblyToObject(Assembly assembly, string scriptName, GameObject gameObj)
    //    {
    //		//Holen der Methode die das Script dem zukünftigen GameObject zuweist
    //		MethodInfo scriptFunc = assembly.GetType (scriptName).GetMethod ("ScriptToObject");
    //
    //
    //		var del = (Func<GameObject, MonoBehaviour>)
    //		//Delagate erzeugen mit GameObject, Script und der Variable(scriptFunc) mit infos der Methode
    //		System.Delegate.CreateDelegate(typeof(Func<GameObject, MonoBehaviour>), scriptFunc);
    //		
    //        //Methode fügt ihre Komponente this.Gameobject hinzu
    //        MonoBehaviour addComponent = del.Invoke(gameObj);
    //    }
}
