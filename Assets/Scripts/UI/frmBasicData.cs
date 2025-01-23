using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmBasicData : BasePanel
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

        Load();
    }


    private void RefreshEdits()
    {
        var s = connection.Table<BasicData>().Where(_ => _.UnitID == selectguid).FirstOrDefault();
        if1.text = s.UnitID;
        if2.text = s.UnitName;
        if3.text = s.flag.ToString();
        toggle9.isOn = s.IsDelete>0;
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

        var query4 = connection.Table<BasicData>();
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
                col.name = s.UnitID;
                col.GetComponentInChildren<Text>(true).text = s.UnitID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.UnitName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.flag.ToString();
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
                col.GetComponentInChildren<Text>(true).text = s.IsDelete.ToString();
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




        var s = connection.Table<BasicData>().Where(_ => _.UnitID == selectguid).FirstOrDefault();
        s.UnitID = if1.text;
        s.UnitName = if2.text;
        s.flag = (int)decimalTryParse(if3.text);
        s.IsDelete = toggle9.isOn ? 1 : 0;

        connection.Update(s);

        Load();
    }


    private void DeleteBillExe()
    {
        connection.Delete<BasicData>(selectguid);
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

        connection.DeleteAll<BasicData>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {

        string orderguid = Guid.NewGuid().ToString();
        
        var s = new BasicData();
        s.UnitID = if1.text;
        s.UnitName = if2.text;
        s.flag = (int)decimalTryParse(if3.text);
        s.IsDelete = toggle9.isOn?1:0;

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
