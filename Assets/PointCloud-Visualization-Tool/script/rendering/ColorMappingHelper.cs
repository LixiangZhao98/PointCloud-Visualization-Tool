using UnityEngine;

public class ColorMappingHelper : MonoBehaviour
{

    public void ColorMappingHelperInit()
    {
        Vector3[] lp = new Vector3[DataStorage.particles.GetParticlenum()];
        for (int i = 0; i < DataStorage.particles.GetParticlenum(); i++)
        {
            lp[i] = new Vector3((float)(DataStorage.particles.GetParticleDensity(i) - DataStorage.particles.MINDEN) / (DataStorage.particles.MAXDEN - DataStorage.particles.MINDEN), 0f, 0f);
        }
        RenderDataRunTime.SetUnselectedUV1(lp);
    }


}
