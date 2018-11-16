using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs.User;
using Senparc.Weixin.MP.AppStore;
using System.Collections.Generic;
using System.Text;

namespace NeuCharAppSample.Controllers
{
    /// <summary>
    /// OAuth 授权
    /// </summary>
    public class OAuthController : Controller
    {
        /// <summary>
        /// 应用AppCode
        /// </summary>
        private const string APPCODE = "Jk8GZ13Jqe/62rs7sdWpvQ$$";
        /// <summary>
        /// 开发者Key
        /// </summary>
        private const string APPKEY = "e19f6ac6ed2a435096480dac7914b056";
        /// <summary>
        /// 开发者Secret
        /// </summary>
        private const string APPSECRET = "b88258e84e9e4b4b96402cc43e1eb2be";

        /// <summary>
        /// Neuchar App 网页授权域名
        ///查看地址： https://neuchar.senparc.com/Developer/App/OAuth/${id}
        /// </summary>
        private const string OAUTHDOMAIN = "http://localhost:8773/";

        public ActionResult Index()
        {
            var redirectUrl = $"{OAUTHDOMAIN}/OAuth/Callback";
            var url = $"https://neuchar.senparc.com/app/weixinOAuth/Authorize?appCode={APPCODE}&redirectUrl={redirectUrl}";
            return Redirect(url);
        }
        //获取Code

        public ActionResult Callback(string code)
        {
            //获取P2pCode
            var p2pCodeUrl = $"https://neuchar.senparc.com/api/GetPassport?appKey={APPKEY}&secret={APPSECRET}";
            var messageResult = Senparc.CO2NET.HttpUtility.Post.PostGetJson<PassportResult>(p2pCodeUrl, formData: new Dictionary<string, string>(), encoding: Encoding.UTF8);

            var userinfoUrl =
                $"https://neuchar.senparc.com/api/GetMember?code={code}&accessToken={messageResult.Data.Token}";
            var result = Senparc.CO2NET.HttpUtility.Post.PostGetJson<UserInfoJson>(userinfoUrl, null, new Dictionary<string, string>(), Encoding.UTF8);
            return View(result);
        }
    }
}
