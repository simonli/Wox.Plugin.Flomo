using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Wox.Plugin.Flomo
{
    public static class Utils
    {
        public static Configuration GetConfig()
        {
            //获取调用当前正在执行的方法的方法的 Assembly  
            var assembly = Assembly.GetCallingAssembly();
            var path = $"{assembly.Location}.config";

            if (File.Exists(path) == false)
            {
                var msg = $"{path}路径下的文件未找到 ";
                throw new FileNotFoundException(msg);
            }

            try
            {
                var configFile = new ExeConfigurationFileMap {ExeConfigFilename = path};
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
                return config;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string PostWebRequest(string postUrl, string paramData)
        {
            string responseContent = string.Empty;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/json;charset=UTF-8";
                webReq.ContentLength = byteArray.Length;
                using (Stream reqStream = webReq.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);//写入参数
                                                                    //reqStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return responseContent;
        }
    }
}
