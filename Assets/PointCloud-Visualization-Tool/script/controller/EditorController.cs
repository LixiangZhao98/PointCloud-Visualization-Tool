using System.Collections.Generic;
using UnityEngine;



public class EditorController : MonoBehaviour
{
    public string loadFileName;
    public List<FlagNamesCollection>  loadFlagNames;
    public bool LoadFlag;
    public int gridNum = 64;
   
    
    // Start is called before the first frame update
    [ContextMenu("LoadCsv and CreateField")]

    public void LoadCSVandCreateField()
    {

        if (loadFlagNames.Count!=0&&LoadFlag)
        DataStorage.LoadCsv(loadFileName);
        DataStorage.CreateField(gridNum);
        DataStorage.DisplayAllParticle(LoadFlag, loadFlagNames);

    }
    [ContextMenu("LoadByte and CreateField")]
    public void LoadByteandCreateField()
    {
        if (loadFlagNames.Count!=0&&LoadFlag)
        DataStorage.LoadByte(loadFileName);
        DataStorage.CreateField(gridNum);
        DataStorage. DisplayAllParticle(LoadFlag, loadFlagNames);

    }

    [ContextMenu("Clear memory")]
    public void ClearMemory()
    {

        DataStorage.ClearParticleMemory();
        DataStorage.StacksInitialize();


    }


}


