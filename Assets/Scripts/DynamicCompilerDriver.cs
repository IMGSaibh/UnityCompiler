// modified version of Mono.CSharp.Driver

// driver.cs: The compiler command line driver.
//
// Authors:
//   Miguel de Icaza (miguel@gnu.org)
//   Marek Safar (marek.safar@gmail.com)
//
// Dual licensed under the terms of the MIT X11 or GNU GPL
//
// Copyright 2001, 2002, 2003 Ximian, Inc (http://www.ximian.com)
// Copyright 2004, 2005, 2006, 2007, 2008 Novell, Inc
// Copyright 2011 Xamarin Inc
//

using System;
using System.Reflection.Emit;
using System.IO;
using Mono.CSharp;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Modifizierte Klasse der driver-Klasse des mcs-compilers
/// </summary>
public class DynamicCompilerDriver
{
    readonly CompilerContext compilerContext;
    CompilerSettings settings;
	List<SourceFile> sources = new List<SourceFile> ();
	
	public DynamicCompilerDriver(CompilerContext compilerContext)
    {
		this.compilerContext = compilerContext;
    }
  
    public void Parse(ModuleContainer module)
    {
		//sources .pdb Dateien (Symbol-Dateien für den Compiler)
        sources = module.Compiler.SourceFiles;

        Location.Initialize(sources);

		ParserSession session = new ParserSession
        {
            UseJayGlobalArrays = true,
        };

        for (int i = 0; i < sources.Count; ++i)
        {
            Parse(sources[i], module, session);
        }
    }

    public void Parse(SourceFile file, ModuleContainer module, ParserSession session)
    {
        Stream input;

        try
        {
            input = file.GetDataStream();
        }
        catch
        {
			Debug.Log("Source file `{0}' konnte nicht gefunden werden" + file.Name);
            return;
        }

		SeekableStreamReader reader = new SeekableStreamReader(input, compilerContext.Settings.Encoding, session.StreamReaderBuffer);
        Parse(reader, file, module, session);

        reader.Dispose();
        input.Close();
    }



    public static void Parse(SeekableStreamReader reader, SourceFile sourceFile, ModuleContainer module, ParserSession session)
    {
		CompilationSourceFile file = new CompilationSourceFile(module, sourceFile);
        module.AddTypeContainer(file);

        CSharpParser parser = new CSharpParser(reader, file, null, session);
        parser.parse();

}
    //
    // Kompilierung durch mcs-Compiler
    //
	public AssemblyBuilder Compile(AppDomain domain, bool generateInMemory)
    {
        Debug.Log("Aufruf Compiler");

        //settings.SourceFiles des CompilerContextes hält alle dll's und das noch zu kompilierende Script
        CompilerSettings settings = compilerContext.Settings;
		AssemblyBuilder assembly = null; 
		ModuleContainer module = new ModuleContainer(compilerContext);

        Parse(module);

		//benötigte Klassen die in der Compile-Methode des mcs Compiler enthalten sind
		AssemblyDefinitionDynamic assemblyDef = new AssemblyDefinitionDynamic(module, settings.OutputFile, settings.OutputFile);
		module.SetDeclaringAssembly(assemblyDef);

		ReflectionImporter importer = new ReflectionImporter(module, compilerContext.BuiltinTypes);
		assemblyDef.Importer = importer;

		DynamicLoader loader = new DynamicLoader(importer, compilerContext);
        loader.LoadReferences(module);

		compilerContext.BuiltinTypes.CheckDefinitions (module);
		assemblyDef.Create (domain, AssemblyBuilderAccess.RunAndSave);

        module.CreateContainer();
		loader.LoadModules(assemblyDef, module.GlobalRootNamespace);
        module.InitializePredefinedTypes();
	
        module.Define();
		assemblyDef.Resolve();
		assemblyDef.Emit();
        module.CloseContainer();

		return assemblyDef.Builder;
    }
}


