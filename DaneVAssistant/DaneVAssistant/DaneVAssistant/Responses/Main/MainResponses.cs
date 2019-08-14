// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TemplateManager;
using Microsoft.Bot.Schema;

namespace DaneVAssistant.Responses.Main
{
    public class MainResponses : TemplateManager
    {
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                {
                    ResponseIds.Cancelled,
                    (context, data) =>
                    MessageFactory.Text(
                        text: MainStrings.CANCELLED,
                        ssml: MainStrings.CANCELLED,
                        inputHint: InputHints.AcceptingInput)
                },
                {
                    ResponseIds.Completed,
                    (context, data) =>
                    MessageFactory.Text(
                        text: MainStrings.COMPLETED,
                        ssml: MainStrings.COMPLETED,

                        inputHint: InputHints.AcceptingInput)
                },
                {
                    ResponseIds.Confused,
                    (context, data) =>
                    MessageFactory.Text(
                        text: MainStrings.CONFUSED,
                        ssml: MainStrings.CONFUSED,
                        inputHint: InputHints.AcceptingInput)
                },
                {
                    ResponseIds.Greeting,
                    (context, data) =>
                    MessageFactory.Text(
                        text: MainStrings.GREETING,
                        ssml: MainStrings.GREETING,
                        inputHint: InputHints.AcceptingInput)
                },
                { ResponseIds.Help, (context, data) => BuildHelpCard(context, data) },
                { ResponseIds.NewUserGreeting, (context, data) => BuildNewUserGreetingCard(context, data) },
                { ResponseIds.MainMenuGreeting, (context, data) => BuildMainMenuGreetingCard(context, data) },
                { ResponseIds.ReturningUserGreeting, (context, data) => BuildReturningUserGreetingCard(context, data) },
            }
        };

        public MainResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }

        public static IMessageActivity BuildNewUserGreetingCard(ITurnContext turnContext, dynamic data)
        {
            var introCard = File.ReadAllText(MainStrings.INTRO_PATH);
            var card = AdaptiveCard.FromJson(introCard).Card;
            var attachment = new Attachment(AdaptiveCard.ContentType, content: card);

            var response = MessageFactory.Attachment(attachment, ssml: card.Speak, inputHint: InputHints.AcceptingInput);

            response.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_1, value: MainStrings.HELP_BTN_VALUE_1),
                                       new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_0, value: MainStrings.HELP_BTN_VALUE_0),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_2, value: MainStrings.HELP_BTN_VALUE_2),
                   // new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_3, value: MainStrings.HELP_BTN_VALUE_3),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_4, value: MainStrings.HELP_BTN_VALUE_4),
              },
            };

            return response;
        }

        public static IMessageActivity BuildMainMenuGreetingCard(ITurnContext turnContext, dynamic data)
        {
            var introCard = File.ReadAllText(MainStrings.MENU_PATH);
            var card = AdaptiveCard.FromJson(introCard).Card;
            var attachment = new Attachment(AdaptiveCard.ContentType, content: card);

            var response = MessageFactory.Attachment(attachment, ssml: card.Speak, inputHint: InputHints.AcceptingInput);

            response.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_1, value: MainStrings.HELP_BTN_VALUE_1),
                                                   new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_0, value: MainStrings.HELP_BTN_VALUE_0),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_2, value: MainStrings.HELP_BTN_VALUE_2),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_3, value: MainStrings.HELP_BTN_VALUE_3),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_4, value: MainStrings.HELP_BTN_VALUE_4),
                },
            };

            return response;
        }

        public static IMessageActivity BuildReturningUserGreetingCard(ITurnContext turnContext, dynamic data)
        {
            var introCard = File.ReadAllText(MainStrings.INTRO_RETURNING);
            var card = AdaptiveCard.FromJson(introCard).Card;
            var attachment = new Attachment(AdaptiveCard.ContentType, content: card);

            var response = MessageFactory.Attachment(attachment, ssml: card.Speak, inputHint: InputHints.AcceptingInput);

            response.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_1, value: MainStrings.HELP_BTN_VALUE_1),
                                                   new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_0, value: MainStrings.HELP_BTN_VALUE_0),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_2, value: MainStrings.HELP_BTN_VALUE_2),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_3, value: MainStrings.HELP_BTN_VALUE_3),
                                        new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_4, value: MainStrings.HELP_BTN_VALUE_4),
                },
            };

            return response;
        }

        public static IMessageActivity BuildHelpCard(ITurnContext turnContext, dynamic data)
        {
            var attachment = new HeroCard()
            {
                Title = MainStrings.HELP_TITLE,
                Text = MainStrings.HELP_TEXT,
            }.ToAttachment();

            var response = MessageFactory.Attachment(attachment, ssml: MainStrings.HELP_TEXT, inputHint: InputHints.AcceptingInput);

            response.SuggestedActions = new SuggestedActions
            {
                Actions = new List<CardAction>()
                {
                     new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_5, value: MainStrings.HELP_BTN_5),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_6, value: MainStrings.HELP_BTN_6),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_7, value: MainStrings.HELP_BTN_7),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_8, value: MainStrings.HELP_BTN_8),

                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_1, value: MainStrings.HELP_BTN_VALUE_1),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_0, value: MainStrings.HELP_BTN_VALUE_0),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_2, value: MainStrings.HELP_BTN_VALUE_2),
                    new CardAction(type: ActionTypes.ImBack, title: MainStrings.HELP_BTN_TEXT_4, value: MainStrings.HELP_BTN_VALUE_4),
                    },
            };

            return response;
        }

        public class ResponseIds
        {
            // Constants
            public const string Cancelled = "cancelled";
            public const string Completed = "completed";
            public const string Confused = "confused";
            public const string Greeting = "greeting";
            public const string Help = "help";
            public const string NewUserGreeting = "newUser";
            public const string MainMenuGreeting = "mainMenu";
            public const string ReturningUserGreeting = "returningUser";
        }
    }
}
