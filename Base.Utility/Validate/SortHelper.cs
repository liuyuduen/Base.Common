using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility
{
    /// <summary>
    /// 排序工具
    /// 示例调用代码：
    ///  SortTool sort = new SortTool();
    ///  IList<string> results = sort.ComputeAndSort(new string[] { "1101", "1102", "1103" }, list);
    /// </summary>
    public class SortHelper
    {

        //定义行映射字典和列映射字典
        private Dictionary<string, int> MappingForRow = new Dictionary<string, int>();
        private Dictionary<string, int> MappingForColumn = new Dictionary<string, int>();

        /// <summary>
        /// 把列表格式的输入参数转成排序项的数据结构(二维数组，一个代表位数，如：万千百十个；一个代表具体字段，如：0123456789)
        /// </summary>
        /// <param name="list">外部的输入值
        /// 格式形如:
        /// 1101001,0.8
        /// 1101002,0.2
        /// 1102001,0.3
        /// 1102002,0.1
        /// </param>
        /// <returns>返回排序项的二维数组</returns>
        private SortItem[][] ListToArray(IList<string> list)
        {

            List<SortItem> results = new List<SortItem>();
            int maxColumnIndex = -1;//最大列下标
            int maxRowIndex = -1;//最大行小标

            //遍历输入列表，给SortItem赋值并顺便获得最大行下标和列下标，便于定义二维数组
            foreach (string item in list)
            {
                //item的值举例："1101001:0.85"

                string[] arr = item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);//分解成编号和遗漏值比（或连出值比）：1101001是字段编号，0.85是遗漏值比
                if (arr.Length == 2)
                {
                    //给排序项赋值"1101001:0.85"->sortitem
                    SortItem sortitem = new SortItem
                    {
                        intRowIndex = MappingForRow[arr[0].Substring(0, 4)],//将字段编号1101001的前四位1101提取出来，获得行下标
                        intColumnIndex = MappingForColumn[arr[0].Substring(4, 3)],//将字段编号1101001的后三位001提取出来，获得列下标
                        dblValue = float.Parse(arr[1]),//将遗漏比值(或连出值比)0.85赋值
                        strFieldId = arr[0]//记录编号1101001
                    };
                    if (maxColumnIndex < sortitem.intColumnIndex)
                        maxColumnIndex = sortitem.intColumnIndex;
                    if (maxRowIndex < sortitem.intRowIndex)
                        maxRowIndex = sortitem.intRowIndex;

                    results.Add(sortitem);
                }
            }

            //定义二维数组
            SortItem[][] arr2 = new SortItem[maxRowIndex + 1][];
            for (int i = 0; i < arr2.Length; i++)
            {
                arr2[i] = new SortItem[maxColumnIndex + 1];
            }

            //将每一个从sortitem赋值给二维数组
            foreach (SortItem item in results)
            {
                arr2[item.intRowIndex][item.intColumnIndex] = item;
            }

            return arr2;
        }

        /// <summary>
        /// 批量乘法运算
        /// </summary>
        /// <param name="firstList">乘数列表</param>
        /// <param name="secondList">被乘数列表</param>
        /// <param name="intMode">1=排列，2=组合</param>
        /// <returns>返回相乘结果的数组,根据inMode的值返回全排列或组合的结果</returns>
        private SortItem[] BatchMultiplication(SortItem[] firstList, SortItem[] secondList, int intMode)
        {
            List<SortItem> list = new List<SortItem>();

            if (intMode == 1)
            {
                foreach (SortItem item in firstList)
                {
                    foreach (SortItem item2 in secondList)
                    {
                        list.Add(item * item2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < firstList.Length; i++)
                {
                    for (int j = i + 1; j < secondList.Length; j++)
                    {
                        list.Add(firstList[i] * secondList[j]);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 计算并排序（主方法）
        /// </summary>
        /// <param name="tableIds">表编号列表，如："1101,1102,1103"代表参与表为万位表、千位表和百位表,注意列表元素顺序要按万千百十个的顺序！</param>
        /// <param name="list">输入数据
        /// 格式形如:
        /// 1101001:0.8
        /// 1101002:0.2
        /// 1102001:0.3
        /// 1102002:0.1
        /// </param>
        /// <param name="intMode">1=排列，2=组合</param>
        /// <returns></returns>
        public IList<string> ComputeAndSort(IList<string> tableIds, IList<string> list, int intMode)
        {
            if (tableIds == null || tableIds.Count == 0) return null;


            #region 生成行、列映射字典

            //利用提供的表编号列表自动生成行映射字典，如：1101,0;1102,1;1103,2;1104
            if (MappingForRow.Count == 0)
            {
                for (int i = 0; i < tableIds.Count; i++)
                {
                    MappingForRow.Add(tableIds[i].Substring(0, 4), i);
                }
            }

            //生成列映射字典
            if (MappingForColumn.Count == 0)
            {
                List<int> tempInt = new List<int>();
                //遍历列表数据（元素格式：1101001:0.85）
                foreach (string item in list)
                {
                    if (item != null && string.IsNullOrEmpty(item.Trim())) continue;
                    int t = int.Parse(item.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[0].Substring(4, 3));//取出字段编号1101001的后三位001
                    //组装不重复元素列表
                    if (!tempInt.Contains(t))
                    {
                        tempInt.Add(t);
                    }
                }
                tempInt.Sort();//排序
                for (int i = 0; i < tempInt.Count; i++)
                {
                    MappingForColumn.Add(tempInt[i].ToString("D3"), i);
                }
            }

            #endregion

            List<SortItem> results = new List<SortItem>();
            SortItem[][] array = ListToArray(list);//列表转二维数组

            SortItem[] first = array[0];//初始化第一行
            for (int i = 1; i < array.Length; i++)
            {
                first = BatchMultiplication(first, array[i], intMode);//批量乘法运算
            }

            //倒排
            List<SortItem> listSort = new List<SortItem>(first);
            listSort.Sort(new Comparison<SortItem>((a, b) =>
            {
                if (a.dblValue > b.dblValue)
                    return -1;
                if (a.dblValue < b.dblValue)
                    return 1;
                return 0;
            }));

            List<string> resutlSorted = new List<string>();
            foreach (SortItem item in listSort)
            {
                resutlSorted.Add(item.ToString());//将列表元素1101002,1102006,1103004转为"021"输出
            }

            //释放相关资源并返回结果
            MappingForRow.Clear();
            MappingForColumn.Clear();
            listSort.Clear();
            return resutlSorted;
        }
    }

    #region 辅助类

    /// <summary>
    /// 表示的是用于排序的每一项，由数据字段编号和遗漏比值组成的字符串，比如:"1101001,0.8"
    /// </summary>
    public class SortItem
    {
        /// <summary>
        /// 映射字典
        /// </summary>
        public static Dictionary<string, string> Mapping = new Dictionary<string, string>();
        static SortItem()
        {

            #region 时时彩万位 1101

            Mapping.Add("1101001", "0"); //NCHaoma0
            Mapping.Add("1101002", "0"); //YLHaoma0
            Mapping.Add("1101003", "1"); //NCHaoma1
            Mapping.Add("1101004", "1"); //YLHaoma1
            Mapping.Add("1101005", "2"); //NCHaoma2
            Mapping.Add("1101006", "2"); //YLHaoma2
            Mapping.Add("1101007", "3"); //NCHaoma3
            Mapping.Add("1101008", "3"); //YLHaoma3
            Mapping.Add("1101009", "4"); //NCHaoma4
            Mapping.Add("1101010", "4"); //YLHaoma4
            Mapping.Add("1101011", "5"); //NCHaoma5
            Mapping.Add("1101012", "5"); //YLHaoma5
            Mapping.Add("1101013", "6"); //NCHaoma6
            Mapping.Add("1101014", "6"); //YLHaoma6
            Mapping.Add("1101015", "7"); //NCHaoma7
            Mapping.Add("1101016", "7"); //YLHaoma7
            Mapping.Add("1101017", "8"); //NCHaoma8
            Mapping.Add("1101018", "8"); //YLHaoma8
            Mapping.Add("1101019", "9"); //NCHaoma9
            Mapping.Add("1101020", "9"); //YLHaoma9
            Mapping.Add("1101021", "大"); //NCDa
            Mapping.Add("1101022", "大"); //YLDa
            Mapping.Add("1101023", "小"); //NCXiao
            Mapping.Add("1101024", "小"); //YLXiao
            Mapping.Add("1101025", "奇"); //NCJi
            Mapping.Add("1101026", "奇"); //YLJi
            Mapping.Add("1101027", "偶"); //NCOu
            Mapping.Add("1101028", "偶"); //YLOu
            Mapping.Add("1101029", "质"); //NCZhi
            Mapping.Add("1101030", "质"); //YLZhi
            Mapping.Add("1101031", "合"); //NCHe
            Mapping.Add("1101032", "合"); //YLHe
            Mapping.Add("1101033", "0"); //NC0120
            Mapping.Add("1101034", "0"); //YL0120
            Mapping.Add("1101035", "1"); //NC0121
            Mapping.Add("1101036", "1"); //YL0121
            Mapping.Add("1101037", "2"); //NC0122
            Mapping.Add("1101038", "2"); //YL0122
            Mapping.Add("1101039", "0"); //NCBuchang0
            Mapping.Add("1101040", "0"); //YLBuchang0
            Mapping.Add("1101041", "1"); //NCBuchang1
            Mapping.Add("1101042", "1"); //YLBuchang1
            Mapping.Add("1101043", "2"); //NCBuchang2
            Mapping.Add("1101044", "2"); //YLBuchang2
            Mapping.Add("1101045", "3"); //NCBuchang3
            Mapping.Add("1101046", "3"); //YLBuchang3
            Mapping.Add("1101047", "4"); //NCBuchang4
            Mapping.Add("1101048", "4"); //YLBuchang4
            Mapping.Add("1101049", "5"); //NCBuchang5
            Mapping.Add("1101050", "5"); //YLBuchang5
            Mapping.Add("1101051", "6"); //NCBuchang6
            Mapping.Add("1101052", "6"); //YLBuchang6
            Mapping.Add("1101053", "7"); //NCBuchang7
            Mapping.Add("1101054", "7"); //YLBuchang7
            Mapping.Add("1101055", "8"); //NCBuchang8
            Mapping.Add("1101056", "8"); //YLBuchang8
            Mapping.Add("1101057", "9"); //NCBuchang9
            Mapping.Add("1101058", "9"); //YLBuchang9

            #endregion

            #region 时时彩千位 1102

            Mapping.Add("1102001", "0"); //NCHaoma0
            Mapping.Add("1102002", "0"); //YLHaoma0
            Mapping.Add("1102003", "1"); //NCHaoma1
            Mapping.Add("1102004", "1"); //YLHaoma1
            Mapping.Add("1102005", "2"); //NCHaoma2
            Mapping.Add("1102006", "2"); //YLHaoma2
            Mapping.Add("1102007", "3"); //NCHaoma3
            Mapping.Add("1102008", "3"); //YLHaoma3
            Mapping.Add("1102009", "4"); //NCHaoma4
            Mapping.Add("1102010", "4"); //YLHaoma4
            Mapping.Add("1102011", "5"); //NCHaoma5
            Mapping.Add("1102012", "5"); //YLHaoma5
            Mapping.Add("1102013", "6"); //NCHaoma6
            Mapping.Add("1102014", "6"); //YLHaoma6
            Mapping.Add("1102015", "7"); //NCHaoma7
            Mapping.Add("1102016", "7"); //YLHaoma7
            Mapping.Add("1102017", "8"); //NCHaoma8
            Mapping.Add("1102018", "8"); //YLHaoma8
            Mapping.Add("1102019", "9"); //NCHaoma9
            Mapping.Add("1102020", "9"); //YLHaoma9
            Mapping.Add("1102021", "大"); //NCDa
            Mapping.Add("1102022", "大"); //YLDa
            Mapping.Add("1102023", "小"); //NCXiao
            Mapping.Add("1102024", "小"); //YLXiao
            Mapping.Add("1102025", "奇"); //NCJi
            Mapping.Add("1102026", "奇"); //YLJi
            Mapping.Add("1102027", "偶"); //NCOu
            Mapping.Add("1102028", "偶"); //YLOu
            Mapping.Add("1102029", "质"); //NCZhi
            Mapping.Add("1102030", "质"); //YLZhi
            Mapping.Add("1102031", "合"); //NCHe
            Mapping.Add("1102032", "合"); //YLHe
            Mapping.Add("1102033", "0"); //NC0120
            Mapping.Add("1102034", "0"); //YL0120
            Mapping.Add("1102035", "1"); //NC0121
            Mapping.Add("1102036", "1"); //YL0121
            Mapping.Add("1102037", "2"); //NC0122
            Mapping.Add("1102038", "2"); //YL0122
            Mapping.Add("1102039", "0"); //NCBuchang0
            Mapping.Add("1102040", "0"); //YLBuchang0
            Mapping.Add("1102041", "1"); //NCBuchang1
            Mapping.Add("1102042", "1"); //YLBuchang1
            Mapping.Add("1102043", "2"); //NCBuchang2
            Mapping.Add("1102044", "2"); //YLBuchang2
            Mapping.Add("1102045", "3"); //NCBuchang3
            Mapping.Add("1102046", "3"); //YLBuchang3
            Mapping.Add("1102047", "4"); //NCBuchang4
            Mapping.Add("1102048", "4"); //YLBuchang4
            Mapping.Add("1102049", "5"); //NCBuchang5
            Mapping.Add("1102050", "5"); //YLBuchang5
            Mapping.Add("1102051", "6"); //NCBuchang6
            Mapping.Add("1102052", "6"); //YLBuchang6
            Mapping.Add("1102053", "7"); //NCBuchang7
            Mapping.Add("1102054", "7"); //YLBuchang7
            Mapping.Add("1102055", "8"); //NCBuchang8
            Mapping.Add("1102056", "8"); //YLBuchang8
            Mapping.Add("1102057", "9"); //NCBuchang9
            Mapping.Add("1102058", "9"); //YLBuchang9

            #endregion

            #region 时时彩百位 1103

            Mapping.Add("1103001", "0"); //NCHaoma0
            Mapping.Add("1103002", "0"); //YLHaoma0
            Mapping.Add("1103003", "1"); //NCHaoma1
            Mapping.Add("1103004", "1"); //YLHaoma1
            Mapping.Add("1103005", "2"); //NCHaoma2
            Mapping.Add("1103006", "2"); //YLHaoma2
            Mapping.Add("1103007", "3"); //NCHaoma3
            Mapping.Add("1103008", "3"); //YLHaoma3
            Mapping.Add("1103009", "4"); //NCHaoma4
            Mapping.Add("1103010", "4"); //YLHaoma4
            Mapping.Add("1103011", "5"); //NCHaoma5
            Mapping.Add("1103012", "5"); //YLHaoma5
            Mapping.Add("1103013", "6"); //NCHaoma6
            Mapping.Add("1103014", "6"); //YLHaoma6
            Mapping.Add("1103015", "7"); //NCHaoma7
            Mapping.Add("1103016", "7"); //YLHaoma7
            Mapping.Add("1103017", "8"); //NCHaoma8
            Mapping.Add("1103018", "8"); //YLHaoma8
            Mapping.Add("1103019", "9"); //NCHaoma9
            Mapping.Add("1103020", "9"); //YLHaoma9
            Mapping.Add("1103021", "大");//NCDa
            Mapping.Add("1103022", "大");//YLDa
            Mapping.Add("1103023", "小");//NCXiao
            Mapping.Add("1103024", "小");//YLXiao
            Mapping.Add("1103025", "奇");//NCJi
            Mapping.Add("1103026", "奇");//YLJi
            Mapping.Add("1103027", "偶");//NCOu
            Mapping.Add("1103028", "偶");//YLOu
            Mapping.Add("1103029", "质");//NCZhi
            Mapping.Add("1103030", "质");//YLZhi
            Mapping.Add("1103031", "合");//NCHe
            Mapping.Add("1103032", "合");//YLHe
            Mapping.Add("1103033", "0"); //NC0120
            Mapping.Add("1103034", "0"); //YL0120
            Mapping.Add("1103035", "1"); //NC0121
            Mapping.Add("1103036", "1"); //YL0121
            Mapping.Add("1103037", "2"); //NC0122
            Mapping.Add("1103038", "2"); //YL0122
            Mapping.Add("1103039", "0"); //NCBuchang0
            Mapping.Add("1103040", "0"); //YLBuchang0
            Mapping.Add("1103041", "1"); //NCBuchang1
            Mapping.Add("1103042", "1"); //YLBuchang1
            Mapping.Add("1103043", "2"); //NCBuchang2
            Mapping.Add("1103044", "2"); //YLBuchang2
            Mapping.Add("1103045", "3"); //NCBuchang3
            Mapping.Add("1103046", "3"); //YLBuchang3
            Mapping.Add("1103047", "4"); //NCBuchang4
            Mapping.Add("1103048", "4"); //YLBuchang4
            Mapping.Add("1103049", "5"); //NCBuchang5
            Mapping.Add("1103050", "5"); //YLBuchang5
            Mapping.Add("1103051", "6"); //NCBuchang6
            Mapping.Add("1103052", "6"); //YLBuchang6
            Mapping.Add("1103053", "7"); //NCBuchang7
            Mapping.Add("1103054", "7"); //YLBuchang7
            Mapping.Add("1103055", "8"); //NCBuchang8
            Mapping.Add("1103056", "8"); //YLBuchang8
            Mapping.Add("1103057", "9"); //NCBuchang9
            Mapping.Add("1103058", "9"); //YLBuchang9

            #endregion

            #region 时时彩十位 1104

            Mapping.Add("1104001", "0"); //NCHaoma0
            Mapping.Add("1104002", "0"); //YLHaoma0
            Mapping.Add("1104003", "1"); //NCHaoma1
            Mapping.Add("1104004", "1"); //YLHaoma1
            Mapping.Add("1104005", "2"); //NCHaoma2
            Mapping.Add("1104006", "2"); //YLHaoma2
            Mapping.Add("1104007", "3"); //NCHaoma3
            Mapping.Add("1104008", "3"); //YLHaoma3
            Mapping.Add("1104009", "4"); //NCHaoma4
            Mapping.Add("1104010", "4"); //YLHaoma4
            Mapping.Add("1104011", "5"); //NCHaoma5
            Mapping.Add("1104012", "5"); //YLHaoma5
            Mapping.Add("1104013", "6"); //NCHaoma6
            Mapping.Add("1104014", "6"); //YLHaoma6
            Mapping.Add("1104015", "7"); //NCHaoma7
            Mapping.Add("1104016", "7"); //YLHaoma7
            Mapping.Add("1104017", "8"); //NCHaoma8
            Mapping.Add("1104018", "8"); //YLHaoma8
            Mapping.Add("1104019", "9"); //NCHaoma9
            Mapping.Add("1104020", "9"); //YLHaoma9
            Mapping.Add("1104021", "大");//NCDa
            Mapping.Add("1104022", "大");//YLDa
            Mapping.Add("1104023", "小");//NCXiao
            Mapping.Add("1104024", "小");//YLXiao
            Mapping.Add("1104025", "奇");//NCJi
            Mapping.Add("1104026", "奇");//YLJi
            Mapping.Add("1104027", "偶");//NCOu
            Mapping.Add("1104028", "偶");//YLOu
            Mapping.Add("1104029", "质");//NCZhi
            Mapping.Add("1104030", "质");//YLZhi
            Mapping.Add("1104031", "合");//NCHe
            Mapping.Add("1104032", "合");//YLHe
            Mapping.Add("1104033", "0"); //NC0120
            Mapping.Add("1104034", "0"); //YL0120
            Mapping.Add("1104035", "1"); //NC0121
            Mapping.Add("1104036", "1"); //YL0121
            Mapping.Add("1104037", "2"); //NC0122
            Mapping.Add("1104038", "2"); //YL0122
            Mapping.Add("1104039", "0"); //NCBuchang0
            Mapping.Add("1104040", "0"); //YLBuchang0
            Mapping.Add("1104041", "1"); //NCBuchang1
            Mapping.Add("1104042", "1"); //YLBuchang1
            Mapping.Add("1104043", "2"); //NCBuchang2
            Mapping.Add("1104044", "2"); //YLBuchang2
            Mapping.Add("1104045", "3"); //NCBuchang3
            Mapping.Add("1104046", "3"); //YLBuchang3
            Mapping.Add("1104047", "4"); //NCBuchang4
            Mapping.Add("1104048", "4"); //YLBuchang4
            Mapping.Add("1104049", "5"); //NCBuchang5
            Mapping.Add("1104050", "5"); //YLBuchang5
            Mapping.Add("1104051", "6"); //NCBuchang6
            Mapping.Add("1104052", "6"); //YLBuchang6
            Mapping.Add("1104053", "7"); //NCBuchang7
            Mapping.Add("1104054", "7"); //YLBuchang7
            Mapping.Add("1104055", "8"); //NCBuchang8
            Mapping.Add("1104056", "8"); //YLBuchang8
            Mapping.Add("1104057", "9"); //NCBuchang9
            Mapping.Add("1104058", "9"); //YLBuchang9

            #endregion

            #region 时时彩个位 1105

            Mapping.Add("1105001", "0"); //NCHaoma0
            Mapping.Add("1105002", "0"); //YLHaoma0
            Mapping.Add("1105003", "1"); //NCHaoma1
            Mapping.Add("1105004", "1"); //YLHaoma1
            Mapping.Add("1105005", "2"); //NCHaoma2
            Mapping.Add("1105006", "2"); //YLHaoma2
            Mapping.Add("1105007", "3"); //NCHaoma3
            Mapping.Add("1105008", "3"); //YLHaoma3
            Mapping.Add("1105009", "4"); //NCHaoma4
            Mapping.Add("1105010", "4"); //YLHaoma4
            Mapping.Add("1105011", "5"); //NCHaoma5
            Mapping.Add("1105012", "5"); //YLHaoma5
            Mapping.Add("1105013", "6"); //NCHaoma6
            Mapping.Add("1105014", "6"); //YLHaoma6
            Mapping.Add("1105015", "7"); //NCHaoma7
            Mapping.Add("1105016", "7"); //YLHaoma7
            Mapping.Add("1105017", "8"); //NCHaoma8
            Mapping.Add("1105018", "8"); //YLHaoma8
            Mapping.Add("1105019", "9"); //NCHaoma9
            Mapping.Add("1105020", "9"); //YLHaoma9
            Mapping.Add("1105021", "大"); //NCDa
            Mapping.Add("1105022", "大"); //YLDa
            Mapping.Add("1105023", "小"); //NCXiao
            Mapping.Add("1105024", "小"); //YLXiao
            Mapping.Add("1105025", "奇"); //NCJi
            Mapping.Add("1105026", "奇"); //YLJi
            Mapping.Add("1105027", "偶"); //NCOu
            Mapping.Add("1105028", "偶"); //YLOu
            Mapping.Add("1105029", "质"); //NCZhi
            Mapping.Add("1105030", "质"); //YLZhi
            Mapping.Add("1105031", "合"); //NCHe
            Mapping.Add("1105032", "合"); //YLHe
            Mapping.Add("1105033", "0"); //NC0120
            Mapping.Add("1105034", "0"); //YL0120
            Mapping.Add("1105035", "1"); //NC0121
            Mapping.Add("1105036", "1"); //YL0121
            Mapping.Add("1105037", "2"); //NC0122
            Mapping.Add("1105038", "2"); //YL0122
            Mapping.Add("1105039", "0"); //NCBuchang0
            Mapping.Add("1105040", "0"); //YLBuchang0
            Mapping.Add("1105041", "1"); //NCBuchang1
            Mapping.Add("1105042", "1"); //YLBuchang1
            Mapping.Add("1105043", "2"); //NCBuchang2
            Mapping.Add("1105044", "2"); //YLBuchang2
            Mapping.Add("1105045", "3"); //NCBuchang3
            Mapping.Add("1105046", "3"); //YLBuchang3
            Mapping.Add("1105047", "4"); //NCBuchang4
            Mapping.Add("1105048", "4"); //YLBuchang4
            Mapping.Add("1105049", "5"); //NCBuchang5
            Mapping.Add("1105050", "5"); //YLBuchang5
            Mapping.Add("1105051", "6"); //NCBuchang6
            Mapping.Add("1105052", "6"); //YLBuchang6
            Mapping.Add("1105053", "7"); //NCBuchang7
            Mapping.Add("1105054", "7"); //YLBuchang7
            Mapping.Add("1105055", "8"); //NCBuchang8
            Mapping.Add("1105056", "8"); //YLBuchang8
            Mapping.Add("1105057", "9"); //NCBuchang9
            Mapping.Add("1105058", "9"); //YLBuchang9

            #endregion

            #region 时时彩五星 1106

            Mapping.Add("1106001", "0"); //NCFenbu0
            Mapping.Add("1106002", "0"); //YLFenbu0
            Mapping.Add("1106003", "1"); //NCFenbu1
            Mapping.Add("1106004", "1"); //YLFenbu1
            Mapping.Add("1106005", "2"); //NCFenbu2
            Mapping.Add("1106006", "2"); //YLFenbu2
            Mapping.Add("1106007", "3"); //NCFenbu3
            Mapping.Add("1106008", "3"); //YLFenbu3
            Mapping.Add("1106009", "4"); //NCFenbu4
            Mapping.Add("1106010", "4"); //YLFenbu4
            Mapping.Add("1106011", "5"); //NCFenbu5
            Mapping.Add("1106012", "5"); //YLFenbu5
            Mapping.Add("1106013", "6"); //NCFenbu6
            Mapping.Add("1106014", "6"); //YLFenbu6
            Mapping.Add("1106015", "7"); //NCFenbu7
            Mapping.Add("1106016", "7"); //YLFenbu7
            Mapping.Add("1106017", "8"); //NCFenbu8
            Mapping.Add("1106018", "8"); //YLFenbu8
            Mapping.Add("1106019", "9"); //NCFenbu9
            Mapping.Add("1106020", "9"); //YLFenbu9
            Mapping.Add("1106021", "0"); //NCKuadu0
            Mapping.Add("1106022", "0"); //YLKuadu0
            Mapping.Add("1106023", "1"); //NCKuadu1
            Mapping.Add("1106024", "1"); //YLKuadu1
            Mapping.Add("1106025", "2"); //NCKuadu2
            Mapping.Add("1106026", "2"); //YLKuadu2
            Mapping.Add("1106027", "3"); //NCKuadu3
            Mapping.Add("1106028", "3"); //YLKuadu3
            Mapping.Add("1106029", "4"); //NCKuadu4
            Mapping.Add("1106030", "4"); //YLKuadu4
            Mapping.Add("1106031", "5"); //NCKuadu5
            Mapping.Add("1106032", "5"); //YLKuadu5
            Mapping.Add("1106033", "6"); //NCKuadu6
            Mapping.Add("1106034", "6"); //YLKuadu6
            Mapping.Add("1106035", "7"); //NCKuadu7
            Mapping.Add("1106036", "7"); //YLKuadu7
            Mapping.Add("1106037", "8"); //NCKuadu8
            Mapping.Add("1106038", "8"); //YLKuadu8
            Mapping.Add("1106039", "9"); //NCKuadu9
            Mapping.Add("1106040", "9"); //YLKuadu9
            Mapping.Add("1106041", "50"); //NCDaxiaobi50
            Mapping.Add("1106042", "50"); //YLDaxiaobi50
            Mapping.Add("1106043", "41"); //NCDaxiaobi41
            Mapping.Add("1106044", "41"); //YLDaxiaobi41
            Mapping.Add("1106045", "32"); //NCDaxiaobi32
            Mapping.Add("1106046", "32"); //YLDaxiaobi32
            Mapping.Add("1106047", "23"); //NCDaxiaobi23
            Mapping.Add("1106048", "23"); //YLDaxiaobi23
            Mapping.Add("1106049", "14"); //NCDaxiaobi14
            Mapping.Add("1106050", "14"); //YLDaxiaobi14
            Mapping.Add("1106051", "05"); //NCDaxiaobi05
            Mapping.Add("1106052", "05"); //YLDaxiaobi05
            Mapping.Add("1106053", "50"); //NCJioubi50
            Mapping.Add("1106054", "50"); //YLJioubi50
            Mapping.Add("1106055", "41"); //NCJioubi41
            Mapping.Add("1106056", "41"); //YLJioubi41
            Mapping.Add("1106057", "32"); //NCJioubi32
            Mapping.Add("1106058", "32"); //YLJioubi32
            Mapping.Add("1106059", "23"); //NCJioubi23
            Mapping.Add("1106060", "23"); //YLJioubi23
            Mapping.Add("1106061", "14"); //NCJioubi14
            Mapping.Add("1106062", "14"); //YLJioubi14
            Mapping.Add("1106063", "05"); //NCJioubi05
            Mapping.Add("1106064", "05"); //YLJioubi05
            Mapping.Add("1106065", "50"); //NCZhihebi50
            Mapping.Add("1106066", "50"); //YLZhihebi50
            Mapping.Add("1106067", "41"); //NCZhihebi41
            Mapping.Add("1106068", "41"); //YLZhihebi41
            Mapping.Add("1106069", "32"); //NCZhihebi32
            Mapping.Add("1106070", "32"); //YLZhihebi32
            Mapping.Add("1106071", "23"); //NCZhihebi23
            Mapping.Add("1106072", "23"); //YLZhihebi23
            Mapping.Add("1106073", "14"); //NCZhihebi14
            Mapping.Add("1106074", "14"); //YLZhihebi14
            Mapping.Add("1106075", "05"); //NCZhihebi05
            Mapping.Add("1106076", "05"); //YLZhihebi05

            #endregion

            #region 时时彩前二 1107

            Mapping.Add("1107001", "0"); //NCFenbu0
            Mapping.Add("1107002", "0"); //YLFenbu0
            Mapping.Add("1107003", "1"); //NCFenbu1
            Mapping.Add("1107004", "1"); //YLFenbu1
            Mapping.Add("1107005", "2"); //NCFenbu2
            Mapping.Add("1107006", "2"); //YLFenbu2
            Mapping.Add("1107007", "3"); //NCFenbu3
            Mapping.Add("1107008", "3"); //YLFenbu3
            Mapping.Add("1107009", "4"); //NCFenbu4
            Mapping.Add("1107010", "4"); //YLFenbu4
            Mapping.Add("1107011", "5"); //NCFenbu5
            Mapping.Add("1107012", "5"); //YLFenbu5
            Mapping.Add("1107013", "6"); //NCFenbu6
            Mapping.Add("1107014", "6"); //YLFenbu6
            Mapping.Add("1107015", "7"); //NCFenbu7
            Mapping.Add("1107016", "7"); //YLFenbu7
            Mapping.Add("1107017", "8"); //NCFenbu8
            Mapping.Add("1107018", "8"); //YLFenbu8
            Mapping.Add("1107019", "9"); //NCFenbu9
            Mapping.Add("1107020", "9"); //YLFenbu9
            Mapping.Add("1107021", "0"); //NCKuadu0
            Mapping.Add("1107022", "0"); //YLKuadu0
            Mapping.Add("1107023", "1"); //NCKuadu1
            Mapping.Add("1107024", "1"); //YLKuadu1
            Mapping.Add("1107025", "2"); //NCKuadu2
            Mapping.Add("1107026", "2"); //YLKuadu2
            Mapping.Add("1107027", "3"); //NCKuadu3
            Mapping.Add("1107028", "3"); //YLKuadu3
            Mapping.Add("1107029", "4"); //NCKuadu4
            Mapping.Add("1107030", "4"); //YLKuadu4
            Mapping.Add("1107031", "5"); //NCKuadu5
            Mapping.Add("1107032", "5"); //YLKuadu5
            Mapping.Add("1107033", "6"); //NCKuadu6
            Mapping.Add("1107034", "6"); //YLKuadu6
            Mapping.Add("1107035", "7"); //NCKuadu7
            Mapping.Add("1107036", "7"); //YLKuadu7
            Mapping.Add("1107037", "8"); //NCKuadu8
            Mapping.Add("1107038", "8"); //YLKuadu8
            Mapping.Add("1107039", "9"); //NCKuadu9
            Mapping.Add("1107040", "9"); //YLKuadu9
            Mapping.Add("1107041", "连号"); //NCLianhao
            Mapping.Add("1107042", "连号"); //YLLianhao
            Mapping.Add("1107043", "对子"); //NCDuizi
            Mapping.Add("1107044", "对子"); //YLDuizi
            Mapping.Add("1107045", "传号"); //NCChuanhao
            Mapping.Add("1107046", "传号"); //YLChuanhao
            Mapping.Add("1107047", "重码"); //NCChongma
            Mapping.Add("1107048", "重码"); //YLChongma
            Mapping.Add("1107049", "20"); //NCDaxiaobi20
            Mapping.Add("1107050", "20"); //YLDaxiaobi20
            Mapping.Add("1107051", "11"); //NCDaxiaobi11
            Mapping.Add("1107052", "11"); //YLDaxiaobi11
            Mapping.Add("1107053", "02"); //NCDaxiaobi02
            Mapping.Add("1107054", "02"); //YLDaxiaobi02
            Mapping.Add("1107055", "20"); //NCJioubi20
            Mapping.Add("1107056", "20"); //YLJioubi20
            Mapping.Add("1107057", "11"); //NCJioubi11
            Mapping.Add("1107058", "11"); //YLJioubi11
            Mapping.Add("1107059", "02"); //NCJioubi02
            Mapping.Add("1107060", "02"); //YLJioubi02
            Mapping.Add("1107061", "20"); //NCZhihebi20
            Mapping.Add("1107062", "20"); //YLZhihebi20
            Mapping.Add("1107063", "11"); //NCZhihebi11
            Mapping.Add("1107064", "11"); //YLZhihebi11
            Mapping.Add("1107065", "02"); //NCZhihebi02
            Mapping.Add("1107066", "02"); //YLZhihebi02
            Mapping.Add("1107067", "0"); //NC012Wanwei0
            Mapping.Add("1107068", "0"); //YL012Wanwei0
            Mapping.Add("1107069", "1"); //NC012Wanwei1
            Mapping.Add("1107070", "1"); //YL012Wanwei1
            Mapping.Add("1107071", "2"); //NC012Wanwei2
            Mapping.Add("1107072", "2"); //YL012Wanwei2
            Mapping.Add("1107073", "0"); //NC012Qianwei0
            Mapping.Add("1107074", "0"); //YL012Qianwei0
            Mapping.Add("1107075", "1"); //NC012Qianwei1
            Mapping.Add("1107076", "1"); //YL012Qianwei1
            Mapping.Add("1107077", "2"); //NC012Qianwei2
            Mapping.Add("1107078", "2"); //YL012Qianwei2
            Mapping.Add("1107079", "0"); //NC012Xingtai00
            Mapping.Add("1107080", "0"); //YL012Xingtai00
            Mapping.Add("1107081", "1"); //NC012Xingtai01
            Mapping.Add("1107082", "1"); //YL012Xingtai01
            Mapping.Add("1107083", "2"); //NC012Xingtai02
            Mapping.Add("1107084", "2"); //YL012Xingtai02
            Mapping.Add("1107085", "10"); //NC012Xingtai10
            Mapping.Add("1107086", "10"); //YL012Xingtai10
            Mapping.Add("1107087", "11"); //NC012Xingtai11
            Mapping.Add("1107088", "11"); //YL012Xingtai11
            Mapping.Add("1107089", "12"); //NC012Xingtai12
            Mapping.Add("1107090", "12"); //YL012Xingtai12
            Mapping.Add("1107091", "20"); //NC012Xingtai20
            Mapping.Add("1107092", "20"); //YL012Xingtai20
            Mapping.Add("1107093", "21"); //NC012Xingtai21
            Mapping.Add("1107094", "21"); //YL012Xingtai21
            Mapping.Add("1107095", "22"); //NC012Xingtai22
            Mapping.Add("1107096", "22"); //YL012Xingtai22
            Mapping.Add("1107097", "0"); //NC012Yu0Haoma0
            Mapping.Add("1107098", "0"); //YL012Yu0Haoma0
            Mapping.Add("1107099", "3"); //NC012Yu0Haoma3
            Mapping.Add("1107100", "3"); //YL012Yu0Haoma3
            Mapping.Add("1107101", "6"); //NC012Yu0Haoma6
            Mapping.Add("1107102", "6"); //YL012Yu0Haoma6
            Mapping.Add("1107103", "9"); //NC012Yu0Haoma9
            Mapping.Add("1107104", "9"); //YL012Yu0Haoma9
            Mapping.Add("1107105", "1"); //NC012Yu1Haoma1
            Mapping.Add("1107106", "1"); //YL012Yu1Haoma1
            Mapping.Add("1107107", "4"); //NC012Yu1Haoma4
            Mapping.Add("1107108", "4"); //YL012Yu1Haoma4
            Mapping.Add("1107109", "7"); //NC012Yu1Haoma7
            Mapping.Add("1107110", "7"); //YL012Yu1Haoma7
            Mapping.Add("1107111", "2"); //NC012Yu2Haoma2
            Mapping.Add("1107112", "2"); //YL012Yu2Haoma2
            Mapping.Add("1107113", "5"); //NC012Yu2Haoma5
            Mapping.Add("1107114", "5"); //YL012Yu2Haoma5
            Mapping.Add("1107115", "8"); //NC012Yu2Haoma8
            Mapping.Add("1107116", "8"); //YL012Yu2Haoma8
            Mapping.Add("1107117", "0"); //NCHz0
            Mapping.Add("1107118", "0"); //YLHz0
            Mapping.Add("1107119", "1"); //NCHz1
            Mapping.Add("1107120", "1"); //YLHz1
            Mapping.Add("1107121", "2"); //NCHz2
            Mapping.Add("1107122", "2"); //YLHz2
            Mapping.Add("1107123", "3"); //NCHz3
            Mapping.Add("1107124", "3"); //YLHz3
            Mapping.Add("1107125", "4"); //NCHz4
            Mapping.Add("1107126", "4"); //YLHz4
            Mapping.Add("1107127", "5"); //NCHz5
            Mapping.Add("1107128", "5"); //YLHz5
            Mapping.Add("1107129", "6"); //NCHz6
            Mapping.Add("1107130", "6"); //YLHz6
            Mapping.Add("1107131", "7"); //NCHz7
            Mapping.Add("1107132", "7"); //YLHz7
            Mapping.Add("1107133", "8"); //NCHz8
            Mapping.Add("1107134", "8"); //YLHz8
            Mapping.Add("1107135", "9"); //NCHz9
            Mapping.Add("1107136", "9"); //YLHz9
            Mapping.Add("1107137", "10"); //NCHz10
            Mapping.Add("1107138", "10"); //YLHz10
            Mapping.Add("1107139", "11"); //NCHz11
            Mapping.Add("1107140", "11"); //YLHz11
            Mapping.Add("1107141", "12"); //NCHz12
            Mapping.Add("1107142", "12"); //YLHz12
            Mapping.Add("1107143", "13"); //NCHz13
            Mapping.Add("1107144", "13"); //YLHz13
            Mapping.Add("1107145", "14"); //NCHz14
            Mapping.Add("1107146", "14"); //YLHz14
            Mapping.Add("1107147", "15"); //NCHz15
            Mapping.Add("1107148", "15"); //YLHz15
            Mapping.Add("1107149", "16"); //NCHz16
            Mapping.Add("1107150", "16"); //YLHz16
            Mapping.Add("1107151", "17"); //NCHz17
            Mapping.Add("1107152", "17"); //YLHz17
            Mapping.Add("1107153", "18"); //NCHz18
            Mapping.Add("1107154", "18"); //YLHz18
            Mapping.Add("1107155", "0-6"); //NCHz0to6
            Mapping.Add("1107156", "0-6"); //YLHz0to6
            Mapping.Add("1107157", "7-9"); //NCHz7to9
            Mapping.Add("1107158", "7-9"); //YLHz7to9
            Mapping.Add("1107159", "9-11"); //NCHz9to11
            Mapping.Add("1107160", "9-11"); //YLHz9to11
            Mapping.Add("1107161", "12-18");//NCHz12to18
            Mapping.Add("1107162", "12-18");//YLHz12to18
            Mapping.Add("1107163", "0"); //NCHzws0
            Mapping.Add("1107164", "0"); //YLHzws0
            Mapping.Add("1107165", "1"); //NCHzws1
            Mapping.Add("1107166", "1"); //YLHzws1
            Mapping.Add("1107167", "2"); //NCHzws2
            Mapping.Add("1107168", "2"); //YLHzws2
            Mapping.Add("1107169", "3"); //NCHzws3
            Mapping.Add("1107170", "3"); //YLHzws3
            Mapping.Add("1107171", "4"); //NCHzws4
            Mapping.Add("1107172", "4"); //YLHzws4
            Mapping.Add("1107173", "5"); //NCHzws5
            Mapping.Add("1107174", "5"); //YLHzws5
            Mapping.Add("1107175", "6"); //NCHzws6
            Mapping.Add("1107176", "6"); //YLHzws6
            Mapping.Add("1107177", "7"); //NCHzws7
            Mapping.Add("1107178", "7"); //YLHzws7
            Mapping.Add("1107179", "8"); //NCHzws8
            Mapping.Add("1107180", "8"); //YLHzws8
            Mapping.Add("1107181", "9"); //NCHzws9
            Mapping.Add("1107182", "9"); //YLHzws9

            #endregion

            #region 时时彩后二 1108

            Mapping.Add("1108001", "0"); //NCFenbu0
            Mapping.Add("1108002", "0"); //YLFenbu0
            Mapping.Add("1108003", "1"); //NCFenbu1
            Mapping.Add("1108004", "1"); //YLFenbu1
            Mapping.Add("1108005", "2"); //NCFenbu2
            Mapping.Add("1108006", "2"); //YLFenbu2
            Mapping.Add("1108007", "3"); //NCFenbu3
            Mapping.Add("1108008", "3"); //YLFenbu3
            Mapping.Add("1108009", "4"); //NCFenbu4
            Mapping.Add("1108010", "4"); //YLFenbu4
            Mapping.Add("1108011", "5"); //NCFenbu5
            Mapping.Add("1108012", "5"); //YLFenbu5
            Mapping.Add("1108013", "6"); //NCFenbu6
            Mapping.Add("1108014", "6"); //YLFenbu6
            Mapping.Add("1108015", "7"); //NCFenbu7
            Mapping.Add("1108016", "7"); //YLFenbu7
            Mapping.Add("1108017", "8"); //NCFenbu8
            Mapping.Add("1108018", "8"); //YLFenbu8
            Mapping.Add("1108019", "9"); //NCFenbu9
            Mapping.Add("1108020", "9"); //YLFenbu9
            Mapping.Add("1108021", "0"); //NCKuadu0
            Mapping.Add("1108022", "0"); //YLKuadu0
            Mapping.Add("1108023", "1"); //NCKuadu1
            Mapping.Add("1108024", "1"); //YLKuadu1
            Mapping.Add("1108025", "2"); //NCKuadu2
            Mapping.Add("1108026", "2"); //YLKuadu2
            Mapping.Add("1108027", "3"); //NCKuadu3
            Mapping.Add("1108028", "3"); //YLKuadu3
            Mapping.Add("1108029", "4"); //NCKuadu4
            Mapping.Add("1108030", "4"); //YLKuadu4
            Mapping.Add("1108031", "5"); //NCKuadu5
            Mapping.Add("1108032", "5"); //YLKuadu5
            Mapping.Add("1108033", "6"); //NCKuadu6
            Mapping.Add("1108034", "6"); //YLKuadu6
            Mapping.Add("1108035", "7"); //NCKuadu7
            Mapping.Add("1108036", "7"); //YLKuadu7
            Mapping.Add("1108037", "8"); //NCKuadu8
            Mapping.Add("1108038", "8"); //YLKuadu8
            Mapping.Add("1108039", "9"); //NCKuadu9
            Mapping.Add("1108040", "9"); //YLKuadu9
            Mapping.Add("1108041", "连号"); //NCLianhao
            Mapping.Add("1108042", "连号"); //YLLianhao
            Mapping.Add("1108043", "对子"); //NCDuizi
            Mapping.Add("1108044", "对子"); //YLDuizi
            Mapping.Add("1108045", "传号"); //NCChuanhao
            Mapping.Add("1108046", "传号"); //YLChuanhao
            Mapping.Add("1108047", "重码"); //NCChongma
            Mapping.Add("1108048", "重码"); //YLChongma
            Mapping.Add("1108049", "20");//NCDaxiaobi20
            Mapping.Add("1108050", "20");//YLDaxiaobi20
            Mapping.Add("1108051", "11");//NCDaxiaobi11
            Mapping.Add("1108052", "11");//YLDaxiaobi11
            Mapping.Add("1108053", "02"); //NCDaxiaobi02
            Mapping.Add("1108054", "02"); //YLDaxiaobi02
            Mapping.Add("1108055", "20"); //NCJioubi20
            Mapping.Add("1108056", "20"); //YLJioubi20
            Mapping.Add("1108057", "11"); //NCJioubi11
            Mapping.Add("1108058", "11"); //YLJioubi11
            Mapping.Add("1108059", "02"); //NCJioubi02
            Mapping.Add("1108060", "02"); //YLJioubi02
            Mapping.Add("1108061", "20	"); //NCZhihebi20
            Mapping.Add("1108062", "20	"); //YLZhihebi20
            Mapping.Add("1108063", "11	"); //NCZhihebi11
            Mapping.Add("1108064", "11	"); //YLZhihebi11
            Mapping.Add("1108065", "02"); //NCZhihebi02
            Mapping.Add("1108066", "02"); //YLZhihebi02
            Mapping.Add("1108067", "0"); //NC012Shiwei0
            Mapping.Add("1108068", "0"); //YL012Shiwei0
            Mapping.Add("1108069", "1"); //NC012Shiwei1
            Mapping.Add("1108070", "1"); //YL012Shiwei1
            Mapping.Add("1108071", "2"); //NC012Shiwei2
            Mapping.Add("1108072", "2"); //YL012Shiwei2
            Mapping.Add("1108073", "0"); //NC012Gewei0
            Mapping.Add("1108074", "0"); //YL012Gewei0
            Mapping.Add("1108075", "1"); //NC012Gewei1
            Mapping.Add("1108076", "1"); //YL012Gewei1
            Mapping.Add("1108077", "2"); //NC012Gewei2
            Mapping.Add("1108078", "2"); //YL012Gewei2
            Mapping.Add("1108079", "00"); //NC012Xingtai00
            Mapping.Add("1108080", "00"); //YL012Xingtai00
            Mapping.Add("1108081", "01"); //NC012Xingtai01
            Mapping.Add("1108082", "01"); //YL012Xingtai01
            Mapping.Add("1108083", "02"); //NC012Xingtai02
            Mapping.Add("1108084", "02"); //YL012Xingtai02
            Mapping.Add("1108085", "10"); //NC012Xingtai10
            Mapping.Add("1108086", "10"); //YL012Xingtai10
            Mapping.Add("1108087", "11"); //NC012Xingtai11
            Mapping.Add("1108088", "11"); //YL012Xingtai11
            Mapping.Add("1108089", "12"); //NC012Xingtai12
            Mapping.Add("1108090", "12"); //YL012Xingtai12
            Mapping.Add("1108091", "20"); //NC012Xingtai20
            Mapping.Add("1108092", "20"); //YL012Xingtai20
            Mapping.Add("1108093", "21"); //NC012Xingtai21
            Mapping.Add("1108094", "21"); //YL012Xingtai21
            Mapping.Add("1108095", "22"); //NC012Xingtai22
            Mapping.Add("1108096", "22"); //YL012Xingtai22
            Mapping.Add("1108097", "0"); //NC012Yu0Haoma0
            Mapping.Add("1108098", "0"); //YL012Yu0Haoma0
            Mapping.Add("1108099", "3"); //NC012Yu0Haoma3
            Mapping.Add("1108100", "3"); //YL012Yu0Haoma3
            Mapping.Add("1108101", "6"); //NC012Yu0Haoma6
            Mapping.Add("1108102", "6"); //YL012Yu0Haoma6
            Mapping.Add("1108103", "9"); //NC012Yu0Haoma9
            Mapping.Add("1108104", "9"); //YL012Yu0Haoma9
            Mapping.Add("1108105", "1"); //NC012Yu1Haoma1
            Mapping.Add("1108106", "1"); //YL012Yu1Haoma1
            Mapping.Add("1108107", "4"); //NC012Yu1Haoma4
            Mapping.Add("1108108", "4"); //YL012Yu1Haoma4
            Mapping.Add("1108109", "7"); //NC012Yu1Haoma7
            Mapping.Add("1108110", "7"); //YL012Yu1Haoma7
            Mapping.Add("1108111", "2"); //NC012Yu2Haoma2
            Mapping.Add("1108112", "2"); //YL012Yu2Haoma2
            Mapping.Add("1108113", "5"); //NC012Yu2Haoma5
            Mapping.Add("1108114", "5"); //YL012Yu2Haoma5
            Mapping.Add("1108115", "8"); //NC012Yu2Haoma8
            Mapping.Add("1108116", "8"); //YL012Yu2Haoma8
            Mapping.Add("1108117", "0"); //NCHz0
            Mapping.Add("1108118", "0"); //YLHz0
            Mapping.Add("1108119", "1"); //NCHz1
            Mapping.Add("1108120", "1"); //YLHz1
            Mapping.Add("1108121", "2"); //NCHz2
            Mapping.Add("1108122", "2"); //YLHz2
            Mapping.Add("1108123", "3"); //NCHz3
            Mapping.Add("1108124", "3"); //YLHz3
            Mapping.Add("1108125", "4"); //NCHz4
            Mapping.Add("1108126", "4"); //YLHz4
            Mapping.Add("1108127", "5"); //NCHz5
            Mapping.Add("1108128", "5"); //YLHz5
            Mapping.Add("1108129", "6"); //NCHz6
            Mapping.Add("1108130", "6"); //YLHz6
            Mapping.Add("1108131", "7"); //NCHz7
            Mapping.Add("1108132", "7"); //YLHz7
            Mapping.Add("1108133", "8"); //NCHz8
            Mapping.Add("1108134", "8"); //YLHz8
            Mapping.Add("1108135", "9"); //NCHz9
            Mapping.Add("1108136", "9"); //YLHz9
            Mapping.Add("1108137", "10"); //NCHz10
            Mapping.Add("1108138", "10"); //YLHz10
            Mapping.Add("1108139", "11"); //NCHz11
            Mapping.Add("1108140", "11"); //YLHz11
            Mapping.Add("1108141", "12"); //NCHz12
            Mapping.Add("1108142", "12"); //YLHz12
            Mapping.Add("1108143", "13"); //NCHz13
            Mapping.Add("1108144", "13"); //YLHz13
            Mapping.Add("1108145", "14"); //NCHz14
            Mapping.Add("1108146", "14"); //YLHz14
            Mapping.Add("1108147", "15"); //NCHz15
            Mapping.Add("1108148", "15"); //YLHz15
            Mapping.Add("1108149", "16"); //NCHz16
            Mapping.Add("1108150", "16"); //YLHz16
            Mapping.Add("1108151", "17"); //NCHz17
            Mapping.Add("1108152", "17"); //YLHz17
            Mapping.Add("1108153", "18"); //NCHz18
            Mapping.Add("1108154", "18"); //YLHz18
            Mapping.Add("1108155", "0-6"); //NCHz0to6
            Mapping.Add("1108156", "0-6"); //YLHz0to6
            Mapping.Add("1108157", "7-9"); //NCHz7to9
            Mapping.Add("1108158", "7-9"); //YLHz7to9
            Mapping.Add("1108159", "9-11"); //NCHz9to11
            Mapping.Add("1108160", "9-11"); //YLHz9to11
            Mapping.Add("1108161", "12-18"); //NCHz12to18
            Mapping.Add("1108162", "12-18"); //YLHz12to18
            Mapping.Add("1108163", "0"); //NCHzws0
            Mapping.Add("1108164", "0"); //YLHzws0
            Mapping.Add("1108165", "1"); //NCHzws1
            Mapping.Add("1108166", "1"); //YLHzws1
            Mapping.Add("1108167", "2"); //NCHzws2
            Mapping.Add("1108168", "2"); //YLHzws2
            Mapping.Add("1108169", "3"); //NCHzws3
            Mapping.Add("1108170", "3"); //YLHzws3
            Mapping.Add("1108171", "4"); //NCHzws4
            Mapping.Add("1108172", "4"); //YLHzws4
            Mapping.Add("1108173", "5"); //NCHzws5
            Mapping.Add("1108174", "5"); //YLHzws5
            Mapping.Add("1108175", "6"); //NCHzws6
            Mapping.Add("1108176", "6"); //YLHzws6
            Mapping.Add("1108177", "7"); //NCHzws7
            Mapping.Add("1108178", "7"); //YLHzws7
            Mapping.Add("1108179", "8"); //NCHzws8
            Mapping.Add("1108180", "8"); //YLHzws8
            Mapping.Add("1108181", "9"); //NCHzws9
            Mapping.Add("1108182", "9"); //YLHzws9
            Mapping.Add("1108183", "大大"); //NCDada
            Mapping.Add("1108184", "大大"); //YLDada
            Mapping.Add("1108185", "大小"); //NCDaxiao
            Mapping.Add("1108186", "大小"); //YLDaxiao
            Mapping.Add("1108187", "大单"); //NCDadan
            Mapping.Add("1108188", "大单"); //YLDadan
            Mapping.Add("1108189", "大双"); //NCDashuang
            Mapping.Add("1108190", "大双"); //YLDashuang
            Mapping.Add("1108191", "小大"); //NCXiaoda
            Mapping.Add("1108192", "小大"); //YLXiaoda
            Mapping.Add("1108193", "小小"); //NCXiaoxiao
            Mapping.Add("1108194", "小小"); //YLXiaoxiao
            Mapping.Add("1108195", "小单"); //NCXiaodan
            Mapping.Add("1108196", "小单"); //YLXiaodan
            Mapping.Add("1108197", "小双"); //NCXiaoshuang
            Mapping.Add("1108198", "小双"); //YLXiaoshuang
            Mapping.Add("1108199", "单大"); //NCDanda
            Mapping.Add("1108200", "单大"); //YLDanda
            Mapping.Add("1108201", "单小"); //NCDanxiao
            Mapping.Add("1108202", "单小"); //YLDanxiao
            Mapping.Add("1108203", "单单"); //NCDandan
            Mapping.Add("1108204", "单单"); //YLDandan
            Mapping.Add("1108205", "单双"); //NCDanshuang
            Mapping.Add("1108206", "单双"); //YLDanshuang
            Mapping.Add("1108207", "双大"); //NCShuangda
            Mapping.Add("1108208", "双大"); //YLShuangda
            Mapping.Add("1108209", "双小"); //NCShuangxiao
            Mapping.Add("1108210", "双小"); //YLShuangxiao
            Mapping.Add("1108211", "双单"); //NCShuangdan
            Mapping.Add("1108212", "双单"); //YLShuangdan
            Mapping.Add("1108213", "双双"); //NCShuangshuang
            Mapping.Add("1108214", "双双"); //YLShuangshuang
            Mapping.Add("1108215", "大"); //NCShiweida
            Mapping.Add("1108216", "大"); //YLShiweida
            Mapping.Add("1108217", "小"); //NCShiweixiao
            Mapping.Add("1108218", "小"); //YLShiweixiao
            Mapping.Add("1108219", "单"); //NCShiweidan
            Mapping.Add("1108220", "单"); //YLShiweidan
            Mapping.Add("1108221", "双"); //NCShiweishuang
            Mapping.Add("1108222", "双"); //YLShiweishuang
            Mapping.Add("1108223", "大"); //NCGeweida
            Mapping.Add("1108224", "大"); //YLGeweida
            Mapping.Add("1108225", "小"); //NCGeweixiao
            Mapping.Add("1108226", "小"); //YLGeweixiao
            Mapping.Add("1108227", "单"); //NCGeweidan
            Mapping.Add("1108228", "单"); //YLGeweidan
            Mapping.Add("1108229", "双"); //NCGeweishuang
            Mapping.Add("1108230", "双"); //YLGeweishuang

            #endregion

            #region 时时彩前三 1109

            Mapping.Add("1109001", "豹子"); //NCBaozi
            Mapping.Add("1109002", "豹子"); //YLBaozi
            Mapping.Add("1109003", "组3"); //NCZ3
            Mapping.Add("1109004", "组3"); //YLZ3
            Mapping.Add("1109005", "组6"); //NCZ6
            Mapping.Add("1109006", "组6"); //YLZ6
            Mapping.Add("1109007", "0"); //NCFenbu0
            Mapping.Add("1109008", "0"); //YLFenbu0
            Mapping.Add("1109009", "1"); //NCFenbu1
            Mapping.Add("1109010", "1"); //YLFenbu1
            Mapping.Add("1109011", "2"); //NCFenbu2
            Mapping.Add("1109012", "2"); //YLFenbu2
            Mapping.Add("1109013", "3"); //NCFenbu3
            Mapping.Add("1109014", "3"); //YLFenbu3
            Mapping.Add("1109015", "4"); //NCFenbu4
            Mapping.Add("1109016", "4"); //YLFenbu4
            Mapping.Add("1109017", "5"); //NCFenbu5
            Mapping.Add("1109018", "5"); //YLFenbu5
            Mapping.Add("1109019", "6"); //NCFenbu6
            Mapping.Add("1109020", "6"); //YLFenbu6
            Mapping.Add("1109021", "7"); //NCFenbu7
            Mapping.Add("1109022", "7"); //YLFenbu7
            Mapping.Add("1109023", "8"); //NCFenbu8
            Mapping.Add("1109024", "8"); //YLFenbu8
            Mapping.Add("1109025", "9"); //NCFenbu9
            Mapping.Add("1109026", "9"); //YLFenbu9
            Mapping.Add("1109027", "0"); //NCKuadu0
            Mapping.Add("1109028", "0"); //YLKuadu0
            Mapping.Add("1109029", "1"); //NCKuadu1
            Mapping.Add("1109030", "1"); //YLKuadu1
            Mapping.Add("1109031", "2"); //NCKuadu2
            Mapping.Add("1109032", "2"); //YLKuadu2
            Mapping.Add("1109033", "3"); //NCKuadu3
            Mapping.Add("1109034", "3"); //YLKuadu3
            Mapping.Add("1109035", "4"); //NCKuadu4
            Mapping.Add("1109036", "4"); //YLKuadu4
            Mapping.Add("1109037", "5"); //NCKuadu5
            Mapping.Add("1109038", "5"); //YLKuadu5
            Mapping.Add("1109039", "6"); //NCKuadu6
            Mapping.Add("1109040", "6"); //YLKuadu6
            Mapping.Add("1109041", "7"); //NCKuadu7
            Mapping.Add("1109042", "7"); //YLKuadu7
            Mapping.Add("1109043", "8"); //NCKuadu8
            Mapping.Add("1109044", "8"); //YLKuadu8
            Mapping.Add("1109045", "9"); //NCKuadu9
            Mapping.Add("1109046", "9"); //YLKuadu9
            Mapping.Add("1109047", "30"); //NCDaxiaobi30
            Mapping.Add("1109048", "30"); //YLDaxiaobi30
            Mapping.Add("1109049", "21"); //NCDaxiaobi21
            Mapping.Add("1109050", "21"); //YLDaxiaobi21
            Mapping.Add("1109051", "12"); //NCDaxiaobi12
            Mapping.Add("1109052", "12"); //YLDaxiaobi12
            Mapping.Add("1109053", "03"); //NCDaxiaobi03
            Mapping.Add("1109054", "03"); //YLDaxiaobi03
            Mapping.Add("1109055", "30"); //NCJioubi30
            Mapping.Add("1109056", "30"); //YLJioubi30
            Mapping.Add("1109057", "21"); //NCJioubi21
            Mapping.Add("1109058", "21"); //YLJioubi21
            Mapping.Add("1109059", "12"); //NCJioubi12
            Mapping.Add("1109060", "12"); //YLJioubi12
            Mapping.Add("1109061", "03"); //NCJioubi03
            Mapping.Add("1109062", "03"); //YLJioubi03
            Mapping.Add("1109063", "30"); //NCZhihebi30
            Mapping.Add("1109064", "30"); //YLZhihebi30
            Mapping.Add("1109065", "21"); //NCZhihebi21
            Mapping.Add("1109066", "21"); //YLZhihebi21
            Mapping.Add("1109067", "12"); //NCZhihebi12
            Mapping.Add("1109068", "12"); //YLZhihebi12
            Mapping.Add("1109069", "03"); //NCZhihebi03
            Mapping.Add("1109070", "03"); //YLZhihebi03
            Mapping.Add("1109071", "0"); //NCKuaduWQ0
            Mapping.Add("1109072", "0"); //YLKuaduWQ0
            Mapping.Add("1109073", "1"); //NCKuaduWQ1
            Mapping.Add("1109074", "1"); //YLKuaduWQ1
            Mapping.Add("1109075", "2"); //NCKuaduWQ2
            Mapping.Add("1109076", "2"); //YLKuaduWQ2
            Mapping.Add("1109077", "3"); //NCKuaduWQ3
            Mapping.Add("1109078", "3"); //YLKuaduWQ3
            Mapping.Add("1109079", "4"); //NCKuaduWQ4
            Mapping.Add("1109080", "4"); //YLKuaduWQ4
            Mapping.Add("1109081", "5"); //NCKuaduWQ5
            Mapping.Add("1109082", "5"); //YLKuaduWQ5
            Mapping.Add("1109083", "6"); //NCKuaduWQ6
            Mapping.Add("1109084", "6"); //YLKuaduWQ6
            Mapping.Add("1109085", "7"); //NCKuaduWQ7
            Mapping.Add("1109086", "7"); //YLKuaduWQ7
            Mapping.Add("1109087", "8"); //NCKuaduWQ8
            Mapping.Add("1109088", "8"); //YLKuaduWQ8
            Mapping.Add("1109089", "9"); //NCKuaduWQ9
            Mapping.Add("1109090", "9"); //YLKuaduWQ9
            Mapping.Add("1109091", "0"); //NCKuaduQB0
            Mapping.Add("1109092", "0"); //YLKuaduQB0
            Mapping.Add("1109093", "1"); //NCKuaduQB1
            Mapping.Add("1109094", "1"); //YLKuaduQB1
            Mapping.Add("1109095", "2"); //NCKuaduQB2
            Mapping.Add("1109096", "2"); //YLKuaduQB2
            Mapping.Add("1109097", "3"); //NCKuaduQB3
            Mapping.Add("1109098", "3"); //YLKuaduQB3
            Mapping.Add("1109099", "4"); //NCKuaduQB4
            Mapping.Add("1109100", "4"); //YLKuaduQB4
            Mapping.Add("1109101", "5"); //NCKuaduQB5
            Mapping.Add("1109102", "5"); //YLKuaduQB5
            Mapping.Add("1109103", "6"); //NCKuaduQB6
            Mapping.Add("1109104", "6"); //YLKuaduQB6
            Mapping.Add("1109105", "7"); //NCKuaduQB7
            Mapping.Add("1109106", "7"); //YLKuaduQB7
            Mapping.Add("1109107", "8"); //NCKuaduQB8
            Mapping.Add("1109108", "8"); //YLKuaduQB8
            Mapping.Add("1109109", "9"); //NCKuaduQB9
            Mapping.Add("1109110", "9"); //YLKuaduQB9
            Mapping.Add("1109111", "3030"); //NCDaxiaojiou3030
            Mapping.Add("1109112", "3030"); //YLDaxiaojiou3030
            Mapping.Add("1109113", "3021"); //NCDaxiaojiou3021
            Mapping.Add("1109114", "3021"); //YLDaxiaojiou3021
            Mapping.Add("1109115", "3012"); //NCDaxiaojiou3012
            Mapping.Add("1109116", "3012"); //YLDaxiaojiou3012
            Mapping.Add("1109117", "3003"); //NCDaxiaojiou3003
            Mapping.Add("1109118", "3003"); //YLDaxiaojiou3003
            Mapping.Add("1109119", "2130"); //NCDaxiaojiou2130
            Mapping.Add("1109120", "2130"); //YLDaxiaojiou2130
            Mapping.Add("1109121", "2121"); //NCDaxiaojiou2121
            Mapping.Add("1109122", "2121"); //YLDaxiaojiou2121
            Mapping.Add("1109123", "2112"); //NCDaxiaojiou2112
            Mapping.Add("1109124", "2112"); //YLDaxiaojiou2112
            Mapping.Add("1109125", "2103"); //NCDaxiaojiou2103
            Mapping.Add("1109126", "2103"); //YLDaxiaojiou2103
            Mapping.Add("1109127", "1230"); //NCDaxiaojiou1230
            Mapping.Add("1109128", "1230"); //YLDaxiaojiou1230
            Mapping.Add("1109129", "1221"); //NCDaxiaojiou1221
            Mapping.Add("1109130", "1221"); //YLDaxiaojiou1221
            Mapping.Add("1109131", "1212"); //NCDaxiaojiou1212
            Mapping.Add("1109132", "1212"); //YLDaxiaojiou1212
            Mapping.Add("1109133", "1203"); //NCDaxiaojiou1203
            Mapping.Add("1109134", "1203"); //YLDaxiaojiou1203
            Mapping.Add("1109135", "0330"); //NCDaxiaojiou0330
            Mapping.Add("1109136", "0330"); //YLDaxiaojiou0330
            Mapping.Add("1109137", "0321"); //NCDaxiaojiou0321
            Mapping.Add("1109138", "0321"); //YLDaxiaojiou0321
            Mapping.Add("1109139", "0312"); //NCDaxiaojiou0312
            Mapping.Add("1109140", "0312"); //YLDaxiaojiou0312
            Mapping.Add("1109141", "0303"); //NCDaxiaojiou0303
            Mapping.Add("1109142", "0303"); //YLDaxiaojiou0303
            Mapping.Add("1109143", "0"); //NC012Wanwei0
            Mapping.Add("1109144", "0"); //YL012Wanwei0
            Mapping.Add("1109145", "1"); //NC012Wanwei1
            Mapping.Add("1109146", "1"); //YL012Wanwei1
            Mapping.Add("1109147", "2"); //NC012Wanwei2
            Mapping.Add("1109148", "2"); //YL012Wanwei2
            Mapping.Add("1109149", "0"); //NC012Qianwei0
            Mapping.Add("1109150", "0"); //YL012Qianwei0
            Mapping.Add("1109151", "1"); //NC012Qianwei1
            Mapping.Add("1109152", "1"); //YL012Qianwei1
            Mapping.Add("1109153", "2"); //NC012Qianwei2
            Mapping.Add("1109154", "2"); //YL012Qianwei2
            Mapping.Add("1109155", "0"); //NC012Baiwei0
            Mapping.Add("1109156", "0"); //YL012Baiwei0
            Mapping.Add("1109157", "1"); //NC012Baiwei1
            Mapping.Add("1109158", "1"); //YL012Baiwei1
            Mapping.Add("1109159", "2"); //NC012Baiwei2
            Mapping.Add("1109160", "2"); //YL012Baiwei2
            Mapping.Add("1109161", "000"); //NC012Xingtai000
            Mapping.Add("1109162", "000"); //YL012Xingtai000
            Mapping.Add("1109163", "001"); //NC012Xingtai001
            Mapping.Add("1109164", "001"); //YL012Xingtai001
            Mapping.Add("1109165", "002"); //NC012Xingtai002
            Mapping.Add("1109166", "002"); //YL012Xingtai002
            Mapping.Add("1109167", "010"); //NC012Xingtai010
            Mapping.Add("1109168", "010"); //YL012Xingtai010
            Mapping.Add("1109169", "011"); //NC012Xingtai011
            Mapping.Add("1109170", "011"); //YL012Xingtai011
            Mapping.Add("1109171", "012"); //NC012Xingtai012
            Mapping.Add("1109172", "012"); //YL012Xingtai012
            Mapping.Add("1109173", "020"); //NC012Xingtai020
            Mapping.Add("1109174", "020"); //YL012Xingtai020
            Mapping.Add("1109175", "021"); //NC012Xingtai021
            Mapping.Add("1109176", "021"); //YL012Xingtai021
            Mapping.Add("1109177", "022"); //NC012Xingtai022
            Mapping.Add("1109178", "022"); //YL012Xingtai022
            Mapping.Add("1109179", "100"); //NC012Xingtai100
            Mapping.Add("1109180", "100"); //YL012Xingtai100
            Mapping.Add("1109181", "101"); //NC012Xingtai101
            Mapping.Add("1109182", "101"); //YL012Xingtai101
            Mapping.Add("1109183", "102"); //NC012Xingtai102
            Mapping.Add("1109184", "102"); //YL012Xingtai102
            Mapping.Add("1109185", "110"); //NC012Xingtai110
            Mapping.Add("1109186", "110"); //YL012Xingtai110
            Mapping.Add("1109187", "111"); //NC012Xingtai111
            Mapping.Add("1109188", "111"); //YL012Xingtai111
            Mapping.Add("1109189", "112"); //NC012Xingtai112
            Mapping.Add("1109190", "112"); //YL012Xingtai112
            Mapping.Add("1109191", "120"); //NC012Xingtai120
            Mapping.Add("1109192", "120"); //YL012Xingtai120
            Mapping.Add("1109193", "121"); //NC012Xingtai121
            Mapping.Add("1109194", "121"); //YL012Xingtai121
            Mapping.Add("1109195", "122"); //NC012Xingtai122
            Mapping.Add("1109196", "122"); //YL012Xingtai122
            Mapping.Add("1109197", "200"); //NC012Xingtai200
            Mapping.Add("1109198", "200"); //YL012Xingtai200
            Mapping.Add("1109199", "201"); //NC012Xingtai201
            Mapping.Add("1109200", "201"); //YL012Xingtai201
            Mapping.Add("1109201", "202"); //NC012Xingtai202
            Mapping.Add("1109202", "202"); //YL012Xingtai202
            Mapping.Add("1109203", "210"); //NC012Xingtai210
            Mapping.Add("1109204", "210"); //YL012Xingtai210
            Mapping.Add("1109205", "211"); //NC012Xingtai211
            Mapping.Add("1109206", "211"); //YL012Xingtai211
            Mapping.Add("1109207", "212"); //NC012Xingtai212
            Mapping.Add("1109208", "212"); //YL012Xingtai212
            Mapping.Add("1109209", "220"); //NC012Xingtai220
            Mapping.Add("1109210", "220"); //YL012Xingtai220
            Mapping.Add("1109211", "221"); //NC012Xingtai221
            Mapping.Add("1109212", "221"); //YL012Xingtai221
            Mapping.Add("1109213", "222"); //NC012Xingtai222
            Mapping.Add("1109214", "222"); //YL012Xingtai222
            Mapping.Add("1109215", "0"); //NC012Yu0Haoma0
            Mapping.Add("1109216", "0"); //YL012Yu0Haoma0
            Mapping.Add("1109217", "3"); //NC012Yu0Haoma3
            Mapping.Add("1109218", "3"); //YL012Yu0Haoma3
            Mapping.Add("1109219", "6"); //NC012Yu0Haoma6
            Mapping.Add("1109220", "6"); //YL012Yu0Haoma6
            Mapping.Add("1109221", "9"); //NC012Yu0Haoma9
            Mapping.Add("1109222", "9"); //YL012Yu0Haoma9
            Mapping.Add("1109223", "1"); //NC012Yu1Haoma1
            Mapping.Add("1109224", "1"); //YL012Yu1Haoma1
            Mapping.Add("1109225", "4"); //NC012Yu1Haoma4
            Mapping.Add("1109226", "4"); //YL012Yu1Haoma4
            Mapping.Add("1109227", "7"); //NC012Yu1Haoma7
            Mapping.Add("1109228", "7"); //YL012Yu1Haoma7
            Mapping.Add("1109229", "2"); //NC012Yu2Haoma2
            Mapping.Add("1109230", "2"); //YL012Yu2Haoma2
            Mapping.Add("1109231", "5"); //NC012Yu2Haoma5
            Mapping.Add("1109232", "5"); //YL012Yu2Haoma5
            Mapping.Add("1109233", "8"); //NC012Yu2Haoma8
            Mapping.Add("1109234", "8"); //YL012Yu2Haoma8
            Mapping.Add("1109235", "0"); //NCHz0
            Mapping.Add("1109236", "0"); //YLHz0
            Mapping.Add("1109237", "1"); //NCHz1
            Mapping.Add("1109238", "1"); //YLHz1
            Mapping.Add("1109239", "2"); //NCHz2
            Mapping.Add("1109240", "2"); //YLHz2
            Mapping.Add("1109241", "3"); //NCHz3
            Mapping.Add("1109242", "3"); //YLHz3
            Mapping.Add("1109243", "4"); //NCHz4
            Mapping.Add("1109244", "4"); //YLHz4
            Mapping.Add("1109245", "5"); //NCHz5
            Mapping.Add("1109246", "5"); //YLHz5
            Mapping.Add("1109247", "6"); //NCHz6
            Mapping.Add("1109248", "6"); //YLHz6
            Mapping.Add("1109249", "7"); //NCHz7
            Mapping.Add("1109250", "7"); //YLHz7
            Mapping.Add("1109251", "8"); //NCHz8
            Mapping.Add("1109252", "8"); //YLHz8
            Mapping.Add("1109253", "9"); //NCHz9
            Mapping.Add("1109254", "9"); //YLHz9
            Mapping.Add("1109255", "10"); //NCHz10
            Mapping.Add("1109256", "10"); //YLHz10
            Mapping.Add("1109257", "11"); //NCHz11
            Mapping.Add("1109258", "11"); //YLHz11
            Mapping.Add("1109259", "12"); //NCHz12
            Mapping.Add("1109260", "12"); //YLHz12
            Mapping.Add("1109261", "13"); //NCHz13
            Mapping.Add("1109262", "13"); //YLHz13
            Mapping.Add("1109263", "14"); //NCHz14
            Mapping.Add("1109264", "14"); //YLHz14
            Mapping.Add("1109265", "15"); //NCHz15
            Mapping.Add("1109266", "15"); //YLHz15
            Mapping.Add("1109267", "16"); //NCHz16
            Mapping.Add("1109268", "16"); //YLHz16
            Mapping.Add("1109269", "17"); //NCHz17
            Mapping.Add("1109270", "17"); //YLHz17
            Mapping.Add("1109271", "18"); //NCHz18
            Mapping.Add("1109272", "18"); //YLHz18
            Mapping.Add("1109273", "19"); //NCHz19
            Mapping.Add("1109274", "19"); //YLHz19
            Mapping.Add("1109275", "20"); //NCHz20
            Mapping.Add("1109276", "20"); //YLHz20
            Mapping.Add("1109277", "21"); //NCHz21
            Mapping.Add("1109278", "21"); //YLHz21
            Mapping.Add("1109279", "22"); //NCHz22
            Mapping.Add("1109280", "22"); //YLHz22
            Mapping.Add("1109281", "23"); //NCHz23
            Mapping.Add("1109282", "23"); //YLHz23
            Mapping.Add("1109283", "24"); //NCHz24
            Mapping.Add("1109284", "24"); //YLHz24
            Mapping.Add("1109285", "25"); //NCHz25
            Mapping.Add("1109286", "25"); //YLHz25
            Mapping.Add("1109287", "26"); //NCHz26
            Mapping.Add("1109288", "26"); //YLHz26
            Mapping.Add("1109289", "27"); //NCHz27
            Mapping.Add("1109290", "27"); //YLHz27
            Mapping.Add("1109291", "0-8"); //NCHz0to8
            Mapping.Add("1109292", "0-8"); //YLHz0to8
            Mapping.Add("1109293", "9-11"); //NCHz9to11
            Mapping.Add("1109294", "9-11"); //YLHz9to11
            Mapping.Add("1109295", "12-13"); //NCHz12to13
            Mapping.Add("1109296", "12-13"); //YLHz12to13
            Mapping.Add("1109297", "14-15"); //NCHz14to15
            Mapping.Add("1109298", "14-15"); //YLHz14to15
            Mapping.Add("1109299", "16-18"); //NCHz16to18
            Mapping.Add("1109300", "16-18"); //YLHz16to18
            Mapping.Add("1109301", "19-27"); //NCHz19to27
            Mapping.Add("1109302", "19-27"); //YLHz19to27
            Mapping.Add("1109303", "0"); //NCHzws0
            Mapping.Add("1109304", "0"); //YLHzws0
            Mapping.Add("1109305", "1"); //NCHzws1
            Mapping.Add("1109306", "1"); //YLHzws1
            Mapping.Add("1109307", "2"); //NCHzws2
            Mapping.Add("1109308", "2"); //YLHzws2
            Mapping.Add("1109309", "3"); //NCHzws3
            Mapping.Add("1109310", "3"); //YLHzws3
            Mapping.Add("1109311", "4"); //NCHzws4
            Mapping.Add("1109312", "4"); //YLHzws4
            Mapping.Add("1109313", "5"); //NCHzws5
            Mapping.Add("1109314", "5"); //YLHzws5
            Mapping.Add("1109315", "6"); //NCHzws6
            Mapping.Add("1109316", "6"); //YLHzws6
            Mapping.Add("1109317", "7"); //NCHzws7
            Mapping.Add("1109318", "7"); //YLHzws7
            Mapping.Add("1109319", "8"); //NCHzws8
            Mapping.Add("1109320", "8"); //YLHzws8
            Mapping.Add("1109321", "9"); //NCHzws9
            Mapping.Add("1109322", "9"); //YLHzws9

            #endregion

            #region 时时彩前三万能码 1110

            Mapping.Add("1110001", "0126"); //NCWanneng0126
            Mapping.Add("1110002", "0126"); //YLWanneng0126
            Mapping.Add("1110003", "0134"); //NCWanneng0134
            Mapping.Add("1110004", "0134"); //YLWanneng0134
            Mapping.Add("1110005", "0159"); //NCWanneng0159
            Mapping.Add("1110006", "0159"); //YLWanneng0159
            Mapping.Add("1110007", "0178"); //NCWanneng0178
            Mapping.Add("1110008", "0178"); //YLWanneng0178
            Mapping.Add("1110009", "0239"); //NCWanneng0239
            Mapping.Add("1110010", "0239"); //YLWanneng0239
            Mapping.Add("1110011", "0247"); //NCWanneng0247
            Mapping.Add("1110012", "0247"); //YLWanneng0247
            Mapping.Add("1110013", "0258"); //NCWanneng0258
            Mapping.Add("1110014", "0258"); //YLWanneng0258
            Mapping.Add("1110015", "0357"); //NCWanneng0357
            Mapping.Add("1110016", "0357"); //YLWanneng0357
            Mapping.Add("1110017", "0368"); //NCWanneng0368
            Mapping.Add("1110018", "0368"); //YLWanneng0368
            Mapping.Add("1110019", "0456"); //NCWanneng0456
            Mapping.Add("1110020", "0456"); //YLWanneng0456
            Mapping.Add("1110021", "0489"); //NCWanneng0489
            Mapping.Add("1110022", "0489"); //YLWanneng0489
            Mapping.Add("1110023", "0679"); //NCWanneng0679
            Mapping.Add("1110024", "0679"); //YLWanneng0679
            Mapping.Add("1110025", "1237"); //NCWanneng1237
            Mapping.Add("1110026", "1237"); //YLWanneng1237
            Mapping.Add("1110027", "1245"); //NCWanneng1245
            Mapping.Add("1110028", "1245"); //YLWanneng1245
            Mapping.Add("1110029", "1289"); //NCWanneng1289
            Mapping.Add("1110030", "1289"); //YLWanneng1289
            Mapping.Add("1110031", "1358"); //NCWanneng1358
            Mapping.Add("1110032", "1358"); //YLWanneng1358
            Mapping.Add("1110033", "1369"); //NCWanneng1369
            Mapping.Add("1110034", "1369"); //YLWanneng1369
            Mapping.Add("1110035", "1468"); //NCWanneng1468
            Mapping.Add("1110036", "1468"); //YLWanneng1468
            Mapping.Add("1110037", "1479"); //NCWanneng1479
            Mapping.Add("1110038", "1479"); //YLWanneng1479
            Mapping.Add("1110039", "1567"); //NCWanneng1567
            Mapping.Add("1110040", "1567"); //YLWanneng1567
            Mapping.Add("1110041", "2348"); //NCWanneng2348
            Mapping.Add("1110042", "2348"); //YLWanneng2348
            Mapping.Add("1110043", "2356"); //NCWanneng2356
            Mapping.Add("1110044", "2356"); //YLWanneng2356
            Mapping.Add("1110045", "2469"); //NCWanneng2469
            Mapping.Add("1110046", "2469"); //YLWanneng2469
            Mapping.Add("1110047", "2579"); //NCWanneng2579
            Mapping.Add("1110048", "2579"); //YLWanneng2579
            Mapping.Add("1110049", "2678"); //NCWanneng2678
            Mapping.Add("1110050", "2678"); //YLWanneng2678
            Mapping.Add("1110051", "3459"); //NCWanneng3459
            Mapping.Add("1110052", "3459"); //YLWanneng3459
            Mapping.Add("1110053", "3467"); //NCWanneng3467
            Mapping.Add("1110054", "3467"); //YLWanneng3467
            Mapping.Add("1110055", "3789"); //NCWanneng3789
            Mapping.Add("1110056", "3789"); //YLWanneng3789
            Mapping.Add("1110057", "4578"); //NCWanneng4578
            Mapping.Add("1110058", "4578"); //YLWanneng4578
            Mapping.Add("1110059", "5689"); //NCWanneng5689
            Mapping.Add("1110060", "5689"); //YLWanneng5689
            Mapping.Add("1110061", "01249"); //NCWanneng01249
            Mapping.Add("1110062", "01249"); //YLWanneng01249
            Mapping.Add("1110063", "01268"); //NCWanneng01268
            Mapping.Add("1110064", "01268"); //YLWanneng01268
            Mapping.Add("1110065", "01346"); //NCWanneng01346
            Mapping.Add("1110066", "01346"); //YLWanneng01346
            Mapping.Add("1110067", "01467"); //NCWanneng01467
            Mapping.Add("1110068", "01467"); //YLWanneng01467
            Mapping.Add("1110069", "01569"); //NCWanneng01569
            Mapping.Add("1110070", "01569"); //YLWanneng01569
            Mapping.Add("1110071", "02357"); //NCWanneng02357
            Mapping.Add("1110072", "02357"); //YLWanneng02357
            Mapping.Add("1110073", "02458"); //NCWanneng02458
            Mapping.Add("1110074", "02458"); //YLWanneng02458
            Mapping.Add("1110075", "03789"); //NCWanneng03789
            Mapping.Add("1110076", "03789"); //YLWanneng03789
            Mapping.Add("1110077", "12359"); //NCWanneng12359
            Mapping.Add("1110078", "12359"); //YLWanneng12359
            Mapping.Add("1110079", "12378"); //NCWanneng12378
            Mapping.Add("1110080", "12378"); //YLWanneng12378
            Mapping.Add("1110081", "12589"); //NCWanneng12589
            Mapping.Add("1110082", "12589"); //YLWanneng12589
            Mapping.Add("1110083", "13478"); //NCWanneng13478
            Mapping.Add("1110084", "13478"); //YLWanneng13478
            Mapping.Add("1110085", "14579"); //NCWanneng14579
            Mapping.Add("1110086", "14579"); //YLWanneng14579
            Mapping.Add("1110087", "23456"); //NCWanneng23456
            Mapping.Add("1110088", "23456"); //YLWanneng23456
            Mapping.Add("1110089", "24679"); //NCWanneng24679
            Mapping.Add("1110090", "24679"); //YLWanneng24679
            Mapping.Add("1110091", "34689"); //NCWanneng34689
            Mapping.Add("1110092", "34689"); //YLWanneng34689
            Mapping.Add("1110093", "35678"); //NCWanneng35678
            Mapping.Add("1110094", "35678"); //YLWanneng35678
            Mapping.Add("1110095", "012346"); //NCWanneng012346
            Mapping.Add("1110096", "012346"); //YLWanneng012346
            Mapping.Add("1110097", "012359"); //NCWanneng012359
            Mapping.Add("1110098", "012359"); //YLWanneng012359
            Mapping.Add("1110099", "012489"); //NCWanneng012489
            Mapping.Add("1110100", "012489"); //YLWanneng012489
            Mapping.Add("1110101", "013789"); //NCWanneng013789
            Mapping.Add("1110102", "013789"); //YLWanneng013789
            Mapping.Add("1110103", "026789"); //NCWanneng026789
            Mapping.Add("1110104", "026789"); //YLWanneng026789
            Mapping.Add("1110105", "045678"); //NCWanneng045678
            Mapping.Add("1110106", "045678"); //YLWanneng045678
            Mapping.Add("1110107", "123457"); //NCWanneng123457
            Mapping.Add("1110108", "123457"); //YLWanneng123457
            Mapping.Add("1110109", "156789"); //NCWanneng156789
            Mapping.Add("1110110", "156789"); //YLWanneng156789
            Mapping.Add("1110111", "234568"); //NCWanneng234568
            Mapping.Add("1110112", "234568"); //YLWanneng234568
            Mapping.Add("1110113", "345679"); //NCWanneng345679
            Mapping.Add("1110114", "345679"); //YLWanneng345679
            Mapping.Add("1110115", "0123489"); //NCWanneng0123489
            Mapping.Add("1110116", "0123489"); //YLWanneng0123489
            Mapping.Add("1110117", "0345679"); //NCWanneng0345679
            Mapping.Add("1110118", "0345679"); //YLWanneng0345679
            Mapping.Add("1110119", "0156789"); //NCWanneng0156789
            Mapping.Add("1110120", "0156789"); //YLWanneng0156789
            Mapping.Add("1110121", "1234567"); //NCWanneng1234567
            Mapping.Add("1110122", "1234567"); //YLWanneng1234567
            Mapping.Add("1110123", "0245678"); //NCWanneng0245678
            Mapping.Add("1110124", "0245678"); //YLWanneng0245678
            Mapping.Add("1110125", "2356789"); //NCWanneng2356789
            Mapping.Add("1110126", "2356789"); //YLWanneng2356789

            #endregion

            #region 时时彩后三 1111

            Mapping.Add("1111001", "豹子");//NCBaozi
            Mapping.Add("1111002", "豹子");//YLBaozi
            Mapping.Add("1111003", "组3");//NCZ3
            Mapping.Add("1111004", "组3");//YLZ3
            Mapping.Add("1111005", "组6");//NCZ6
            Mapping.Add("1111006", "组6");//YLZ6
            Mapping.Add("1111007", "0"); //NCFenbu0
            Mapping.Add("1111008", "0"); //YLFenbu0
            Mapping.Add("1111009", "1"); //NCFenbu1
            Mapping.Add("1111010", "1"); //YLFenbu1
            Mapping.Add("1111011", "2"); //NCFenbu2
            Mapping.Add("1111012", "2"); //YLFenbu2
            Mapping.Add("1111013", "3"); //NCFenbu3
            Mapping.Add("1111014", "3"); //YLFenbu3
            Mapping.Add("1111015", "4"); //NCFenbu4
            Mapping.Add("1111016", "4"); //YLFenbu4
            Mapping.Add("1111017", "5"); //NCFenbu5
            Mapping.Add("1111018", "5"); //YLFenbu5
            Mapping.Add("1111019", "6"); //NCFenbu6
            Mapping.Add("1111020", "6"); //YLFenbu6
            Mapping.Add("1111021", "7"); //NCFenbu7
            Mapping.Add("1111022", "7"); //YLFenbu7
            Mapping.Add("1111023", "8"); //NCFenbu8
            Mapping.Add("1111024", "8"); //YLFenbu8
            Mapping.Add("1111025", "9"); //NCFenbu9
            Mapping.Add("1111026", "9"); //YLFenbu9
            Mapping.Add("1111027", "0"); //NCKuadu0
            Mapping.Add("1111028", "0"); //YLKuadu0
            Mapping.Add("1111029", "1"); //NCKuadu1
            Mapping.Add("1111030", "1"); //YLKuadu1
            Mapping.Add("1111031", "2"); //NCKuadu2
            Mapping.Add("1111032", "2"); //YLKuadu2
            Mapping.Add("1111033", "3"); //NCKuadu3
            Mapping.Add("1111034", "3"); //YLKuadu3
            Mapping.Add("1111035", "4"); //NCKuadu4
            Mapping.Add("1111036", "4"); //YLKuadu4
            Mapping.Add("1111037", "5"); //NCKuadu5
            Mapping.Add("1111038", "5"); //YLKuadu5
            Mapping.Add("1111039", "6"); //NCKuadu6
            Mapping.Add("1111040", "6"); //YLKuadu6
            Mapping.Add("1111041", "7"); //NCKuadu7
            Mapping.Add("1111042", "7"); //YLKuadu7
            Mapping.Add("1111043", "8"); //NCKuadu8
            Mapping.Add("1111044", "8"); //YLKuadu8
            Mapping.Add("1111045", "9"); //NCKuadu9
            Mapping.Add("1111046", "9"); //YLKuadu9
            Mapping.Add("1111047", "30");//NCDaxiaobi30
            Mapping.Add("1111048", "30");//YLDaxiaobi30
            Mapping.Add("1111049", "21");//NCDaxiaobi21
            Mapping.Add("1111050", "21");//YLDaxiaobi21
            Mapping.Add("1111051", "12");//NCDaxiaobi12
            Mapping.Add("1111052", "12");//YLDaxiaobi12
            Mapping.Add("1111053", "03");//NCDaxiaobi03
            Mapping.Add("1111054", "03");//YLDaxiaobi03
            Mapping.Add("1111055", "30");//NCJioubi30
            Mapping.Add("1111056", "30");//YLJioubi30
            Mapping.Add("1111057", "21");//NCJioubi21
            Mapping.Add("1111058", "21");//YLJioubi21
            Mapping.Add("1111059", "12");//NCJioubi12
            Mapping.Add("1111060", "12");//YLJioubi12
            Mapping.Add("1111061", "03");//NCJioubi03
            Mapping.Add("1111062", "03");//YLJioubi03
            Mapping.Add("1111063", "30");//NCZhihebi30
            Mapping.Add("1111064", "30");//YLZhihebi30
            Mapping.Add("1111065", "21");//NCZhihebi21
            Mapping.Add("1111066", "21");//YLZhihebi21
            Mapping.Add("1111067", "12");//NCZhihebi12
            Mapping.Add("1111068", "12");//YLZhihebi12
            Mapping.Add("1111069", "03");//NCZhihebi03
            Mapping.Add("1111070", "03");//YLZhihebi03
            Mapping.Add("1111071", "0"); //NCKuaduBS0
            Mapping.Add("1111072", "0"); //YLKuaduBS0
            Mapping.Add("1111073", "1"); //NCKuaduBS1
            Mapping.Add("1111074", "1"); //YLKuaduBS1
            Mapping.Add("1111075", "2"); //NCKuaduBS2
            Mapping.Add("1111076", "2"); //YLKuaduBS2
            Mapping.Add("1111077", "3"); //NCKuaduBS3
            Mapping.Add("1111078", "3"); //YLKuaduBS3
            Mapping.Add("1111079", "4"); //NCKuaduBS4
            Mapping.Add("1111080", "4"); //YLKuaduBS4
            Mapping.Add("1111081", "5"); //NCKuaduBS5
            Mapping.Add("1111082", "5"); //YLKuaduBS5
            Mapping.Add("1111083", "6"); //NCKuaduBS6
            Mapping.Add("1111084", "6"); //YLKuaduBS6
            Mapping.Add("1111085", "7"); //NCKuaduBS7
            Mapping.Add("1111086", "7"); //YLKuaduBS7
            Mapping.Add("1111087", "8"); //NCKuaduBS8
            Mapping.Add("1111088", "8"); //YLKuaduBS8
            Mapping.Add("1111089", "9"); //NCKuaduBS9
            Mapping.Add("1111090", "9"); //YLKuaduBS9
            Mapping.Add("1111091", "0"); //NCKuaduSG0
            Mapping.Add("1111092", "0"); //YLKuaduSG0
            Mapping.Add("1111093", "1"); //NCKuaduSG1
            Mapping.Add("1111094", "1"); //YLKuaduSG1
            Mapping.Add("1111095", "2"); //NCKuaduSG2
            Mapping.Add("1111096", "2"); //YLKuaduSG2
            Mapping.Add("1111097", "3"); //NCKuaduSG3
            Mapping.Add("1111098", "3"); //YLKuaduSG3
            Mapping.Add("1111099", "4"); //NCKuaduSG4
            Mapping.Add("1111100", "4"); //YLKuaduSG4
            Mapping.Add("1111101", "5"); //NCKuaduSG5
            Mapping.Add("1111102", "5"); //YLKuaduSG5
            Mapping.Add("1111103", "6"); //NCKuaduSG6
            Mapping.Add("1111104", "6"); //YLKuaduSG6
            Mapping.Add("1111105", "7"); //NCKuaduSG7
            Mapping.Add("1111106", "7"); //YLKuaduSG7
            Mapping.Add("1111107", "8"); //NCKuaduSG8
            Mapping.Add("1111108", "8"); //YLKuaduSG8
            Mapping.Add("1111109", "9"); //NCKuaduSG9
            Mapping.Add("1111110", "9"); //YLKuaduSG9
            Mapping.Add("1111111", "3030"); //NCDaxiaojiou3030
            Mapping.Add("1111112", "3030"); //YLDaxiaojiou3030
            Mapping.Add("1111113", "3021"); //NCDaxiaojiou3021
            Mapping.Add("1111114", "3021"); //YLDaxiaojiou3021
            Mapping.Add("1111115", "3012"); //NCDaxiaojiou3012
            Mapping.Add("1111116", "3012"); //YLDaxiaojiou3012
            Mapping.Add("1111117", "3003"); //NCDaxiaojiou3003
            Mapping.Add("1111118", "3003"); //YLDaxiaojiou3003
            Mapping.Add("1111119", "2130"); //NCDaxiaojiou2130
            Mapping.Add("1111120", "2130"); //YLDaxiaojiou2130
            Mapping.Add("1111121", "2121"); //NCDaxiaojiou2121
            Mapping.Add("1111122", "2121"); //YLDaxiaojiou2121
            Mapping.Add("1111123", "2112"); //NCDaxiaojiou2112
            Mapping.Add("1111124", "2112"); //YLDaxiaojiou2112
            Mapping.Add("1111125", "2103"); //NCDaxiaojiou2103
            Mapping.Add("1111126", "2103"); //YLDaxiaojiou2103
            Mapping.Add("1111127", "1230"); //NCDaxiaojiou1230
            Mapping.Add("1111128", "1230"); //YLDaxiaojiou1230
            Mapping.Add("1111129", "1221"); //NCDaxiaojiou1221
            Mapping.Add("1111130", "1221"); //YLDaxiaojiou1221
            Mapping.Add("1111131", "1212"); //NCDaxiaojiou1212
            Mapping.Add("1111132", "1212"); //YLDaxiaojiou1212
            Mapping.Add("1111133", "1203"); //NCDaxiaojiou1203
            Mapping.Add("1111134", "1203"); //YLDaxiaojiou1203
            Mapping.Add("1111135", "0330"); //NCDaxiaojiou0330
            Mapping.Add("1111136", "0330"); //YLDaxiaojiou0330
            Mapping.Add("1111137", "0321"); //NCDaxiaojiou0321
            Mapping.Add("1111138", "0321"); //YLDaxiaojiou0321
            Mapping.Add("1111139", "0312"); //NCDaxiaojiou0312
            Mapping.Add("1111140", "0312"); //YLDaxiaojiou0312
            Mapping.Add("1111141", "0303"); //NCDaxiaojiou0303
            Mapping.Add("1111142", "0303"); //YLDaxiaojiou0303
            Mapping.Add("1111143", "0"); //NC012Baiwei0
            Mapping.Add("1111144", "0"); //YL012Baiwei0
            Mapping.Add("1111145", "1"); //NC012Baiwei1
            Mapping.Add("1111146", "1"); //YL012Baiwei1
            Mapping.Add("1111147", "2"); //NC012Baiwei2
            Mapping.Add("1111148", "2"); //YL012Baiwei2
            Mapping.Add("1111149", "0"); //NC012Shiwei0
            Mapping.Add("1111150", "0"); //YL012Shiwei0
            Mapping.Add("1111151", "1"); //NC012Shiwei1
            Mapping.Add("1111152", "1"); //YL012Shiwei1
            Mapping.Add("1111153", "2"); //NC012Shiwei2
            Mapping.Add("1111154", "2"); //YL012Shiwei2
            Mapping.Add("1111155", "0"); //NC012Gewei0
            Mapping.Add("1111156", "0"); //YL012Gewei0
            Mapping.Add("1111157", "1"); //NC012Gewei1
            Mapping.Add("1111158", "1"); //YL012Gewei1
            Mapping.Add("1111159", "2"); //NC012Gewei2
            Mapping.Add("1111160", "2"); //YL012Gewei2
            Mapping.Add("1111161", "000"); //NC012Xingtai000
            Mapping.Add("1111162", "000"); //YL012Xingtai000
            Mapping.Add("1111163", "001"); //NC012Xingtai001
            Mapping.Add("1111164", "001"); //YL012Xingtai001
            Mapping.Add("1111165", "002"); //NC012Xingtai002
            Mapping.Add("1111166", "002"); //YL012Xingtai002
            Mapping.Add("1111167", "010"); //NC012Xingtai010
            Mapping.Add("1111168", "010"); //YL012Xingtai010
            Mapping.Add("1111169", "011"); //NC012Xingtai011
            Mapping.Add("1111170", "011"); //YL012Xingtai011
            Mapping.Add("1111171", "012"); //NC012Xingtai012
            Mapping.Add("1111172", "012"); //YL012Xingtai012
            Mapping.Add("1111173", "020"); //NC012Xingtai020
            Mapping.Add("1111174", "020"); //YL012Xingtai020
            Mapping.Add("1111175", "021"); //NC012Xingtai021
            Mapping.Add("1111176", "021"); //YL012Xingtai021
            Mapping.Add("1111177", "022"); //NC012Xingtai022
            Mapping.Add("1111178", "022"); //YL012Xingtai022
            Mapping.Add("1111179", "100"); //NC012Xingtai100
            Mapping.Add("1111180", "100"); //YL012Xingtai100
            Mapping.Add("1111181", "101"); //NC012Xingtai101
            Mapping.Add("1111182", "101"); //YL012Xingtai101
            Mapping.Add("1111183", "102"); //NC012Xingtai102
            Mapping.Add("1111184", "102"); //YL012Xingtai102
            Mapping.Add("1111185", "110"); //NC012Xingtai110
            Mapping.Add("1111186", "110"); //YL012Xingtai110
            Mapping.Add("1111187", "111"); //NC012Xingtai111
            Mapping.Add("1111188", "111"); //YL012Xingtai111
            Mapping.Add("1111189", "112"); //NC012Xingtai112
            Mapping.Add("1111190", "112"); //YL012Xingtai112
            Mapping.Add("1111191", "120"); //NC012Xingtai120
            Mapping.Add("1111192", "120"); //YL012Xingtai120
            Mapping.Add("1111193", "121"); //NC012Xingtai121
            Mapping.Add("1111194", "121"); //YL012Xingtai121
            Mapping.Add("1111195", "122"); //NC012Xingtai122
            Mapping.Add("1111196", "122"); //YL012Xingtai122
            Mapping.Add("1111197", "200"); //NC012Xingtai200
            Mapping.Add("1111198", "200"); //YL012Xingtai200
            Mapping.Add("1111199", "201"); //NC012Xingtai201
            Mapping.Add("1111200", "201"); //YL012Xingtai201
            Mapping.Add("1111201", "202"); //NC012Xingtai202
            Mapping.Add("1111202", "202"); //YL012Xingtai202
            Mapping.Add("1111203", "210"); //NC012Xingtai210
            Mapping.Add("1111204", "210"); //YL012Xingtai210
            Mapping.Add("1111205", "211"); //NC012Xingtai211
            Mapping.Add("1111206", "211"); //YL012Xingtai211
            Mapping.Add("1111207", "212"); //NC012Xingtai212
            Mapping.Add("1111208", "212"); //YL012Xingtai212
            Mapping.Add("1111209", "220"); //NC012Xingtai220
            Mapping.Add("1111210", "220"); //YL012Xingtai220
            Mapping.Add("1111211", "221"); //NC012Xingtai221
            Mapping.Add("1111212", "221"); //YL012Xingtai221
            Mapping.Add("1111213", "222"); //NC012Xingtai222
            Mapping.Add("1111214", "222"); //YL012Xingtai222
            Mapping.Add("1111215", "0"); //NC012Yu0Haoma0
            Mapping.Add("1111216", "0"); //YL012Yu0Haoma0
            Mapping.Add("1111217", "3"); //NC012Yu0Haoma3
            Mapping.Add("1111218", "3"); //YL012Yu0Haoma3
            Mapping.Add("1111219", "6"); //NC012Yu0Haoma6
            Mapping.Add("1111220", "6"); //YL012Yu0Haoma6
            Mapping.Add("1111221", "9"); //NC012Yu0Haoma9
            Mapping.Add("1111222", "9"); //YL012Yu0Haoma9
            Mapping.Add("1111223", "1"); //NC012Yu1Haoma1
            Mapping.Add("1111224", "1"); //YL012Yu1Haoma1
            Mapping.Add("1111225", "4"); //NC012Yu1Haoma4
            Mapping.Add("1111226", "4"); //YL012Yu1Haoma4
            Mapping.Add("1111227", "7"); //NC012Yu1Haoma7
            Mapping.Add("1111228", "7"); //YL012Yu1Haoma7
            Mapping.Add("1111229", "2"); //NC012Yu2Haoma2
            Mapping.Add("1111230", "2"); //YL012Yu2Haoma2
            Mapping.Add("1111231", "5"); //NC012Yu2Haoma5
            Mapping.Add("1111232", "5"); //YL012Yu2Haoma5
            Mapping.Add("1111233", "8"); //NC012Yu2Haoma8
            Mapping.Add("1111234", "8"); //YL012Yu2Haoma8
            Mapping.Add("1111235", "0"); //NCHz0
            Mapping.Add("1111236", "0"); //YLHz0
            Mapping.Add("1111237", "1"); //NCHz1
            Mapping.Add("1111238", "1"); //YLHz1
            Mapping.Add("1111239", "2"); //NCHz2
            Mapping.Add("1111240", "2"); //YLHz2
            Mapping.Add("1111241", "3"); //NCHz3
            Mapping.Add("1111242", "3"); //YLHz3
            Mapping.Add("1111243", "4"); //NCHz4
            Mapping.Add("1111244", "4"); //YLHz4
            Mapping.Add("1111245", "5"); //NCHz5
            Mapping.Add("1111246", "5"); //YLHz5
            Mapping.Add("1111247", "6"); //NCHz6
            Mapping.Add("1111248", "6"); //YLHz6
            Mapping.Add("1111249", "7"); //NCHz7
            Mapping.Add("1111250", "7"); //YLHz7
            Mapping.Add("1111251", "8"); //NCHz8
            Mapping.Add("1111252", "8"); //YLHz8
            Mapping.Add("1111253", "9"); //NCHz9
            Mapping.Add("1111254", "9"); //YLHz9
            Mapping.Add("1111255", "10");//NCHz10
            Mapping.Add("1111256", "10");//YLHz10
            Mapping.Add("1111257", "11");//NCHz11
            Mapping.Add("1111258", "11");//YLHz11
            Mapping.Add("1111259", "12");//NCHz12
            Mapping.Add("1111260", "12");//YLHz12
            Mapping.Add("1111261", "13");//NCHz13
            Mapping.Add("1111262", "13");//YLHz13
            Mapping.Add("1111263", "14");//NCHz14
            Mapping.Add("1111264", "14");//YLHz14
            Mapping.Add("1111265", "15");//NCHz15
            Mapping.Add("1111266", "15");//YLHz15
            Mapping.Add("1111267", "16");//NCHz16
            Mapping.Add("1111268", "16");//YLHz16
            Mapping.Add("1111269", "17");//NCHz17
            Mapping.Add("1111270", "17");//YLHz17
            Mapping.Add("1111271", "18");//NCHz18
            Mapping.Add("1111272", "18");//YLHz18
            Mapping.Add("1111273", "19");//NCHz19
            Mapping.Add("1111274", "19");//YLHz19
            Mapping.Add("1111275", "20");//NCHz20
            Mapping.Add("1111276", "20");//YLHz20
            Mapping.Add("1111277", "21");//NCHz21
            Mapping.Add("1111278", "21");//YLHz21
            Mapping.Add("1111279", "22");//NCHz22
            Mapping.Add("1111280", "22");//YLHz22
            Mapping.Add("1111281", "23");//NCHz23
            Mapping.Add("1111282", "23");//YLHz23
            Mapping.Add("1111283", "24");//NCHz24
            Mapping.Add("1111284", "24");//YLHz24
            Mapping.Add("1111285", "25");//NCHz25
            Mapping.Add("1111286", "25");//YLHz25
            Mapping.Add("1111287", "26");//NCHz26
            Mapping.Add("1111288", "26");//YLHz26
            Mapping.Add("1111289", "27");//NCHz27
            Mapping.Add("1111290", "27");//YLHz27
            Mapping.Add("1111291", "0-8");//NCHz0to8
            Mapping.Add("1111292", "0-8");//YLHz0to8
            Mapping.Add("1111293", "9-11");//NCHz9to11
            Mapping.Add("1111294", "9-11");//YLHz9to11
            Mapping.Add("1111295", "12-13"); //NCHz12to13
            Mapping.Add("1111296", "12-13"); //YLHz12to13
            Mapping.Add("1111297", "14-15"); //NCHz14to15
            Mapping.Add("1111298", "14-15"); //YLHz14to15
            Mapping.Add("1111299", "16-18"); //NCHz16to18
            Mapping.Add("1111300", "16-18"); //YLHz16to18
            Mapping.Add("1111301", "19-27"); //NCHz19to27
            Mapping.Add("1111302", "19-27"); //YLHz19to27
            Mapping.Add("1111303", "0"); //NCHzws0
            Mapping.Add("1111304", "0"); //YLHzws0
            Mapping.Add("1111305", "1"); //NCHzws1
            Mapping.Add("1111306", "1"); //YLHzws1
            Mapping.Add("1111307", "2"); //NCHzws2
            Mapping.Add("1111308", "2"); //YLHzws2
            Mapping.Add("1111309", "3"); //NCHzws3
            Mapping.Add("1111310", "3"); //YLHzws3
            Mapping.Add("1111311", "4"); //NCHzws4
            Mapping.Add("1111312", "4"); //YLHzws4
            Mapping.Add("1111313", "5"); //NCHzws5
            Mapping.Add("1111314", "5"); //YLHzws5
            Mapping.Add("1111315", "6"); //NCHzws6
            Mapping.Add("1111316", "6"); //YLHzws6
            Mapping.Add("1111317", "7"); //NCHzws7
            Mapping.Add("1111318", "7"); //YLHzws7
            Mapping.Add("1111319", "8"); //NCHzws8
            Mapping.Add("1111320", "8"); //YLHzws8
            Mapping.Add("1111321", "9"); //NCHzws9
            Mapping.Add("1111322", "9"); //YLHzws9

            #endregion

            #region 时时彩后三万能码 1112

            Mapping.Add("1112001", "0126"); //NCWanneng0126
            Mapping.Add("1112002", "0126"); //YLWanneng0126
            Mapping.Add("1112003", "0134"); //NCWanneng0134
            Mapping.Add("1112004", "0134"); //YLWanneng0134
            Mapping.Add("1112005", "0159"); //NCWanneng0159
            Mapping.Add("1112006", "0159"); //YLWanneng0159
            Mapping.Add("1112007", "0178"); //NCWanneng0178
            Mapping.Add("1112008", "0178"); //YLWanneng0178
            Mapping.Add("1112009", "0239"); //NCWanneng0239
            Mapping.Add("1112010", "0239"); //YLWanneng0239
            Mapping.Add("1112011", "0247"); //NCWanneng0247
            Mapping.Add("1112012", "0247"); //YLWanneng0247
            Mapping.Add("1112013", "0258"); //NCWanneng0258
            Mapping.Add("1112014", "0258"); //YLWanneng0258
            Mapping.Add("1112015", "0357"); //NCWanneng0357
            Mapping.Add("1112016", "0357"); //YLWanneng0357
            Mapping.Add("1112017", "0368"); //NCWanneng0368
            Mapping.Add("1112018", "0368"); //YLWanneng0368
            Mapping.Add("1112019", "0456"); //NCWanneng0456
            Mapping.Add("1112020", "0456"); //YLWanneng0456
            Mapping.Add("1112021", "0489"); //NCWanneng0489
            Mapping.Add("1112022", "0489"); //YLWanneng0489
            Mapping.Add("1112023", "0679"); //NCWanneng0679
            Mapping.Add("1112024", "0679"); //YLWanneng0679
            Mapping.Add("1112025", "1237"); //NCWanneng1237
            Mapping.Add("1112026", "1237"); //YLWanneng1237
            Mapping.Add("1112027", "1245"); //NCWanneng1245
            Mapping.Add("1112028", "1245"); //YLWanneng1245
            Mapping.Add("1112029", "1289"); //NCWanneng1289
            Mapping.Add("1112030", "1289"); //YLWanneng1289
            Mapping.Add("1112031", "1358"); //NCWanneng1358
            Mapping.Add("1112032", "1358"); //YLWanneng1358
            Mapping.Add("1112033", "1369"); //NCWanneng1369
            Mapping.Add("1112034", "1369"); //YLWanneng1369
            Mapping.Add("1112035", "1468"); //NCWanneng1468
            Mapping.Add("1112036", "1468"); //YLWanneng1468
            Mapping.Add("1112037", "1479"); //NCWanneng1479
            Mapping.Add("1112038", "1479"); //YLWanneng1479
            Mapping.Add("1112039", "1567"); //NCWanneng1567
            Mapping.Add("1112040", "1567"); //YLWanneng1567
            Mapping.Add("1112041", "2348"); //NCWanneng2348
            Mapping.Add("1112042", "2348"); //YLWanneng2348
            Mapping.Add("1112043", "2356"); //NCWanneng2356
            Mapping.Add("1112044", "2356"); //YLWanneng2356
            Mapping.Add("1112045", "2469"); //NCWanneng2469
            Mapping.Add("1112046", "2469"); //YLWanneng2469
            Mapping.Add("1112047", "2579"); //NCWanneng2579
            Mapping.Add("1112048", "2579"); //YLWanneng2579
            Mapping.Add("1112049", "2678"); //NCWanneng2678
            Mapping.Add("1112050", "2678"); //YLWanneng2678
            Mapping.Add("1112051", "3459"); //NCWanneng3459
            Mapping.Add("1112052", "3459"); //YLWanneng3459
            Mapping.Add("1112053", "3467"); //NCWanneng3467
            Mapping.Add("1112054", "3467"); //YLWanneng3467
            Mapping.Add("1112055", "3789"); //NCWanneng3789
            Mapping.Add("1112056", "3789"); //YLWanneng3789
            Mapping.Add("1112057", "4578"); //NCWanneng4578
            Mapping.Add("1112058", "4578"); //YLWanneng4578
            Mapping.Add("1112059", "5689"); //NCWanneng5689
            Mapping.Add("1112060", "5689"); //YLWanneng5689
            Mapping.Add("1112061", "01249"); //NCWanneng01249
            Mapping.Add("1112062", "01249"); //YLWanneng01249
            Mapping.Add("1112063", "01268"); //NCWanneng01268
            Mapping.Add("1112064", "01268"); //YLWanneng01268
            Mapping.Add("1112065", "01346"); //NCWanneng01346
            Mapping.Add("1112066", "01346"); //YLWanneng01346
            Mapping.Add("1112067", "01467"); //NCWanneng01467
            Mapping.Add("1112068", "01467"); //YLWanneng01467
            Mapping.Add("1112069", "01569"); //NCWanneng01569
            Mapping.Add("1112070", "01569"); //YLWanneng01569
            Mapping.Add("1112071", "02357"); //NCWanneng02357
            Mapping.Add("1112072", "02357"); //YLWanneng02357
            Mapping.Add("1112073", "02458"); //NCWanneng02458
            Mapping.Add("1112074", "02458"); //YLWanneng02458
            Mapping.Add("1112075", "03789"); //NCWanneng03789
            Mapping.Add("1112076", "03789"); //YLWanneng03789
            Mapping.Add("1112077", "12359"); //NCWanneng12359
            Mapping.Add("1112078", "12359"); //YLWanneng12359
            Mapping.Add("1112079", "12378"); //NCWanneng12378
            Mapping.Add("1112080", "12378"); //YLWanneng12378
            Mapping.Add("1112081", "12589"); //NCWanneng12589
            Mapping.Add("1112082", "12589"); //YLWanneng12589
            Mapping.Add("1112083", "13478"); //NCWanneng13478
            Mapping.Add("1112084", "13478"); //YLWanneng13478
            Mapping.Add("1112085", "14579"); //NCWanneng14579
            Mapping.Add("1112086", "14579"); //YLWanneng14579
            Mapping.Add("1112087", "23456"); //NCWanneng23456
            Mapping.Add("1112088", "23456"); //YLWanneng23456
            Mapping.Add("1112089", "24679"); //NCWanneng24679
            Mapping.Add("1112090", "24679"); //YLWanneng24679
            Mapping.Add("1112091", "34689"); //NCWanneng34689
            Mapping.Add("1112092", "34689"); //YLWanneng34689
            Mapping.Add("1112093", "35678"); //NCWanneng35678
            Mapping.Add("1112094", "35678"); //YLWanneng35678
            Mapping.Add("1112095", "012346"); //NCWanneng012346
            Mapping.Add("1112096", "012346"); //YLWanneng012346
            Mapping.Add("1112097", "012359"); //NCWanneng012359
            Mapping.Add("1112098", "012359"); //YLWanneng012359
            Mapping.Add("1112099", "012489"); //NCWanneng012489
            Mapping.Add("1112100", "012489"); //YLWanneng012489
            Mapping.Add("1112101", "013789"); //NCWanneng013789
            Mapping.Add("1112102", "013789"); //YLWanneng013789
            Mapping.Add("1112103", "026789"); //NCWanneng026789
            Mapping.Add("1112104", "026789"); //YLWanneng026789
            Mapping.Add("1112105", "045678"); //NCWanneng045678
            Mapping.Add("1112106", "045678"); //YLWanneng045678
            Mapping.Add("1112107", "123457"); //NCWanneng123457
            Mapping.Add("1112108", "123457"); //YLWanneng123457
            Mapping.Add("1112109", "156789"); //NCWanneng156789
            Mapping.Add("1112110", "156789"); //YLWanneng156789
            Mapping.Add("1112111", "234568"); //NCWanneng234568
            Mapping.Add("1112112", "234568"); //YLWanneng234568
            Mapping.Add("1112113", "345679"); //NCWanneng345679
            Mapping.Add("1112114", "345679"); //YLWanneng345679
            Mapping.Add("1112115", "0123489"); //NCWanneng0123489
            Mapping.Add("1112116", "0123489"); //YLWanneng0123489
            Mapping.Add("1112117", "0345679"); //NCWanneng0345679
            Mapping.Add("1112118", "0345679"); //YLWanneng0345679
            Mapping.Add("1112119", "0156789"); //NCWanneng0156789
            Mapping.Add("1112120", "0156789"); //YLWanneng0156789
            Mapping.Add("1112121", "1234567"); //NCWanneng1234567
            Mapping.Add("1112122", "1234567"); //YLWanneng1234567
            Mapping.Add("1112123", "0245678"); //NCWanneng0245678
            Mapping.Add("1112124", "0245678"); //YLWanneng0245678
            Mapping.Add("1112125", "2356789"); //NCWanneng2356789
            Mapping.Add("1112126", "2356789"); //YLWanneng2356789

            #endregion

        }

        /// <summary>
        /// 获取或设置当前排序项所处的行下标
        /// </summary>
        internal int intRowIndex { get; set; }

        /// <summary>
        /// 获取或设置当前排序项所处的列下标
        /// </summary>
        internal int intColumnIndex { get; set; }

        /// <summary>
        /// 获取或设置当前排序项的值
        /// </summary>
        internal double dblValue { get; set; }

        /// <summary>
        /// 获取或设置当前排序项的行列信息，比如"1101001"
        /// </summary>
        internal string strFieldId { get; set; }

        /// <summary>
        /// 获取或设置当前排序项经过计算后所包含的字段编号列表
        /// </summary>
        /// <remarks>
        /// 列表元素值举例：1101002,1102006,1103004(分别代表万位0、千位2、百位1)
        /// </remarks>
        internal List<string> listFieldIds = new List<string>();

        /// <summary>
        /// 将字段编号列表转化为拼接好的字符串
        /// </summary>
        /// <returns>返回可以直接下注的号码，如：将列表元素1101002,1102006,1103004转为"021"输出</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in listFieldIds)
            {
                sb.Append(Mapping[item]);//通过映射字典将字段编号1101001还原为对应的号码"0"
            }
            return sb.ToString();
        }

        /// <summary>
        /// 重载两个项相乘的方法，并将参加运算的对象从未出现的字段编号加入字段编号列表
        /// </summary>
        /// <param name="left">乘数</param>
        /// <param name="right">被乘数</param>
        /// <returns></returns>
        public static SortItem operator *(SortItem left, SortItem right)
        {
            SortItem sortitem = new SortItem();
            sortitem.dblValue = left.dblValue * right.dblValue;//完成乘法运算

            #region 将参加运算的对象从未出现的字段编号加入字段编号列表

            if (left.listFieldIds.Count == 0)//如果该数从来没参加过乘法运算，那么就将该字段的编号加入字段编号列表
                sortitem.listFieldIds.Add(left.strFieldId);
            else//如果该数是曾经过乘法运算的结果，那么将曾经记录的字段编号列表传递给新的(积)sortitem对象
                sortitem.listFieldIds.AddRange(left.listFieldIds.ToArray());

            if (right.listFieldIds.Count == 0)
                sortitem.listFieldIds.Add(right.strFieldId);
            else
                sortitem.listFieldIds.AddRange(right.listFieldIds.ToArray());

            #endregion

            return sortitem;
        }
    }

    #endregion

    #region 一维排序方法

    /// <summary>
    /// 自定义 CustomerSortList类，继承SortList类，可包含重复键s
    /// 用于对TEntity<float,string>类的集合进行排序
    /// </summary>
    public class CustomSortList : SortedList<float, string>
    {
        public CustomSortList()
            : base(ListComparer.InitListComparerEntity)
        { }
    }

    public class ListComparer : IComparer<float>
    {
        /// <summary>
        /// 设置排序方式，该排序方式由用户自定义设置，来自枚举OrderDirection
        /// </summary>
        public static OrderDirection orderDirection { set; get; }

        /// <summary>
        /// 定义ListComparer类对象
        /// </summary>
        private static ListComparer entity;

        /// <summary>
        /// 初始化ListCompare类对象
        /// </summary>
        public static ListComparer InitListComparerEntity
        {

            get
            {
                if (entity == null) entity = new ListComparer();
                return entity;
            }
        }

        /// <summary>
        /// 比较方法，接受float类型参数，
        /// 本方法为IComparer内部方法，实现即可
        /// </summary>
        /// <param name="floatParamOne">float参数1</param>
        /// <param name="floatParamTwo">floa参数2</param>
        /// <returns>返回1：表示降序，返回-1：表示升序</returns>
        public int Compare(float floatParamOne, float floatParamTwo)
        {
            //如果两个数相等则需要将按默认顺序排列
            //如：list.add(10,"a"),list.add(8,"b"),list.add(10,"c")
            //那么key=10的记录会按照集合中的次序排序，输出的值分别为，a,c,b or b,a,c 
            if (floatParamOne == floatParamTwo)
            {
                return -1;
            }
            if (orderDirection == OrderDirection.DESC)//1表示降序，-1表示升序
            {
                if (floatParamOne < floatParamTwo) return 1;
                else return -1;
            }
            else
            {
                if (floatParamOne < floatParamTwo) return -1;
                else return 1;
            }
        }
    }

    #endregion

    /// <summary>
    /// 设置排序方向
    /// </summary>
    public enum OrderDirection
    {
        /// <summary>
        /// 降序排序
        /// </summary>
        DESC,
        /// <summary>
        /// 升序排序
        /// </summary>
        ASC
    }
}
