using System.Text;

namespace WebParser.Models
{
    class FiltrationStatsModel
    {
        public Type Filter { get; set; }
        public string Message { get; set; } = "There is no custom message.";
        public int? FilerItemsCount { get; set; }
        public int IncomeItemsCount { get; set; } = 0;
        public int OutcomeItemsCount { get; set; } = 0;
        public int FilteredItems { get => IncomeItemsCount - OutcomeItemsCount; }


        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Filtration report.\n\tUsed filter: {Filter.Name}");
            if (FilerItemsCount.HasValue)
                sb.AppendLine($"\tFilter check items: {FilerItemsCount}");
            sb.AppendLine($"\tIncome items: {IncomeItemsCount}");
            sb.AppendLine($"\tOutcome items: {OutcomeItemsCount}");
            sb.AppendLine($"\tFiltered items: {FilteredItems}");

            var indentedReport = string.Join("\n\t\t", Message.Split('\n'));
            sb.AppendLine($"\n\tAdditional filter message: \n\t\t{indentedReport}");

            return sb.ToString();
        }
    }

}

