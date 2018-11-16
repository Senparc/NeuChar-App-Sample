using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.Request;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeuCharAppSample.MessageHandlers
{
    public class AppMessageHandler : MessageHandler<AppMessageContext>
    {
        public AppMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0, DeveloperInfo developerInfo = null)
            : base(inputStream, postModel, maxRecordCount, developerInfo)
        {
        }

        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();

            var requestHandler =
              requestMessage.StartHandler()
              .Keyword("APP测试", () =>
              {
                  //进入关键词（必须），内容及消息类型可自定义
                  responseMessage.Content = @"欢迎使用【APP测试】！
您现在可以发送不同内容进行测试：
1、输入数字计算平方
2、输入下列任一关键词返回当前系统时间：time、t、now
3、上传图片消息返回相同的图片

输入【退出】退出应用状态。";//也可以创建任意类型
                  return responseMessage;
              })
              .Keyword("退出", () =>
              {
                  //退出管家此（必须），内容及消息类型可自定义
                  responseMessage.Content = "感谢您使用【APP测试】";
                  return responseMessage;
              })
              .Regex(@"\d+", () =>
              {
                  //其他任意逻辑
                  responseMessage.Content = "您输入了数字：{0}\r\n {0} 的平方是：{1}".FormatWith(requestMessage.Content, int.Parse(requestMessage.Content) ^ 2);
                  return responseMessage;
              })
              .Keywords(new[] { "time", "t", "now" }, () =>
              {
                  responseMessage.Content = "当前时间：" + SystemTime.Now.ToString();
                  return responseMessage;
              })
              .Default(() =>
              {
                  //之前所有条件未命中的默认消息
                  var responseMessageNews = requestMessage.CreateResponseMessage<ResponseMessageNews>();
                  responseMessageNews.Articles.Add(new Article()
                  {
                      Title = "输入错误！",
                      Description = "请输入正确的文字！",
                      PicUrl= "https://sdk.weixin.senparc.com/images/v2/logo .png",//你没看错，图片文件名就是有一个空格
                      Url = "https://github.com/Senparc/NeuChar-App-Sample"
                  });
                  return responseMessageNews;
              });

            return requestHandler.GetResponseMessage();
        }

        public override IResponseMessageBase OnImageRequest(RequestMessageImage requestMessage)
        {
            var responseMessageImage = requestMessage.CreateResponseMessage<ResponseMessageImage>();
            responseMessageImage.Image.MediaId = requestMessage.MediaId;
            return responseMessageImage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "无法识别您的请求，请输入任意类型的文字（或数字）！";
            return responseMessage;
        }
    }
}
