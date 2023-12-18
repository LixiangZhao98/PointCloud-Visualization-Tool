using UnityEngine;

public class RenderDataRunTime : MonoBehaviour
{
    public GameObject visCenter;
    public static float  visSize=100f; //real size of wim
    
    public static float ratio;
   public Material unselected_mat;
      public Material target_mat;

    [SerializeField]
   public static Mesh unselected_mesh;

    [SerializeField]
  public static Mesh[] target_mesh;
  
    public static bool drawEnable;
  
   static int tarMeshNum=0;
     Matrix4x4 m;
    void Start()
    {
        drawEnable=true;

    }
   void Awake()
   {
    
      target_mesh=new Mesh[tarMeshNum];
      for(int i=0;i<tarMeshNum;i++)
      target_mesh[i]=new Mesh();
   }
    public static void GenerateMesh(bool fromStarck=true)
    {
   
        DestroyImmediate(unselected_mesh, true);
            foreach(var m in target_mesh)
        DestroyImmediate(m, true);
          
        unselected_mesh = new Mesh();
        target_mesh = new Mesh[tarMeshNum];
      for(int i=0;i<tarMeshNum;i++)
        target_mesh[i]=new Mesh();
        DisplayParticles.GenerateMeshFromPg( unselected_mesh, null, target_mesh, DataMemory.allParticle, fromStarck);
        ratio = 1f / (DataMemory.allParticle.XMAX - DataMemory.allParticle.XMIN) * visSize;
        drawEnable=true;
    }



       private void Update()
    {
       
        
       if(drawEnable)
       {
         m = Matrix4x4.TRS(visCenter.transform.position, visCenter.transform.rotation, new Vector3(ratio, ratio, ratio));
          visCenter.transform.localScale = new Vector3(ratio, ratio, ratio);
       }
      
    }
       void LateUpdate()
    {
     
  if(drawEnable)
       { Graphics.DrawMesh(unselected_mesh, m, unselected_mat, 1);
            for(int i=0;i<target_mesh.Length;i++)
            Graphics.DrawMesh(target_mesh[i], m,target_mat, 1);}
    }
}


