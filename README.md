# Glazed Skincare Chatbot 💬✨

A smart and friendly chatbot built with **C# ASP.NET MVC** to assist customers of **Glazed Skincare** with product recommendations, skincare tips, order support, and general queries.

---

## 🧴 Project Overview

The Glazed Skincare Chatbot is designed to enhance user interaction on the Glazed Skincare website by providing real-time support and guidance. It simulates a human conversation and is capable of answering FAQs, guiding users to suitable products based on their skin types, and providing order-related assistance.

---

## 🔧 Built With

- **C#**
- **ASP.NET MVC (.NET 8)**
- **Entity Framework Core**
- **Razor Views**

---

## ✨ Features

- 💬 Real-time chat interface
- 🧠 Smart response handling using keyword and intent recognition
- 🧴 Product recommendation engine (basic rule-based logic)
- 📦 Order support and tracking
---

## 🖥️ Project Structure

/assistbox

│

├── Controllers

│ └── HomeController.cs

│

├── Models

│ ├── ChatMessage.cs

│ └── ChatViewModel.cs

│ └── ErrorViewModel.cs

│ └── SkincareProducts.cs



│

├── Views

│ ├── Home.cshtml

│ └── Index.cshtml

│

├── Shared

│ ├── Layout.cshtml

│ └── _ValidationScriptsPartial.cshtml

│ └──Error.cshtml

│

├── wwwroot

│ └── css, js, images

│

├── Services

│ └── SkincareChatBotService.cs

│

├── appsettings.json

└── Program.cs 



---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- Visual Studio 2022+

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/debugdawn/assitbox.git
   cd assistbox

