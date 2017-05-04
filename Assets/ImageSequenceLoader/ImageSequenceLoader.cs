using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Load a sequence of image into a variable, anywhere in the scene, from the resources folder
/// </summary>
public class ImageSequenceLoader : MonoBehaviour {

    [Header("Sequence")]

    /// <summary>
    /// Folder where the images should be initialy located (in Resources)
    /// </summary>
    [SerializeField]
    string resourcesFolder;

    [Header("Target")]

    /// <summary>
    /// Name of the component containing the variable where to save the image sequence
    /// </summary>
    [SerializeField]
    string className;

    /// <summary>
    /// Name of the gameobject that has this component in the scene
    /// </summary>
    [SerializeField]
    GameObject gameObject;

    /// <summary>
    /// Name of the variable that should contain the image sequence
    /// </summary>
    [SerializeField]
    string variableName;

    enum DataStructure { Array,List}

    /// <summary>
    /// Type of the variable
    /// </summary>
    [SerializeField]
    DataStructure dataStructureType;

    /// <summary>
    /// Load the images into the variable, available in the editor with a button
    /// </summary>
    [EditorButton]
    public void LoadImageSequence()
    {
        //Tell the user that we started loading
        print("Loading images...");

        //If we couldn't find it, notify the user and stop
        if(gameObject==null)
        {
            print("Failed : No gameobject");
            return;
        }

        //Try to find the variable of the class the user is looking for
        FieldInfo[] fields= null;
        try
        {
            fields = gameObject.GetComponent(className).GetType().GetFields();
        }
        //If we couldn't access the class on this gameobject, notify the user and stops
        catch(Exception e)
        {
            print("Failed : GameObject found, but couldn't access the component " + className +"\n Error message : "+e.Message);
            return;
        }

        //If we couldn't get access variables of this class, notify the user and stops
        if (fields == null)
        {
            print("Failed : Couldn't access the variables of the component");
            return;
        }

        //Try to find a variable that has the same name as the one we are looking for
        bool found = false;
        int id = 0;
        for(int i=0;i<fields.Length;i++)
        {
            if(fields[i].Name==variableName)
            {
                found = true;
                id = i;
            }
        }

        //If we didn't find it, notify the user and stop
        if (!found)
        {
            print("Failed : Couldn't find the variable " + variableName + " in the class, make sure it is a public variable without property.");
            return;
        }

        //Try to Load the array/list into the variable
        int imageCount = 0;
        try
        {
            switch (dataStructureType)
            {
                case DataStructure.Array:
                    object seqArr = (object)GetImageSequenceAsArray(out imageCount);
                    fields[id].SetValue(gameObject.GetComponent(className), seqArr);
                    break;
                case DataStructure.List:
                    object seqList = (object)GetImageSequenceAsList(out imageCount);
                    fields[id].SetValue(gameObject.GetComponent(className), seqList);
                    break;
            }
        }
        //If an error happened, most likely cause of wrong types, notify the user and stop
        catch(Exception e)
        {
            print("Failed : Type doesn't match, make sure you are using the right datastructure type.\n" + e);
            return;
        }

        print("Loaded the image sequence ("+imageCount+" images) into " + fields[id].Name+" successfully !");
    }

    /// <summary>
    /// Load the images located in [resourcesFolder] and returne them as an array
    /// </summary>
    /// <param name="imageCount">Out variable, the lenght of the array</param>
    /// <returns>The image sequence as an array of Texture2D</returns>
    Texture2D[] GetImageSequenceAsArray(out int imageCount)
    {
        Texture2D[] seq = Resources.LoadAll<Texture2D>(resourcesFolder);
        if(seq.Length==0)
        {
            print("Couldn't find any texture2D in the given folder path in resources. Make sure the path Resources/"+resourcesFolder+" exists and have supported textures in it.");
        }

        imageCount = seq.Length;

        return seq;
    }

    /// <summary>
    /// Load the images located in [resourcesFolder] and returne them as a list
    /// </summary>
    /// <param name="imageCount">Out variable, the lenght of the list</param>
    /// <returns>The image sequence as a list of Texture2D</returns>
    List<Texture2D> GetImageSequenceAsList(out int imageCount)
    {
        List<Texture2D> seq = new List<Texture2D>(Resources.LoadAll<Texture2D>(resourcesFolder));
        if (seq.Count == 0)
        {
            print("Couldn't find any texture2D in the given folder path in resources. Make sure the path Resources/" + resourcesFolder + " exists and have supported textures in it.");
        }

        imageCount = seq.Count;

        return seq;
    }
}
