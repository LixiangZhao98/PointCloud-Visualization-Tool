using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
    public class FieldNode
    {
        [SerializeField]
        private Vector3 nodePosition;
        [SerializeField]
        private double nodeDensity;
        [SerializeField]
        private double enclosedParticleDis;
        [SerializeField]
        private Vector3 nodeGradient;
        [SerializeField]
        private Vector3 nodeGridPos;
        public FieldNode(Vector3 pos,Vector3 gridPos)
        {
            nodePosition = pos;
            nodeGridPos = gridPos;
        }
        public void SetEnclosedParticleDis(double dis)
        {
            enclosedParticleDis = dis;
        }
        public double GetEnclosedParticleDis()
        {
            return enclosedParticleDis;
        }
        public double GetNodeDensity()
        {
            return nodeDensity;
        }
        public Vector3 GetNodePosition()
        {
            return nodePosition;
        }
        public Vector3 GetNodeGradient()
        {
            return nodeGradient;
        }
        public Vector3 GetNodeGridPos()
        {
            return nodeGridPos;
        }
        public void SetNodeDensity(double density)
        {
            nodeDensity = density;
        }
        public void SetNodeGradient(Vector3 g)
        {
            nodeGradient = g;
        }

        public void NodeDensityPlusDis(double dis)
        {
            enclosedParticleDis = enclosedParticleDis + dis;
        }
    }
    [System.Serializable]
    public class DensityField
    {
        #region  Variables

        [SerializeField]
        public string name;
        [SerializeField]
        private List<FieldNode> fieldNode;
        [SerializeField]
        private List<LUTUnit> LUT_;
        [SerializeField]
        private int xNum;  //total nodes number on x axis
        [SerializeField]
        private int yNum;
        [SerializeField]
        private int zNum;
        [SerializeField]
        private float xStep;  //distance between two nodes along x axis
        [SerializeField]
        private float yStep;
        [SerializeField]
        private float zStep;
        private float maxNodeDen;
        private float minNodeDen;
        private float aveNodeDensity;
        public float XSTEP { get { return xStep; } }
        public float YSTEP { get { return yStep; } }
        public float ZSTEP { get { return zStep; } }
        public int XNUM { get { return xNum; } }
        public int YNUM { get { return yNum; } }
        public int ZNUM { get { return zNum; } }
        public float MAXNODEDEN { get { return maxNodeDen; } set { maxNodeDen = value; } }
        public float MINNODEDEN { get { return minNodeDen; } set { minNodeDen = value; } }
        public float AVENODEDENSITY { get { return aveNodeDensity; } set { aveNodeDensity = value; }}

        ComputeBuffer slModified;
        ComputeBuffer parPos;
        ComputeBuffer nodePos;
        ComputeBuffer nodeDen;
        ComputeBuffer parDen;
        Vector3[] parPos_;
        Vector3[] parSL_;
        Vector3[]  gridPos_;
        float [] dens;
        int GROUPSIZE=8;
        const int PARGROUPSIZE=512;
        #endregion

        #region Get
        public Vector3 GetNodeGradient(int i)
        {
            return fieldNode[i].GetNodeGradient();
        }
        public Vector3 GetNodedPos(int i)
        {
            return fieldNode[i].GetNodePosition();
        }
        public Vector3 GetNodeGridPos(int i)
        {
            return fieldNode[i].GetNodeGridPos();
        }
        public double GetNodeDensity(int i)
        {
            return fieldNode[i].GetNodeDensity();
        }
        public double GetEnclosedParticleDis(int i)
        {
            return fieldNode[i].GetEnclosedParticleDis();
        }
        public int GetNodeNum()
        {
            return fieldNode.Count;
        }
        public int GetLUTUnitNum(int index)
        {
            return LUT_[index].GetLTUnitNum();
        }
        public List<int> GetLUTUnit(int index)
        {
            return LUT_[index].GetLTUnit();
        }
        
        #endregion

        #region Set
        public void SetNodeDensity(int i, double density)
        {
            fieldNode[i].SetNodeDensity(density);
        }
        public void SetNodeGradient(int i, Vector3 g)
        {
            fieldNode[i].SetNodeGradient(g);
        }
        public void SetEnclosedDis(int i, double dis)
        {
            fieldNode[i].SetEnclosedParticleDis(dis);
        }

        #endregion
        
        #region Other Funtions
        
         public void KDEGpu(ParticleGroup pG,ComputeShader kde_Cs)
    {
        parPos = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float), ComputeBufferType.Default);
        nodePos = new ComputeBuffer(GetNodeNum(), 3 * sizeof(float), ComputeBufferType.Default);
        nodeDen = new ComputeBuffer(GetNodeNum(), sizeof(float));
        parDen = new ComputeBuffer(pG.GetParticlenum(), sizeof(float));
        slModified = new ComputeBuffer(pG.GetParticlenum(), 3 * sizeof(float));
        
        
        int kernel_Pilot = kde_Cs.FindKernel("Pilot");
        int kernel_ParDen = kde_Cs.FindKernel("ParDen");
        int kernel_SL_Modified = kde_Cs.FindKernel("SL_Modified");
        int kernel_FinalDensity = kde_Cs.FindKernel("FinalDensity");

        kde_Cs.SetVector("parMinPos_", new Vector4(pG.XMIN, pG.YMIN, pG.ZMIN, 0f));
        kde_Cs.SetVector("nodeStep_", new Vector4(xStep, yStep, zStep, 0f));
        kde_Cs.SetVector("nodeNum_", new Vector4(xNum, yNum, zNum, 0f));
        kde_Cs.SetFloat("parCount_",pG.GetParticlenum() );
        kde_Cs.SetVector("SL_", new Vector4(pG.GetSmoothLength()[0], pG.GetSmoothLength()[1], pG.GetSmoothLength()[2], 0f));
        
        parPos_ = new Vector3[pG.GetParticlenum()];
        parSL_ = new Vector3[pG.GetParticlenum()];
        gridPos_ = new Vector3[GetNodeNum()]; 
        dens = new float[GetNodeNum()];

        for (int i = 0; i < pG.GetParticlenum(); i++)
        {
            parPos_[i] = pG.GetParticleObjectPos(i);
        }
        for (int i = 0; i < GetNodeNum(); i++)
        {
            gridPos_[i] = GetNodedPos(i);
        }
        parPos.SetData(parPos_);
        nodePos.SetData(gridPos_);
        
        kde_Cs.SetBuffer(kernel_Pilot, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_Pilot, "nodePos_", nodePos);
        kde_Cs.SetBuffer(kernel_Pilot, "nodeDen_", nodeDen);
        
        kde_Cs.SetBuffer(kernel_ParDen, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_ParDen, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_ParDen, "parDen_", parDen);

        kde_Cs.SetBuffer(kernel_SL_Modified, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_SL_Modified, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_SL_Modified, "parDen_", parDen);
        kde_Cs.SetBuffer(kernel_SL_Modified, "SL_ModifiedRW_", slModified);
        
        kde_Cs.SetBuffer(kernel_FinalDensity, "partiPos_", parPos);
        kde_Cs.SetBuffer(kernel_FinalDensity, "nodePos_", nodePos);
        kde_Cs.SetBuffer(kernel_FinalDensity, "nodeDen_", nodeDen);
        kde_Cs.SetBuffer(kernel_FinalDensity, "SL_ModifiedRW_", slModified);
        
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
            kde_Cs.Dispatch(kernel_Pilot, XNUM / GROUPSIZE, YNUM / GROUPSIZE, ZNUM / GROUPSIZE);  
            kde_Cs.Dispatch(kernel_ParDen, pG.GetParticlenum() / PARGROUPSIZE, 1, 1);
            kde_Cs.Dispatch(kernel_SL_Modified, pG.GetParticlenum()  / PARGROUPSIZE, 1, 1);
            kde_Cs.Dispatch(kernel_FinalDensity, XNUM / GROUPSIZE, YNUM / GROUPSIZE, ZNUM / GROUPSIZE);
            nodeDen.GetData(dens);
            slModified.GetData(parSL_);
        sw.Stop();
        
        UnityEngine.Debug.Log("Kernel density estimation on GPU in " + sw.ElapsedMilliseconds+" ms");
        
        // //density normalization
        //
        //  //z-score method
        //  float sum = 0f;
        //  for (int i = 0; i < dens.Length; i++)
        //  {
        //      sum += dens[i];
        //  }
        //  float mean = sum / dens.Length;
        //
        //  sum = 0;
        //  for (int i = 0; i < dens.Length; i++)
        //  {
        //      sum += Mathf.Pow(dens[i]-mean,2);
        //  }
        //  float std =Mathf.Sqrt(sum / dens.Length) ;
        //  
        //  float min=float.MaxValue;float max=float.MinValue;
        //  for (int i = 0; i < dens.Length; i++)
        //  {
        //      dens[i] = (dens[i]-mean)/std;
        //      if(dens[i]>max)
        //          max=dens[i];
        //      if(dens[i]<min)
        //          min=dens[i];
        //  }
        //  
        //  //min-max method
        //  for (int i = 0; i < dens.Length; i++)
        //  {
        //      dens[i] = (dens[i]-min)/ (max-min);
        //  }
        
        
        
        
        //(1) set node density and (2) calculate the max, min, average node density
        float minNodeDen_=float.MaxValue;float maxNodeDen_=float.MinValue;
        float sumNodeDen_ = 0f;
        for (int i = 0; i < GetNodeNum(); i++)
        {
            sumNodeDen_ += dens[i];
            if(dens[i]>minNodeDen_)
                maxNodeDen_=dens[i];
            if(dens[i]<minNodeDen_)
                maxNodeDen_=dens[i];
            SetNodeDensity(i, dens[i]);
        }

        maxNodeDen = maxNodeDen_;
        minNodeDen = minNodeDen_;
        aveNodeDensity=sumNodeDen_ / GetNodeNum();
        
        
        //if the node is on the boundary, set density and gradient to 0/Vector.Zero. if not, calculate the gradient
        float delta = 0.1f * (xStep + yStep + zStep) / 3;
        for (int i = 0; i < GetNodeNum(); i++)
        {
            if (!NodeOnBoundary(i))
            {
                Vector3 nodePos = GetNodedPos(i);
                float gradx = ((float)Utility.InterpolateVector(new Vector3(nodePos.x + delta, nodePos.y, nodePos.z), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x - delta, nodePos.y, nodePos.z), pG, this)) / (2 * delta);
                float grady = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y + delta, nodePos.z), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y - delta, nodePos.z), pG, this)) / (2 * delta);
                float gradz = ((float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z + delta), pG, this) - (float)Utility.InterpolateVector(new Vector3(nodePos.x, nodePos.y, nodePos.z - delta), pG, this)) / (2 * delta);

                SetNodeGradient(i, new Vector3(gradx, grady, gradz));
            }
            else
            {
                SetNodeDensity(i, 0f);
                SetNodeGradient(i, Vector3.zero);
            }
        }

        //(1) calculate the particle density, (2) assign smooth length and (3) the max, min and average particle density
        float minParDen=float.MaxValue;float maxParDen=float.MinValue;
        float sumParDen = 0f;
        for (int i=0;i<pG.GetParticlenum(); i++)
        {
            float density=(float)Utility.InterpolateVector(pG.GetParticleObjectPos(i), pG, this);
            sumParDen += density;
            pG.SetParticleDensity(i,density);//set particle density
            if(density>maxParDen)
                maxParDen=density;
            if(density<minParDen)
                minParDen=density;
            pG.SetMySmoothLength(parSL_[i].x, parSL_[i].y, parSL_[i].z, i);//set smooth length of all the particles

            // List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
            // pG.SetFlowEnd(i, (v[v.Count - 1]));
        };
        pG.MAXPARDEN=maxParDen; pG.MINPARDEN=minParDen;
        pG.AVEPARDEN=sumParDen / pG.GetParticlenum();
        
        
        // Parallel.For(0, pG.GetParticlenum(), i =>
        // {
        //     pG.SetParticleDensity(i, Utility.InterpolateVector(pG.GetParticlePosition(i), pG, dF));//set particle density
        //     List<Vector3> v = Utility.Emit(pG.GetParticlePosition(i), Vector3.zero, dF, pG);
        //     pG.SetFlowEnd(i, (v[v.Count - 1]));
        // });


        UpdateLUT(pG);
        
        slModified.Release();
        parPos.Release();
        nodePos.Release();
        nodeDen.Release();
        parDen.Release();
    }
        public  Vector3 BoxIndexToNodeVector(int index)  
        {
            int z = index / (xNum * yNum);
            int left = index - z * xNum * yNum;
            int y = left / xNum;
            int x = left - y * xNum;
            return new Vector3(x, y, z);
        }
        public  int VectorToBoxIndex(Vector3 v, ParticleGroup pG)  //return the box index by inputting a random pos  //can judge the outside pointing,
        {
            int a = (int)((v.x - pG.XMIN) / XSTEP) + (int)((v.y - pG.YMIN) / YSTEP) * XNUM + (int)((v.z - pG.ZMIN) / ZSTEP) * XNUM * YNUM;
            if (a >= GetNodeNum() || a <= 0 || v.x > pG.XMAX || v.x < pG.XMIN || v.y > pG.YMAX || v.y < pG.YMIN || v.z > pG.ZMAX || v.z < pG.ZMIN)
            { return -1; }
            else
                return a;
        }

        public void CreateFieldFromPg(ParticleGroup pG, int xAxisNum, int yAxisNum, int zAxisNum)
        {
            float scalingFactor = 0f;
            float xBias = (pG.XMAX - pG.XMIN) / xAxisNum;
            float yBias = (pG.XMAX - pG.XMIN) / yAxisNum;
            float zBias = (pG.XMAX - pG.XMIN) / zAxisNum;
            this.InitializeField(pG.name, pG.XMIN-scalingFactor*xBias, pG.XMAX+scalingFactor*xBias, xAxisNum, pG.YMIN-scalingFactor*yBias, pG.YMAX+scalingFactor*yBias, yAxisNum, pG.ZMIN-scalingFactor*zBias, pG.ZMAX+scalingFactor*zBias, zAxisNum);
        }

        public void InitializeField(string pgName,float xmin, float xmax, int xAxisNum, float ymin, float ymax, int yAxisNum, float zmin, float zmax, int zAxisNum)
        {   
            name = pgName+"_DF";
            fieldNode = new List<FieldNode>();

            xStep = (xmax - xmin) / (xAxisNum-1);
            yStep = (ymax - ymin) / (yAxisNum-1);
            zStep = (zmax - zmin) / (zAxisNum-1);

            for (int k = 0; k < zAxisNum; k ++)
            {
                for (int j = 0; j < yAxisNum; j ++)
                {
                    for (int i = 0; i <xAxisNum; i ++)
                    {
                        FieldNode fd = new FieldNode(new Vector3(xmin+i*xStep,ymin+j*yStep,zmin+k*zStep), new Vector3(i, j, k));
                      
                        fieldNode.Add(fd);
                    }
                }
            }


            xNum = xAxisNum;
            yNum = yAxisNum;
            zNum = zAxisNum;
            Debug.Log("Create density field "+name+" success. xNodeNum: "+xNum+" yNodeNum: "+yNum+" zNodeNum: "+zNum+ " xStep: "+xStep+" yStep: "+yStep+" zStep: "+zStep);
        }
        public int NodePosToIndex(int z, int y, int x)
        {
            return (z) * xNum * yNum + (y) * xNum + x;

        }

        public void LUTInit()
        {
            LUT_ = new List<LUTUnit>();
            for (int i = 0; i < xNum*yNum*zNum; i++)
                LUT_.Add(new LUTUnit());
        }

        private void AddToLUT(int index, int targetint)
        {
            LUT_[index].AddToLUT(targetint);
        }
        
        public void UpdateLUT(ParticleGroup pG)
        {
            LUTInit();

            for (int i = 0; i < pG.GetParticlenum(); i++)
            {
                int x = Mathf.FloorToInt((pG.GetParticleObjectPos(i).x - pG.XMIN) / xStep);
                int y = Mathf.FloorToInt((pG.GetParticleObjectPos(i).y - pG.YMIN) / yStep);
                int z = Mathf.FloorToInt((pG.GetParticleObjectPos(i).z - pG.ZMIN) / zStep);
                int index = z * xNum * yNum + y * xNum + x;
                AddToLUT(index, i);
            }

        }
        
        public bool NodeOnBoundary(int index)
        {
            float _xNum, _yNum, _zNum = 0;
            _xNum = GetNodeGridPos(index).x;
            _yNum = GetNodeGridPos(index).y;
            _zNum = GetNodeGridPos(index).z;
            if (_xNum == 0 || _xNum == xNum - 1 || _xNum == xNum || _yNum == 0 || _yNum == yNum - 1 || _yNum == yNum || _zNum == 0 || _zNum == zNum - 1 || _zNum == zNum)
            { return true; }
            else
            {
                return false;
            }
        }
        #endregion
    }

    [System.Serializable]
    public class LUTUnit
    {[SerializeField]
        List<int> LUTUnit_;
        public LUTUnit()
        {
            LUTUnit_ = new List<int>();
        }
        public void AddToLUT(int targetint)
        {
            LUTUnit_.Add(targetint);
        }
        public int GetLTUnitNum()
        {
            return LUTUnit_.Count;
        }
        public List<int> GetLTUnit()
        {
            return LUTUnit_;
        }
    }

