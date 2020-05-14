using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreWebAPI.Controllers
{
    [Route("apiV1/[controller]")]
    public class ExperimentController : Controller
    {
        private readonly Service_Main client;

        public ExperimentController(CoreLibrary.Service_Main service_Main)
        {
            this.client = service_Main;
        }



        // GET <controller>/Get/SayHello
        /// <summary>
        /// 这仅仅是测试API是不是可用CC
        /// </summary>
        /// <returns></returns>        
        [HttpGet("SayHello")]
        public API_Response SayHello()
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.SayHello();
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;

        }
        


        /// <summary>
        /// 查询某个被管理的试验
        /// </summary>
        /// <param name="id">被管理试验的ID</param>
        /// <returns></returns>
        [HttpGet("GetMessageByID/{id}")]
        public API_Response GetMessageByID(int id)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.GetMessageByID(id);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }

        [HttpGet("GetMessageS_Single_ByID/{id}")]
        public API_Response GetMessageS_Single_ByID(int id)
        {
            API_Response api = new API_Response();
            #region MyRegion
            try
            {
                //Service_MainClient client = new Service_MainClient();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.GetMessageS_Sigle_ByID(id);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion
            return api;
        }
        /// <summary>
        /// 查询全部正处于管理状态的试验
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllMessage")]
        public API_Response GetAllMessage()
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.GetAllMessage();
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }

        /// <summary>
        /// 与试验模型进行通讯
        /// </summary>
        /// <param name="message">发送给它的数据，采用逗号形式隔开</param>
        /// <param name="id">被管理试验的ID</param>
        /// <returns></returns>
        [HttpPut("SetSendDataStr/{id}/{message}"), HttpPost("SetSendDataStr")]
        public API_Response SetSendDataStr(string message, int id)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.SetSendDataStr(message, id);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }
        /// <summary>
        /// 设置某个试验的状态，此方法仅在测试使用，部署时会删除。注意提醒我！！
        /// </summary>
        /// <param name="t">试验状态，在测试时基本只会使用最后一个。  New = 0, FilesCopying = 201,FilesCopyCompleted = 202,CmdCommand = 203,Running = 204,Error = 205,Error_NoFind_SourcePath = 206,Error_NoFind_FuPanPath = 207,Error_No_MotherPath = 208,Error_No_THIS_HR_ID = 209,Error_Write_MotherPathName = 210,Error_UID_NOT_Equal_LookOtherUID = 211,End = 212,</param>
        /// <param name="id">被管理试验的ID</param>
        /// <returns></returns>
        [HttpPost("SetStatus")]
        public API_Response SetStatus(ShiYanStatus t, int id)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.SetStatus(t, id);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="motherPathName"></param>
        /// <returns></returns>
        [HttpDelete("DeleteFolder/{motherPathName}")]
        public API_Response DeleteFolder(string motherPathName)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.DeleteFolder(motherPathName);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }

        /// <summary>
        /// 查询华人设置试验项目的通讯协议信息
        /// </summary>
        /// <param name="hrID">华人ID</param>
        /// <param name="xieyi">协议类型
        /// tcpReceiveBytes = 0,
        /// tcpReceiveBytesNames = 1, 
        /// tcpSendBytes = 2,
        /// tcpSendBytesNames = 3,</param>
        /// <returns></returns>
        [HttpGet("GetTag_By_HRID_And_XIEYI/{hrID}/{xieyi}")]
        public API_Response GetTag_By_HRID_And_XIEYI(int hrID, ShiYanXieYi xieyi)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.GetTag_By_HRID_And_XIEYI(hrID, xieyi);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }
        /// <summary>
        /// 创建试验
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="type">创建试验类型
        /// New = 101,
        /// ReView = 102,
        /// LookOthers = 103,</param>
        /// <param name="hrID">华人设置的原型ID</param>
        /// <param name="look_uid">其他用户ID，如果在自己创建或者是复盘自己的试验时，可以写UID，复盘他人试验时，填写其他用户ID</param>
        /// <param name="motherPath">在复盘和查看他人试验时，填写此项。创建新试验时可以写空格</param>
        /// <returns></returns>
        [HttpGet("CreateNew_ShiYan/{uid}/{type}/{hrID}/{m_Setting_Arguments}/{look_uid}/{motherPath}"),HttpPut("CreateNew_ShiYan"), HttpPost("CreateNew_ShiYan")]
        public API_Response CreateNew_ShiYan(int uid, ShiYanCreateType type, int hrID, string m_Setting_Arguments, int look_uid, string motherPath)
        {
            API_Response api = new API_Response();

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.CreateNew_ShiYan(uid, type, hrID, m_Setting_Arguments, look_uid, motherPath);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }
        /// <summary>
        /// 上传.m文件
        /// </summary>
        /// <param name="id">被管理的试验ID</param>
        /// <param name="file">文件</param>
        /// <returns>如果上传成功后，用户可以自行重启自己正在做的试验，</returns>
        [HttpPut("UpLoad_Mfile"), HttpPost("UpLoad_Mfile")]
        public API_Response UpLoad_Mfile(int id, IFormFile file)
        {
            API_Response api = new API_Response();

            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\AllUserUpLoad\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //string fullfilename = string.Format(@"{0}\{1}{2}", path, DateTime.Now.ToString("yyyyMMddHHmmss"), file.FileName);//这个有用，一会还得删掉
            //using (var stream = System.IO.File.Create(fullfilename))
            //{
            //    file.CopyTo(stream); 

            //}

            Stream fs = file.OpenReadStream();
            byte[] byteArray = new byte[fs.Length];
            fs.Read(byteArray, 0, byteArray.Length);

            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                api.Data = client.UpLoad_Mfile(id, byteArray, file.FileName);
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion



            return api;
        }
        /// <summary>
        /// 从指定文件夹下载指定文件
        /// </summary>
        /// <param name="motherPathName">用户某个试验用的文件夹</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        [HttpGet("DownLoad_Mfile/{motherPathName}/{fileName}")]
        public API_Response DownLoad_Mfile(string motherPathName, string fileName)
        {
            API_Response api = new API_Response();
            #region MyRegion
            try
            {
                //Service_Main client = new Service_Main();
                api.Status = "API正常";
                api.ErrorMsg = "";
                byte[] tempResult = client.DownLoad_Mfile(motherPathName, fileName);
                int[] r = new int[tempResult.Length];//手动转成数组，不适用byte[]传递
                for (int i = 0; i < r.Length; i++)
                {
                    r[i] = tempResult[i];
                }
                api.Data = r;
            }
            catch (Exception ex)
            {
                api.Status = "API异常";
                api.ErrorMsg = ex.Message;
                api.Data = null;
            }
            #endregion

            return api;
        }

    }
    /// <summary>
    /// API返回结果
    /// </summary>
    public class API_Response
    {
        private string status;
        private string msg;
        private object data;
        /// <summary>
        /// API状态
        /// </summary>
        public string Status { get => status; set => status = value; }
        /// <summary>
        /// API错误信息
        /// </summary>
        public string ErrorMsg { get => msg; set => msg = value; }
        /// <summary>
        /// API返回的数据
        /// </summary>
        public object Data { get => data; set => data = value; }
    }
}
