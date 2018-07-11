using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.ComponentModel;

public class DebugConsole : MonoBehaviour
{

    List<Command> commands = new List<Command>();


    private void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void AddComand(string command)
    {

    }

    private void Update()
    {
    }

    public void Command(string input)
    {
        string[] parts = input.Split(' ');

        if (parts[0] == "add_cmd")
        {
            parts[2] = "System." + parts[2];
            Command cmd = CreateCommand(parts[1], parts[2]);
            commands.Add(cmd);
        }
        else
        {
            Command cmd = commands.FirstOrDefault(i => i.Equals(parts[0]));

            if (cmd != null) //any comand has this command in the allowed names list 
            {
                ApplyCommand(cmd, parts[1]);

                bool value = (bool) cmd.LinkedVar.GetValue(BindingFlags.Static | BindingFlags.Public);
                Debug.Log(value);
            }
            else
            {
                //poner en la comand pront que no se ha podido en rojo
            }
        }
    }

    private Command CreateCommand(string variableName, string typeName)
    {
        //reflection - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection
        Type type = Type.GetType(typeName);

        Type container = typeof(StaticDataContainer);
        FieldInfo placeToSet = container.GetField(variableName, BindingFlags.Static | BindingFlags.Public);

        List<String> names = new List<string> { variableName };

        return new Command(placeToSet, type, names);
    }

    private void ApplyCommand(Command cmd, string toSet)
    {
        var value = TypeDescriptor.GetConverter(cmd.Type).ConvertFrom(toSet);
        cmd.LinkedVar.SetValue(null, value);
    }
}

class Command : IEquatable<String>
{
    List<string> _names;
    FieldInfo _linkedVar;
    Type _type;

    public Command(FieldInfo v, System.Type t, List<string> allowedCommands = null)
    {
        if (allowedCommands != null)
            _names = allowedCommands;
        else
            _names = new List<string>();
        _linkedVar = v;
        _type = t;
    }

    public bool Equals(String com)
    {
        return _names.Contains(com);
    }

    public FieldInfo LinkedVar
    {
        get
        {
            return _linkedVar;
        }
    }
    public Type Type
    {
        get
        {
            return _type;
        }
    }
}


