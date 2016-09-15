// -----------------------------------------------------------------------
// <copyright file="Condition.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents a condition that can be evaluated
    /// TODO: support for than literal eg previous answers
    /// </summary>
    public class Condition
    {
        public enum OperatorType
        {
            Equals,
            LessThan,
            GreaterThan,
            NotEquals,
            Between
        }

        /// <summary>
        /// The value type for the condition.
        /// </summary>
        public enum Value_Type
        {
            LiteralString,
            LiteralDate,
            LiteralNumber            
        }

        #region Properties

        public virtual string Label
        {
            get;
            set;
        }

        public virtual OperatorType Operator
        {
            get;
            set;
        }

        public virtual Value_Type ValueType
        {
            get;
            set;
        }

        public virtual string Value1
        {
            get;
            set;
        }

        public virtual string Value2
        {
            get;
            set;
        }       
        
        #endregion

        #region Methods

        public bool Evaluate(string value)
        {
            return Evaluate(this, value);
        }

        #endregion

        #region Static Methods

        public static bool Evaluate(Condition condition,string value)
        {            
            switch (condition.ValueType)
            {
                case Value_Type.LiteralString:
                    switch (condition.Operator)
                    {
                        case OperatorType.Equals:
                            return condition.Value1 == value;
                        case OperatorType.NotEquals:
                            return condition.Value1 != value;                        
                        default:
                            throw new NotImplementedException(string.Format("Condition.Evaluate {0} for {1} not supported",condition.ValueType.ToString(),condition.Operator.ToString()));
                    }              
                case Value_Type.LiteralDate:
                    switch (condition.Operator)
	                {
		                case OperatorType.Equals:
                            return DateTime.Parse(condition.Value1) == DateTime.Parse(value);
                        case OperatorType.LessThan:
                            return DateTime.Parse(value) < DateTime.Parse(condition.Value1);
                        case OperatorType.GreaterThan:
                            return DateTime.Parse(value) > DateTime.Parse(condition.Value1);
                        case OperatorType.NotEquals:
                            return DateTime.Parse(value) != DateTime.Parse(condition.Value1);
                        case OperatorType.Between:
                            return DateTime.Parse(value) > DateTime.Parse(condition.Value1) && DateTime.Parse(value) <DateTime.Parse(condition.Value2);
                        default:
                            throw new NotImplementedException(string.Format("Condition.Evaluate {0} for {1} not supported", condition.ValueType.ToString(), condition.Operator.ToString()));
	                }                 
                case Value_Type.LiteralNumber:
                    switch (condition.Operator)
                    {
                        case OperatorType.Equals:
                            return decimal.Parse(condition.Value1) == decimal.Parse(value);
                        case OperatorType.LessThan:
                            return decimal.Parse(value) < decimal.Parse(condition.Value1);
                        case OperatorType.GreaterThan:
                            return decimal.Parse(value) > decimal.Parse(condition.Value1);
                        case OperatorType.NotEquals:
                            return decimal.Parse(value) != decimal.Parse(condition.Value1);
                        case OperatorType.Between:
                            return decimal.Parse(value) > decimal.Parse(condition.Value1) && decimal.Parse(value) < decimal.Parse(condition.Value2);
                        default:
                            throw new NotImplementedException(string.Format("Condition.Evaluate {0} for {1} not supported", condition.ValueType.ToString(), condition.Operator.ToString()));
                    } 
                default:
                    throw new NotImplementedException("Condition.Evaluate ValueType not supported");
            }
        }

        #endregion
    }
}
