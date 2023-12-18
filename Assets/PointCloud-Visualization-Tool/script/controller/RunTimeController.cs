using System.Collections.Generic;
using UnityEngine;
using static Enum;

public class RunTimeController : MonoBehaviour
{

        private void Start()
    {   
        SwitchDatasetFromFile(dataset.ToString());

    }




  [SerializeField, SetProperty("DATASET")]
    private Dataset dataset;
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

    private List<string> dataset_generator = new List<string> { "random_sphere" };
  public bool LoadFlag;

    public string StoreFlagName;
  
    




    [ContextMenu("StoreFlags")]
    public void StoreFlages()
    {
      DataMemory.StoreFlags(StoreFlagName);
    }

    public void SwitchDatasetFromFile(string name)
    {
        DataMemory.StacksInitialize();
      
        if (!dataset_generator.Contains(name))
        {
                   DataMemory.LoadDataByByte(name);
                    if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);
           
        }
        else
        {
                    DataMemory.LoadDataByVec3s(DataGenerator.Generate(name), name);
                 if (LoadFlagNames.Count!=0&&LoadFlag)
            DataMemory.LoadFlagsToStack(LoadFlagNames);
        }
        

       

        RenderDataRunTime_demo.GenerateMesh();
   

    }

    }




