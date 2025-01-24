using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ErpManageLibrary;
using System.Linq;

public class frmClientOrder : BasePanel
{
    public GridLayoutGroup table;
    public Toggle toggle0;
    public GameObject edit0;
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

        if1.text = IDGenerator();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;
    }

    private void RefreshEdits()
    {
        var cod = connection.Table<ClientOrderDetail>().Where(_ => _.ClientOrderDetailGuid == selectguid).FirstOrDefault();
        var co = connection.Table<ClientOrder>().Where(_ => _.ClientOrderGuid == cod.ClientOrderGuid).FirstOrDefault();
        if1.text = co.ClientOrderID;
        if2.text = co.ClientOrderDate.ToString();
        //if3.text = so.Client;
        if4.text = co.CheckBatchID;
        if5.text = co.ContractID;
        //if6.text = so.BatchNO;
        if7.text = cod.MaterialSum.ToString();

        var depts = connection.Table<Dept>().ToList();
        for (int i = 0; i < depts.Count; i++)
        {
            if (depts[i].DeptGuid == co.DownDept)
            {
                dd3.value = i;
                break;
            }
        }

        var materials = connection.Table<ErpManageLibrary.Material>().ToList();
        for (int i = 0; i < materials.Count; i++)
        {
            if (materials[i].MaterialGuID == cod.MaterialGuid)
            {
                dd6.value = i;
                break;
            }
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

        var query1 = connection.Table<ClientOrderDetail>();
        var query2 = connection.Table<ClientOrder>();
        var query3 = connection.Table<ErpManageLibrary.Material>();
        var query4 = connection.Table<Supplier>();
        var query5 = connection.Table<Dept>();

        Dictionary<string, ClientOrder> map2 = new Dictionary<string, ClientOrder>();
        foreach(var s in query2)
        {
            map2[s.ClientOrderGuid] = s;
        }

        Dictionary<string, ErpManageLibrary.Material> map3 = new Dictionary<string, ErpManageLibrary.Material>();
        foreach (var s in query3)
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
                col.name = s.ClientOrderDetailGuid;
                col.GetComponentInChildren<Text>(true).text = co.ClientOrderID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = co.ClientOrderDate.ToString();
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = dept.DeptName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = co.CheckBatchID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = co.ContractID;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = m.MaterialName;
            }
            {
                GameObject col = GameObject.Instantiate(edit0, table.transform);
                col.SetActive(true);
                col.GetComponentInChildren<Text>(true).text = s.MaterialSum.ToString();
            }
        }
    }

    //private void LoadBasicData(int Flag)
    //{
    //    //string ps_Sql = "select  UnitName,UnitID  from BasicData where  IsDelete=0 and flag=" + Flag.ToString() + " order by UnitID";
    //    //var command = connection.CreateCommand(ps_Sql);
    //    //var query = command.ExecuteQuery<BasicData>();

    //    var query = connection.Table<BasicData>().Where(_ => _.IsDelete == 0 && _.flag==Flag);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
