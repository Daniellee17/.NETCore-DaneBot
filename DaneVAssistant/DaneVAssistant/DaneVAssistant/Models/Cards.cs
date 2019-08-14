using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DaneVAssistant.Models
{
    public static class Cards
    {
       
        public static ThumbnailCard GetDatabaseThumbnailCard()
        {
            //   var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\ChevronLogo.png");
            //  var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            var heroCard = new ThumbnailCard
            {
                Title = "DMSS",
                Subtitle = "Database Management Services & Support",

                Images = new List<CardImage> { new CardImage($"") },

                Buttons = new List<CardAction>
                        {

                           new CardAction(ActionTypes.OpenUrl, "Create A Request", value: "t"),

                            new CardAction(ActionTypes.ImBack, title: "Quit", value: "Quit"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetHelpHeroCard()
        {

            var heroCard = new HeroCard
            {
                Title = "Help",

                /// Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                           new CardAction(ActionTypes.OpenUrl, title: "About me", value: ""),
                           new CardAction(ActionTypes.OpenUrl, title: "Help", value: "Help"),
                           new CardAction(ActionTypes.OpenUrl, title: "Help", value: "Help"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetSQLHeroCard()
        {

            var heroCard = new HeroCard
            {
                Title = "Standard Query Language",

                Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                           new CardAction(ActionTypes.ImBack, title: "Select", value: "SQL"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetOracleHeroCard()
        {

            var heroCard = new HeroCard
            {
                Title = "Oracle",

                Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                           new CardAction(ActionTypes.ImBack, title: "Select", value: "Oracle"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetPowerBIHeroCard()
        {

            var heroCard = new HeroCard
            {
                Title = "Power BI / Power Apps",

                Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                           new CardAction(ActionTypes.ImBack, title: "Select", value: "Power BI / Power Apps"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetGeneralHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = "General",

                Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                           new CardAction(ActionTypes.ImBack, title: "Select", value: "General"),
                        },
            };

            return heroCard;
        }

        public static HeroCard GetDatabaseHeroCard()
        {
            //  var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\bg.jpg");
            //  var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            var heroCard = new HeroCard
            {
                //Title = "Database Management Services & Support",
                //Subtitle = "Greetings from the Database Management Services & Support Team!",
                //Text = "I'm here to help",


                Images = new List<CardImage> { new CardImage($"") },
                Buttons = new List<CardAction>
                        {
                            // Note that some channels require different values to be used in order to get buttons to display text.
                            // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                            // need to provide a value for other parameters like 'text' or 'displayText'.
                            new CardAction(ActionTypes.ImBack, title: "Self Service", value: "Self Service"),
                            new CardAction(ActionTypes.ImBack, title: "Send an Email", value: "Send an Email"),
                            new CardAction(ActionTypes.OpenUrl, title: "Create a Request", value: ""),
                            new CardAction(ActionTypes.ImBack, title: "Quit", value: "Quit"),
                        },
            };

            return heroCard;
        }
        public static Attachment ByeGIF()
        {
            //  var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\bg.jpg");
            // var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            var attachment = new Attachment
            {
                Name = @"Pic",
                ContentType = "image/jpg",
                ContentUrl = $"",
            };

            return attachment;

        }
        public static AnimationCard GetAnimationCard()
        {
            //var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\robot.gif");
            //var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));
            var animationCard = new AnimationCard
            {
                Title = "Microsoft Bot Framework",
                Subtitle = "Animation Card",
                Image = new ThumbnailUrl
                {
                    Url = "",
                },
                Media = new List<MediaUrl>
                {

                    new MediaUrl()
                    {
                        Url = $"",
                    },
                },
            };

            return animationCard;
        }




    }




}
