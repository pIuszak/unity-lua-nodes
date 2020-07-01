using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class FileUtilities
{
    public static void WriteString(string content, string fileName)
    {
        string path = "Assets/Resources/" + fileName;

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        // todo write whole file,not single line
        writer.WriteLine(content);
        //writer.
        writer.Close();

        //Re-import the file to update the reference in the editor
#if UNITY_EDITOR
        AssetDatabase.ImportAsset(path);
#endif

        TextAsset asset = (TextAsset) Resources.Load("save");

        //Print the text from the file
        Debug.Log(asset.text);
    }

    public static void ClearFile(string fileName)
    {
        File.WriteAllText("Assets/Resources/" + fileName, string.Empty);
    }

    public static StreamReader ReadString()
    {
        string path = "Assets/Resources/save.txt";
        //Read the text from directly from the test.txt file
        return new StreamReader(path);
    }
}