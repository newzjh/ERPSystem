using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMainForm : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonStockInOrder()
    {
        SwitchFrm<frmStockInOrder>();
    }

    public void OnButtonBuyOrder()
    {
        SwitchFrm<frmBuyOrder>();
    }

    public void OnButtonSellOrder()
    {
        SwitchFrm<frmSellOrder>();
    }

    public void OnButtonClientOrder()
    {
        SwitchFrm<frmClientOrder>();
    }

    public void OnButtonStockReport1()
    {
        var frm = FindFirstObjectByType<frmSingleStockReport>(FindObjectsInactive.Include);
        frm.currentstorage = 0;
        frm.RefreshStorageTitle();
        frm.Load();
        SwitchFrm<frmSingleStockReport>();
    }
    public void OnButtonStockReport2()
    {
        var frm = FindFirstObjectByType<frmSingleStockReport>(FindObjectsInactive.Include);
        frm.currentstorage = 1;
        frm.RefreshStorageTitle();
        frm.Load();
        SwitchFrm<frmSingleStockReport>();
    }
}
