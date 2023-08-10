using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBanking.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _accountRepository;
        private IClientRepository _clientRepository;

        public AccountsController(IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _accountRepository.GetAllAccounts();
                var accountsDTO = new List<AccountDTO>();
                foreach (Account account in accounts)
                {
                    accountsDTO.Add(new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        Balance = account.Balance,
                        CreationDate = account.CreationDate,
                        Transactions = account.Transactions.Select(t => new TransactionDTO
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            Date = t.Date,
                            Description = t.Description,
                            Type = t.Type,
                        }).ToList()
                    });
                }

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("accounts/{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var account = _accountRepository.FindById(id);

                if (account == null)
                {
                    return NotFound();
                }

                var accountDTO = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    Balance = account.Balance,
                    CreationDate = account.CreationDate,
                    Transactions = account.Transactions.Select(t => new TransactionDTO
                    {
                        Id = t.Id,
                        Amount = t.Amount,
                        Date = t.Date,
                        Description = t.Description,
                        Type = t.Type,
                    }).ToList()
                };

                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("clients/current/accounts")]
        public IActionResult Post()
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
                    return Forbid();
                }

                if (client.Accounts.Count > 3)
                {
                    return Forbid();
                }

                Random random = new Random();
                int randomNum = random.Next(0, 100000000);
                string accountNumber = $"VIN-{randomNum:D8}";

                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                };
                _accountRepository.Save(newAccount);
                AccountDTO newAccountDTO = new AccountDTO
                {
                    Number = accountNumber,
                    CreationDate = newAccount.CreationDate,
                    Balance = 0,
                };
                return Ok(newAccountDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("clients/current/accounts")]
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
                    return Forbid();
                }

                var accountsDTO = new List<AccountDTO>();
                foreach (Account account in client.Accounts)
                {
                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        Balance = account.Balance,
                        CreationDate = account.CreationDate,
                        Transactions = account.Transactions.Select(t => new TransactionDTO
                        {
                            Id = t.Id,
                            Amount = t.Amount,
                            Date = t.Date,
                            Description = t.Description,
                            Type = t.Type,
                        }).ToList()
                    };
                    accountsDTO.Add(newAccountDTO);
                }
                return Ok(accountsDTO);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}