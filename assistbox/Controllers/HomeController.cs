using assistbox.Models;
using assistbox.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // Make sure this is present
using System; // For DateTime

namespace assistbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly SkincareChatbotService _chatbotService;

        public HomeController(SkincareChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        public IActionResult Index()
        {
            var chatHistoryJson = HttpContext.Session.GetString("ChatHistory");
            List<ChatMessage> messages = string.IsNullOrEmpty(chatHistoryJson)
                ? new List<ChatMessage>()
                : JsonSerializer.Deserialize<List<ChatMessage>>(chatHistoryJson);

            var currentState = HttpContext.Session.GetString("ChatState") ?? "AwaitingUserName";
            var userName = HttpContext.Session.GetString("UserName");

            // Retrieve the pending bot response for the typing effect from TempData
            string botResponsePending = TempData["BotResponsePending"] as string;

            // SPECIAL CASE: Initial load with no history and no pending response from a POST
            if (currentState == "AwaitingUserName" && !messages.Any() && string.IsNullOrEmpty(botResponsePending))
            {
                // This is the very first visit. Prompt for name.
                botResponsePending = "?? Welcome to Glazed Skincare! We're here to help you achieve your healthiest, most radiant skin. What's your name?";
            }
            // SPECIAL CASE: User just entered their name, and we're moving to AwaitingMenuChoice.
            // The initial greeting for the menu needs to be set for typing.
            else if (currentState == "AwaitingMenuChoice" && messages.Count == 0 && !string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(botResponsePending))
            {
                // This scenario happens after the user submits their name.
                // The botResponse for the greeting was calculated in SendMessage and put in TempData.
                // It should have been retrieved above. If it wasn't, this indicates a flow issue.
                // For safety, let's ensure it's here, but the main logic is in SendMessage.
                // This else if might be redundant if SendMessage always correctly sets TempData.
                // We'll rely on SendMessage to correctly populate TempData.
            }


            var viewModel = new ChatViewModel
            {
                Messages = messages,
                MenuOptions = _chatbotService.GetMainMenuOptions(),
                CurrentState = currentState,
                UserName = userName,
                BotResponsePending = botResponsePending 
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SendMessage(string userInput, string currentState)
        {
            var chatHistoryJson = HttpContext.Session.GetString("ChatHistory");
            List<ChatMessage> messages = string.IsNullOrEmpty(chatHistoryJson)
                ? new List<ChatMessage>()
                : JsonSerializer.Deserialize<List<ChatMessage>>(chatHistoryJson);

            var userName = HttpContext.Session.GetString("UserName");
            string botResponseForNextDisplay; 
            string nextState;

            string previouslyPendingBotResponse = TempData["BotResponsePending"] as string;
            if (!string.IsNullOrEmpty(previouslyPendingBotResponse))
            {
                messages.Add(new ChatMessage { Sender = "Bot", Message = previouslyPendingBotResponse });
            }
           
            if (!string.IsNullOrWhiteSpace(userInput) && currentState != "AwaitingUserName")
            {
                messages.Add(new ChatMessage { Sender = "User", Message = userInput });
            }
            else if (currentState == "AwaitingUserName" && !string.IsNullOrWhiteSpace(userInput))
            {
                messages.Add(new ChatMessage { Sender = "User", Message = userInput });
            }


            if (currentState == "AwaitingUserName")
            {
                userName = userInput.Trim();
                HttpContext.Session.SetString("UserName", userName);
                botResponseForNextDisplay = _chatbotService.GetInitialBotResponse(userName);
                nextState = "AwaitingMenuChoice";
            }
            else
            {
                string storedSkinType = null;
                if (currentState == "AwaitingConcerns")
                {
                    storedSkinType = HttpContext.Session.GetString("SkinType");
                }
                (botResponseForNextDisplay, nextState) = _chatbotService.ProcessUserInput(userInput, currentState, storedSkinType);

                if (currentState == "AwaitingSkinType" && nextState == "AwaitingConcerns")
                {
                    HttpContext.Session.SetString("SkinType", userInput.Trim().ToLower());
                }
                else if (nextState != "AwaitingConcerns" && nextState != "AwaitingSkinType") 
                {
                    HttpContext.Session.Remove("SkinType");
                }
            }

            currentState = nextState; 

            HttpContext.Session.SetString("ChatHistory", JsonSerializer.Serialize(messages));
            HttpContext.Session.SetString("ChatState", currentState);

            TempData["BotResponsePending"] = botResponseForNextDisplay;


            if (currentState == "Exit")
            {
                HttpContext.Session.Clear(); 
                TempData.Clear(); 
                return RedirectToAction("Index"); 
            }

            return RedirectToAction("Index"); 
        }
    }
}