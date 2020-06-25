using UnityEngine;
using UnityEditor;
using System.IO;

public static class FileUtilities
{
    [MenuItem("Tools/Write file")]
    public static void WriteString(string content, string fileName)
    {
        string path = "Assets/Resources/"+fileName;

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        // todo write whole file,not single line
        writer.WriteLine("Test");
        //writer.
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path); 
        TextAsset asset = (TextAsset) Resources.Load("save");

        //Print the text from the file
        Debug.Log(asset.text);
    }

    [MenuItem("Tools/Read file")]
    public static void ReadString()
    {
        string path = "Assets/Resources/save.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

}