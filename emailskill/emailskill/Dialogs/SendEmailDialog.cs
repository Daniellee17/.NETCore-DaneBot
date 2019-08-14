using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DaneVAssistant.Models;
using EmailSkill.Models;
using EmailSkill.Prompts;
using EmailSkill.Responses.SendEmail;
using EmailSkill.Responses.Shared;
using EmailSkill.Services;
using EmailSkill.Services.MSGraphAPI;
using EmailSkill.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Builder.Solutions.Responses;
using Microsoft.Bot.Builder.Solutions.Util;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Graph;

namespace EmailSkill.Dialogs
{
    public class SendEmailDialog : EmailSkillDialogBase
    {
        public string _platform;
        public string _server;
        public string _description;
        private readonly IGraphServiceClient _graphClient;
        private readonly TimeZoneInfo _timeZoneInfo;


        public SendEmailDialog(
            BotSettings settings,
            BotServices services,
            ResponseManager responseManager,
            ConversationState conversationState,
            FindContactDialog findContactDialog,
            IServiceManager serviceManager,
            IBotTelemetryClient telemetryClient,
            MicrosoftAppCredentials appCredentials)
            : base(nameof(SendEmailDialog), settings, services, responseManager, conversationState, serviceManager, telemetryClient, appCredentials)
        {
            TelemetryClient = telemetryClient;

            var sendEmail = new WaterfallStep[]
            {
                IfClearContextStep,
                GetAuthToken,
                AfterGetAuthToken,
                CollectRecipient,
                GetMyNameAsync,
                CollectPlatform,
                CollectServer,
                CollectDescription,
                CollectText,
                SendEmail,
            };

            var collectRecipients = new WaterfallStep[]
            {
                PromptRecipientCollection,
                GetRecipients,
            };


            var getRecreateInfo = new WaterfallStep[]
            {
                GetRecreateInfo,
                AfterGetRecreateInfo,
            };

            // Define the conversation flow using a waterfall model.
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(Actions.Send, sendEmail) { TelemetryClient = telemetryClient });
            AddDialog(new WaterfallDialog(Actions.CollectRecipient, collectRecipients) { TelemetryClient = telemetryClient });
            AddDialog(new FindContactDialog(settings, services, responseManager, conversationState, serviceManager, telemetryClient));
            AddDialog(new WaterfallDialog(Actions.GetRecreateInfo, getRecreateInfo) { TelemetryClient = telemetryClient });
            AddDialog(new GetRecreateInfoPrompt(Actions.GetRecreateInfoPrompt));
            InitialDialogId = Actions.Send;
        }
        private async Task<DialogTurnResult> CollectPlatform(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {

            var attachments = new List<Microsoft.Bot.Schema.Attachment>();

            // Reply to the activity we received with an activity.
            var reply = MessageFactory.Attachment(attachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments.Add(Cards.GetSQLHeroCard().ToAttachment());
            reply.Attachments.Add(Cards.GetOracleHeroCard().ToAttachment());
            reply.Attachments.Add(Cards.GetPowerBIHeroCard().ToAttachment());
            reply.Attachments.Add(Cards.GetGeneralHeroCard().ToAttachment());



            var promptOptions = new PromptOptions { Prompt = MessageFactory.Text("Ok, now for the technical questions, what platform are you using? You can select from the choices or type in your answer.") };
            await sc.Context.SendActivityAsync(reply, cancellationToken);
            return await sc.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);


        }

        private async Task<DialogTurnResult> CollectServer(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await EmailStateAccessor.GetAsync(sc.Context);




            state._platform = (string)sc.Result;

            
            return await sc.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("How about the server / database?") }, cancellationToken);


        }

        private async Task<DialogTurnResult> CollectDescription(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
          
            var state = await EmailStateAccessor.GetAsync(sc.Context);
            state._server = (string)sc.Result;
         

            return await sc.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Can you describe your problem briefly?") }, cancellationToken);


        }

        public async Task<DialogTurnResult> CollectText(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {

        
           
            var state = await EmailStateAccessor.GetAsync(sc.Context);

            state._description = (string)sc.Result;
            state.Subject = "Attention DMSS team";

            HttpClient client1 = new HttpClient();
            string textFromFile = await client1.GetStringAsync("https://aaa3nvq67c.blob.core.windows.net/dane/EmailV2.html");
            // string qBody = System.IO.File.ReadAllText("https://mystorage891032.blob.core.windows.net/daneblob/htmlpage.html");
            string pattern = @"\bCustomerName\b";
            //string replace = state.UserInfo.Name;
     
            string pattern2 = @"\bCustomerPlatform\b";
            string replace2 = state._platform;
            string result2 = Regex.Replace(textFromFile, pattern2, replace2);

            string pattern3 = @"\bCustomerServer\b";
            string replace3 = state._server;
            string result3 = Regex.Replace(result2, pattern3, replace3);

            string pattern4 = @"\bCustomerDescription\b";
            string replace4 = state._description;
            string result4 = Regex.Replace(result3, pattern4, replace4);


          
            state.Content = result4;



            return await sc.NextAsync();


        }



        public async Task<DialogTurnResult> SendEmail(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {


                var state = await EmailStateAccessor.GetAsync(sc.Context);
                var token = state.Token;

                var service = ServiceManager.InitMailService(token, state.GetUserTimeZone(), state.MailSourceType);

                // send user message.
                var subject = state.Subject.Equals(EmailCommonStrings.EmptySubject) ? string.Empty : state.Subject;
                var content = state.Content.Equals(EmailCommonStrings.EmptyContent) ? string.Empty : state.Content;
                await service.SendMessageAsync(content, subject, state.FindContactInfor.Contacts);

                var replyMessage = MessageFactory.Text("Thank you! An email has been sent to the team.");

                await sc.Context.SendActivityAsync(replyMessage);

            }
            catch (SkillException ex)
            {
                await HandleDialogExceptions(sc, ex);

                return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
            }
            catch (Exception ex)
            {
                await HandleDialogExceptions(sc, ex);

                return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
            }

            await ClearConversationState(sc);
            return await sc.EndDialogAsync(true);
        }

        public async Task<DialogTurnResult> GetRecreateInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return await sc.PromptAsync(Actions.GetRecreateInfoPrompt, new PromptOptions
                {
                    Prompt = ResponseManager.GetResponse(SendEmailResponses.GetRecreateInfo),
                    RetryPrompt = ResponseManager.GetResponse(SendEmailResponses.GetRecreateInfoRetry)
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleDialogExceptions(sc, ex);
                return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
            }
        }

        public async Task<DialogTurnResult> AfterGetRecreateInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await EmailStateAccessor.GetAsync(sc.Context, cancellationToken: cancellationToken);
                var skillOptions = (EmailSkillDialogOptions)sc.Options;
                skillOptions.SubFlowMode = true;
                if (sc.Result != null)
                {
                    var recreateState = sc.Result as ResendEmailState?;
                    switch (recreateState.Value)
                    {
                        case ResendEmailState.Cancel:
                            await sc.Context.SendActivityAsync(ResponseManager.GetResponse(EmailSharedResponses.CancellingMessage));
                            await ClearConversationState(sc);
                            return await sc.EndDialogAsync(false, cancellationToken);
                        case ResendEmailState.Participants:
                            state.ClearParticipants();
                            return await sc.ReplaceDialogAsync(Actions.Send, options: skillOptions, cancellationToken: cancellationToken);
                        case ResendEmailState.Subject:
                            state.ClearSubject();
                            return await sc.ReplaceDialogAsync(Actions.Send, options: skillOptions, cancellationToken: cancellationToken);
                        case ResendEmailState.Content:
                            state.ClearContent();
                            return await sc.ReplaceDialogAsync(Actions.Send, options: skillOptions, cancellationToken: cancellationToken);
                        default:
                            // should not go to this part. place an error handling for save.
                            await HandleDialogExceptions(sc, new Exception("Get unexpect state in recreate."));
                            return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
                    }
                }
                else
                {
                    // should not go to this part. place an error handling for save.
                    await HandleDialogExceptions(sc, new Exception("Get unexpect result in recreate."));
                    return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
                }
            }
            catch (Exception ex)
            {
                await HandleDialogExceptions(sc, ex);
                return new DialogTurnResult(DialogTurnStatus.Cancelled, CommonUtil.DialogTurnResultCancelAllDialogs);
            }
        }
    }
}