using UnityEngine;

namespace UTJ
{
    // スプリングボーン用の力を与えるベースクラス
    public class ForceProvider : MonoBehaviour
    {
        public virtual Vector3 GetForceOnBone(SpringBone springBone)
        {
            return Vector3.zero;
        }
    }
}