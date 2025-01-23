using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmEmployee : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
    public InputField if1;
    public InputField if2;
    public Dropdown dd3;
    public InputField if4;
    public InputField if5;
    public InputField if6;
    public Dropdown dd7;
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
    private void LoadSexs()
    {
        //string ps_Sql = "select Guid,SimpName as  供应商简称,[Name] as 供应商名称,LinkMan as 联系人,Telephone as 电话,Fax as 传真,Address as 地址,Zip as 邮编,ProduceType as 生产类型,Remark as  备注,Case when IsEnable=1 then '停用' when IsEnable=0 then '可用' end as 是否可用 from Supplier   ";
        dd3.options.Clear();
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "man";
            dd3.options.Add(od);
        }
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = "woman";
            dd3.options.Add(od);
        }
    }

    private void LoadDepts()
    {
        var query = connection.Table<Dept>();
        dd7.options.Clear();
        foreach (var s in query)
        {
            Dropdown.OptionData od = new Dropdown.OptionData();
            od.text = s.DeptName;
            dd7.options.Add(od);
        }
    }

    private int FindIndex(string deptguid)
    {
        var query4 = connection.Table<Dept>();
        var depts = new Dictionary<string, Dept>();
        int index = 0;
        foreach (var q in query4)
        {
            if (q.DeptGuid == deptguid)
                return index;
            index++;
        }
        return 0;
    }

    private void RefreshEdits()
    {
        var s = connection.Table<Employee>().Where(_ => _.EmpGuid == selectguid).FirstOrDefault();
        if1.text = s.EmpID;
        if2.text = s.EmpName;
        dd3.value = s.Sex == "男" || s.Sex == "man" ? 0 : 1;
        if4.text = s.Telephone;
        if5.text = s.Address;
        if6.text = s.CardID;
        dd7.value = FindIndex(s.Dept);
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
        var depts = new Dictionary<string,Dept>();
        foreach (var q in query4)
        {
            depts[q.DeptGuid] = q;
        }

        var queryx = connection.Table<Employee>();
        foreach (var s in queryx)
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
                                foreach(var _t in table.GetComponentsInChildren<Toggle>().Where(_ => _ != toggle)) _t.isOn = false;
                                selectguid = toggle.name;
                                RefreshEdits();
                            }
                        }
                    );
                }
                col.name = s.EmpGuid;
                col.GetComponentInChildren<Text>(true).text = s.EmpID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.EmpName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Sex;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Telephone;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.Address;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.CardID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = depts[s.Dept].DeptName;
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
        LoadSexs();
        LoadDepts();
        LoadBills();
    }



    public void EditBill()
    {
        if (string.IsNullOrEmpty(selectguid))
            return;

        var query4 = connection.Table<Dept>();
        List<Dept> depts = new List<Dept>();
        foreach (var q in query4)
        {
            depts.Add(q);
        }

        var s = connection.Table<Employee>().Where(_ => _.EmpGuid == selectguid).FirstOrDefault();
        s.EmpID = if1.text;
        s.EmpName = if2.text;
        s.Sex = dd3.options[dd3.value].text;
        s.Telephone = if4.text;
        s.Address = if5.text;
        s.CardID = if6.text;
        s.Dept = depts[dd7.value].DeptGuid;
        s.IsEnable = toggle9.isOn ? 1 : 0;

        connection.Update(s);

        Load();
    }


    private void DeleteBillExe()
    {
        connection.Delete<Employee>(selectguid);
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

        connection.DeleteAll<Employee>();
    }

    public void DeleteBillAll()
    {
        DeleteBillAllExe();

        Load();
    }


    private void NewBillExe()
    {
        var query4 = connection.Table<Dept>();
        List<Dept> depts = new List<Dept>();
        foreach (var q in query4)
        {
            depts.Add(q);
        }

        string orderguid = Guid.NewGuid().ToString();

        var s = new Employee();
        s.EmpGuid = orderguid;
        s.EmpID = if1.text;
        s.EmpName = if2.text;
        s.Sex = dd3.options[dd3.value].text;
        s.Telephone = if4.text;
        s.Address = if5.text;
        s.CardID = if6.text;
        s.Dept = depts[dd7.value].DeptGuid;
        s.IsEnable = toggle9.isOn ? 1 : 0;

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
