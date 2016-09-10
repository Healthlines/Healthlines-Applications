// -----------------------------------------------------------------------
// <copyright file="ConfigurationException.cs" company="NHS Direct">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Questionnaires.Core.Services.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ConfigurationException : Exception 
    {
        public ConfigurationException(string message)
            : base(message) 
        {

        }

        public ConfigurationException(string message, Exception innerException)
            :base(message,innerException)
        {
           
        }
    }
}
