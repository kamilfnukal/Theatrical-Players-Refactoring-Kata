using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                thisAmount = ComputeAmount(play, perf);
                volumeCredits += CalculateVolumeCredits(perf, play);

                // print line for this order
                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
            }
            result += string.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        private static int CalculateVolumeCredits( Performance perf, Play play)
        {
            // add volume credits
            int volumeCredits = 0;
            volumeCredits += Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type) volumeCredits += (int) Math.Floor((decimal) perf.Audience / 5);
            return volumeCredits;
        }

        private int ComputeAmount(Play play, Performance perf)
        {
            int thisAmount = play.Type switch
            {
                "tragedy" => computeTragedyAmount(perf.Audience),
                "comedy" => computeComedyAmount(perf.Audience),
                _ => throw new Exception("unknown type: " + play.Type)
            };

            return thisAmount;
        }

        int computeTragedyAmount(int audianceCount)
        {
            var thisAmount = 40000;
            if (audianceCount > 30)
            {
                thisAmount += 1000 * (audianceCount - 30);
            }
            return thisAmount;
        }

        int computeComedyAmount(int audianceCount)
        {
            var thisAmount = 30000;
            if (audianceCount > 20)
            {
                thisAmount += 10000 + 500 * (audianceCount - 20);
            }
            thisAmount += 300 * audianceCount;
            return thisAmount;
        }
    }
}
