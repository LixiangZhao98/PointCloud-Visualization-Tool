using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enum 
{
[Serializable]
public enum Dataset { disk,uniform_Lines, ball_hemisphere, ununiform_Lines, Flocculentcube1, Flocculentcube2, nbody1, nbody2, training_torus ,training_sphere,training_pyramid,training_cylinder, three_rings, fiveellipsolds};
[Serializable]
public enum GRIDNum { none,grid64,grid100,grid200 };
}
