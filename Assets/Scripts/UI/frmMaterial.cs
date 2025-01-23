using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmMaterial : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public InputField if3;
    public InputField if4;
    public InputField if5;
    public InputField if6;
    public InputField if7;
    public InputField if8;
    public Toggle toggle9;
    public Dropdown dd10;

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
    private void RefreshEdits()
    {
        var s = connection.Table<ErpManageLibrary.Material>().Where(_ => _.MaterialGuID == selectguid).FirstOrDefault();
        if1.text = s.MaterialID;
        if2.text = s.MaterialName;
        if3.text = s.ClassId;
        if4.text = s.Unit;
        if5.text = s.Spec;
        if6.text = s.Price.ToString();
        if7.text = s.CalculateMethod;
        if8.text = s.Remark;
        toggle9.isOn = s.IsEnable>0;
        //if10.text = s.ProduceType;
    }

    private void LoadSuppliers()
    {
        //string ps_Sql = "select Guid,SimpName as  供应商简称,[Name] as 供应商名称,LinkMan as 联系人,Telephone as 电话,Fax as 传真,Address as 地址,Zip as 邮编,ProduceType as 生产类型,Remark as  备注,Case when IsEnable=1 then '停用' when IsEnable=0 then '可用' end as 是否可用 from Supplier   ";
        var query = connection.Table<Supplier>();
        dd10.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.Name;
            dd10.options.Add(od);
        }
        if (dd10.options.Count <= 0)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "UnknownSupplier";
            dd10.options.Add(od);
        }
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

        var query3 = connection.Table<Supplier>();
        Dictionary<string, Supplier> map3 = new Dictionary<string, Supplier>();
        foreach (var s in query3)
        {
            map3[s.Guid] = s;
        }

        var query4 = connection.Table<ErpManageLibrary.Material>();
        foreach (var s in query4)
        {
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
                col.GetComponentInChildren<Text>(true).text = s.ClassId;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Unit;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Spec;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Price.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.CalculateMethod;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Remark;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.IsEnable.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                if (!string.IsNullOrEmpty(s.SupplierGuid))
                    col.GetComponentInChildren<Text>(true).text = map3[s.SupplierGuid].Name;
                else
                    col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
        }
    }


    private void Load()
    {
        LoadSuppliers();
        LoadBills();
    }

    private void EditBillExe()
    {
        var query4 = connection.Table<Supplier>();
        List<Supplier> suppliers = new List<Supplier>();
        foreach (var q in query4)
        {
            suppliers.Add(q);
        }

        var s = connection.Table<ErpManageLibrary.Material>().Where(_ => _.MaterialGuID == selectguid).FirstOrDefault();
        s.MaterialID = if1.text;
        s.MaterialName = if2.text;
        s.ClassId = if3.text;
        s.Unit = if4.text;
        s.Spec = if5.text;
        s.Price = decimalTryParse(if6.text);
        s.CalculateMethod = if7.text;
        s.Remark = if8.text;
        s.IsEnable = toggle9.isOn ? 1 : 0;
        s.SupplierGuid = suppliers[dd10.value].Guid;
        s.SafeStockSum = 0;
        s.PicID = string.Empty;
        s.ClientID = string.Empty;
        s.Place = string.Empty;

        connection.Update(s);
    }

    public void EditBill()
    {
        if (string.IsNullOrEmpty(selectguid))
            return;

        EditBillExe();

        Load();
    }


    private void DeleteBillExe()
    {
        connection.Delete<ErpManageLibrary.Material>(selectguid);
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

        connection.DeleteAll<ErpManageLibrary.Material>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {
        var query4 = connection.Table<Supplier>();
        List<Supplier> suppliers = new List<Supplier>();
        foreach (var q in query4)
        {
            suppliers.Add(q);
        }

        string orderguid = Guid.NewGuid().ToString();

        var s = new ErpManageLibrary.Material();
        s.MaterialGuID = orderguid;
        s.MaterialID = if1.text;
        s.MaterialName = if2.text;
        s.ClassId = if3.text;
        s.Unit = if4.text;
        s.Spec = if5.text;
        s.Price = decimalTryParse(if6.text);
        s.CalculateMethod = if7.text;
        s.Remark = if8.text;
        s.IsEnable = toggle9.isOn?1:0;
        s.SupplierGuid = suppliers[dd10.value].Guid;
        s.SafeStockSum = 0;
        s.PicID = string.Empty;
        s.ClientID = string.Empty;
        s.Place = string.Empty;

        connection.Insert(s);

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
