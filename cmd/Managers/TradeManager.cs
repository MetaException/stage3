﻿using cmd.Interface;
using Microsoft.Extensions.Logging;

namespace cmd.Managers;

internal static class TradeManager
{
    public record Transaction(int day, int hour, string from, string to, int quantity, double sum); // ??

    public static async void PerformDeal(IPaymentSubject from, IPaymentSubject to, int countToTrade)
    {
        double priceToPay = CalculatePriceSum(from, countToTrade);

        if (to.Balance < priceToPay || from.GetSharesCount() < countToTrade)
        {
            return;
        }

        TradeShares(from, to, countToTrade);
        TradeMoney(to, from, priceToPay);

        Transaction transaction = new Transaction(
            day: Simulator.day,
            hour: Simulator.hour,
            from: from.Name,
            to: to.Name,
            quantity: countToTrade,
            sum: priceToPay
        );
        Simulator.WaitDelay().Wait();
        SimulatorLogger.Logger.Information("[День {@dayn}] [Час {@hourn}] {@from_name} продал {@shares_count} акций {@to_name} на сумму {@sum}", transaction.day, transaction.hour, transaction.from, transaction.quantity, transaction.to, transaction.sum);
    }

    public static async void TradeShares(IPaymentSubject from, IPaymentSubject to, int count)
    {
        if (from.GetSharesCount() < count)
        {
            count = from.GetSharesCount();
        }
        var shares = from.TakeShares(count);
        to.AddShares(shares);

        Simulator.WaitDelay().Wait();
        SimulatorLogger.Logger.Debug("[День {@dayn}] [Час {@hourn}] {@from_name} передал {@shares_count} акций {@to_name}", Simulator.day, Simulator.hour, from.Name, count, to.Name);
    }

    public static void TradeMoney(IPaymentSubject from, IPaymentSubject to, double priceToPay)
    {
        if (priceToPay == -1) // -1 сохранить весь баланс
            priceToPay = from.Balance;

        var money = from.TakeMoney(priceToPay); //...
        to.AddMoney(money);

        Simulator.WaitDelay().Wait();
        SimulatorLogger.Logger.Debug("[День {@dayn}] [Час {@hourn}] {@from_name} передал {@shares_count} денег {@to_name}", Simulator.day, Simulator.hour, from.Name, priceToPay, to.Name);
    }

    public static double CalculatePriceSum(IPaymentSubject company, int countToBuy)
    {
        return company.SharesPrice * countToBuy;
    }
}