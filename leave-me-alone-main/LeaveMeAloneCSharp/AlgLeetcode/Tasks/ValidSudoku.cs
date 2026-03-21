public static class ValidSudoku
{
    //Runtime 2ms Beats 87.29%
    public static bool IsValidSudoku(char[][] board)
    {
        var rws = new HashSet<char>[9];
        var clms = new HashSet<char>[9];
        var bxs = new HashSet<char>[9];

        //3х3
        for (int i = 0; i < 9; i++)
        {
            rws[i] = new HashSet<char>();
            clms[i] = new HashSet<char>();
            bxs[i] = new HashSet<char>();
        }

        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                char num = board[r][c];
                if(!char.IsDigit(num)) continue;
                
                int bxIndx =  (r/3)*3 + (c/3);
                if(rws[r].Contains(num) || clms[c].Contains(num)|| bxs[bxIndx].Contains(num)){
                    return false;
                } 

                rws[r].Add(num);
                clms[r].Add(num);
                bxs[bxIndx].Add(num);
            }
        }

        return true;
    }
}