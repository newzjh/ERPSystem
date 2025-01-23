using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmStockReport : BasePanel
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
            if (usecondition)
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
        string storage = dd5.options[dd5.value].text;

        var query0 = connection.Table<ErpManageLibrary.Material>();
        if (usecondition)
        {
            query0 = query0.Where(_ => _.SupplierGuid == supplierguid);
            if (!string.IsNullOrEmpty(if1.text))
                query0 = query0.Where(_ => _.MaterialID.Contains(if1.text));
            if (!string.IsNullOrEmpty(if6.text))
                query0 = query0.Where(_ => _.MaterialName.Contains(if6.text));
        }

        var stat = new Dictionary<string, MaterialStockOrderDetailStat>();
        foreach (var s in query0)
        {
            stat[s.MaterialGuID] = new MaterialStockOrderDetailStat();
        }

        //期初入库
        TemplateStat<InitialGoodsDetail, InitialGoods>(ref stat, usecondition, storage, true);
        //{
        //    var query1 = connection.Table<InitialGoodsDetail>();
        //    var query1x = connection.Table<InitialGoods>();
        //    var map1 = new Dictionary<string, InitialGoods>();
        //    foreach (var s in query1x)
        //    {
        //        map1[s.InitialGoodsGuid] = s;
        //    }
        //    foreach (var s in query1)
        //    {
        //        if (!stat.ContainsKey(s.MaterialGuID))
        //            continue;

        //        if (!map1.ContainsKey(s.InitialGoodsGuid))
        //            continue;

        //        var x = map1[s.InitialGoodsGuid];

        //        if (usecondition && storage!=x.IncomeDepot)
        //            continue;

        //        var c = stat[s.MaterialGuID];
        //        c.Number += s.MaterialSum;
        //        stat[s.MaterialGuID] = c;
        //    }
        //}

        //采购入库
        TemplateStat<StockInOrderDetail, StockInOrder>(ref stat, usecondition, storage, true);

        //{
        //    var query2 = connection.Table<StockInOrderDetail>();
        //    var query2x = connection.Table<StockInOrder>();
        //    var map2 = new Dictionary<string, StockInOrder>();
        //    foreach (var s in query2x)
        //    {
        //        map2[s.StockInOrderGuid] = s;
        //    }
        //    foreach (var s in query2)
        //    {
        //        if (!stat.ContainsKey(s.MaterialGuID))
        //            continue;

        //        if (!map2.ContainsKey(s.StockInOrderGuid))
        //            continue;

        //        var x = map2[s.StockInOrderGuid];

        //        if (usecondition && storage != x.InStorage)
        //            continue;

        //        var c = stat[s.MaterialGuID];
        //        c.Number += s.MaterialSum;
        //        stat[s.MaterialGuID] = c;
        //    }
        //}

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

        //{
        //    var query4 = connection.Table<HalfGoodsDetail>();
        //    var query4x = connection.Table<HalfGoods>();
        //    var map4 = new Dictionary<string, HalfGoods>();
        //    foreach (var s in query4x)
        //    {
        //        map4[s.HalfGoodsGuid] = s;
        //    }
        //    foreach (var s in query4)
        //    {
        //        if (!stat.ContainsKey(s.MaterialGuID))
        //            continue;

        //        if (!map4.ContainsKey(s.HalfGoodsGuid))
        //            continue;

        //        var x = map4[s.HalfGoodsGuid];

        //        if (usecondition && storage != x.IncomeDepot)
        //            continue;

        //        var c = stat[s.MaterialGuID];
        //        c.Number += s.MaterialSum;
        //        stat[s.MaterialGuID] = c;
        //    }
        //}

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

        //{
        //    var query11 = connection.Table<SellOrderDetail>();
        //    var query11x = connection.Table<SellOrder>();
        //    var map4 = new Dictionary<string, SellOrder>();
        //    foreach (var s in query11x)
        //    {
        //        map4[s.SellOrderGuid] = s;
        //    }
        //    foreach (var s in query11)
        //    {
        //        if (!stat.ContainsKey(s.MaterialGuID))
        //            continue;

        //        if (!map4.ContainsKey(s.SellOrderGuid))
        //            continue;

        //        var x = map4[s.SellOrderGuid];

        //        if (usecondition && storage != x.OutStorage)
        //            continue;

        //        var c = stat[s.MaterialGuID];
        //        c.Number -= s.MaterialSum;
        //        stat[s.MaterialGuID] = c;
        //    }
        //}


        foreach (var s in query0)
        {
            MaterialStockOrderDetailStat c = new MaterialStockOrderDetailStat();
            c.Number = 0;
            c.TotalMoney = 0;
            if (stat.ContainsKey(s.MaterialGuID))
                c = stat[s.MaterialGuID];
            c.TotalMoney = s.Price * c.Number;

            //if (c.Number <= 0)
            //    continue;

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
                col.GetComponentInChildren<Text>(true).text = s.Spec;
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
                int shuiprice = (int)((float)s.Price * 0.83f);
                col.GetComponentInChildren<Text>(true).text = shuiprice.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                int shuitotalmoney = (int)((float)c.TotalMoney * 0.83f);
                col.GetComponentInChildren<Text>(true).text = shuitotalmoney.ToString();
            }
        }
    }

    private void LoadBasicData(int Flag)
    {
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
