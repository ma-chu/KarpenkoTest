using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
// Загрузчик параметров
public class DatabaseLoader : MonoBehaviour
{
    public List<ConfigurableParams> configurableParams = new List<ConfigurableParams>();

    // Extension of the database files.
    private const string FILE_EXTENSION = @".json";

    // Database file name.
    private const string DATABASE_NAME = @"ResourceLoader/config.json";

    private void Awake()
    {
        configurableParams = ReturnDatabase<ConfigurableParams>(DATABASE_NAME);
    }

 
    // Removes the default file extension from path.
    private string RemoveFileExtension(string path)
    {
        if (path.Length >= FILE_EXTENSION.Length)
        {
            //If file extension exist, remove it.
            if (path.ToLower().Substring(path.Length - FILE_EXTENSION.Length, FILE_EXTENSION.Length) == FILE_EXTENSION.ToLower())
                return path.Substring(0, path.Length - FILE_EXTENSION.Length);
            //File extension doesn't exist.
            else
                return path;
        }
        //Path isn't long enough to contain file extension.
        else
        {
            return path;
        }
    }

    
    // Removes the directory separator if at the begining of path.
    private string RemoveLeadingDirectorySeparator(string path)
    {
        //Remove directory separate character if it exist on the first character.
        if (char.Parse(path.Substring(0, 1)) == Path.DirectorySeparatorChar || char.Parse(path.Substring(0, 1)) == Path.AltDirectorySeparatorChar)
            return path.Substring(1);
        else
            return path;
    }

    
    // Returns string result of a text file from Resources.
    private string ReturnFileResource(string path)
    {
        //Remove default file extension and format the path to the platform.
        path = RemoveFileExtension(path);
        path = RemoveLeadingDirectorySeparator(path);

        if (path == string.Empty)
        {
            Debug.LogError("ReturnFileResource -> path is empty.");
            return string.Empty;
        }

        //Try to load text from file path.
        TextAsset textAsset = Resources.Load(path) as TextAsset;

        if (textAsset != null)
            return textAsset.text;
        else
            return string.Empty;
    }


    // Returns a database at the file path.
    private List<T> ReturnDatabase<T>(string path)
    {
        string result = ReturnFileResource(path);

        if (result.Length != 0)
        {
            return JsonConvert.DeserializeObject<List<T>>(result).ToList();
        }
        else
        {
            Debug.LogWarning("ReturnDatabase -> result text is empty.");
            return new List<T>();
        }
    }

}
