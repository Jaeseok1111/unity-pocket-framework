using System.Text;

public struct BackendQueryResult
{
    public bool IsSuccess;
    public string ClassName;
    public string FunctionName;
    public string TableName;
    public string ErrorInfo;

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.AppendLine("{");
        {
            builder.AppendLine($"IsSuccess: {IsSuccess},");
            builder.AppendLine($"ClassName: {ClassName},");
            builder.AppendLine($"FunctionName: {FunctionName},");
            builder.AppendLine($"TableName: {TableName},");
            builder.AppendLine($"ErrorInfo: {ErrorInfo}");
        }
        builder.AppendLine("}");

        return builder.ToString();
    }
}

public delegate void AfterBackendLoadDelegate(BackendQueryResult result);
public delegate void AfterBackendCallback(BackendQueryResult result);