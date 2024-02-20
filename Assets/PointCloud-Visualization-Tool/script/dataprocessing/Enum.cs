using System;

namespace LixaingZhao.PointcloudTool
{
    public class Enum
    {
        [Serializable]
        public enum Dataset { disk, uniform_Lines, ball_hemisphere, ununiform_Lines, Flocculentcube1, Flocculentcube2, nbody1, nbody2, training_torus, training_sphere, training_pyramid, training_cylinder, three_rings, fiveellipsolds, dragon_vrip };
        [Serializable]
        public enum GRIDNum { grid64, grid100, grid200 };
    }
}