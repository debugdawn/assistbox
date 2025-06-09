using assistbox.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System; 

namespace assistbox.Services
{
    public class SkincareChatbotService
    {
        private readonly Dictionary<string, List<SkincareProduct>> _skincareProducts;
        private readonly Dictionary<string, string> _faqAnswers;
        private readonly List<string> _validSkinTypes = new List<string> { "oily", "dry", "combination", "normal", "sensitive" };
        private readonly List<string> _validConcerns = new List<string> { "acne", "dryness", "fine lines", "dullness", "redness", "oil control", "enlarged pores", "hyperpigmentation" };


        public SkincareChatbotService()
        {
            _skincareProducts = new Dictionary<string, List<SkincareProduct>>(StringComparer.OrdinalIgnoreCase)
            {
                {"oily", new List<SkincareProduct>
                    {
                        new SkincareProduct { Name = "Glazed Clarifying Cleanser", Description = "A gentle cleanser that deeply purifies pores without stripping moisture.", Concerns = new List<string>{"acne", "oil control", "enlarged pores"}},
                        new SkincareProduct { Name = "Glazed Mattifying Serum", Description = "Controls excess oil and minimizes shine for a balanced complexion.", Concerns = new List<string>{"oil control", "enlarged pores"}},
                        new SkincareProduct { Name = "Glazed Oil-Free Moisturizer", Description = "Lightweight hydration that won't clog pores.", Concerns = new List<string>{"hydration", "oil control"}}
                    }
                },
                {"dry", new List<SkincareProduct>
                    {
                        new SkincareProduct { Name = "Glazed Hydrating Cream Cleanser", Description = "Nourishing cleanser that leaves skin soft and supple.", Concerns = new List<string>{"dryness", "hydration"}},
                        new SkincareProduct { Name = "Glazed Hyaluronic Acid Serum", Description = "Delivers intense, lasting hydration to thirsty skin.", Concerns = new List<string>{"dryness", "fine lines", "hydration"}},
                        new SkincareProduct { Name = "Glazed Rich Moisture Balm", Description = "Deeply moisturizes and restores the skin's barrier.", Concerns = new List<string>{"dryness", "redness", "hydration"}}
                    }
                },
                {"combination", new List<SkincareProduct>
                    {
                        new SkincareProduct { Name = "Glazed Balanced Purifying Wash", Description = "Effectively cleanses without over-drying or over-moisturizing.", Concerns = new List<string>{"oil control", "acne", "hydration"}},
                        new SkincareProduct { Name = "Glazed Niacinamide Serum", Description = "Helps balance oil production while improving skin tone.", Concerns = new List<string>{"oil control", "enlarged pores", "redness", "hyperpigmentation"}},
                        new SkincareProduct { Name = "Glazed Adaptive Hydrator", Description = "Provides hydration where you need it, balances oily zones.", Concerns = new List<string>{"hydration", "oil control"}}
                    }
                },
                {"normal", new List<SkincareProduct>
                    {
                        new SkincareProduct { Name = "Glazed Gentle Daily Cleanser", Description = "Maintains skin's natural balance while cleansing.", Concerns = new List<string>{"cleansing"}},
                        new SkincareProduct { Name = "Glazed Vitamin C Brightening Serum", Description = "Boosts radiance and protects against environmental damage.", Concerns = new List<string>{"dullness", "hyperpigmentation", "fine lines"}},
                        new SkincareProduct { Name = "Glazed Everyday Moisturizer", Description = "Light yet effective hydration for healthy-looking skin.", Concerns = new List<string>{"hydration"}}
                    }
                },
                {"sensitive", new List<SkincareProduct>
                    {
                        new SkincareProduct { Name = "Glazed Soothing Oat Cleanser", Description = "Calms and cleanses irritated skin gently.", Concerns = new List<string>{"redness", "sensitivity", "dryness"}},
                        new SkincareProduct { Name = "Glazed Barrier Repair Serum", Description = "Strengthens and protects sensitive skin.", Concerns = new List<string>{"redness", "sensitivity", "dryness"}},
                        new SkincareProduct { Name = "Glazed Calm & Restore Cream", Description = "Provides gentle, non-irritating hydration.", Concerns = new List<string>{"redness", "sensitivity", "dryness", "hydration"}}
                    }
                }
            };

            _faqAnswers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"animal testing", "No, Glazed Skincare is proudly cruelty-free and does not test on animals. 🐰"},
                {"shipping", "We offer standard and expedited shipping. Standard shipping usually takes 3-5 business days. 🚚"},
                {"returns", "We have a 30-day return policy for unused products. Please visit our website for full details. ↩️"},
                {"ingredients", "We prioritize high-quality, effective, and often natural ingredients. Specific ingredient lists are available on each product page on our website. 🌿"},
                {"skincare order", "The general order for skincare is: Cleanser, Toner, Serum, Moisturizer, SPF (in the morning). 🧴"},
                {"acne", "For acne, focus on gentle cleansing, non-comedogenic products, and ingredients like salicylic acid or benzoyl peroxide. Always consult a dermatologist for severe cases. ✨"},
                {"dry skin", "For dry skin, look for hydrating ingredients like hyaluronic acid, ceramides, and glycerin. Use rich moisturizers and avoid hot showers. 💧"},
                {"oily skin", "For oily skin, choose oil-free and non-comedogenic products. Ingredients like niacinamide can help regulate oil production. ⚖️"},
                {"hyperpigmentation", "To address hyperpigmentation, consider products with Vitamin C, Niacinamide, Alpha Arbutin, or mild exfoliants. Always use SPF daily! ☀️"}
            };
        }

        public string GetInitialBotResponse(string userName)
        {
            return $"Hii, {userName}! How can I assist you today? Select an option or type 'menu' at any time.";
        }

        public List<string> GetMainMenuOptions()
        {
            return new List<string>
            {
                "1. 🔬 Get Skincare Advice & Product Recommendations",
                "2. ❓ Ask a General Skincare Question / FAQ",
                "3. 📦 Check Order Status",
                "4. 🌟 Learn About Glazed Skincare",
                "5. 💬 Connect with a Skincare Expert (Live Chat)",
                "6. ❌ Exit"
            };
        }

        /// <summary>
        /// Handles user input based on the current conversation state.
        /// </summary>
        /// <param name="userInput">The user's message or choice.</param>
        /// <param name="currentState">The current state of the conversation.</param>
        /// <param name="storedSkinType">The skin type previously stored in the session, if applicable.</param>
        /// <returns>A tuple containing the bot's response and the next conversation state.</returns>
        public (string response, string nextState) ProcessUserInput(string userInput, string currentState, string storedSkinType = "")
        {
            userInput = userInput.Trim().ToLower();

            if (userInput == "menu" || userInput == "main menu" || userInput == "back")
            {
                return (GetInitialBotResponse(""), "AwaitingMenuChoice");
            }

            switch (currentState)
            {
                case "AwaitingMenuChoice":
                    return ProcessMainMenuChoice(userInput);
                case "AwaitingSkinType":
                    return ProcessSkinType(userInput);
                case "AwaitingConcerns":
                    if (string.IsNullOrEmpty(storedSkinType) || !_skincareProducts.ContainsKey(storedSkinType))
                    {
                        return ("It looks like we lost your skin type. Let's start the recommendations over. Please choose your skin type: Oily, Dry, Combination, Normal, or Sensitive.", "AwaitingSkinType");
                    }
                    return ProcessConcernsAndRecommendProducts(storedSkinType, userInput);
                case "AwaitingFAQQuestion":
                    return ProcessFAQQuestion(userInput);
                case "AwaitingOrderID":
                    return ProcessOrderID(userInput);
                default:
                    return ("I'm sorry, I'm a bit lost. Please choose from the menu again, or type 'menu' to see options.", "AwaitingMenuChoice");
            }
        }

        private (string response, string nextState) ProcessMainMenuChoice(string choice)
        {
            if (choice == "1" || choice.Contains("advice") || choice.Contains("recommendations"))
            {
                return ("🔬 Let's find the best routine for you! First, how would you describe your skin type? (Oily, Dry, Combination, Normal, Sensitive)", "AwaitingSkinType");
            }
            else if (choice == "2" || choice.Contains("faq") || choice.Contains("question"))
            {
                var faqTopics = string.Join(", ", _faqAnswers.Keys.Select(k => $"'{k}'"));
                return ($"❓ What would you like to know about skincare or Glazed Skincare? You can ask about topics like: {faqTopics}.", "AwaitingFAQQuestion");
            }
            else if (choice == "3" || choice.Contains("order status"))
            {
                return ("📦 You chose Order Status. Please enter your 7-digit order number:", "AwaitingOrderID");
            }
            else if (choice == "4" || choice.Contains("about glazed") || choice.Contains("learn about"))
            {
                return ("🌟 Welcome to Glazed Skincare! We believe in radiant skin through effective, gentle, and joyful routines. Our products are crafted with high-quality ingredients, designed to nourish and transform your complexion. We are proudly **cruelty-free** and committed to sustainable practices. Explore our range and discover your glow!", "AwaitingMenuChoice");
            }
            else if (choice == "5" || choice.Contains("live chat") || choice.Contains("expert"))
            {
                return ("💬 Connecting you with a Glazed Skincare Expert... Please note: This is a placeholder for a live chat feature. In a real application, you would be directed to a live agent or a contact form. For immediate assistance, you can email us at **support@glazedskincare.com** or call us at **+27 11 555 1234**.", "AwaitingMenuChoice");
            }
            else if (choice == "6" || choice.Contains("exit") || choice.Contains("bye"))
            {
                return ("👋 Thank you for chatting with Glazed Skincare! Have a radiant day!", "Exit");
            }
            else
            {
                return ("⚠️ Invalid choice. Please enter a number from the menu (1-6) or type 'menu' to see options again.", "AwaitingMenuChoice");
            }
        }

        private (string response, string nextState) ProcessSkinType(string skinTypeInput)
        {
            var skinType = skinTypeInput.Trim().ToLower();
            if (!_validSkinTypes.Contains(skinType))
            {
                return ("Hmm, I'm not familiar with that skin type. Please choose from **Oily**, **Dry**, **Combination**, **Normal**, or **Sensitive**.", "AwaitingSkinType");
            }

            var responseBuilder = new StringBuilder();
            responseBuilder.AppendLine($"Got it, your skin type is **{skinType.ToUpper()}**.");
            responseBuilder.AppendLine("What are your primary skin concerns? (e.g., **Acne**, **Dryness**, **Fine Lines**, **Dullness**, **Redness**, **Oil Control**, **Enlarged Pores**, **Hyperpigmentation**)");
            responseBuilder.AppendLine("You can list multiple concerns, separated by commas.");

            return (responseBuilder.ToString(), "AwaitingConcerns");
        }

        public (string response, string nextState) ProcessConcernsAndRecommendProducts(string skinType, string concernsInput)
        {
            var responseBuilder = new StringBuilder();
            var userConcerns = concernsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(c => c.Trim().ToLower())
                                            .ToList();

            var validUserConcerns = userConcerns.Where(c => _validConcerns.Contains(c)).ToList();

            if (!validUserConcerns.Any() && userConcerns.Any())
            {
                responseBuilder.AppendLine("I couldn't identify any of those concerns. Please try again with terms like 'acne', 'dryness', 'redness', etc. Or type 'menu' to go back.");
                return (responseBuilder.ToString(), "AwaitingConcerns"); 
            }
            else if (!userConcerns.Any())
            {
                responseBuilder.AppendLine("Please tell me your primary skin concerns, separated by commas. (e.g., Acne, Dryness, Fine Lines)");
                return (responseBuilder.ToString(), "AwaitingConcerns");
            }


            responseBuilder.AppendLine("---");
            responseBuilder.AppendLine("Here's some general advice based on your concerns:");

            if (validUserConcerns.Contains("acne"))
                responseBuilder.AppendLine("For **acne**: Focus on gentle cleansing, non-comedogenic products, and consider ingredients like salicylic acid. Avoid harsh scrubbing.");
            if (validUserConcerns.Contains("dryness"))
                responseBuilder.AppendLine("For **dryness**: Hydration is key! Look for products with hyaluronic acid and ceramides. Apply moisturizer to damp skin.");
            if (validUserConcerns.Contains("fine lines") || validUserConcerns.Contains("dullness"))
                responseBuilder.AppendLine("For **fine lines/dullness**: Antioxidants like Vitamin C and gentle exfoliation can help. Sun protection is crucial for prevention.");
            if (validUserConcerns.Contains("redness") || skinType.Equals("sensitive", StringComparison.OrdinalIgnoreCase))
                responseBuilder.AppendLine("For **redness/sensitivity**: Opt for fragrance-free, gentle formulas. Ingredients like ceramides and centella asiatica can be soothing.");
            if (validUserConcerns.Contains("oil control") || validUserConcerns.Contains("enlarged pores"))
                responseBuilder.AppendLine("For **oil control/enlarged pores**: Niacinamide and clay masks can be beneficial. Opt for oil-free and non-comedogenic products.");
            if (validUserConcerns.Contains("hyperpigmentation"))
                responseBuilder.AppendLine("For **hyperpigmentation**: Vitamin C, Alpha Arbutin, and consistent SPF use are key. Exfoliation can also help.");

            responseBuilder.AppendLine("---");

            responseBuilder.AppendLine("\n---");
            responseBuilder.AppendLine($"Based on your **{skinType.ToUpper()}** skin and your concerns, here are some Glazed Skincare product recommendations:");

            var recommendedProducts = new List<SkincareProduct>();

            if (_skincareProducts.TryGetValue(skinType, out var productsForType))
            {
                recommendedProducts.AddRange(productsForType);
            }

            var filteredProductsByConcern = recommendedProducts
                .Where(p => p.Concerns != null && p.Concerns.Any(pc => validUserConcerns.Contains(pc.ToLower())))
                .ToList();

            if (!filteredProductsByConcern.Any() && recommendedProducts.Any())
            {
                responseBuilder.AppendLine("I'll suggest some core products for your skin type, as your concerns are quite specific or we don't have direct matches yet:");
                filteredProductsByConcern = recommendedProducts;
            }


            var uniqueRecommendedProducts = filteredProductsByConcern.GroupBy(p => p.Name).Select(g => g.First()).ToList();


            if (uniqueRecommendedProducts.Any())
            {
                foreach (var product in uniqueRecommendedProducts)
                {
                    responseBuilder.AppendLine($"- **{product.Name}**: {product.Description}");
                }
            }
            else
            {
                responseBuilder.AppendLine("I couldn't find specific products matching all your criteria right now. However, you can explore our full range on our website!");
            }
            responseBuilder.AppendLine("---");
            responseBuilder.AppendLine("Remember, consistency is key in skincare! If you have severe concerns, please consult a dermatologist. Type 'menu' to return to the main options.");

            return (responseBuilder.ToString(), "AwaitingMenuChoice");
        }


        private (string response, string nextState) ProcessFAQQuestion(string question)
        {
            foreach (var entry in _faqAnswers)
            {
                if (question.ToLower().Contains(entry.Key.ToLower()))
                {
                    return ($"\n💡 Here's what I found: {entry.Value}\n\nIs there anything else I can help you with? Type 'menu' to go back to options.", "AwaitingMenuChoice");
                }
            }
            return ("I'm sorry, I don't have an answer to that specific question in my FAQ. Please try rephrasing or choose from the suggested topics. If you need more help, you can always connect with a skincare expert. Type 'menu' to go back.", "AwaitingMenuChoice");
        }

        private (string response, string nextState) ProcessOrderID(string orderId)
        {
        
            if (orderId.All(char.IsDigit) && orderId.Length == 7)
            {
                
                return ($"Thank you! Your order **#{orderId}** is currently being processed and is expected to ship within 2 business days. You will receive a tracking number via email once it ships! 📧\n\nIs there anything else I can help you with? Type 'menu' to go back to options.", "AwaitingMenuChoice");
            }
            else
            {
                return ("That doesn't look like a valid order number. Please ensure it's a 7-digit number, or type 'menu' to go back to main options.", "AwaitingOrderID");
            }
        }
    }
}