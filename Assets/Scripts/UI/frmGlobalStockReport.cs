using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmGlobalStockReport : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public Dropdown dd4;
    //public Dropdown dd5;
    public InputField if6;
    public Text tstorage;

    [NonSerialized]
    public string selectguid = string.Empty;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        LoadBasicData(5);

    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;
        RefreshStorageTitle();
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
        public decimal TotalMoney;
    }

    private void TemplateStat<T1, T2>(ref Dictionary<string, MaterialStockOrderDetailStat> stat, bool usecondition, string storage, bool increase)
        where T1 : new()
        where T2 : new()
    {
        var query1 = connection.Table<T1>();
        var query2 = connection.Table<T2>();
        var map2 = new Dictionary<string, T2>();

        var t1 = typeof(T1);
        var t2 = typeof(T2);

        var property0 = t1.GetProperty("MaterialGuID");
        var property1 = t1.GetProperty(t2.Name + "Guid");
        var propertysum = t1.GetProperty("MaterialSum");

        var property2 = t2.GetProperty(t2.Name + "Guid");
        var property3 = t2.GetProperty("IncomeDepot");
        var property4 = t2.GetProperty("InStorage");

        foreach (var s in query2)
        {
            string guid = property2.GetValue(s) as string;
            map2[guid] = s;
        }

        foreach (var s in query1)
        {
            string materialguid = property0.GetValue(s) as string;
            if (!stat.ContainsKey(materialguid))
                continue;

            string orderguid = property1.GetValue(s) as string;
            if (!map2.ContainsKey(orderguid))
                continue;

            var x = map2[orderguid];
            if (!string.IsNullOrEmpty(storage))
            {
                if (property3 != null)
                {
                    if ((property3.GetValue(x) as string) != storage)
                        continue;
                }
                if (property4 != null)
                {
                    if ((property4.GetValue(x) as string) != storage)
                        continue;
                }
            }

            var c = stat[materialguid];
            if (increase)
                c.Number += (decimal)propertysum.GetValue(s);
            else
                c.Number -= (decimal)propertysum.GetValue(s);
            stat[materialguid] = c;
        }
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
                query0 = query0.Where(_ => _.MaterialID.Contains(if1.text));
            if (!string.IsNullOrEmpty(if6.text))
                query0 = query0.Where(_ => _.MaterialName.Contains(if6.text));
        }

        //var stat = new Dictionary<string, MaterialStockOrderDetailStat>();
        List<Dictionary<string, MaterialStockOrderDetailStat>> global_stat = new List<Dictionary<string, MaterialStockOrderDetailStat>>();
        for (int i = 0; i<storages.Count; i++)
        {
            global_stat.Add(new Dictionary<string, MaterialStockOrderDetailStat>());
            foreach (var s in query0)
            {
                global_stat[i][s.MaterialGuID] = new MaterialStockOrderDetailStat();
            }
        }

        for (int i = 0; i < storages.Count; i++)
        {
            string storage = storages[i];
            var stat = global_stat[i];

            //期初入库
            TemplateStat<InitialGoodsDetail, InitialGoods>(ref stat, usecondition, storage, true);
            //采购入库
            TemplateStat<StockInOrderDetail, StockInOrder>(ref stat, usecondition, storage, true);
            //成品入库
            TemplateStat<GoodsOrderDetail, GoodsOrder>(ref stat, usecondition, storage, true);
            //半成品入库
            TemplateStat<HalfGoodsDetail, HalfGoods>(ref stat, usecondition, storage, true);
            //退料入库
            TemplateStat<QuitStorageOrderDetail, QuitStorageOrder>(ref stat, usecondition, storage, true);
            //调拨入仓库
            TemplateStat<RemoveBillDetail, RemoveBill>(ref stat, usecondition, storage, true);
            //委外入库
            TemplateStat<ConsignDetail, Consign>(ref stat, usecondition, storage, true);
            //其它入库
            TemplateStat<OtherInOrderDetail, OtherInOrder>(ref stat, usecondition, storage, true);
            //报废入库
            TemplateStat<RejectOrderDetail, RejectOrder>(ref stat, usecondition, storage, true);
            //领料出库
            TemplateStat<DrawOrderDetail, DrawOrder>(ref stat, usecondition, storage, false);
            //销售出库
            TemplateStat<SellOrderDetail, SellOrder>(ref stat, usecondition, storage, false);
            //其它出库
            TemplateStat<OtherSellOrderDetail, OtherSellOrder>(ref stat, usecondition, storage, false);
            //委外出库
            TemplateStat<ConsignOutDetail, ConsignOut>(ref stat, usecondition, storage, false);
            //超领单
            TemplateStat<ExcessOrderDetail, ExcessOrder>(ref stat, usecondition, storage, false);
            //调拨出仓库
            TemplateStat<RemoveBillDetail, RemoveBill>(ref stat, usecondition, storage, false);
            //退料单出仓库
            TemplateStat<QuitOrderDetail, QuitOrder>(ref stat, usecondition, storage, false);

        }



        foreach (var s in query0)
        {
            decimal totalNumber = 0;
            decimal totalMoney = 0;
            MaterialStockOrderDetailStat c = new MaterialStockOrderDetailStat();
            for (int i = 0; i < storages.Count; i++)
            {
                var stat = global_stat[i];
                if (stat.ContainsKey(s.MaterialGuID))
                    c = stat[s.MaterialGuID];
                totalNumber += c.Number;
                totalMoney += s.Price * c.Number;
            }
            c.Number = 0;
            c.TotalMoney = 0;
            for (int i = 0; i < storages.Count; i++)
            {
                var stat = global_stat[i];
                if (stat.ContainsKey(s.MaterialGuID))
                    c = stat[s.MaterialGuID];
                c.TotalMoney = s.Price * c.Number;

                if (c.Number <= 0)
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
                                    if1.text = toggle.GetComponentInChildren<Text>().text;
                                }
                            }
                        );
                    }
                    col.name = s.MaterialGuID;
                    col.GetComponentInChildren<Text>(true).text = s.MaterialID;
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = s.MaterialName;
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = storages[i];
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = s.Unit;
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = c.Number.ToString();
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = s.Price.ToString();
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    col.GetComponentInChildren<Text>(true).text = c.TotalMoney.ToString();
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    int shuiprice = (int)((float)totalNumber);
                    col.GetComponentInChildren<Text>(true).text = shuiprice.ToString();
                }
                {
                    GameObject col = GameObject.Instantiate(edit0, table.transform);
                    col.SetActive(true);
                    int shuitotalmoney = (int)((float)totalMoney);
                    col.GetComponentInChildren<Text>(true).text = shuitotalmoney.ToString();
                }
            }
        }
    }

    public int currentstorage = -1;
    List<string> storages = new List<string>();
    private void LoadBasicData(int Flag)
    {
        var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag==Flag);
        storages.Clear();
        foreach (var s in query)
        {
            storages.Add(s.UnitName);
        }
        if (storages.Count > 0)
        {
            currentstorage = 0;
        }
    }

    public void NextStorage()
    {
        currentstorage = (currentstorage + 1) % storages.Count;

        RefreshStorageTitle();
        Load();
    }

    public void PreStorage()
    {
        currentstorage = (currentstorage - 1 + storages.Count) % storages.Count;

        RefreshStorageTitle();
        Load();
    }

    public void RefreshStorageTitle()
    {
        string storage = string.Empty;
        if (storages.Count > 0 && currentstorage != -1)
            storage = storages[currentstorage];
        tstorage.text = storage;
    }

    public void Load()
    {
        LoadSuppliers();
        LoadEmployees();
        LoadBills();
        
        if2.text = DateTime.Now.ToString();
    }

    public void Query()
    {
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
