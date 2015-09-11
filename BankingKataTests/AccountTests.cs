using BankingKata;
using NSubstitute;
using NUnit.Framework;
using System;

namespace BankingKataTests
{
    [TestFixture]
    public class AccountTests
    {
        [Test]
        public void AccountRecordsDepositInTransactionLog()
        {
            var ledger = Substitute.For<ILedger>();
            var money = new Money(3m);
            var account = new Account(ledger);

            account.Deposit(DateTime.Now, money);

            CreditEntry deposit = new CreditEntry(DateTime.Now, money);
            ledger.Received().Record(deposit);
        }

        [Test]
        public void AccountRecordsWithdrawalInTransactionLog()
        {
            var expectedBalance = new Money(0m);
            var ledger = Substitute.For<ILedger>();
            var money = new Money(3m);
            var account = new Account(ledger);
            ledger.Accept(Arg.Any<BalanceCalculatingVisitor>(), new Money(0m)).Returns(expectedBalance);
            var debitEntry = new ATMDebitEntry(DateTime.Now, money);
            account.Withdraw(debitEntry);

            ledger.Received().Record(debitEntry);
        }

        [Test]
        public void AccountRecordsChequeWithdrawalInTransactionLog()
        {
            var expectedBalance = new Money(0m);
            var ledger = Substitute.For<ILedger>();
            var money = new Money(3m);
            var account = new Account(ledger);
            ledger.Accept(Arg.Any<BalanceCalculatingVisitor>(), new Money(0m)).Returns(expectedBalance);
            var myCheque = new ChequeDebitEntry(new DateTime(2015, 07, 13), money, 100001);
            account.Withdraw(myCheque);

            ledger.Received().Record(myCheque);
        }

        [Test]
        public void CalculateBalanceTotalsAllDepositsMadeToTheAccount()
        {
            var ledger = Substitute.For<ILedger>();
            var account = new Account(ledger);

            account.CalculateBalance();

            ledger.Received().Accept(Arg.Any<BalanceCalculatingVisitor>(), new Money(0m));
        }

        [Test]
        public void LedgerTotalIsReturnedByCalculate()
        {
            var expectedBalance = new Money(13m);
            var ledger = Substitute.For<ILedger>();
            ledger.Accept(Arg.Any<BalanceCalculatingVisitor>(), new Money(0m)).Returns(expectedBalance);
            var account = new Account(ledger);

            var actualBalance = account.CalculateBalance();

            Assert.That(actualBalance, Is.EqualTo(expectedBalance));
        }

        [Test]
        public void HardLimitCannotBeExceeded()
        {
            var expectedBalance = new Money(0m);
            var ledger = Substitute.For<ILedger>();
            var money = new Money(1002m);
            var account = new Account(ledger);
            ledger.Accept(Arg.Any<BalanceCalculatingVisitor>(), new Money(0m)).Returns(expectedBalance);
            var myCheque = new ChequeDebitEntry(new DateTime(2015, 07, 13), money, 100001);

            Assert.Throws<OverdraftLimitExceededException>(() => account.Withdraw(myCheque));
        
            ledger.DidNotReceive().Record(myCheque);
        }
    }
}
