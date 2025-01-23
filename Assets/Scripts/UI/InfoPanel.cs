using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InfoPanel : BasePanel
{
    private Text _text = null;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        if (_text == null)
            _text = GetComponentInChildren<Text>();
    }

    private Queue<string> messages = new Queue<string>();

    private void AddMessage(string msg)
    {
        if (_text==null)
            _text = GetComponentInChildren<Text>();

        messages.Enqueue(msg);
        while (messages.Count > 10)
            messages.Dequeue();

        msg = string.Empty;
        foreach(var s in messages)
             msg = msg + "<<"+s;
        _text.text = msg;
    }

        // Update is called once per frame
    void Update()
    {
        //coordtext.text = string.Format("  {0:0.00000},{1:0.00000} << {2:0.00000}", coord.longitude, coord.latitude, coord.altitude);
    }

    public static void ShowMessage(string msg)
    {
        var panel = FindFirstObjectByType<InfoPanel>();
        panel.AddMessage(msg);
    }
}
