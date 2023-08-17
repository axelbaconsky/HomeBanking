using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using HomeBanking.Repositories;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository, IAccountRepository accountRepository, ILoanRepository loanRepository, IClientLoanRepository clientLoanRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();
                List<LoanDTO> loanDTOs = new List<LoanDTO>();
                foreach (var loan in loans)
                {
                    LoanDTO loanDTO = new LoanDTO()
                    {
                        Id = loan.Id,
                        Name = loan.Name,
                        MaxAmount = loan.MaxAmount,
                        Payments = loan.Payments,
                    };
                    loanDTOs.Add(loanDTO);
                }
                return Ok(loanDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanApplicationDTO)
        {
            try
            {
                if (loanApplicationDTO.Amount < 1 || String.IsNullOrEmpty(loanApplicationDTO.ToAccountNumber) || String.IsNullOrEmpty(loanApplicationDTO.Payments))
                    return Forbid();

                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                    return Forbid();

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                    return Forbid();

                var loan = _loanRepository.FindById(loanApplicationDTO.LoanId);

                if (loan == null || loanApplicationDTO.Amount >= loan.MaxAmount || loanApplicationDTO.Payments == null || loanApplicationDTO.Payments == string.Empty)
                    return Forbid();

                var account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                if (account == null || account.ClientId != client.Id)
                    return Forbid();

                double interestAmount = loanApplicationDTO.Amount * 0.2;

                ClientLoan clientLoan = new ClientLoan
                {
                    ClientId = client.Id,
                    Amount = loanApplicationDTO.Amount + interestAmount,
                    Payments = loanApplicationDTO.Payments,
                    LoanId = loanApplicationDTO.LoanId,
                };

                _clientLoanRepository.Save(clientLoan);

                Transaction transaction = new Transaction
                {
                    AccountId = account.Id,
                    Amount = loanApplicationDTO.Amount,
                    Date = DateTime.Now,
                    Description = "Prestamo " + loan.Name +  " aprobado",
                    Type = TransactionType.CREDIT.ToString(),
                };

                _transactionRepository.Save(transaction);

                account.Balance += loanApplicationDTO.Amount;

                _accountRepository.Save(account);

                return Ok(clientLoan);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
