using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyEvent : MonoBehaviour
{
  public void MarchingCubeGpuCsHelperInit()
    {
        if(transform.parent.GetComponentInChildren<RunTimeController>().calDen)  
        {
            this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().enabled=true;
            this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().Init();
        }
        else
        {
            this.transform.parent.GetComponentInChildren<MarchingCubeGPU>().enabled = false;
        }
    }
    public void ColorMapping()
    {
        Vector3[] lp = new Vector3[DataMemory.allParticle.GetParticlenum()];
        for (int i = 0; i < DataMemory.allParticle.GetParticlenum(); i++)
        {
            lp[i] = new Vector3((float)(DataMemory.allParticle.GetParticleDensity(i) - DataMemory.allParticle.MINDEN) / (DataMemory.allParticle.MAXDEN - DataMemory.allParticle.MINDEN), 0f, 0f);
        }
        RenderDataRunTime.SetUnselectedUV1(lp);
    }

    public void HaloMapping()
    {
        this.transform.parent.GetComponentInChildren<HaloDrawIndirectCsHelper>().Init(DataMemory.allParticle.GetParticlenum());
        Vector3[] lp = new Vector3[DataMemory.allParticle.GetParticlenum()];
        for (int i = 0; i < DataMemory.allParticle.GetParticlenum(); i++)
        {
            lp[i] = new Vector3((float)(DataMemory.allParticle.GetParticleDensity(i) - DataMemory.allParticle.MINDEN) / (DataMemory.allParticle.MAXDEN - DataMemory.allParticle.MINDEN), 0f, 0f);
        }
        RenderDataRunTime.SetUnselectedUV1(lp);
    }
}
