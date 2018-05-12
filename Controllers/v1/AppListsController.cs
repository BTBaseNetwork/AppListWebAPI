using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AppListWebAPI.DAL;
using AppListWebAPI.Models;
using BahamutCommon.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppListWebAPI.Controllers.v1
{
    [Route("api/v1/[controller]")]
    public class AppListsController : Controller
    {
        private readonly BTBaseDbContext _context;

        public BTBaseDbContext DBContext { get { return _context; } }

        public AppListsController(BTBaseDbContext context)
        {
            _context = context;
        }

        class AppListRequestModel
        {
            public int platform { get; set; }
            public string deviceId { get; set; }
            public string uniqueId { get; set; }
            public long ts { get; set; }
            public string channel { get; set; }
            public string bundleId { get; set; }
            public string urlSchemes { get; set; }
            public string aesKey { get; set; }
            public string signature { get; set; }
            public bool TestSignature()
            {
                return BahamutCommon.Utils.SignatureUtil.TestStringParametersSignature(signature, platform.ToString(), deviceId, uniqueId, aesKey, ts.ToString());
            }
        }

        private static string AESEncryptPayload(object payloadModel, string secret)
        {
            var modelJson = Newtonsoft.Json.JsonConvert.SerializeObject(payloadModel);
            var payload = BahamutCommon.Encryption.AESHelper.AESEncrypt(modelJson, secret);
            return payload;
        }

        private static T RSADecryptPayload<T>(string payload, string rsaPriKey)
        {
            using (var rsap = new RSACryptoServiceProvider())
            {
                var cspBlob = Convert.FromBase64String(rsaPriKey);
                rsap.ImportCspBlob(cspBlob);

                var payloadB64Bytes = Convert.FromBase64String(payload);
                var modelJsonBytes = rsap.DecryptValue(payloadB64Bytes);
                var modelJson = System.Text.Encoding.UTF8.GetString(modelJsonBytes);
                var payloadModel = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(modelJson);
                return payloadModel;
            }
        }

        [HttpPost("{payload}")]
        public object GetAppList(string payload)
        {
            using (var rsap = new RSACryptoServiceProvider())
            {
                var model = RSADecryptPayload<AppListRequestModel>(payload, Startup.APIRequestPayloadRSAPrivateKey);

                if (string.IsNullOrEmpty(model.deviceId) || string.IsNullOrEmpty(model.uniqueId) || model.ts == 0)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return new ApiResult
                    {
                        code = Response.StatusCode,
                        msg = "Invalid Parameters"
                    };
                }

                if (!model.TestSignature())
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return new ApiResult
                    {
                        code = Response.StatusCode,
                        msg = "Invalid Signature"
                    };
                }

                if (string.IsNullOrEmpty(model.channel))
                {
                    model.channel = BTAppLaunchRecord.CHANNEL_UNKNOW;
                }

                var a = from u in DBContext.BTAppLaunchRecord where u.DeviceId == model.deviceId && u.Platform == model.platform && u.UniqueId == model.uniqueId select u;
                if (a.Any())
                {
                    var record = a.First();
                    record.Channel = model.channel;
                    record.BundleId = model.bundleId;
                    record.UrlSchemes = model.urlSchemes;
                    record.LaunchDateTs = DateTimeUtil.UnixTimeSpanSec;
                    DBContext.BTAppLaunchRecord.Update(record);
                }
                else
                {
                    DBContext.BTAppLaunchRecord.Add(new BTAppLaunchRecord
                    {
                        DeviceId = model.deviceId,
                        Platform = model.platform,
                        UniqueId = model.uniqueId,
                        Channel = model.channel,
                        BundleId = model.bundleId,
                        UrlSchemes = model.urlSchemes,
                        LaunchDateTs = DateTimeUtil.UnixTimeSpanSec
                    });
                }
                DBContext.SaveChanges();

                var dateTimeLimited = DateTimeUtil.UnixTimeSpanOfDateTime(DateTime.Now.AddDays(-30)).TotalSeconds;

                var resList = from u in DBContext.BTAppLaunchRecord
                              where u.DeviceId == model.deviceId && u.Platform == model.platform && u.LaunchDateTs > dateTimeLimited
                              select new
                              {
                                  UniqueId = u.UniqueId,
                                  Channel = u.Channel,
                                  BundleId = u.BundleId,
                                  UrlSchemes = u.UrlSchemes,
                                  LaunchDateTs = u.LaunchDateTs
                              };
                return new ApiResult
                {
                    code = 200,
                    msg = "Success",
                    content = AESEncryptPayload(new
                    {
                        DeviceId = model.deviceId,
                        Platform = model.platform,
                        AppList = resList
                    }, model.aesKey)
                };
            }
        }
    }
}