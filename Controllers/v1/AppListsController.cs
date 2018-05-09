using System;
using System.Collections.Generic;
using System.Linq;
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

        bool IsValidSignature(int platform, string deviceId, string uniqueId, long ts)
        {
            if (Request.Headers.ContainsKey("signature") && Request.Headers.ContainsKey("signcode"))
            {
                var signature = Request.Headers["signature"];
                var signcodeKey = Request.Headers["signcode"];
                if (Startup.APISigncodesDict.ContainsKey(signcodeKey))
                {
                    var signcode = Startup.APISigncodesDict[signcodeKey];
                    return SignatureUtil.TestStringParametersSignature(signature, signcode, platform.ToString(), deviceId, uniqueId, ts.ToString());
                }
            }
            return false;
        }

        [HttpPost("{platform}/{deviceId}/{uniqueId}/{ts}")]
        public object GetAppList(int platform, string deviceId, string uniqueId, long ts, string channel = "", string bundleId = "", string urlSchemes = "")
        {
            if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(uniqueId))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return new ApiResult
                {
                    code = Response.StatusCode,
                    msg = "Invalid Parameters"
                };
            }

            if (!IsValidSignature(platform, deviceId, uniqueId, ts))
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return new ApiResult
                {
                    code = Response.StatusCode,
                    msg = "Invalid Signature"
                };
            }

            if (string.IsNullOrEmpty(channel))
            {
                channel = BTAppLaunchRecord.CHANNEL_UNKNOW;
            }

            var a = from u in DBContext.BTAppLaunchRecord where u.DeviceId == deviceId && u.Platform == platform && u.UniqueId == uniqueId select u;
            if (a.Any())
            {
                var record = a.First();
                record.Channel = channel;
                record.BundleId = bundleId;
                record.UrlSchemes = urlSchemes;
                record.LaunchDateTs = DateTimeUtil.UnixTimeSpanSec;
                DBContext.BTAppLaunchRecord.Update(record);
            }
            else
            {
                DBContext.BTAppLaunchRecord.Add(new BTAppLaunchRecord
                {
                    DeviceId = deviceId,
                    Platform = platform,
                    UniqueId = uniqueId,
                    Channel = channel,
                    BundleId = bundleId,
                    UrlSchemes = urlSchemes,
                    LaunchDateTs = DateTimeUtil.UnixTimeSpanSec
                });
            }
            DBContext.SaveChanges();

            var dateTimeLimited = DateTimeUtil.UnixTimeSpanOfDateTime(DateTime.Now.AddDays(-30)).TotalSeconds;

            var resList = from u in DBContext.BTAppLaunchRecord
                          where u.DeviceId == deviceId && u.Platform == platform && u.LaunchDateTs > dateTimeLimited
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
                content = new
                {
                    DeviceId = deviceId,
                    Platform = platform,
                    AppList = resList
                }
            };
        }
    }
}