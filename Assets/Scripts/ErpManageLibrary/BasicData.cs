using System;
using System.Collections.Generic;
using System.Text;


namespace ErpManageLibrary
{
    /// <summary>
    /// ����ѡ��ά��
    /// </summary>
    public class BasicData
    {

        #region Model
        private string _unitid;
        private string _unitname;
        private int _flag;
        private int _isdelete;
        /// <summary>
        /// ������λ����
        /// </summary>
        public string UnitID
        {
            set { _unitid = value; }
            get { return _unitid; }
        }
        /// <summary>
        /// ������λ����
        /// </summary>
        public string UnitName
        {
            set { _unitname = value; }
            get { return _unitname; }
        }
        /// <summary>
        /// �����������(1-��λ 2-��� 3:��װ 4:�Ƽ۷�)
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
