using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmSupplier : BasePanel
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

    private void RefreshEdits()
    {
        var s = connection.Table<Supplier>().Where(_ => _.Guid == selectguid).FirstOrDefault();
        if1.text = s.SimpName;
        if2.text = s.Name;
        if3.text = s.LinkMan;
        if4.text = s.Telephone;
        if5.text = s.Fax;
        if6.text = s.Address;
        if7.text = s.Zip;
        if8.text = s.Remark;
        toggle9.isOn = s.IsEnable>0;
        if10.text = s.ProduceType;
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

        var query4 = connection.Table<Supplier>();
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
                col.name = s.Guid;
                col.GetComponentInChildren<Text>(true).text = s.SimpName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Name;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.LinkMan;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Telephone;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Fax;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Address;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Zip;
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
                col.GetComponentInChildren<Text>(true).text = s.ProduceType;
            }
        }
    }


    private void Load()
    {
        LoadBills();
        
    }



    public void EditBill()
    {
        if (string.IsNullOrEmpty(selectguid))
            return;




        var s = connection.Table<Supplier>().Where(_ => _.Guid == selectguid).FirstOrDefault();
        s.SimpName = if1.text;
        s.Name = if2.text;
        s.LinkMan = if3.text;
        s.Telephone = if4.text;
        s.Fax = if5.text;
        s.Address = if6.text;
        s.Zip = if7.text;
        s.Remark = if8.text;
        s.IsEnable = toggle9.isOn?1:0;
        s.ProduceType = if10.text;

        connection.Update(s);

        Load();
    }


    private void DeleteBillExe()
    {
        connection.Delete<Supplier>(selectguid);
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

        connection.DeleteAll<Supplier>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {

        string orderguid = Guid.NewGuid().ToString();

        Supplier s = new Supplier();
        s.Guid = orderguid;
        s.SimpName = if1.text;
        s.Name = if2.text;
        s.LinkMan = if3.text;
        s.Telephone = if4.text;
        s.Fax = if5.text;
        s.Address = if6.text;
        s.Zip = if7.text;
        s.Remark = if8.text;
        s.IsEnable = toggle9.isOn?1:0;
        s.ProduceType = if10.text;

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
