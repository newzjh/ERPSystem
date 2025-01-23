using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmBuyOrder : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public InputField if4;
    public InputField if5;
    public Dropdown dd6;
    public InputField if7;
    public InputField if8;
    public InputField if9;
    public InputField if10;

    [NonSerialized]
    public string selectguid = string.Empty;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;
        Load();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;
    }
    private void LoadSuppliers()
    {
        //string ps_Sql = "select Guid,SimpName as  供应商简称,[Name] as 供应商名称,LinkMan as 联系人,Telephone as 电话,Fax as 传真,Address as 地址,Zip as 邮编,ProduceType as 生产类型,Remark as  备注,Case when IsEnable=1 then '停用' when IsEnable=0 then '可用' end as 是否可用 from Supplier   ";
        var query = connection.Table<Supplier>();
        dd3.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.Name;
            dd3.options.Add(od);
        }
        if (dd3.options.Count <= 0)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "UnknownSupplier";
            dd3.options.Add(od);
        }
    }

    private void RefreshEdits()
    {
        var sod = connection.Table<StockOrderDetail>().Where(_ => _.StockOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<StockOrder>().Where(_ => _.StockOrderGuid == sod.StockOrderGuid).FirstOrDefault();
        if1.text = so.StockOrderID;
        if2.text = so.StockOrderDate.ToString();
        //if3.text = s.LinkMan;
        if4.text = so.MALinkman;
        if5.text = so.MATelephone;
        //if6.text = so.BatchNO;
        if7.text = sod.MaterialPrice.ToString();
        if8.text = sod.MaterialSum.ToString();
        if9.text = sod.MaterialTotalMoney.ToString();
        if10.text = sod.ArriveDate.ToString();
    }

    private void LoadBills()
    {
        //string strsql = "";
        //string ps_Sql = "select  *   from  " + tablename + " " + strsql + " order by CreateDate desc";

        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < table.transform.childCount; i++)
        {
            deletelist.Add(table.transform.GetChild(i).gameObject);
        }
        foreach (var go in deletelist)
            GameObject.DestroyImmediate(go);

        var query1 = connection.Table<StockOrderDetail>();
        var query2 = connection.Table<StockOrder>();
        var query3 = connection.Table<ErpManageLibrary.Material>();
        var query4 = connection.Table<Supplier>();

        Dictionary<string, StockOrder> map2 = new Dictionary<string, StockOrder>();
        foreach(var s in query2)
        {
            map2[s.StockOrderGuid] = s;
        }

        Dictionary<string, ErpManageLibrary.Material> map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in query3)
        {
            map3[s.MaterialGuID] = s;
        }

        Dictionary<string, Supplier> map4 = new Dictionary<string, Supplier>();
        foreach (var s in query4)
        {
            map4[s.Guid] = s;
        }

        foreach (var s in query1)
        {
            if (!map2.ContainsKey(s.StockOrderGuid))
                continue;

            StockOrder so = map2[s.StockOrderGuid];

            if (!map3.ContainsKey(s.MaterialGuID))
                continue;

            ErpManageLibrary.Material m = map3[s.MaterialGuID];

            if (!map4.ContainsKey(so.SupplierGuid))
                continue;

            Supplier supplier = map4[so.SupplierGuid];

            {
                GameObject col = GameObject.Instantiate(toggle0.gameObject, table.transform);
                col.SetActive(true);
                {
                    Toggle toggle = col.GetComponentInChildren<Toggle>(true);
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(
                        delegate (bool value)
                        {
                            if (value)
                            {
                                foreach (var _t in table.GetComponentsInChildren<Toggle>().Where(_ => _ != toggle)) _t.isOn = false;
                                selectguid = toggle.name;
                                RefreshEdits();
                            }
                        }
                    );
                }
                col.name = s.StockOrderDetailGuid;
                col.GetComponentInChildren<Text>(true).text = so.StockOrderID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.StockOrderDate.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = supplier.Name;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.MALinkman;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.MATelephone;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = m.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialPrice.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialSum.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialTotalMoney.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.ArriveDate;
            }
        }
    }

    //private void LoadBasicData(int Flag)
    //{
    //    //string ps_Sql = "select  UnitName,UnitID  from BasicData where  IsDelete=0 and flag=" + Flag.ToString() + " order by UnitID";
    //    //var command = connection.CreateCommand(ps_Sql);
    //    //var query = command.ExecuteQuery<BasicData>();

    //    var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag==Flag);
    //    dd5.options.Clear();
    //    foreach (var s in query)
    //    {
    //        Dropdown.OptionData od = new Dropdown.OptionData();
    //        od.text = s.UnitName;
    //        dd5.options.Add(od);
    //    }
    //}

    private void LoadMaterials()
    {
        var query = connection.Table<ErpManageLibrary.Material>();
        dd6.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.MaterialName;
            dd6.options.Add(od);
        }
    }

    private void Load()
    {
        //LoadBasicData(5);
        LoadMaterials();
        LoadSuppliers();
        LoadBills();

        
        if2.text = DateTime.Now.ToString();


        
    }



    public void EditBill()
    {
        if (string.IsNullOrEmpty(selectguid))
            return;

        var query3 = connection.Table<ErpManageLibrary.Material>();
        List<ErpManageLibrary.Material> map3 = new List<ErpManageLibrary.Material>();
        foreach (var s in query3)
        {
            map3.Add(s);
        }

        var query4 = connection.Table<Supplier>();
        List<Supplier> suppliers = new List<Supplier>();
        foreach (var s in query4)
        {
            suppliers.Add(s);
        }

        var stockOrderDetail = connection.Table<StockOrderDetail>().Where(_ => _.StockOrderDetailGuid == selectguid).FirstOrDefault();
        var stockOrder = connection.Table<StockOrder>().Where(_ => _.StockOrderGuid == stockOrderDetail.StockOrderGuid).FirstOrDefault();

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        stockOrder.StockOrderID = if1.text;
        stockOrder.StockOrderDate = datetime;
        stockOrder.SupplierGuid = suppliers[dd3.value].Guid;
        stockOrder.Linkman = if4.text;
        stockOrder.Telephone = if5.text;
        stockOrder.Fax = string.Empty;
        stockOrder.MALinkman = string.Empty;
        stockOrder.MATelephone = string.Empty;
        stockOrder.MAFax = string.Empty;
        stockOrder.Remark = string.Empty;
        stockOrder.CreateGuid = Guid.Empty.ToString();
        stockOrder.CreateDate = datetime;
        stockOrder.CheckGuid = Guid.Empty.ToString();
        stockOrder.CheckDate = datetime;
        stockOrder.EndGuid = Guid.Empty.ToString();
        stockOrder.EndDate = datetime;

        stockOrderDetail.MaterialGuID = map3[dd6.value].MaterialGuID;
        stockOrderDetail.MaterialPrice = decimalTryParse(if7.text);
        stockOrderDetail.MaterialSum = decimalTryParse(if8.text);
        stockOrderDetail.MaterialTotalMoney = decimalTryParse(if9.text);
        stockOrderDetail.ArriveDate = if10.text;

        connection.Update(stockOrderDetail);
        connection.Update(stockOrder);

        Load();
    }


    private void DeleteBillExe()
    {
        var stockOrderDetailGuid = selectguid;
        var stockOrderGuid = connection.Table<StockOrderDetail>().Where(_ => _.StockOrderDetailGuid == stockOrderDetailGuid).FirstOrDefault().StockOrderGuid;

        connection.Delete<StockOrderDetail>(stockOrderDetailGuid);
        int count = connection.Table<StockOrderDetail>().Where(_ => _.StockOrderGuid == stockOrderGuid).Count();
        if (count<=0)
        {
            connection.Delete<StockOrder>(stockOrderGuid);
        }
    }

    public void DeleteBill()
    {
        if (string.IsNullOrEmpty(selectguid))
            return;

        DeleteBillExe();

        Load();
    }

    private void DeleteBillAllExe()
    {
        //string ps_Sql = "Delete from " + tablename;
        //SQLiteCommand command = connection.CreateCommand(ps_Sql);
        //command.ExecuteNonQuery();

        connection.DeleteAll<StockOrderDetail>();
        connection.DeleteAll<StockOrder>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {
        var query3 = connection.Table<ErpManageLibrary.Material>();
        List<ErpManageLibrary.Material> map3 = new List<ErpManageLibrary.Material>();
        foreach (var s in query3)
        {
            map3.Add(s);
        }

        var query4 = connection.Table<Supplier>();
        List<Supplier> suppliers = new List<Supplier>();
        foreach (var s in query4)
        {
            suppliers.Add(s);
        }

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        string orderguid = Guid.NewGuid().ToString();
        string detailorderguid = Guid.NewGuid().ToString();

        StockOrder stockOrder = new StockOrder();
        stockOrder.StockOrderGuid = orderguid;
        stockOrder.StockOrderID = if1.text;
        stockOrder.StockOrderDate = datetime;
        stockOrder.SupplierGuid = suppliers[dd3.value].Guid;
        stockOrder.Linkman = if4.text;
        stockOrder.Telephone = if5.text;
        stockOrder.Fax = string.Empty;
        stockOrder.MALinkman = string.Empty;
        stockOrder.MATelephone = string.Empty;
        stockOrder.MAFax = string.Empty;
        stockOrder.Remark = string.Empty;
        stockOrder.CreateGuid = Guid.Empty.ToString();
        stockOrder.CreateDate = datetime;
        stockOrder.CheckGuid = Guid.Empty.ToString();
        stockOrder.CheckDate = datetime;
        stockOrder.EndGuid = Guid.Empty.ToString();
        stockOrder.EndDate = datetime;
        
        StockOrderDetail stockOrderDetail = new StockOrderDetail();
        stockOrderDetail.StockOrderDetailGuid = detailorderguid;
        stockOrderDetail.StockOrderGuid = orderguid;
        stockOrderDetail.MaterialGuID = map3[dd6.value].MaterialGuID;
        stockOrderDetail.MaterialPrice = decimalTryParse(if7.text);
        stockOrderDetail.MaterialSum = decimalTryParse(if8.text);
        stockOrderDetail.MaterialTotalMoney = decimalTryParse(if9.text);
        stockOrderDetail.ArriveDate = if10.text;

        connection.Insert(stockOrder);
        connection.Insert(stockOrderDetail);

        //string ps_Sql = GetAddBillSQL(stockOrderDetail, stockOrder);
        //SQLiteCommand command = connection.CreateCommand(ps_Sql);
        //command.ExecuteNonQuery();

    }

    public void NewBill()
    {
        NewBillExe();

        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
