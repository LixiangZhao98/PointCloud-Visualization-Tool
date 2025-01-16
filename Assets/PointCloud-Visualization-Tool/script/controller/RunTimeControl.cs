using UnityEngine;
public class RunTimeControl : MonoBehaviour
{
    public Dataset dataset;
    private Dataset dataset_LastFrame;
    [Tooltip("Use your own function to generate the dataset?")]
    public bool custom=false;
    public bool custom_LastFrame=false;

    public CustomGeneratorEnum customGenerator; 
    public CustomGeneratorEnum customGenerator_LastFrame;  
    // public bool loadTarget;
    // public List<FlagNamesCollection> loadTargetNames;

    private void Start()
    {
        custom_LastFrame = custom;
        dataset_LastFrame = dataset;
        customGenerator_LastFrame = customGenerator;
        GetComponentInChildren<DataLoader>().LoadData((int)dataset,(int) customGenerator,custom);
    }

    private void Update()
    {
        if (dataset_LastFrame != dataset||customGenerator_LastFrame != customGenerator|custom_LastFrame != custom)
        {
            GetComponentInChildren<DataLoader>().LoadData((int)dataset,(int) customGenerator,custom);
            dataset_LastFrame = dataset;
            customGenerator_LastFrame = customGenerator;
            custom_LastFrame = custom;
        }
    }
    


}



 