using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;
using System.Xml.Schema;

public class frmNewClientOrder : BasePanel
{
    public VerticalLayoutGroup table;
    public GridLayoutGroup table1;

    public Toggle toggle0;
    public GameObject edit0;
    public Button button0;
    public GameObject ordertemplate;

    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public InputField if4;
    public InputField if5;
    public Dropdown dd6;
    public InputField if7;


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

        OnSelectMaterial(dd6.value);
        dd6.onValueChanged.AddListener(OnSelectMaterial);

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
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table1.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
        }

        
    }

    private void RefreshEdits()
    {
        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        var depts = connection.Table<Dept>().ToList();

        var cod = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderDetailGuid == selectguid).FirstOrDefault();
        var co = connection.Table<ClientOrder>().Where(_ => _.ClientOrderGuid == cod.ClientOrderGuid).FirstOrDefault();
        if1.text = co.ClientOrderID;
        if2.text = co.ClientOrderDate.ToString();
        dd3.value = depts.IndexOf(depts.Where(_ => _.DeptGuid == co.DownDept).FirstOrDefault());
        if4.text = co.CheckBatchID;
        if5.text = co.ContractID;
        dd6.value = materials.IndexOf(materials.Where(_ => _.MaterialGuID == cod.MaterialGuid).FirstOrDefault());
        if7.text = cod.MaterialSum.ToString();
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

        var query1 = connection.Table<ClientOrderDetail>();
        var query2 = connection.Table<ClientOrder>();
        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        var suppliers = connection.Table<Supplier>().ToList();
        var query5 = connection.Table<Dept>();

        var map2 = new Dictionary<string, ClientOrder>();
        foreach(var s in query2)
        {
            map2[s.ClientOrderGuid] = s;
        }

        var map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in materials)
        {
            map3[s.MaterialGuID] = s;
        }
        
        var depts = new Dictionary<string, Dept>();
        foreach (var q in query5)
        {
            depts[q.DeptGuid] = q;
        }

        foreach (var s in query1)
        {
            if (!map2.ContainsKey(s.ClientOrderGuid))
                continue;

            ClientOrder co = map2[s.ClientOrderGuid];

            if (!map3.ContainsKey(s.MaterialGuid))
                continue;

            ErpManageLibrary.Material m = map3[s.MaterialGuid];

            if (!depts.ContainsKey(co.DownDept))
                continue;

            Dept dept = depts[co.DownDept];

            {
                GameObject ordergo = GameObject.Instantiate(ordertemplate, table.transform);
                ordergo.SetActive(true);
                var texts = ordergo.GetComponentsInChildren<Text>(true);
                texts[0].text = "Order ID:"+co.ClientOrderID;
                texts[1].text = "Order Date:"+co.ClientOrderDate.ToString();
                texts[2].text = "Handle Dept:"+dept.DeptName;
                texts[3].text = "CheckBatch ID:" + co.CheckBatchID;
                texts[4].text = "Contract ID:" + co.ContractID;
                texts[5].text = m.MaterialName;
                texts[6].text = "Number:"+s.MaterialSum.ToString();
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
                        int id = 21 + materials.IndexOf(materials.Where(_ => _.MaterialGuID == m.MaterialGuID).FirstOrDefault());
                        img.sprite = Resources.Load<Sprite>("Icons/Books/books-vector-free-icons-set-"+id.ToString());
                    }
                }
            }

 
        }
    }

    //private void LoadBasicData(int Flag)
    //{
    //    var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag == Flag);
    //    dd5.options.Clear();
    //    foreach (var s in query)
    //    {
    //        Dropdown.OptionData od = new Dropdown.OptionData();
    //        od.text = s.UnitName;
    //        dd5.options.Add(od);
    //    }
    //}

    private void LoadDepts()
    {
        var query = connection.Table<Dept>();
        dd3.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.DeptName;
            dd3.options.Add(od);
        }
        dd3.value = 3;
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
        //LoadBasicData(5);
        LoadMaterials();
        LoadDepts();
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

        var query4 = connection.Table<Dept>();
        List<Dept> depts = new List<Dept>();
        foreach (var s in query4)
        {
            depts.Add(s);
        }

        var cod = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderDetailGuid == selectguid).FirstOrDefault();
        var co = connection.Table<ClientOrder>().Where(_ => _.ClientOrderGuid == cod.ClientOrderGuid).FirstOrDefault();

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        co.ClientOrderID = if1.text;
        co.ClientOrderDate = datetime;
        co.DownDept = depts[dd3.value].DeptGuid;
        co.CheckBatchID = if4.text;
        co.ContractID = if5.text;
        co.EncasementDate = datetime;
        co.ReceiveDept = string.Empty;
        co.Remark = string.Empty;
        co.CreateGuid = Guid.Empty.ToString();
        co.CreateDate = datetime;
        co.CheckGuid = Guid.Empty.ToString();
        co.CheckDate = datetime;
        co.EndGuid = Guid.Empty.ToString();
        co.EndDate = datetime;
        co.CheckGuid2 = Guid.Empty.ToString();
        co.CheckDate2 = datetime;
        co.OrderType = string.Empty;

        cod.MaterialGuid = map3[dd6.value].MaterialGuID;
        cod.MaterialSum = decimalTryParse(if7.text);
        cod.Remark = string.Empty;

        connection.Update(cod);
        connection.Update(co);

        Load();
    }

    private void DeleteBillExe()
    {
        var clientOrderDetailGuid = selectguid;
        var clientOrderGuid = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderDetailGuid == clientOrderDetailGuid).FirstOrDefault().ClientOrderGuid;
        connection.Delete<ClientOrderDetail>(clientOrderDetailGuid);
        int count = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderGuid == clientOrderGuid).Count();
        if (count<=0)
        {
            connection.Delete<ClientOrder>(clientOrderGuid);
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
        connection.DeleteAll<ClientOrderDetail>();
        connection.DeleteAll<ClientOrder>();
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

        var query4 = connection.Table<Dept>();
        List<Dept> depts = new List<Dept>();
        foreach (var s in query4)
        {
            depts.Add(s);
        }

        DateTime datetime = DateTime.Now;
        DateTime.TryParse(if2.text, out datetime);

        string orderguid = Guid.NewGuid().ToString();
        string detailorderguid = Guid.NewGuid().ToString();

        ClientOrder co = new ClientOrder();
        co.ClientOrderGuid = orderguid;
        co.ClientOrderID = if1.text;
        co.ClientOrderDate = datetime;
        co.DownDept = depts[dd3.value].DeptGuid;
        co.ContractID = if4.text;
        co.CheckBatchID = if5.text;
        co.EncasementDate = datetime;
        co.ReceiveDept = string.Empty;
        co.Remark = string.Empty;
        co.CreateGuid = Guid.Empty.ToString();
        co.CreateDate = datetime;
        co.CheckGuid = Guid.Empty.ToString();
        co.CheckDate = datetime;
        co.EndGuid = Guid.Empty.ToString();
        co.EndDate = datetime;
        co.CheckGuid2 = Guid.Empty.ToString();
        co.CheckDate2 = datetime;
        co.OrderType = string.Empty;

        ClientOrderDetail cod = new ClientOrderDetail();
        cod.ClientOrderDetailGuid = detailorderguid;
        cod.ClientOrderGuid = orderguid;
        cod.MaterialGuid = map3[dd6.value].MaterialGuID;
        cod.MaterialSum = decimalTryParse(if7.text);
        cod.Remark = string.Empty;

        connection.Insert(co);
        connection.Insert(cod);

    }

    public void NewBill()
    {
        NewBillExe();

        Load();
    }

    public void SubmitOrder()
    {
        if1.text = IDGenerator();

        NewBillExe();

        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
