// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using DaneVAssistant.Responses.Cancel;

namespace DaneVAssistant.Dialogs
{
    public class CancelDialog : ComponentDialog
    {
        private static CancelResponses _responder = new CancelResponses();

        public CancelDialog()
            : base(nameof(CancelDialog))
        {
            InitialDialogId = nameof(CancelDialog);

            var cancel = new WaterfallStep[]
            {
           
                    FinishCancelDialog,
            };

            AddDialog(new WaterfallDialog(InitialDialogId, cancel));
            AddDialog(new ConfirmPrompt(DialogIds.CancelPrompt));
        }

        protected override async Task<DialogTurnResult> EndComponentAsync(DialogContext outerDc, object result, CancellationToken cancellationToken)
        {
            
                // If user chose to cancel
                await _responder.ReplyWith(outerDc.Context, CancelResponses.ResponseIds.CancelConfirmedMessage);

                // Cancel all in outer stack of component i.e. the stack the component belongs to
                return await outerDc.CancelAllDialogsAsync();
          
        }


        private async Task<DialogTurnResult> FinishCancelDialog(WaterfallStepContext sc, CancellationToken cancellationToken)
        {
            return await sc.EndDialogAsync(null, cancellationToken);
        }

        private class DialogIds
        {
            public const string CancelPrompt = "cancelPrompt";
        }
    }
}
