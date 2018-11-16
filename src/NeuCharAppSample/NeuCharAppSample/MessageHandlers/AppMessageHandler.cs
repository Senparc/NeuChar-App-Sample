using Senparc.NeuChar.Entities;
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

            responseMessage.Content = $"您刚才输入了文字：{requestMessage}";

            return responseMessage;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "无法识别您的请求，请输入任意类型的文字（或数字）！";
            return responseMessage;
        }
    }
}
