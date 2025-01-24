using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using SQLite;
using ErpManageLibrary;
using System.Runtime.Serialization;
using System.Text;

public class BasePanel : MonoBehaviour
{

    //protected LocationManager loc;
    protected static SQLiteConnection connection = null;

    protected virtual void Awake()
    {
        var tbar = transform.Find("Bar");
        if (tbar)
        {
            var b = tbar.GetComponentInChildren<Button>();
            if (b)
            {
                b.onClick.AddListener(SwitchVisiblity);
            }
        }

        //loc = GameObject.FindFirstObjectByType<LocationManager>(FindObjectsInactive.Include);
        if (connection == null)
        {
#if UNITY_EDITOR
            connection = new SQLiteConnection(Application.dataPath+ "/Resources/JXC.db.bytes");
#else
            ConnectDatebase();
#endif
        }
    }

    private void ConnectDatebase()
    {
        InfoPanel.ShowMessage("start loading database...");
        string localpath = Application.persistentDataPath + "/JXC.db";
        //if (!System.IO.File.Exists(localpath))
        {
            var asset = Resources.Load<TextAsset>("JXC.db");
            InfoPanel.ShowMessage(asset==null?"asset==null":asset.bytes.Length.ToString());
            System.IO.File.WriteAllBytes(localpath, asset.bytes);
            connection = new SQLiteConnection(localpath);
            InfoPanel.ShowMessage("finish loading database...");
        }
    }

    public class WebRequestSkipCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    protected async UniTask<Texture2D> GetTextureFromURL(string url, bool skipcertificate = false, int timeout = 120)
    {
        var www = UnityWebRequestTexture.GetTexture(url);
        //www.timeout = 30;
        if (skipcertificate)
            www.certificateHandler = new WebRequestSkipCertificate();

        Texture2D tex = null;

        try
        {
            var task = www.SendWebRequest().ToUniTask();
            if (www.downloadedBytes > 0 || www.result == UnityWebRequest.Result.InProgress || www.result == UnityWebRequest.Result.Success)
                await task;
            else
                await task.TimeoutWithoutException(TimeSpan.FromSeconds(timeout));
        }
        catch (Exception e)
        {
            //Debug.Log("request " + url + " exception :" + e);
        }

        if (www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone)
        {
            Debug.Log("[BaseUrlRequest] is successd for: " + url);
            tex = (www.downloadHandler as DownloadHandlerTexture).texture;
        }
        else if (!string.IsNullOrEmpty(www.error) && (www.error.Contains("429") || www.error.Contains("timed out")))
        {
            Debug.Log("[BaseUrlRequest] data reload : " + www.error + " " + url);
            await UniTask.WaitForSeconds(1);
            return await GetTextureFromURL(url);
        }
        else
        {
            Debug.Log("[BaseUrlRequest] data missing :" + www.error + " " + url);
        }

        www.Abort();

        return tex;
    }

    protected async UniTask<byte[]> GetDataFromURL(string url,bool skipcertificate=false, int timeout = 120)
    {
        var www = UnityWebRequest.Get(url);
        //www.timeout = 30;
        if (skipcertificate)
            www.certificateHandler = new WebRequestSkipCertificate();

        try
        {
            var task = www.SendWebRequest().ToUniTask();
            if (www.downloadedBytes > 0 || www.result == UnityWebRequest.Result.InProgress || www.result == UnityWebRequest.Result.Success)
                await task;
            else
                await task.TimeoutWithoutException(TimeSpan.FromSeconds(timeout));
        }
        catch (Exception e)
        {
            //Debug.Log("request " + url + " exception :" + e);
        }

        if (www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone && www.downloadHandler.data.Length > 0)
        {
            Debug.Log("[BaseUrlRequest] is successd for: " + url);
        }
        else if (!string.IsNullOrEmpty(www.error) && (www.error.Contains("429") || www.error.Contains("timed out")))
        {
            Debug.Log("[BaseUrlRequest] data reload :" + www.error + " " + url);
            await UniTask.WaitForSeconds(1);
            return await GetDataFromURL(url);
        }
        else
        {
            Debug.Log("[BaseUrlRequest] data missing :" + www.error + " " + url);
        }

        byte[] data = www.downloadHandler.data;

        www.Abort();

        return data;
    }

    protected async UniTask<string> GetTextFromURL(string url, bool skipcertificate = false, int timeout = 120)
    {
        var www = UnityWebRequest.Get(url);
        //www.timeout = 30;
        if (skipcertificate)
            www.certificateHandler = new WebRequestSkipCertificate();

        try
        {
            var task = www.SendWebRequest().ToUniTask();
            if (www.downloadedBytes > 0 || www.result == UnityWebRequest.Result.InProgress || www.result == UnityWebRequest.Result.Success)
                await task; 
            else
                await task.TimeoutWithoutException(TimeSpan.FromSeconds(timeout)); 
        }
        catch (Exception e)
        {
            //Debug.Log("request " + url + " exception :" + e);
        }

        if (www.result != UnityWebRequest.Result.ConnectionError && www.downloadHandler.isDone && www.downloadHandler.data.Length > 0)
        {
            Debug.Log("[BaseUrlRequest] is successd for: " + url);
        }
        else if (!string.IsNullOrEmpty(www.error) && (www.error.Contains("429") || www.error.Contains("timed out")))
        {
            Debug.Log("[BaseUrlRequest] data reload :" + www.error + " " + url);
            await UniTask.WaitForSeconds(1);
            return await GetTextFromURL(url);
        }
        else
        {
            Debug.Log("[BaseUrlRequest] data missing :" + www.error + " " + url);
        }

        byte[] data = www.downloadHandler.data;

        string text = string.Empty;
        if (data!=null)
        { 
            MemoryStream ms = new MemoryStream(data);
            StreamReader sr = new StreamReader(ms);
            text = await sr.ReadToEndAsync();
            sr.Dispose();
            ms.Dispose();
        }

        www.Abort();

        return text;
    }

    public void SwitchVisiblity()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SwitchFrm<T>() where T: BasePanel
    {
        var frm = GameObject.FindFirstObjectByType<T>(FindObjectsInactive.Include);
        frm.gameObject.SetActive(!frm.gameObject.activeSelf);
    }

    protected decimal decimalTryParse(string text)
    {
        decimal ret = 0;
        decimal.TryParse(text, out ret);
        return ret;
    }

    protected string IDGenerator()
    {
        var now = DateTime.Now;
        string r1 = now.Year.ToString() + now.Month.ToString() + now.Day.ToString();

        var rnd = new System.Random();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            sb.Append(rnd.Next(10));
        }
        string r2 = sb.ToString();

        return r1 + r2; ;
    }
}
