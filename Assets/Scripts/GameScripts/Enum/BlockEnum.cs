/* 프리팹으로 저장해둔 블록들*/ 
public enum BlockEnum
{
    IBlock, LBlock, RLBlock, RZBlock, SquareBlock, TBlock, ZBlock, BlockEnd
}


/* 이상한 블록들 모음.. ex) 뿌요? */ 
public enum UnusualBlockEnum
{
    
}

// Light Gray 색은 일반적으로 안쓰도록. (디폴트랑 색상 같음)
public enum BlockColor
{
    LightRed, LightGreen, Blue, LightYellow, 
    LightPurple, SkyBlue, LightBlue, LightGray
}