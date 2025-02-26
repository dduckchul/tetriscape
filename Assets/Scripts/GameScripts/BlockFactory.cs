using System.Collections;
using GameScripts;
using UnityEngine;

// 블록 찍어내는 유형
public enum FactoryType
{
    Random,
    Sequential
}

public class BlockFactory : MonoBehaviour
{
    private int _sequentialIndex = 0;
    // 랜덤으로 생성할지, 정해진 순서대로 생성할지
    [SerializeField] private FactoryType _factoryType;
    // 정해진 순서에 따라 생성시 참조할 스크립터블 오브젝트 파일
    [SerializeField] public SequentialBlocks sequentialBlocks;
    public void SetFactoryType(FactoryType type)
    {
        _factoryType = type;
    }

    // 방어코드 작성, 암튼 조건에 안맞으면 랜덤으로 생성하도록 설정
    private FactoryType ValidateFactoryProcess()
    {
        if(_factoryType == FactoryType.Sequential && sequentialBlocks == null)
        {
            Debug.LogError("SequentialBlocks is null");
            return FactoryType.Random;
        }
        
        if(_factoryType == FactoryType.Sequential && (sequentialBlocks.blocks == null || sequentialBlocks.blocks.Length == 0))
        {
            Debug.LogError("SequentialBlocks is empty");
            return FactoryType.Random;
        }        
        
        return _factoryType;
    }
    
    public Block CreateBlock(Vector3 pos)
    {
        // 방어코딩. 명시적으로 한번 덮어써줌
        _factoryType = ValidateFactoryProcess();
        
        if(_factoryType == FactoryType.Random)
        {
            return CreateRandomBlock(pos);
        }
        else
        {
            return CreateSequentialBlock(pos);
        }
    }

    // 공장에서 랜덤 블록 만들기
    private Block CreateRandomBlock(Vector3 pos)
    {
        BlockEnum randomBlock = (BlockEnum)Random.Range((int)BlockEnum.IBlock, (int)BlockEnum.BlockEnd);
        // 특정 블록으로 테스트하기
        // randomBlock = BlockEnum.SquareBlock;
        BlockColor blockColor = BlockUtility.BlockToColor(randomBlock);
        GameObject blockObj = Instantiate(BlockUtility.EnumToPrefab(randomBlock));
        Block block = blockObj.GetComponent<Block>();
        block.InitBlock(randomBlock, blockColor, pos);

        return block;
    }
    
    // 공장에서 순차적 블록 만들기
    private Block CreateSequentialBlock(Vector3 pos)
    {
        BlockEnum blockEnum = sequentialBlocks.blocks[_sequentialIndex++];
        BlockColor blockColor = BlockUtility.BlockToColor(blockEnum);
        GameObject blockObj = Instantiate(BlockUtility.EnumToPrefab(blockEnum));
        Block block = blockObj.GetComponent<Block>();
        block.InitBlock(blockEnum, blockColor, pos);
        
        if(_sequentialIndex >= sequentialBlocks.blocks.Length)
        {
            _sequentialIndex = 0;
        }

        return block;
    }
}