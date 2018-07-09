using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class DebugConsole : MonoBehaviour {

    List<Command> commands = new List<Command>();

    /*private void Start()
    {
    }

    public void AddComand(string command)
    {
        if (!allowedCommands.Contains(command))
            allowedCommands.Add(command);
    }

    public void Command(string input)
    {
        string[] parts = input.Split(new char[] { ' ' }, 2);

        Command cmd = commands.FirstOrDefault(i => i.Equals(parts[0]));

        if (cmd != null) //any comand has this command in the allowed names list 
        {
            auto type = 
        }
        else
        {
            //poner en la comand pront que no se ha podido en rojo
        }
    }

    private void SaveLevel()
    {
        //...
        List<int[]> tiles = new List<int[]>();
        System.IO.StreamWriter file = new System.IO.StreamWriter("VMO");


        foreach (int[] tileMix in tiles)
        {
            file.WriteLine(tileMix[0] + "," + tileMix[1]);
        }


    }*/
}

class Command : IEquatable<String>
{
    List<string> names;
    System.Type type;

    public Command(System.Type t, List<string> allowedCommands = null)
    {
        if (allowedCommands != null)
            names = allowedCommands;
        else
            names = new List<string>();
        type = t;
    }

    public bool Equals(String com)
    {
        return names.Contains(com);
    }
}

enum dataType
{
    System_Boolean = 0,
    System_Double = 1,
    System_String = 2
}


