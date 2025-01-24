using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMainForm : BasePanel
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

    public void OnButtonNewClientOrder()
    {
        SwitchFrm<frmNewClientOrder>();
    }

    public void OnButtonStockReport()
    {
        SwitchFrm<frmGlobalStockReport>();
    }
    public void OnButtonNewSellOrder()
    {
        SwitchFrm<frmNewSellOrder>();
    }
}
