using UnityEngine;

public class ColorMappingHelper : MonoBehaviour
{

    public void ColorMappingHelperInit()
    {
        Vector3[] lp = new Vector3[DataMemory.particles.GetParticlenum()];
        for (int i = 0; i < DataMemory.particles.GetParticlenum(); i++)
        {
            lp[i] = new Vector3((float)(DataMemory.particles.GetParticleDensity(i) - DataMemory.particles.MINDEN) / (DataMemory.particles.MAXDEN - DataMemory.particles.MINDEN), 0f, 0f);
        }
        RenderDataRunTime.SetUnselectedUV1(lp);
    }


}
