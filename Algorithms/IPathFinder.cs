using System.Collections.Generic;
using System.Drawing;

namespace Algorithms
{
    [Author("Franco, Gustavo")]
    interface IPathFinder
    {
        #region Events
        event PathFinderDebugHandler PathFinderDebug;
        #endregion

        #region Properties
        bool Stopped
        {
            get;
        }

        HeuristicFormula Formula
        {
            get;
            set;
        }

        bool Diagonals //对角线的
        {
            get;
            set;
        }

        bool HeavyDiagonals
        {
            get;
            set;
        }

        int HeuristicEstimate //启发系数
        {
            get;
            set;
        }

        bool PunishChangeDirection //惩罚变换方向
        {
            get;
            set;
        }

        bool TieBreaker // 引入微小变量，解决比较时相等引入多分支的问题
        {
            get;
            set;
        }

        int SearchLimit ///Closed节点数限制，即找不到路径
        {
            get;
            set;
        }

        double CompletedTime
        {
            get;
            set;
        }

        bool DebugProgress
        {
            get;
            set;
        }

        bool DebugFoundPath
        {
            get;
            set;
        }
        #endregion

        #region Methods
        void FindPathStop();
        List<PathFinderNode> FindPath(Point start, Point end);
        #endregion

    }
}
