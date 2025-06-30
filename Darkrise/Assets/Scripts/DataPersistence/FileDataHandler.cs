using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";
    private readonly string codeword = "ESIRKRAD";
    private bool useEncryption = false;

    public FileDataHandler(string dataDirPath, string dataFileName, bool _useEncrpytion)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = _useEncrpytion;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = ""; 
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd(); // read file
                    }
                }

                if (useEncryption)
                {
                    dataToLoad = EncryptDecrpyt(dataToLoad);
                }

                // deseralize
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.LogError($"Error when trying to load data from the file path {fullPath} - {e}");
            }
        }

        return loadedData;
    }

    public void Save(GameData gameData)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        Debug.Log(fullPath);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)); // make the directory
            string dataToStore = JsonUtility.ToJson(gameData, true); // serialize the data

            if(useEncryption)
            {
                dataToStore = EncryptDecrpyt(dataToStore);
            }

            using(FileStream stream = new FileStream(fullPath,FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore); // write to file
                }
            }

        }
        catch(Exception e)
        {
            Debug.LogError($"Error when trying to save data to the file path {fullPath} - {e}");
        }
    }

    private string EncryptDecrpyt(string data)
    {
        string modifiedData = "";
        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ codeword[i % codeword.Length]);
        }
        return modifiedData;
    }
}
