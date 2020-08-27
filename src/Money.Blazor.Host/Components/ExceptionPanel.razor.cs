using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Services;
using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExceptionPanel
    {
        public static IReadOnlyCollection<Type> SkippedExceptions { get; } = new[] { typeof(UnauthorizedAccessException), typeof(ServerNotRespondingException) };

        public Exception LastException { get; private set; }

        [Inject]
        protected MessageBuilder MessageBuilder { get; set; }

        [Inject]
        protected ExceptionHandlerBuilder ExceptionHandlerBuilder { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public AlertMode AlertMode { get; set; } = AlertMode.Error;

        protected override void OnInitialized()
        {
            ExceptionHandlerBuilder.Handler(e =>
            {
                bool isSkipped = false;
                Type exceptionType = e.GetType();
                foreach (Type type in SkippedExceptions)
                {
                    if (type.IsAssignableFrom(exceptionType))
                    {
                        isSkipped = true;
                        break;
                    }
                }

                if (!isSkipped)
                {
                    LastException = e;

                    if (e is Neptuo.Models.AggregateRootException)
                    {
                        string message = null;

                        if (e is CurrencyAlreadyAsDefaultException)
                            message = MessageBuilder.CurrencyAlreadyAsDefault();
                        else if (e is CurrencyAlreadyExistsException)
                            message = MessageBuilder.CurrencyAlreadyExists();
                        else if (e is CurrencyDoesNotExistException)
                            message = MessageBuilder.CurrencyDoesNotExist();
                        else if (e is CurrencyExchangeRateDoesNotExistException)
                            message = MessageBuilder.CurrencyExchangeRateDoesNotExist();
                        else if (e is OutcomeAlreadyDeletedException)
                            message = MessageBuilder.OutcomeAlreadyDeleted();
                        else if (e is OutcomeAlreadyHasCategoryException)
                            message = MessageBuilder.OutcomeAlreadyHasCategory();
                        else if (e is CantDeleteDefaultCurrencyException)
                            message = MessageBuilder.CantDeleteDefaultCurrency();
                        else if (e is CantDeleteLastCurrencyException)
                            message = MessageBuilder.CantDeleteLastCurrency();
                        else if (e is DemoUserCantBeChangedException)
                            message = MessageBuilder.DemoUserCantBeChanged();
                        else if (e is PasswordChangeFailedException passwordChangeFailed)
                            message = MessageBuilder.PasswordChangeFailed(passwordChangeFailed.ErrorDescription);
                        else if (e is EmailChangeFailedException)
                            message = MessageBuilder.EmailChangeFailed();

                        Message = message;
                    }
                    else if (e is InternalServerException)
                    {
                        Message = MessageBuilder.InternalServerError();
                    }
                }

                if (isSkipped)
                {
                    Title = null;
                    Message = null;
                    LastException = null;
                }
                else if (Message == null)
                {
                    Title = LastException.GetType().FullName;
                    Message = LastException.Message;
                }
                else
                {
                    Title = null;
                }

                StateHasChanged();
            });

            base.OnInitialized();
        }
    }
}
