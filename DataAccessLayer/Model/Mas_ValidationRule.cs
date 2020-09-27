using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.Model
{
    public class Mas_ValidationRule
    {
        public int ID { get; set; }
        public string RuleName { get; set; }
        public int RuleStep { get; set; }
        public string RuleTarget { get; set; }
        public string Operator { get; set; }
        public string StartNumber { get; set; }
        public string EndNumber { get; set; }
        public string OperationStartText { get; set; }
        public string OperationEndText { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
