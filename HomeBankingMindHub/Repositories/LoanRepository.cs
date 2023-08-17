﻿using HomeBanking.Repositories;
using HomeBankingMindHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
       public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }
       
        public IEnumerable<Loan> GetAll()
        {
            return FindAll().ToList();
        }
        
        public Loan FindById(long Id)
        {
            return FindByCondition(loan =>  loan.Id == Id).FirstOrDefault();
        }

    }
}