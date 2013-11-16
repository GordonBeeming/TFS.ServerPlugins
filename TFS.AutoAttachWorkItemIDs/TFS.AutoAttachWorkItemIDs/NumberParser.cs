using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TFS.AutoAttachWorkItemIDs
{
    public class NumberParser
    {
        public List<int> Parse(string input)
        {
            List<int> result = new List<int>();

            if (!string.IsNullOrEmpty(input))
            {
                foreach (Match match in Regex.Matches(input, @"#\d{1,}"))
                {
                    result.Add(Convert.ToInt32(match.Value.TrimStart('#')));
                }
            }

            return result;
        }
    }
}
