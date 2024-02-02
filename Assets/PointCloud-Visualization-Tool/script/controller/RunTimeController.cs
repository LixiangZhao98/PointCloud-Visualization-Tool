using System.Collections.Generic;
using UnityEngine;
using static Enum;
namespace LixaingZhao.PointcloudTool{
public class RunTimeController : MonoBehaviour
{

        private void Start()
    {   

        SwitchDatasetFromFile(dataset.ToString());

    }




  [SerializeField, SetProperty("DATASET")]
    protected Dataset dataset;
    public Dataset DATASET
        {
            get { return dataset; }
            set { 
              dataset=value;
              if(Application.isPlaying)
            SwitchDatasetFromFile(dataset.ToString());
                    
            }
        }


    
    [SerializeField]
    public List<FlagNamesCollection> LoadFlagNames;

    protected List<string> dataset_generator = new List<string> { "random_sphere" };
      protected List<string> dataset_ply = new List<string> { "dragon_vrip"};
  public bool LoadFlag;

    public string StoreFlagName;
  




    [ContextMenu("StoreFlags")]
    public void StoreFlages()
    {
      DataMemory.StoreFlags(StoreFlagName);
    }

    public virtual void  SwitchDatasetFromFile(string name)
    {
        DataMemory.StacksInitialize();
      
        if (dataset_generator.Contains(name))
        {
                  DataMemory.LoadDataByVec3s(DataGenerator.Generate(name), name);    
        }
        else if(dataset_ply.Contains(name))
        {        
                  DataMemory.LoadDataByPly(name);    
        }
        else
        {
               DataMemory.LoadDataByByte(name);
        }
                            
        if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);

       
        RenderDataRunTime_demo.GenerateMesh();
   

    }

    }



}