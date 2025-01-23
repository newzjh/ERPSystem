using System;
using System.Collections.Generic;
using System.Text;


namespace ErpManageLibrary
{
    /// <summary>
    /// 常用选项维护
    /// </summary>
    public class BasicData
    {

        #region Model
        private string _unitid;
        private string _unitname;
        private int _flag;
        private int _isdelete;
        /// <summary>
        /// 计量单位名称
        /// </summary>
        public string UnitID
        {
            set { _unitid = value; }
            get { return _unitid; }
        }
        /// <summary>
        /// 计量单位名称
        /// </summary>
        public string UnitName
        {
            set { _unitname = value; }
            get { return _unitname; }
        }
        /// <summary>
        /// 辅助数据类别：(1-单位 2-规格 3:封装 4:计价法)
        /// </summary>
        public int flag
        {
            set { _flag = value; }
            get { return _flag; }
        }

        public int IsDelete
        {
            set { _isdelete = value; }
            get { return _isdelete; }
        }
        #endregion Model

    }
}
