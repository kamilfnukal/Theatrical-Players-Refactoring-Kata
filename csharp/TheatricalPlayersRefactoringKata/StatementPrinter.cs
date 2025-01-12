﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var perfomanceAmount = new Dictionary<string, int>();
            var totalAmount = 0;
            
            var volumeCredits = Calculate(invoice, plays, perfomanceAmount, ref totalAmount);

            var result = PrintTxt(invoice, plays, perfomanceAmount, totalAmount, volumeCredits);
            return result;
        }

        public string PrintAsHtml(Invoice invoice, Dictionary<string, Play> plays)
        {
            var perfomanceAmount = new Dictionary<string, int>();
            var totalAmount = 0;
            
            var volumeCredits = Calculate(invoice, plays, perfomanceAmount, ref totalAmount);

            var result = PrintHtml(invoice, plays, perfomanceAmount, totalAmount, volumeCredits);
            return result;
        }

        private int Calculate(Invoice invoice, Dictionary<string, Play> plays, Dictionary<string, int> perfomanceAmount,
            ref int totalAmount)
        {
            var volumeCredits = 0;
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                thisAmount = ComputeAmount(play, perf);
                volumeCredits += CalculateVolumeCredits(perf, play);

                perfomanceAmount.Add(perf.PlayID, thisAmount);
                totalAmount += thisAmount;
            }

            return volumeCredits;
        }

        private static string PrintTxt(Invoice invoice, Dictionary<string, Play> plays, Dictionary<string, int> perfomanceAmount, int totalAmount, int volumeCredits)
        {
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");
            
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = perfomanceAmount[perf.PlayID];
                // print line for this order
                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name,
                    Convert.ToDecimal(thisAmount / 100), perf.Audience);
            }

            result += string.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        private static string PrintHtml(Invoice invoice, Dictionary<string, Play> plays, Dictionary<string, int> perfomanceAmount, int totalAmount, int volumeCredits)
        {
            var result = "<html>\n";
            result +=  string.Format("<h1>Statement for {0}</h1>\n", invoice.Customer);
            result += "<table>\n";
            CultureInfo cultureInfo = new CultureInfo("en-US");
            result += "  <tr><th>play</th><th>seats</th><th>cost</th></tr>\n";
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = perfomanceAmount[perf.PlayID];
                // print line for this order
                
                result += string.Format(cultureInfo, "  <tr><td>{0}</td><td>{1}</td><td>${2:C}</td></tr>\n", play.Name,
                    perf.Audience, Convert.ToDecimal(thisAmount / 100));
            }

            result += "</table>\n";
            result += string.Format(cultureInfo, "<p>Amount owed is <em>${0:C}</em></p>\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("<p>You earned <em>{0}</em> credits</p>\n", volumeCredits);
            result += "</html>\n";
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
