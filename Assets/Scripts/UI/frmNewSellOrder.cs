using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmNewSellOrder : BasePanel
{
    public VerticalLayoutGroup table;
    public GridLayoutGroup table1;
    public GridLayoutGroup table2;

    public Toggle toggle0;
    public GameObject edit0;
    public Button button0;
    public GameObject ordertemplate;

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

    [NonSerialized]
    public string selectguid2 = string.Empty;


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

        LoadOrders();

        if1.text = IDGenerator();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;

        dd6.onValueChanged.RemoveAllListeners();
    }

    private decimal TemplateStat<T1, T2>(string statmaterailguid, string storage, bool increase)
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

        decimal number = 0;
        foreach (var s in query1)
        {
            string materialguid = property0.GetValue(s) as string;
            if (materialguid != statmaterailguid)
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

            if (increase)
                number += (decimal)propertysum.GetValue(s);
            else
                number -= (decimal)propertysum.GetValue(s);
        }
        return number;
    }



    private void OnSelectMaterial(int index)
    {

        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < table1.transform.childCount; i++)
        {
            deletelist.Add(table1.transform.GetChild(i).gameObject);
        }
        foreach (var go in deletelist)
            GameObject.DestroyImmediate(go);

        var storages = new Dictionary<string, int>();
        var querystorage = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5);
        var querymaterial = connection.Table<ErpManageLibrary.Material>();
        var materials = new List<ErpManageLibrary.Material>();
        foreach(var q in querymaterial)
        {
            materials.Add(q);
        }
        var materailguid = materials[index].MaterialGuID;

        foreach (var q in querystorage)
        {
            string storage = q.UnitName;
            storages[storage] = 0;           

            decimal number = 0;
            number += TemplateStat<InitialGoodsDetail, InitialGoods>(materailguid, storage, true);
            number += TemplateStat<StockInOrderDetail, StockInOrder>(materailguid, storage, true);
            number += TemplateStat<GoodsOrderDetail, GoodsOrder>(materailguid, storage, true);
            number += TemplateStat<HalfGoodsDetail, HalfGoods>(materailguid, storage, true);
            number += TemplateStat<QuitStorageOrderDetail, QuitStorageOrder>(materailguid, storage, true);
            number += TemplateStat<RemoveBillDetail, RemoveBill>(materailguid, storage, true);
            number += TemplateStat<ConsignDetail, Consign>(materailguid, storage, true);
            number += TemplateStat<OtherInOrderDetail, OtherInOrder>(materailguid, storage, true);
            number += TemplateStat<RejectOrderDetail, RejectOrder>(materailguid, storage, true);

            //领料出库
            number += TemplateStat<DrawOrderDetail, DrawOrder>(materailguid, storage, false);
            //销售出库
            number += TemplateStat<SellOrderDetail, SellOrder>(materailguid, storage, false);
            //其它出库
            number += TemplateStat<OtherSellOrderDetail, OtherSellOrder>(materailguid, storage, false);
            //委外出库
            number += TemplateStat<ConsignOutDetail, ConsignOut>(materailguid, storage, false);
            //超领单
            number += TemplateStat<ExcessOrderDetail, ExcessOrder>(materailguid, storage, false);
            //调拨出仓库
            number += TemplateStat<RemoveBillDetail, RemoveBill>(materailguid, storage, false);
            //退料单出仓库
            number += TemplateStat<QuitOrderDetail, QuitOrder>(materailguid, storage, false);

            if (number <= 0)
                continue;

            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = storage;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = number.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = materials[index].Price.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = (materials[index].Price * number).ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
        }   
    }

    private void RefreshEdits2()
    {
        var sod = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderDetailGuid == selectguid2).FirstOrDefault();

        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].MaterialGuID == sod.MaterialGuid)
            {
                OnSelectMaterial(i);
                break;
            }
        }
    }

    private void RefreshEdits()
    {
        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        var depts = connection.Table<Dept>().ToList();
        var employees = connection.Table<Employee>().ToList();
        var storages = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5).ToList();

        var sod = connection.Table<SellOrderDetail>().Where(_ => _.SellOrderDetailGuid == selectguid).FirstOrDefault();
        var so = connection.Table<SellOrder>().Where(_ => _.SellOrderGuid == sod.SellOrderGuid).FirstOrDefault();
        if1.text = so.SellOrderID;
        if2.text = so.SellOrderDate.ToString();
        if3.text = so.Client;
        dd4.value = employees.IndexOf(employees.Where(_ => _.EmpGuid == so.StoragePerson).FirstOrDefault());
        dd5.value = storages.IndexOf(storages.Where(_ => _.UnitName == so.OutStorage).FirstOrDefault());
        //if4.text = so.MALinkman;
        //if5.text = so.OutStorage;
        //if6.text = so.BatchNO;
        dd6.value = materials.IndexOf(materials.Where(_ => _.MaterialGuID == sod.MaterialGuID).FirstOrDefault());
        if7.text = sod.Price.ToString();
        if8.text = sod.MaterialSum.ToString();
        if9.text = sod.MaterialMoney.ToString();
        if10.text = sod.BoxSum.ToString();

    }

    private void LoadOrders()
    {
        List<GameObject> deletelist = new List<GameObject>();
        for (int i = 0; i < table2.transform.childCount; i++)
        {
            deletelist.Add(table2.transform.GetChild(i).gameObject);
        }
        foreach (var go in deletelist)
            GameObject.DestroyImmediate(go);

        var query1 = connection.Table<ClientOrderDetail>().ToList();
        var query2 = connection.Table<ClientOrder>().ToList();
        var query3 = connection.Table<ErpManageLibrary.Material>().ToList();

        Dictionary<string, ClientOrder> map2 = new Dictionary<string, ClientOrder>();
        foreach (var s in query2)
        {
            map2[s.ClientOrderGuid] = s;
        }

        Dictionary<string, ErpManageLibrary.Material> map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in query3)
        {
            map3[s.MaterialGuID] = s;
        }


        foreach (var s in query1)
        {
            if (!map2.ContainsKey(s.ClientOrderGuid))
                continue;

            ClientOrder so = map2[s.ClientOrderGuid];

            if (!map3.ContainsKey(s.MaterialGuid))
                continue;

            ErpManageLibrary.Material m = map3[s.MaterialGuid];

            {
                GameObject col = GameObject.Instantiate(toggle0.gameObject, table2.transform);
                col.SetActive(true);
                {
                    Toggle toggle = col.GetComponentInChildren<Toggle>(true);
                    toggle.isOn = false;
                    toggle.onValueChanged.AddListener(
                        delegate (bool value)
                        {
                            if (value)
                            {
                                foreach (var _t in table2.GetComponentsInChildren<Toggle>().Where(_ => _ != toggle)) _t.isOn = false;
                                selectguid2 = toggle.name;
                                RefreshEdits2();
                            }
                        }
                    );
                }
                col.name = s.ClientOrderDetailGuid;
                col.GetComponentInChildren<Text>(true).text = so.ClientOrderID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table2.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = so.ClientOrderDate.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table2.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = m.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table2.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialSum.ToString();
            }
            //{
            //    GameObject col = GameObject.Instantiate(edit0, table2.transform);
            //    col.SetActive(true);
            //    col.GetComponentInChildren<Text>(true).text = so.ContractID;
            //}
        }
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

        var query1 = connection.Table<SellOrderDetail>();
        var query2 = connection.Table<SellOrder>();
        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        var query4 = connection.Table<Employee>();
        var query5 = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == 5);

        var map2 = new Dictionary<string, SellOrder>();
        foreach (var s in query2)
        {
            map2[s.SellOrderGuid] = s;
        }

        var map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in materials)
        {
            map3[s.MaterialGuID] = s;
        }

        var map4 = new Dictionary<string, Employee>();
        foreach (var s in query4)
        {
            map4[s.EmpGuid] = s;
        }

        var map5 = new Dictionary<string, BasicData>();
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
                GameObject ordergo = GameObject.Instantiate(ordertemplate, table.transform);
                ordergo.SetActive(true);
                var texts = ordergo.GetComponentsInChildren<Text>(true);
                texts[0].text = "Order ID:" + so.SellOrderID;
                texts[1].text = "Order Date:" + so.SellOrderDate.ToString();
                texts[2].text = "Handle Person:" + map4[so.StoragePerson].EmpName;
                texts[3].text = "OutStorage:" + so.OutStorage;
                texts[4].text = "Client:" + so.Client;
                texts[5].text = m.MaterialName;
                texts[6].text = "Number:" + s.MaterialSum.ToString();
                texts[7].text = "Price:" + s.Price.ToString();
                texts[8].text = "Total Money:" + s.MaterialMoney.ToString();
                var toggle = ordergo.GetComponentInChildren<Toggle>(true);
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
                toggle.name = s.ClientOrderDetailGuid;
                var tIcon = ordergo.transform.Find("Icon");
                if (tIcon)
                {
                    var img = tIcon.GetComponentInChildren<Image>();
                    if (img)
                    {
                        int id = materials.IndexOf(materials.Where(_ => _.MaterialGuID == m.MaterialGuID).FirstOrDefault()) + 21;
                        id = Math.Clamp(id, 21, 40);
                        img.sprite = Resources.Load<Sprite>("Icons/Books/books-vector-free-icons-set-" + id.ToString());
                    }
                }
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
