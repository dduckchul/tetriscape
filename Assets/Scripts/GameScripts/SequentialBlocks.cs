using UnityEngine;

namespace GameScripts
{
    [CreateAssetMenu(fileName = "SequaltialBlocks", menuName = "Blocks/SequentialBlocks", order = 0)]
    public class SequentialBlocks : ScriptableObject
    {
        public BlockEnum[] blocks;
    }
}