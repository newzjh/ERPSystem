using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsPanel : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonSupplier()
    {
        SwitchFrm<frmSupplier>();
    }

    public void OnButtonModel()
    {
        SwitchFrm<frmMaterial>();
    }

    public void OnButtonDept()
    {
        SwitchFrm<frmDept>();
    }

    public void OnButtonEmployee()
    {
        SwitchFrm<frmEmployee>();
    }

    public void OnButtonBasicData()
    {
        SwitchFrm<frmBasicData>();
    }

    public void OnButtonClientData()
    {
        SwitchFrm<frmClientData>();
    }

    public void OnButtonQuit()
    {
        Application.Quit();
    }
}
