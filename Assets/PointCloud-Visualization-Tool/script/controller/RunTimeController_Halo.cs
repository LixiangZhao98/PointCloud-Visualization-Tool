using System.Collections.Generic;
using UnityEngine;

public class RunTimeController_Halo : RunTimeController
{

        private void Start()
    {   

        SwitchDatasetFromFile(dataset.ToString());

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

       
        RenderDataRunTime.GenerateMesh();
     this.transform.parent.GetComponentInChildren<HaloDrawIndirectCsHelper>(). Init(DataMemory.allParticle.GetParticlenum());
 Vector3[] lp=new Vector3[DataMemory.allParticle.GetParticlenum()];
           for (int i = 0; i < DataMemory.allParticle.GetParticlenum(); i++)
        {
          lp[i]=new Vector3((float)(DataMemory.allParticle.GetParticleDensity(i)-DataMemory.allParticle.MINDEN)/(DataMemory.allParticle.MAXDEN-DataMemory.allParticle.MINDEN),0f,0f);
        }
       this.transform.parent.GetComponentInChildren<RenderDataRunTime>(). SetUnselectedUV1(lp);
    }

    }


