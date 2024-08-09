using UnityEngine;

public class ColorMappingHelper : MonoBehaviour
{
    private ParticleGroup pG;
    public void ColorMappingHelperInit()
    {
        pG = transform.parent.GetComponentInChildren<DataLoader>().particles;
        Vector3[] lp = new Vector3[pG.GetParticlenum()];
        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            lp[i] = new Vector3((float)(pG.GetParticleDensity(i) -pG.MINPARDEN) / (pG.MAXPARDEN - pG.MINPARDEN), 0f, 0f);
        }
        this.transform.parent.GetComponentInChildren<PointRenderer>().SetUnselectedUV1(lp);
    }


}
