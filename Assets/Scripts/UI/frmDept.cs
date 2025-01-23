using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmDept : BasePanel
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

        //var tButtonGrid = transform.Find("ButtonGrid");
        //var tEditGrid = transform.Find("EditGrid");
        //var tHeaderGrid = transform.Find("HeaderGrid");
        
        //var t = typeof(Dept);
        //var properties = t.GetProperties();
        //int index = 0;
        //foreach(var p in properties)
        //{
        //    if (index == 0)
        //    {
        //    }
        //    else
        //    {
        //        tEditGrid.GetChild(index - 1).GetComponentInChildren<Text>(true).text = " "+p.Name;
        //        tHeaderGrid.GetChild(index-1).GetComponentInChildren<Text>(true).text = p.Name;
        //    }
        //    index++;
        //}

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
        var s = connection.Table<Dept>().Where(_ => _.DeptGuid == selectguid).FirstOrDefault();
        if1.text = s.DeptName;
        if2.text = s.DeptPerson;
        if3.text = s.Telephone;
        if4.text = s.Fax;
        if5.text = s.Address;
        if6.text = s.Address;
        if7.text = string.Empty;
        if8.text = string.Empty;
        toggle9.isOn = s.IsEnable>0;
        if10.text = string.Empty;
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

        var query4 = connection.Table<Dept>();
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
                col.name = s.DeptGuid;
                col.GetComponentInChildren<Text>(true).text = s.DeptName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.DeptPerson;
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
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.IsEnable.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = string.Empty;
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




        var s = connection.Table<Dept>().Where(_ => _.DeptGuid == selectguid).FirstOrDefault();
        s.DeptName = if1.text;
        s.DeptPerson = if2.text;
        s.Telephone = if3.text;
        s.Fax = if4.text;
        s.Address = if5.text;
        s.IsEnable = toggle9.isOn ? 1 : 0;

        connection.Update(s);

        Load();
    }


    private void DeleteBillExe()
    {
        connection.Delete<Dept>(selectguid);
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

        connection.DeleteAll<Dept>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {

        string orderguid = Guid.NewGuid().ToString();

        var s = new Dept();
        s.DeptGuid = orderguid;
        s.DeptName = if1.text;
        s.DeptPerson = if2.text;
        s.Telephone = if3.text;
        s.Fax = if4.text;
        s.Address = if5.text;
        s.IsEnable = toggle9.isOn?1:0;

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
