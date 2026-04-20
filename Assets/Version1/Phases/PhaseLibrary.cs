namespace Version1.Phases
{
    public static class PhaseLibrary
    {
        public static Phase Login { get; } = new("Login", "LoginScene");
        public static Phase MarketPhase { get; } = new("Market", "MarketScene");
        public static Phase LoadingPhase { get; } = new("Loading", "Loading");
        public static Phase MoneyCorrectionPhase { get; } = new("MoneyCorrection", "MoneyCorrectionScene");
        public static Phase MoneyToPointPhase { get; } = new("MoneyToPoint", "MoneyToPointScene");
        public static Phase DonatePointsPhase { get; } = new("DonatePoints", "DonatePointsScene");
        public static Phase PayDebtPhase { get; } = new("PayDebt", "PayDebtScene");
        public static Phase TakeALoanPhase { get; } = new("TakeALoan", "TakeALoanScene");
        public static Phase EndPhase { get; } = new("End", "EndScene");
        public static Phase BankExplanation { get; } = new("BankExplanation", "BankExplanationScene");
        public static Phase BankOverview { get; } = new("BankOverview", "BankOverviewScene");
    }
}
