using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeBanking.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CardsController : Controller
    {
        private ICardRepository _cardRepository;
        private IClientRepository _clientRepository;
        public CardsController(ICardRepository cardRepository,IClientRepository clientRepository)
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
        }

        [HttpGet("clients/current/cards")]
        public IActionResult GetCurrent() 
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();

                }

                var cardsDTO = new List<CardDTO>();

                foreach (Card card in client.Cards)
                {
                    var newCardDTO = new CardDTO()
                    {
                        Id = card.Id,
                        CardHolder = card.CardHolder,
                        Color = card.Color,
                        Cvv = card.Cvv,
                        FromDate = card.FromDate,
                        Number = card.Number,
                        ThruDate = card.ThruDate,
                        Type = card.Type
                    };

                    cardsDTO.Add(newCardDTO);
                }

                return Ok(cardsDTO);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("clients/current/cards")]
        public IActionResult Post([FromBody] Card card) 
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return NotFound();

                }

                if (client.Cards.Count() > 2)
                {
                    return Forbid();
                }

                //Numero de tarjeta random
                Random random = new Random();
                string numCard = "";
                for (int i = 0; i < 4; i++)
                {
                    string section = random.Next(0, 10000).ToString("D4");
                    numCard += section + " ";
                }
                numCard = numCard.Trim();

                Card newCard = new Card()
                {
                    ClientId = client.Id,
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = card.Type,
                    Color = card.Color,
                    Number = numCard,
                    Cvv = random.Next(100, 1000),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(4),
                };
                _cardRepository.Save(newCard);

                CardDTO cardDTO = new CardDTO()
                {
                    CardHolder = newCard.CardHolder,
                    Type = newCard.Type,
                    Color = newCard.Color,
                    Number = newCard.Number,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate
                };

                return Created("", cardDTO);
            }
            catch(Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
