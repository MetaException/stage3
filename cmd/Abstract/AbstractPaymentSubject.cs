﻿using cmd.Interface;
using cmd.Units;

namespace cmd.Abstract
{
    public abstract class AbstractPaymentSubject : IPaymentSubject
    {
        public double Balance
        { get { return balance; } set { balance = value; } }
        public string Name
        { get { return name; } }
        public List<Share> Shares
        { get { return shares; } }
        public double SharesPrice
        { get { return sharesPrice; } set { sharesPrice = value; } }

        protected double balance;
        protected List<Share> shares;
        protected double sharesPrice;
        protected string name;

        public List<Share> TakeShares(int count)
        {
            var takenShares = shares.Take(count).ToList();
            shares.RemoveRange(0, count);
            return takenShares;
        }

        public void AddMoney(double amount)
        {
            balance += amount;
        }

        public int GetSharesCount()
        {
            return shares.Count;
        }

        public double TakeMoney(double amount)
        {
            balance -= amount; //...
            return amount;
        }

        public void AddShares(List<Share> sharesToAdd)
        {
            shares.AddRange(sharesToAdd);
        }
    }
}