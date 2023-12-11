using PavelKouril.MarchingCubesGPU;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunTimeController : MonoBehaviour
{

        private void Start()
    {   
        SwitchDatasetFromFile(dataset.ToString());
        // Vector3[] v= new Vector3[DataMemory.allParticle.GetParticlenum()];
        // for(int i=0;i<DataMemory.allParticle.GetParticlenum();i++)
        // {
        //     v[i]=DataMemory.allParticle.GetParticlePosition(i);
        // }
        // DataMemory.LoadDataByVec3s(v,"a");
        // RenderDataRunTime.GenerateMesh();
    }




  [SerializeField, SetProperty("DATASET")]
    private Dataset dataset;
    public Dataset DATASET
        {
            get { return dataset; }
            set { 
            SwitchDatasetFromFile(dataset.ToString());
                    
            }
        }
   [Serializable]
    public class FlagNamesCollection
    {
        
        public string[] FlagNames;
        public Material target_mat;
    }

    public Material unselected_mat;
    public Material selected_mat;
    
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
        

       

        RenderDataRunTime.GenerateMesh();
   

    }

    }

[Serializable]
public enum Dataset { disk,uniform_Lines, ball_hemisphere, ununiform_Lines, Flocculentcube1, Flocculentcube2, nbody1, nbody2, training_torus ,training_sphere,training_pyramid,training_cylinder, three_rings, fiveellipsolds};
[Serializable]
public enum GRIDNum { none,grid64,grid100,grid200 };


