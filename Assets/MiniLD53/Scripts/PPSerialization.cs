using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class PPSerialization {
	public static BinaryFormatter binaryFormatter = new BinaryFormatter();

	public static void Save(string saveTag,object obj){
		MemoryStream memoryStream = new MemoryStream ();
		binaryFormatter.Serialize (memoryStream, obj);
        PlayerPrefs.SetString (saveTag, System.Convert.ToBase64String (memoryStream.ToArray()));
	}

    public static object Load(string saveTag) {
        string temp = PlayerPrefs.GetString(saveTag);
        if (temp == null) {
            return temp;
        }
        MemoryStream memoryStream = new MemoryStream(System.Convert.FromBase64String(temp));
        return binaryFormatter.Deserialize(memoryStream);
    }

    public static bool HasSave(string saveTag) {
        return PlayerPrefs.GetString(saveTag) != null;
    }
}
