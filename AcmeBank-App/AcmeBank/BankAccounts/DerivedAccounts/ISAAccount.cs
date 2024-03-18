﻿using AcmeBank.BankAccounts;
using AcmeBank.BankAccounts.AccountInterfaces;
using System.Collections.Generic;

namespace BankPayments.BankAccounts.DerivedAccounts;

public class ISAAccount : Account, IDepositLimitedAccount
{
    #region Attributes
    private const decimal _yearlyDepositLimit = 20_000m;
    private decimal _remainingDepositLimit; //store remaining deposit limit
    #endregion

    #region Constructors
    // Used on account creation
    public ISAAccount(string accountNumber, string sortCode, decimal balance) : base(accountNumber, sortCode, balance, AccountType.ISA)
    {
        _remainingDepositLimit = DepositLimit;
    }

    // Used on loading account from file
    public ISAAccount(string accountNumber, string sortCode, decimal balance, decimal remainingDepositLimit) : base(accountNumber, sortCode, balance, AccountType.ISA)
    {
        _remainingDepositLimit = remainingDepositLimit;
    }


    #endregion

    #region Getters/Setters
    public decimal DepositLimit { get { return _yearlyDepositLimit; } } // £20,000 deposit limit for ISA account

    public decimal RemainingDepositLimit
    {
        get => _remainingDepositLimit;
        set
        {
            // Ensure the new value is clamped between 0 and the deposit limit
            _remainingDepositLimit = Math.Clamp(value, 0 , DepositLimit); // Used when withdrawing money.
        }
    }

    #endregion

    #region Methods
    protected override void DisplayAccountOptions()
    {
        Console.WriteLine("""
                --- Account options ---
                1. Deposit
                2. Withdraw
                3. Payment
                4. Transfer
                5. Test
                X. Exit
                -----------------------
                """);
    }

    public bool UpdateDepositLimit(decimal amount)
    {
        // Check if the deposit amount exceeds the remaining deposit limit
        if (RemainingDepositLimit - amount < 0)
        { 
            return false;
        }
        else
        {
            // Deduct the deposit amount from the remaining deposit limit
            RemainingDepositLimit -= amount;
            return true;
        }
    }

    public void ResetDepositLimit()
    {
        // Reset the remaining deposit limit to the deposit limit
        RemainingDepositLimit = DepositLimit;
    }

    protected override void Deposit()
    {
        if (RemainingDepositLimit > 0)
        {
            base.Deposit();
        } 
        else
        {
            // Display error prompt if deposit is over the yearly limit
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("!!! Yearly deposit limit of £20,000 reached !!!");
            Console.ResetColor();

            // Provide a pause to read prompt 1.5seconds
            Thread.Sleep(1500);
            Console.Clear();
        }
    }
    #endregion

}