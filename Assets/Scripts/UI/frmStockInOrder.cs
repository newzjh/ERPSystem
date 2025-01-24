using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmStockInOrder : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public Dropdown dd4;
    public Dropdown dd5;
    public InputField if6;
    public Dropdown dd7;
    public InputField if8;
    public InputField if9;
    public InputField if10;

    [NonSerialized]
    public string selectguid = string.Empty;
    private string tablename = "StockInOrder";



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

    private void RefreshEdits()
    {
        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        var suppliers = connection.Table<Supplier>().ToList();
        var employees = connection.Table<Employee>().ToList();
        var storages = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5).ToList();

        var sod = connection.Table<StockInOrderDetail>().Where(_ => _.StockInOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<StockInOrder>().Where(_ => _.StockInOrderGuid == sod.StockInOrderGuid).FirstOrDefault();
        if1.text = so.StockInOrderID;
        if2.text = so.StockInOrderDate.ToString();
        //if3.text = s.LinkMan;
        dd3.value = suppliers.IndexOf(suppliers.Where(_ => _.Guid == so.SupplierGuid).FirstOrDefault());
        //if4.text = s.Telephone;
        dd4.value = employees.IndexOf(employees.Where(_ => _.EmpGuid == so.StoragePerson).FirstOrDefault());
        //if5.text = s.Fax;
        dd5.value = storages.IndexOf(storages.Where(_ => _.UnitName == so.InStorage).FirstOrDefault());
        if6.text = so.BatchNO;
        //dd7.value = sod.MaterialGuID;
        dd7.value = materials.IndexOf(materials.Where(_ => _.MaterialGuID == sod.MaterialGuID).FirstOrDefault());
        if8.text = sod.MaterialSum.ToString();
        if9.text = sod.DeliverySum.ToString();
        if10.text = sod.StorageSum.ToString();


    }

    private void LoadBills()
    {
        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < table.transform.childCount; i++)
        {
            deletelist.Add(table.transform.GetChild(i).gameObject);
        }
        foreach (var go in deletelist)
            GameObject.DestroyImmediate(go);

        var query1 = connection.Table<StockInOrderDetail>();
        var query2 = connection.Table<StockInOrder>();
        var query3 = connection.Table<ErpManageLibrary.Material>();
        var query4 = connection.Table<Employee>();
        var query5 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5);
        var query6 = connection.Table<Supplier>();

        Dictionary<string, StockInOrder> map2 = new Dictionary<string, StockInOrder>();
        foreach (var s in query2)
        {
            map2[s.StockInOrderGuid] = s;
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

        Dictionary<string, Supplier> map6 = new Dictionary<string, Supplier>();
        foreach (var s in query6)
        {
            map6[s.Guid] = s;
        }

        foreach (var s in query1)
        {
            if (!map2.ContainsKey(s.StockInOrderGuid))
                continue;

            StockInOrder so = map2[s.StockInOrderGuid];

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
                col.name = s.StockInOrderDetailGuid;
                col.GetComponentInChildren<Text>(true).text = so.StockInOrderID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.StockInOrderDate.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = map6[so.SupplierGuid].Name;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = map4[so.StockPerson].EmpName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.InStorage;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.BatchNO;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = m.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialSum.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.DeliverySum.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.StorageSum.ToString();
            }
        }
    }

    private void LoadBasicData(int Flag)
    {
        //string ps_Sql = "select  UnitName,UnitID  from BasicData where  IsDelete=0 and flag=" + Flag.ToString() + " order by UnitID";
        //var command = connection.CreateCommand(ps_Sql);
        //var query = command.ExecuteQuery<BasicData>();

        var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag==Flag);
        dd5.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.UnitName;
            dd5.options.Add(od);
        }
    }

    private void LoadMaterials()
    {
        var query = connection.Table<ErpManageLibrary.Material>();
        dd7.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.MaterialName;
            dd7.options.Add(od);
        }
    }

    private void Load()
    {
        LoadBasicData(5);
        LoadSuppliers();
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

        var query4 = connection.Table<Supplier>();
        List<Supplier> suppliers = new List<Supplier>();
        foreach (var s in query4)
        {
            suppliers.Add(s);
        }

        var query5 = connection.Table<Employee>();
        List<Employee> employees = new List<Employee>();
        foreach (var s in query5)
        {
            employees.Add(s);
        }

        var query6 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5); ;
        List<BasicData> basicdatas = new List<BasicData>();
        foreach (var s in query6)
        {
            basicdatas.Add(s);
        }

        var sod = connection.Table<StockInOrderDetail>().Where(_ => _.StockInOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<StockInOrder>().Where(_ => _.StockInOrderGuid == sod.StockInOrderGuid).FirstOrDefault();

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        so.StockInOrderID = if1.text;
        so.StockInOrderDate = datetime;
        so.SupplierGuid = suppliers[dd3.value].Guid;
        so.StockPerson = employees[dd4.value].EmpGuid;
        so.StoragePerson = employees[dd4.value].EmpGuid;
        so.InStorage = basicdatas[dd5.value].UnitName;
        so.BatchNO = if6.text;
        so.CreateGuid = Guid.Empty.ToString();
        so.CreateDate = datetime;
        so.CheckGuid = Guid.Empty.ToString();
        so.CheckDate = datetime;
        so.Remark = string.Empty;

        sod.StockOrderDetailGuid = Guid.Empty.ToString();
        sod.StockOrderGuid = Guid.Empty.ToString();
        sod.StockOrderID = string.Empty;
        sod.MaterialGuID = map3[dd7.value].MaterialGuID;
        sod.MaterialSum = decimalTryParse(if8.text);
        sod.DeliverySum = decimalTryParse(if9.text);
        sod.StorageSum = decimalTryParse(if10.text);
        sod.Remark = string.Empty;

        connection.Update(sod);
        connection.Update(so);

        Load();
    }

    private void DeleteBillExe()
    {
        var stockinOrderDetailGuid = selectguid;
        var stockinOrderGuid = connection.Table<StockInOrderDetail>().Where(_ => _.StockInOrderDetailGuid == stockinOrderDetailGuid).FirstOrDefault().StockInOrderGuid;

        connection.Delete<StockInOrderDetail>(stockinOrderDetailGuid);
        int count = connection.Table<StockInOrderDetail>().Where(_ => _.StockInOrderGuid == stockinOrderGuid).Count();
        if (count <= 0)
        {
            connection.Delete<StockInOrder>(stockinOrderGuid);
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
        connection.DeleteAll<StockInOrderDetail>();
        connection.DeleteAll<StockInOrder>();
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

        var query5 = connection.Table<Employee>();
        List<Employee> employees = new List<Employee>();
        foreach (var s in query5)
        {
            employees.Add(s);
        }

        var query6 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5); ;
        List<BasicData> basicdatas = new List<BasicData>();
        foreach (var s in query6)
        {
            basicdatas.Add(s);
        }

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        string orderguid = Guid.NewGuid().ToString();
        string detailorderguid = Guid.NewGuid().ToString();

        StockInOrder stockinOrder = new StockInOrder();
        stockinOrder.StockInOrderGuid = orderguid;
        stockinOrder.StockInOrderID = if1.text;
        stockinOrder.StockInOrderDate = datetime;
        stockinOrder.SupplierGuid = suppliers[dd3.value].Guid;
        stockinOrder.StockPerson = employees[dd4.value].EmpGuid;
        stockinOrder.StoragePerson = employees[dd4.value].EmpGuid;
        stockinOrder.InStorage = basicdatas[dd5.value].UnitName;
        stockinOrder.BatchNO = if6.text;
        stockinOrder.CreateGuid = Guid.Empty.ToString();
        stockinOrder.CreateDate = datetime;
        stockinOrder.CheckGuid = Guid.Empty.ToString();
        stockinOrder.CheckDate = datetime;
        stockinOrder.Remark = string.Empty;

        StockInOrderDetail stockinOrderDetail = new StockInOrderDetail();
        stockinOrderDetail.StockInOrderDetailGuid = detailorderguid;
        stockinOrderDetail.StockInOrderGuid = orderguid;
        stockinOrderDetail.StockOrderDetailGuid = Guid.Empty.ToString();
        stockinOrderDetail.StockOrderGuid = Guid.Empty.ToString();
        stockinOrderDetail.StockOrderID = string.Empty;
        stockinOrderDetail.MaterialGuID = map3[dd7.value].MaterialGuID;
        stockinOrderDetail.MaterialSum = decimalTryParse(if8.text);
        stockinOrderDetail.DeliverySum = decimalTryParse(if9.text);
        stockinOrderDetail.StorageSum = decimalTryParse(if10.text);
        stockinOrderDetail.Remark = string.Empty;

        connection.Insert(stockinOrder);
        connection.Insert(stockinOrderDetail);
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
