namespace Sample._1_Adventure.Scripts.Data
{
    /// <summary>
    /// ゲームで使用するパラメータ
    /// </summary>
    public static class AdvParameter
    {
        /// <summary>
        /// 現在のマップ番号
        /// <para>デフォルトは1だが、任意のマップからテストしたい時は変更する</para>
        /// </summary>
        public static int CurrentMapNumber = 1;

        // マップ1用のパラメータ
        public static bool _1_isEndStartLabel;
        public static bool _1_isGetMashRoom;
        
        // マップ2用のパラメータ
        public static bool _2_isGetAcorn1;
        public static bool _2_isGetAcorn2;
    }
}