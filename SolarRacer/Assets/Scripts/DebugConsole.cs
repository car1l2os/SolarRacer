using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.ComponentModel;
using System.IO;

public class DebugConsole : MonoBehaviour
{

    private List<Command> _commands = new List<Command>();
    private CanvasGroup _canvasGroup;
    private bool _visible = false;



    private void Start()
    {
        DontDestroyOnLoad(transform.parent.gameObject);

        JSONObject commandsData = new JSONObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Commands.json"));
        ConstructCommandsDatabase(commandsData);

        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    void OnApplicationQuit()
    {
        StoreCommands();
    }

    public void StoreCommands()
    {
        string fileName = Application.dataPath + "/StreamingAssets/Commands.json";

        List<string> txtLines = new List<string>();
        txtLines.Add("[");
        foreach (Command cmd in _commands)
        {
            txtLines.Add("\t" + "{");

            txtLines.Add("\t" + "\t" + '"' + "variable_name" + '"' + ':' + ' ' + '"' +
                            cmd.LinkedVar.Name + '"' + ',');

            txtLines.Add("\t" + "\t" + '"' + "type" + '"' + ':' + ' ' + '"' +
                            cmd.Type.Name + '"' + ',');

            string names = "\t" + "\t" + '"' + "variable_name" + '"' + ':' + ' ' + '"';
            foreach (string name in cmd.Names)
            {
                names += name + ',';
            }
            names = names.Remove(names.Length - 1);
            names += '"';
            txtLines.Add(names);

            txtLines.Add("\t" + "}");
        }
        txtLines.Add("]");
        File.WriteAllLines(fileName, txtLines.ToArray());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (_visible)
                Hide();
            else
                Show();
        }
        Debug.Log(StaticDataContainer._controlledByIA);
    }

    private void Hide()
    {
        _canvasGroup.alpha = 0f; //this makes everything transparent
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
        _visible = false;
    }

    private void Show()
    {
        _canvasGroup.alpha = 1f; //this makes everything transparent
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true; //this prevents the UI element to receive input events
        _visible = true;
    }

    public void Command(string input)
    {
        string[] parts = input.Split(' ');

        if (parts[0] == "add_cmd")
        {
            parts[2] = "System." + parts[2];
            Command cmd = new Command(parts[1], parts[2]);
            _commands.Add(cmd);
        }
        else
        {
            Command cmd = _commands.FirstOrDefault(i => i.Equals(parts[0]));

            if (cmd != null) //any comand has this command in the allowed names list 
            {
                ApplyCommand(cmd, parts[1]);

                bool value = (bool)cmd.LinkedVar.GetValue(BindingFlags.Static | BindingFlags.Public);
                Debug.Log(value);
            }
            else
            {
                //poner en la comand pront que no se ha podido en rojo
            }
        }
    }

    private void ApplyCommand(Command cmd, string toSet)
    {
        var value = TypeDescriptor.GetConverter(cmd.Type).ConvertFrom(toSet);
        cmd.LinkedVar.SetValue(null, value);
    }


    private void ConstructCommandsDatabase(JSONObject commandsData)
    {
        switch (commandsData.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < commandsData.list.Count; i++)
                {
                    string key = (string)commandsData.keys[i];
                    JSONObject j = (JSONObject)commandsData.list[i];
                    //Debug.Log(characterData["id"]);
                    ConstructCommandsDatabase(j);
                }
                _commands.Add(new Command(commandsData["variable_name"].str,
                                          commandsData["type"].str,
                                          commandsData["names"].str
                                          ));


                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in commandsData.list)
                {
                    ConstructCommandsDatabase(j);
                }
                break;
            case JSONObject.Type.STRING:
                //Debug.Log(obj.str);
                break;
            case JSONObject.Type.NUMBER:
                //Debug.Log(obj.n);
                break;
            case JSONObject.Type.BOOL:
                //Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                //Debug.Log("NULL");
                break;
        }
    }
}

class Command : IEquatable<String>
{
    List<string> _names;
    FieldInfo _linkedVar;
    Type _type;

    public Command(FieldInfo v, System.Type t, List<string> allowedNames = null)
    {
        if (allowedNames != null)
            _names = allowedNames;
        else
            _names = new List<string>();
        _linkedVar = v;
        _type = t;
    }

    public Command(string variableName, string typeName, List<string> allowedNames = null)
    {
        //reflection - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection
        if (allowedNames == null)
            _names = new List<string> { variableName };
        else
            _names = allowedNames;

        Type container = typeof(StaticDataContainer);
        _linkedVar = container.GetField(variableName, BindingFlags.Static | BindingFlags.Public);

        _type = Type.GetType(typeName);
    }

    public Command(string variableName, string typeName, string allowedNames)
    {
        _names = new List<string>();
        string[] tmp = allowedNames.Split(',');
        foreach (string name in tmp)
            _names.Add(name);

        Type container = typeof(StaticDataContainer);
        _linkedVar = container.GetField(variableName, BindingFlags.Static | BindingFlags.Public);

        _type = Type.GetType(typeName);
    }

    public bool Equals(String com)
    {
        return _names.Contains(com);
    }

    public void AddName(string name)
    {
        _names.Add(name);
    }

    public void RemoveName(string name)
    {
        _names.Remove(name);
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

    public List<string> Names
    {
        get
        {
            return _names;
        }
    }
}


