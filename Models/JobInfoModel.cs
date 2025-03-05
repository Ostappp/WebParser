namespace WebParser.Models
{
    class JobInfoModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Phones { get; set; }
        public IEnumerable<string> Emails { get; set; }
        public string Location { get; set; }

        /// <summary>
        /// Checks if current model contain a given string in any of its fields.
        /// </summary>
        /// <param name="value">A string for comparing with fields of model.</param>
        /// <returns>Returns True if any field contains given string, otherwise returns False.</returns>
        public bool Contains(string value)
        {
            return Title.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                   Description.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                   Phones.Any(phone => phone.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                   Emails.Any(email => email.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                   Location.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if current model contains at least one of given strings in any of its fields.
        /// </summary>
        /// <param name="values">A string collection for comparing with fields of model.</param>
        /// <returns>Returns True if any field contains any of given strings, otherwise returns False.</returns>
        public bool Contains(IEnumerable<string> values)
        {            
            return values.ToList().Any(Contains);
        }
    }
}
