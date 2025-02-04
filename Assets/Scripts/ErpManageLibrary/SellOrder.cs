﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ErpManageLibrary
{
    /// <summary>
    /// 销售出库单
    /// </summary>
    public class SellOrder
    {
        #region Model
        private string _sellorderguid;
        private string _sellorderid;
        private DateTime _sellorderdate;
        private string _client;
        private string _outstorage;
        private string _storageperson;
        private string _qualityperson;
        private string _remark;
        private string _createguid;
        private DateTime _createdate;
        private string _checkguid;
        private DateTime _checkdate;
        private string _checkguid2;
        private DateTime _checkdate2;
        private string _endguid;
        private DateTime _enddate;
        private string _Shipping;



        /// <summary>
        /// 
        /// </summary>
        public string SellOrderGuid
        {
            set { _sellorderguid = value; }
            get { return _sellorderguid; }
        }
        /// <summary>
        /// 销售出库单号
        /// </summary>
        public string SellOrderID
        {
            set { _sellorderid = value; }
            get { return _sellorderid; }
        }
        /// <summary>
        /// 开单日期
        /// </summary>
        public DateTime SellOrderDate
        {
            set { _sellorderdate = value; }
            get { return _sellorderdate; }
        }
        /// <summary>
        /// 客户
        /// </summary>
        public string Client
        {
            set { _client = value; }
            get { return _client; }
        }
        /// <summary>
        /// 出货仓库
        /// </summary>
        public string OutStorage
        {
            set { _outstorage = value; }
            get { return _outstorage; }
        }
        /// <summary>
        /// 仓管员
        /// </summary>
        public string StoragePerson
        {
            set { _storageperson = value; }
            get { return _storageperson; }
        }
        /// <summary>
        /// 质检员
        /// </summary>
        public string QualityPerson
        {
            set { _qualityperson = value; }
            get { return _qualityperson; }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 制单人
        /// </summary>
        public string CreateGuid
        {
            set { _createguid = value; }
            get { return _createguid; }
        }
        /// <summary>
        /// 制单时间
        /// </summary>
        public DateTime CreateDate
        {
            set { _createdate = value; }
            get { return _createdate; }
        }
        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckGuid
        {
            set { _checkguid = value; }
            get { return _checkguid; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime CheckDate
        {
            set { _checkdate = value; }
            get { return _checkdate; }
        }
        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckGuid2
        {
            set { _checkguid2 = value; }
            get { return _checkguid2; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime CheckDate2
        {
            set { _checkdate2 = value; }
            get { return _checkdate2; }
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string EndGuid
        {
            set { _endguid = value; }
            get { return _endguid; }
        }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime EndDate
        {
            set { _enddate = value; }
            get { return _enddate; }
        }

        /// <summary>
        /// 装运
        /// </summary>
        public string Shipping
        {
            set { _Shipping = value; }
            get { return _Shipping; }
        }

        #endregion Model
    }
}
