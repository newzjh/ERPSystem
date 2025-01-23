using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmTotalStockReport : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public Dropdown dd4;
    public Dropdown dd5;
    public InputField if6;

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

    List<string> supplierguids = new List<string>();
    private void LoadSuppliers()
    {
        supplierguids.Clear();

        //string ps_Sql = "select Guid,SimpName as  供应商简称,[Name] as 供应商名称,LinkMan as 联系人,Telephone as 电话,Fax as 传真,Address as 地址,Zip as 邮编,ProduceType as 生产类型,Remark as  备注,Case when IsEnable=1 then '停用' when IsEnable=0 then '可用' end as 是否可用 from Supplier   ";
        var query = connection.Table<Supplier>();
        dd3.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.Name;
            dd3.options.Add(od);
            supplierguids.Add(s.Guid);
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
        if (dd3.options.Count <= 0)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "UnknownName";
            dd4.options.Add(od);
        }

    }

    public struct MaterialStockOrderDetailStat
    {
        public decimal Number;
        public decimal Price;
        public decimal TotalMoney;
    }

    private void LoadBills(bool usecondition=false)
    {
        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < table.transform.childCount; i++)
        {
            deletelist.Add(table.transform.GetChild(i).gameObject);
        }
        foreach (var go in deletelist)
            GameObject.DestroyImmediate(go);

        //string[,] par;
        //par = new string[4, 2];
        //par[0, 0] = "@MaterialGuid";
        //par[0, 1] = if6.text;
        //par[1, 0] = "@StorageID";
        //par[1, 1] = dd5.options[dd5.value].text;
        //par[2, 0] = "@MaterialTypeGuid";
        //par[2, 1] = dd3.options[dd3.value].text;
        //par[3, 0] = "@EndDate";
        //par[3, 1] = if2.text;
        //var command = connection.CreateCommand("sp_GetStorageReport", par);

        //command.ExecuteQuery()

        string supplierguid = supplierguids[dd3.value];

        var query0 = connection.Table<ErpManageLibrary.Material>();
        if (usecondition)
        {
            query0 = query0.Where(_ => _.SupplierGuid == supplierguid);
            if (!string.IsNullOrEmpty(if1.text))
                query0 = query0.Where(_ => _.MaterialID == if1.text);
            if (!string.IsNullOrEmpty(if6.text))
                query0 = query0.Where(_ => _.MaterialName == if6.text);
        }
        var query2 = connection.Table<StockOrderDetail>();

        Dictionary<string, MaterialStockOrderDetailStat> stat = new Dictionary<string, MaterialStockOrderDetailStat>();
        foreach (var s in query0)
        {
            stat[s.MaterialGuID] = new MaterialStockOrderDetailStat();
        }

        foreach(var s in query2)
        {
            if (!stat.ContainsKey(s.MaterialGuID))
                continue;

            var c = stat[s.MaterialGuID];
            c.Number += s.MaterialSum;
            c.Price += s.MaterialPrice;
            c.TotalMoney += s.MaterialTotalMoney;
            stat[s.MaterialGuID] = c;
        }

        foreach (var s in query0)
        {
            MaterialStockOrderDetailStat c = new MaterialStockOrderDetailStat();
            c.Number = 0;
            c.Price = 0;
            c.TotalMoney = 0;
            if (stat.ContainsKey(s.MaterialGuID))
                c = stat[s.MaterialGuID];
            decimal price = c.Price;
            if (c.Number > 0)
                price = c.TotalMoney / c.Number;

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
                                selectguid = toggle.GetComponentInChildren<Text>().text;
                                if1.text = selectguid;
                            }
                        }
                    );
                }
                col.GetComponentInChildren<Text>().text = s.MaterialID;
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = s.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = s.Spec;
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = s.Unit;
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = c.Number.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = price.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = c.TotalMoney.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = price.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(if1.gameObject, table.transform);
                col.GetComponentInChildren<InputField>().text = c.TotalMoney.ToString();
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

    private void Load()
    {
        LoadBasicData(5);
        LoadSuppliers();
        LoadEmployees();
        LoadBills();
        
        if2.text = DateTime.Now.ToString();
    }

    public void Query()
    {
        LoadBasicData(5);
        LoadSuppliers();
        LoadEmployees();
        LoadBills(true);
    }

    public void QueryAll()
    {
        LoadBasicData(5);
        LoadSuppliers();
        LoadEmployees();
        LoadBills();
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
