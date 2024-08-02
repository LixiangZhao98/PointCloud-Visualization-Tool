﻿using System;
using System.Collections.Generic;
using UnityEngine;


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
        [SerializeField]
        private float AveNodeDensity;
        public float XSTEP { get { return xStep; } }
        public float YSTEP { get { return yStep; } }
        public float ZSTEP { get { return zStep; } }
        public int XNUM { get { return xNum; } }
        public int YNUM { get { return yNum; } }
        public int ZNUM { get { return zNum; } }

        public float AVE_NODE_DENSITY { get { return AveNodeDensity; } }

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

        public void CreateField(string pgName, ParticleGroup pG, int xAxisNum, int yAxisNum, int zAxisNum)
        {
            this.InitializeDensityFieldByGapDis(pgName, pG.XMIN, pG.XMAX, xAxisNum, pG.YMIN, pG.YMAX, yAxisNum, pG.ZMIN, pG.ZMAX, zAxisNum);
        }

        public void InitializeDensityFieldByGapDis(string pgName,float xmin, float xmax, int xAxisNum, float ymin, float ymax, int yAxisNum, float zmin, float zmax, int zAxisNum)
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

        public void AddToLUT(int index, int targetint)
        {
            LUT_[index].AddToLUT(targetint);
        }

       
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
        public void SetAveNodeDensity(float f)
        {
            AveNodeDensity = f;
        }
        public void SetEnclosedDis(int i, double dis)
        {
            fieldNode[i].SetEnclosedParticleDis(dis);
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

