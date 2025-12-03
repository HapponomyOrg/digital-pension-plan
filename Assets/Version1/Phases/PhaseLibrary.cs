using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Version1.Phases
{
    public static class PhaseLibrary
    {
        public static Phase MarketPhase { get; } = new("Market", "MarketScene");
        public static Phase LoadingPhase { get; } = new("Loading", "Loading");
        public static Phase MoneyCorrectionPhase { get; } = new("MoneyCorrection", "MoneyCorrectionScene");
        public static Phase MoneyToPointPhase { get; } = new("MoneyToPoint", "MoneyToPointScene");
        public static Phase DonatePointsPhase { get; } = new("DonatePoints", "DonatePointsScene");
        public static Phase PayDebtPhase { get; } = new("PayDebt", "PayDeptScene");
        public static Phase TakeALoanPhase { get; } = new("TakeALoan", "TakeALoanScene");
        public static Phase EndPhase { get; } = new("End", "EndScene");
    }
}
