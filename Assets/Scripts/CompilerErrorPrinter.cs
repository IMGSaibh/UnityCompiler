using System.CodeDom.Compiler;
using Mono.CSharp;

public class CompilerErrorPrinter : ReportPrinter
{

    readonly CompilerResults compilerResults;
    #region Properties

    public new int ErrorsCount { get; protected set; }

    public new int WarningsCount { get; private set; }

    #endregion
    public CompilerErrorPrinter(CompilerResults compilerResults)
    {
        this.compilerResults = compilerResults;
    }

    public override void Print(AbstractMessage msg, bool showFullPath)
    {
        if (msg.IsWarning)
        {
            ++WarningsCount;
        }
        else
        {
            ++ErrorsCount;
        }
        compilerResults.Errors.Add(new CompilerError()
        {
            IsWarning = msg.IsWarning,
            Column = msg.Location.Column,
            Line = msg.Location.Row,
            ErrorNumber = msg.Code.ToString(),
            ErrorText = msg.Text,
            FileName = showFullPath ? msg.Location.SourceFile.FullPathName : msg.Location.SourceFile.Name,
        });
				
    }

}
	