using System.Collections.Generic;
using UnityEngine;

public class EditorController : MonoBehaviour
{
    public string loadFileName;
    public List<RunTimeController.FlagNamesCollection>  loadFlagNames;
    public bool LoadFlag;
    public int gridNum = 64;
   
    
    // Start is called before the first frame update
    [ContextMenu("LoadCsv and CreateField")]

    public void LoadCSVandCreateField()
    {

        if (loadFlagNames.Count!=0&&LoadFlag)
        DataMemory.LoadDataByCsv(loadFileName);
        DataMemory.CreateDensityField(gridNum);
        DataMemory.DisplayAllParticle(LoadFlag, loadFlagNames);

    }
    [ContextMenu("LoadByte and CreateField")]
    public void LoadByteandCreateField()
    {
        if (loadFlagNames.Count!=0&&LoadFlag)
        DataMemory.LoadDataByByte(loadFileName);
        DataMemory.CreateDensityField(gridNum);
        DataMemory. DisplayAllParticle(LoadFlag, loadFlagNames);

    }

    [ContextMenu("Clear memory")]
    public void ClearMemory()
    {

        DataMemory.ClearDensityMemory();
        DataMemory.ClearParticleMemory();
        DataMemory.StacksInitialize();


    }




}
