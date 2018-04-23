using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;


namespace BotMajor.Dialogs
{

    [Serializable]
    public class CarDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Welcome to the Hotels finder!");
            if (Querys.MoreOneCar(RootDialog.chatId))
            {
                ShowCarsOptions(context);
            }
            else
            {
                //var hotelsFormDialog = FormDialog.FromForm(this.BuildCarForm, FormOptions.PromptInStart);
                await GetCarInfo(context);
                //context.Call(SelectCarFrom, this.ResumeAfterCarFormDialog);
            }

        }
        private async Task GetCarInfo(IDialogContext context)
        {
            string carNumber = StringEngine.ExtractFromBrackets(Querys.GetCarByChatId(RootDialog.chatId));
            //await context.PostAsync(optionSelected);
            await context.PostAsync("Дата ТО - " + Convert.ToString(Querys.GetTIDate(carNumber)));
            context.Done<object>(null);
        }
        private async Task OnCarOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {

                string optionSelected = await result; // одна из машин
                string carNumber = StringEngine.ExtractFromBrackets(optionSelected);
                await context.PostAsync("Дата ТО - " + Convert.ToString(Querys.GetTIDate(carNumber)));
                context.Done<object>(null);

            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Упс! Слишком много попыток :( Но не волнуйтесь, я обрабатываю это исключение, и вы можете попробовать еще раз!" + ex);

                //context.Wait(this.MessageReceivedAsync);
            }
        }
        private void ShowCarsOptions(IDialogContext context)
        {
            //string[] cars = GetCarsAsync().ToString;
            //GetCarsAsync();
            //PromptDialog.Choice(context, this.OnCarOptionSelected, new List<string>() {cars.Select(car => car.ToString()) }, "Вы в главном меню. Что вы хотите", "Вариант не найден. Пожалуйста, попробуйте еще раз", 3);
            var carsList = Querys.GetCarsByChatID(RootDialog.chatId);
            PromptDialog.Choice(context, this.OnCarOptionSelected, carsList.Select(car => car.ToString()).ToList(), "Выбирете автомобиль", "Вариант не найден. Пожалуйста, попробуйте еще раз", 3);
        }

        private async Task ResumeAfterCarOptionDialog(IDialogContext context, IAwaitable<CarInfoQuery> result)
        {
            try
            {
                var searchQuery = await result;



                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();



                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the HotelsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task ResumeAfterCarFormDialog(IDialogContext context, IAwaitable<CarInfoQuery> result)
        {
            try
            {
                var searchQuery = await result;



                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                await context.PostAsync(resultMessage);
            }
            catch (FormCanceledException ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "You have canceled the operation. Quitting from the HotelsDialog";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

    }
}
