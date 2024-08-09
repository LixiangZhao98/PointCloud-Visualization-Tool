using UnityEngine;
public class RunTimeControl : MonoBehaviour
{
    public Dataset dataset;
    private Dataset dataset_LastFrame;
    
    // public bool loadTarget;
    // public List<FlagNamesCollection> loadTargetNames;
    
    private void Start()
    {
        dataset_LastFrame = dataset;
        GetComponentInChildren<DataLoader>().LoadData((int)dataset);
    }

    private void Update()
    {
        if (dataset_LastFrame != dataset)
        {
            GetComponentInChildren<DataLoader>().LoadData((int)dataset);
            dataset_LastFrame = dataset;
        }
    }
}



 