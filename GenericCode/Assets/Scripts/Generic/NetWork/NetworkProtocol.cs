using System;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class NetworkManager : LazySingleton<NetworkManager>
{
    
    private async UniTask<T> SendNetwork<T>(string url, string sendType, string body = null)
    {
        string requestURL = $"{scheme}://{ip}:{port}/{url}";

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(timeout));

        UnityWebRequest request = new UnityWebRequest(requestURL, sendType);
        request.downloadHandler = new DownloadHandlerBuffer();
        if (!string.IsNullOrEmpty(body))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

        try
        {
            var res = await request.SendWebRequest().WithCancellation(cts.Token);
            T result = JsonUtility.FromJson<T>(res.downloadHandler.text);
            return result;
        }
        catch (OperationCanceledException ex)
        {
            if (ex.CancellationToken == cts.Token)
            {
                Debug.LogWarning("TimeOut");

                return default(T);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            string response = e.Message;

            return default(T);
        }

        return default(T);
    }

}
