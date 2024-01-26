using System;
using System.Linq;
using System.Reflection;
using UnityCodeGen;

[Generator]
public class GameChartCodeGen : CodeGeneratorBase
{
    public override string FolderPath => "Assets/Scripts/Backend/GameChart";
    public override string Name => "GameChart.Generated.cs";

    protected override void Generate()
    {
        WriteLine("using System;");
        WriteLine();

        GenerateClasses();
    }

    private void GenerateClasses()
    {
        Type[] classes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IGameChart).IsAssignableFrom(p))
            .ToArray();

        foreach (Type gameChart in classes)
        {
            GameChartAttribute attribute = gameChart.GetCustomAttribute<GameChartAttribute>();
            if (attribute == null)
            {
                continue;
            }

            GenerateClass(gameChart, attribute);
        }
    }

    private void GenerateClass(Type gameChart, GameChartAttribute attribute)
    {
        WriteLine($"public partial class {gameChart.Name} : IGameChartForAutoGeneration");
        WriteLine("{");
        PushIndent();
        {
            CreateGetNameFunction(attribute);
            WriteLine();

            CreateToLocalFunction(gameChart);
            WriteLine();

            CreateQueryFunction();
        }
        PopIndent();
        WriteLine("}");
    }

    private void CreateGetNameFunction(GameChartAttribute attribute)
    {
        WriteLine($"public string GetName()");
        WriteLine("{");
        PushIndent();
        {
            WriteLine($"return \"{attribute.Name}\";");
        }
        PopIndent();
        WriteLine("}");
    }

    private void CreateToLocalFunction(Type gameData)
    {
        WriteLine($"public void ToLocal(LitJson.JsonData gameChartJson)");
        WriteLine("{");
        PushIndent();
        {
            foreach (FieldInfo field in gameData.GetFields())
            {
                GameChartColumnAttribute column = field.GetCustomAttribute<GameChartColumnAttribute>();
                if (column == null)
                {
                    continue;
                }

                if (field.FieldType == typeof(string))
                {
                    WriteLine($"{field.Name} = gameChartJson[\"{column.Name}\"].ToString();");
                    continue;
                }

                Write($"{field.Name} = ");
                Write($"{field.FieldType.Name}.Parse(gameChartJson[\"{column.Name}\"].ToString());");
                WriteLine();
            }
        }
        PopIndent();
        WriteLine("}");
    }

    private void CreateQueryFunction()
    {
        WriteLine($"public GameChartQuery CreateQuery()");
        WriteLine("{");
        PushIndent();
        {
            WriteLine($"return new GameChartQuery(this);");
        }
        PopIndent();
        WriteLine("}");
    }
}
