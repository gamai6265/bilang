using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Cloth3D;
using UnityEngine;

namespace HttpRequest {
    [Serializable]
    public class RequestInfo {
        public RequestInfo(string url) {
            Url = url;
            AllowAutoRedirect = true;
        }

        public string Url { private set; get; }
        public byte[] PostData { set; get; }
        public WebHeaderCollection Headers { set; get; }
        public bool AllowAutoRedirect { set; get; }
        public Dictionary<string, string> ExternalData { set; get; }
    }

    [Serializable]
    public class ResponseInfo {
        public RequestInfo RequestInfo { set; get; }
        public Stream ResponseContent { set; get; }
        public HttpStatusCode StatusCode { set; get; }
        public WebHeaderCollection Headers { set; get; }

        public string GetString(Encoding coding) {
            var str = new StringBuilder();
            var sr = ResponseContent;
            sr.Seek(0, SeekOrigin.Begin);
            var data = new byte[1024*1024];
            var readcount = sr.Read(data, 0, data.Length);
            while (readcount > 0) {
                str.Append(coding.GetString(data, 0, readcount));
                readcount = sr.Read(data, 0, data.Length);
            }
            return str.ToString();
        }
    }

    internal class StateObject {
        public byte[] Buffer { set; get; }
        public ResponseInfo ResponseInfo { set; get; }
        public Stream ReadStream { set; get; }
        public HttpWebRequest HttpWebRequest { set; get; }
        public Action<ResponseInfo> Action { set; get; }
        public bool IsBusy { set; get; }
    }

    public class RequestHttpWebRequest {
        static RequestHttpWebRequest() {
            ServicePointManager.DefaultConnectionLimit = 100;
        }

        public RequestHttpWebRequest() {
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
        }

        private StateObject State { set; get; }

        public bool IsBusy {
            get {
                if (State != null)
                    return State.IsBusy;
                return false;
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors) {
            //直接确认，否则打不开  
            return true;
        }
        public void GetResponseAsync(RequestInfo info, Action<ResponseInfo> act) {
            HttpWebRequest webRequest;
            StateObject state;
            InitWebRequest(info, act, out webRequest, out state);
            try {
                if (info.PostData != null && info.PostData.Length > 0) {
                    webRequest.Method = "POST";
                    var contentType = info.Headers.GetValues("ContentType");
                    if (contentType == null) {
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                    } else if(contentType.Length>0){
                        webRequest.ContentType = contentType[0];
                        LogSystem.Debug("ContenType:{0}",contentType[0]);
                    }
                    webRequest.BeginGetRequestStream(EndRequest, state);
                } else {
                    webRequest.BeginGetResponse(EndResponse, state);
                }
            } catch (Exception ex) {
                HandException(ex, state);
            }
        }

        private void EndRequest(IAsyncResult ar) {
            var state = ar.AsyncState as StateObject;
            try {
                var webRequest = state.HttpWebRequest;
                using (var stream = webRequest.EndGetRequestStream(ar)) {
                    var data = state.ResponseInfo.RequestInfo.PostData;
                    stream.Write(data, 0, data.Length);
                }
                webRequest.BeginGetResponse(EndResponse, state);
            } catch (Exception ex) {
                HandException(ex, state);
            }
        }

        private void EndResponse(IAsyncResult ar) {
            var state = ar.AsyncState as StateObject;
            try {
                if (state != null) {
                    var webResponse = state.HttpWebRequest.EndGetResponse(ar) as HttpWebResponse;
                    if (webResponse != null) {
                        state.ResponseInfo.StatusCode = webResponse.StatusCode;
                        state.ResponseInfo.Headers = new WebHeaderCollection();
                        foreach (var key in webResponse.Headers.AllKeys) {
                            state.ResponseInfo.Headers.Add(key, webResponse.Headers[key]);
                        }
                        state.ReadStream = webResponse.GetResponseStream();
                    }
                    if (state.ReadStream != null)
                        state.ReadStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ReadCallBack, state);
                }
            } catch (Exception ex) {
                //404 & other error happen here.
                if (state != null && state.Action != null) {
                    state.ResponseInfo.StatusCode = HttpStatusCode.NotFound;
                    var msg = Encoding.UTF8.GetBytes(ex.Message);
                    state.ResponseInfo.ResponseContent.Write(msg, 0, msg.Length);
                    state.Action(state.ResponseInfo);
                    state.IsBusy = false;
                }
                HandException(ex, state);
            }
        }

        private void ReadCallBack(IAsyncResult ar) {
            var state = ar.AsyncState as StateObject;
            try {
                if (state != null) {
                    var read = state.ReadStream.EndRead(ar);
                    if (read > 0) {
                        state.ResponseInfo.ResponseContent.Write(state.Buffer, 0, read);
                        state.ReadStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ReadCallBack, state);
                    } else {
                        state.ReadStream.Close();
                        state.HttpWebRequest.Abort();
                        if (state.Action != null) {
                            state.Action(state.ResponseInfo);
                        }
                        state.IsBusy = false;
                    }
                }
            } catch (Exception ex) {
                HandException(ex, state);
            }
        }

        private void InitWebRequest(RequestInfo info, Action<ResponseInfo> act, out HttpWebRequest webRequest,
            out StateObject state) {
            webRequest = WebRequest.CreateDefault(new Uri(info.Url)) as HttpWebRequest;
            if (webRequest != null) {
                webRequest.KeepAlive = true;
                webRequest.AllowAutoRedirect = info.AllowAutoRedirect;
                if (info.Headers != null && info.Headers.Count > 0) {
                    foreach (string key in info.Headers.Keys) {
                        webRequest.Headers.Add(key, info.Headers[key]);
                    }
                }
                //webRequest.Proxy = WebProxy.GetDefaultProxy();
                //webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;  
                // webResponse.Headers.Get("Set-Cookie");
            }
            state = new StateObject {
                Buffer = new byte[1024 * 1024],
                HttpWebRequest = webRequest,
                Action = act,
                IsBusy = true,
                ResponseInfo = new ResponseInfo {
                    RequestInfo = info,
                    ResponseContent = new MemoryStream()
                }
            };
        }

        private void HandException(Exception ex, StateObject state) {
            var message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + state.ResponseInfo.RequestInfo.Url +
                          " : " + ex.Message;
            Console.WriteLine(message);
            //LogManager.LogException(message);
        }
    }
}