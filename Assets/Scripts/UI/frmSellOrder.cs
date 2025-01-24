using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;
using System.Linq;

public class frmSellOrder : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public InputField if3;
    public Dropdown dd4;
    public Dropdown dd5;
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

        if1.text = IDGenerator();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;
    }
    private void RefreshEdits()
    {
        var sod = connection.Table<SellOrderDetail>().Where(_ => _.SellOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<SellOrder>().Where(_ => _.SellOrderGuid == sod.SellOrderGuid).FirstOrDefault();
        if1.text = so.SellOrderID;
        if2.text = so.SellOrderDate.ToString();
        if3.text = so.Client;
        //if4.text = so.MALinkman;
        //if5.text = so.OutStorage;
        //if6.text = so.BatchNO;
        if7.text = sod.Price.ToString();
        if8.text = sod.MaterialSum.ToString();
        if9.text = sod.MaterialMoney.ToString();
        if10.text = sod.BoxSum.ToString();
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

        var query1 = connection.Table<SellOrderDetail>();
        var query2 = connection.Table<SellOrder>();
        var query3 = connection.Table<ErpManageLibrary.Material>();
        var query4 = connection.Table<Employee>();
        var query5 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5);

        Dictionary<string, SellOrder> map2 = new Dictionary<string, SellOrder>();
        foreach(var s in query2)
        {
            map2[s.SellOrderGuid] = s;
        }

        Dictionary<string, ErpManageLibrary.Material> map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in query3)
        {
            map3[s.MaterialGuID] = s;
        }

        Dictionary<string, Employee> map4 = new Dictionary<string, Employee>();
        foreach (var s in query4)
        {
            map4[s.EmpGuid] = s;
        }

        Dictionary<string, BasicData> map5 = new Dictionary<string, BasicData>();
        foreach (var s in query5)
        {
            map5[s.UnitName] = s;
        }

        foreach (var s in query1)
        {
            if (!map2.ContainsKey(s.SellOrderGuid))
                continue;

            SellOrder so = map2[s.SellOrderGuid];

            if (!map3.ContainsKey(s.MaterialGuID))
                continue;

            ErpManageLibrary.Material m = map3[s.MaterialGuID];

            if (!map4.ContainsKey(so.StoragePerson))
                continue;

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
                col.name = s.SellOrderDetailGuid;
                col.GetComponentInChildren<Text>(true).text = so.SellOrderID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.SellOrderDate.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.Client;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = map4[so.StoragePerson].EmpName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.OutStorage;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = m.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Price.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialSum.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialMoney.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.BoxSum.ToString();
            }
        }
    }

    private void LoadBasicData(int Flag)
    {
        //string ps_Sql = "select  UnitName,UnitID  from BasicData where  IsDelete=0 and flag=" + Flag.ToString() + " order by UnitID";
        //var command = connection.CreateCommand(ps_Sql);
        //var query = command.ExecuteQuery<BasicData>();

        var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == Flag);
        dd5.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.UnitName;
            dd5.options.Add(od);
        }
    }

    private void LoadEmployees()
    {
        //string ps_Sql = "select EmpGuid,EmpID as 员工编号,EmpName as  员工姓名,(select deptname  from Dept where Dept.deptguid=Employee.Dept) as 所在部门,Sex as 性别,Telephone as 电话,Address as  地址,CardID  as 身份证号,"
        //       + "  Case when IsEnable=1 then '停用' when IsEnable=0 then '可用' end as 是否可用 from Employee ";
        //var command = connection.CreateCommand(ps_Sql);
        //var query = command.ExecuteQuery<Employee>();

        var query = connection.Table<Employee>();
        dd4.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.EmpName;
            dd4.options.Add(od);
        }
        if (dd4.options.Count <= 0)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "UnknownName";
            dd4.options.Add(od);
        }

    }

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
        LoadBasicData(5);
        LoadEmployees();
        LoadMaterials();
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

        var query4 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5); ;
        List<BasicData> basicdatas = new List<BasicData>();
        foreach (var s in query4)
        {
            basicdatas.Add(s);
        }

        var query5 = connection.Table<Employee>();
        List<Employee> employees = new List<Employee>();
        foreach (var s in query5)
        {
            employees.Add(s);
        }

        var sod = connection.Table<SellOrderDetail>().Where(_ => _.SellOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<SellOrder>().Where(_ => _.SellOrderGuid == sod.SellOrderGuid).FirstOrDefault();

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        so.SellOrderID = if1.text;
        so.SellOrderDate = datetime;
        so.Client = if3.text;
        so.OutStorage = basicdatas[dd4.value].UnitName;
        so.StoragePerson = employees[dd5.value].EmpGuid;
        so.QualityPerson = Guid.Empty.ToString();
        so.Remark = Guid.Empty.ToString();
        so.CreateGuid = Guid.Empty.ToString();
        so.CreateDate = datetime;
        so.CheckGuid = Guid.Empty.ToString();
        so.CheckDate = datetime;
        so.CheckGuid2 = Guid.Empty.ToString();
        so.CheckDate2 = datetime;
        so.EndGuid = Guid.Empty.ToString();
        so.EndDate = datetime;
        so.Shipping = string.Empty;

        sod.ClientOrderDetailGuid = Guid.Empty.ToString();
        sod.ClientOrderGuid = Guid.Empty.ToString();
        sod.ClientOrderID = string.Empty;
        sod.MaterialGuID = map3[dd6.value].MaterialGuID;
        sod.Price = decimalTryParse(if7.text);
        sod.MaterialSum = decimalTryParse(if8.text);
        sod.MaterialMoney = decimalTryParse(if9.text);
        sod.BoxSum = decimalTryParse(if10.text);
        sod.Remark = string.Empty;

        connection.Update(sod);
        connection.Update(so);

        Load();
    }

    private void DeleteBillExe()
    {
        //string ps_Sql = "Delete from " + tablename + " where  StockOrderDetailGuid='" + selectguid + "'";
        //SQLiteCommand command = connection.CreateCommand(ps_Sql);
        //command.ExecuteNonQuery();

        var sellOrderDetailGuid = selectguid;
        var sellOrderGuid = connection.Table<SellOrderDetail>().Where(_ => _.SellOrderDetailGuid == sellOrderDetailGuid).FirstOrDefault().SellOrderGuid;
        connection.Delete<SellOrderDetail>(sellOrderDetailGuid);
        int count = connection.Table<SellOrderDetail>().Where(_ => _.SellOrderGuid == sellOrderGuid).Count();
        if (count<=0)
        {
            connection.Delete<SellOrder>(sellOrderGuid);
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

        connection.DeleteAll<SellOrderDetail>();
        connection.DeleteAll<SellOrder>();
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

        var query4 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5); ;
        List<BasicData> basicdatas = new List<BasicData>();
        foreach (var s in query4)
        {
            basicdatas.Add(s);
        }

        var query5 = connection.Table<Employee>();
        List<Employee> employees = new List<Employee>();
        foreach (var s in query5)
        {
            employees.Add(s);
        }

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        string orderguid = Guid.NewGuid().ToString();
        string detailorderguid = Guid.NewGuid().ToString();

        SellOrder so = new SellOrder();
        so.SellOrderGuid = orderguid;
        so.SellOrderID = if1.text;
        so.SellOrderDate = datetime;
        so.Client = if3.text;
        so.OutStorage = basicdatas[dd4.value].UnitName;
        so.StoragePerson = employees[dd5.value].EmpGuid;
        so.QualityPerson = Guid.Empty.ToString();
        so.Remark = Guid.Empty.ToString();
        so.CreateGuid = Guid.Empty.ToString();
        so.CreateDate = datetime;
        so.CheckGuid = Guid.Empty.ToString();
        so.CheckDate = datetime;
        so.CheckGuid2 = Guid.Empty.ToString();
        so.CheckDate2 = datetime;
        so.EndGuid = Guid.Empty.ToString();
        so.EndDate = datetime;
        so.Shipping = string.Empty;

        SellOrderDetail sod = new SellOrderDetail();
        sod.SellOrderDetailGuid = detailorderguid;
        sod.SellOrderGuid = orderguid;
        sod.ClientOrderDetailGuid = Guid.Empty.ToString();
        sod.ClientOrderGuid = Guid.Empty.ToString();
        sod.ClientOrderID = string.Empty;
        sod.MaterialGuID = map3[dd6.value].MaterialGuID;
        sod.Price = decimalTryParse(if7.text);
        sod.MaterialSum = decimalTryParse(if8.text);
        sod.MaterialMoney = decimalTryParse(if9.text);
        sod.BoxSum = decimalTryParse(if10.text);
        sod.Remark = string.Empty;

        connection.Insert(so);
        connection.Insert(sod);

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
