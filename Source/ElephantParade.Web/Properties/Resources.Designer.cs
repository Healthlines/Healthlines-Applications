﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NHSD.ElephantParade.Web.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NHSD.ElephantParade.Web.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///        Dear {GP.name}
        ///
        ///        Re: {patient.name} {patient.DOB} Please find attached a letter about this patient with regard to their blood pressures.
        ///
        ///        Kind regards
        ///        Healthlines
        ///    .
        /// </summary>
        public static string GPBloodPressureEmailText {
            get {
                return ResourceManager.GetString("GPBloodPressureEmailText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Please be advised that the information sent in this e-mail by the Healthlines Service is only for the person named in the message. If you are not this person, or you are not expecting information from us please disregard and delete the message.&lt;/p&gt;
        ///&lt;p&gt;Dear {GP.name}&lt;/p&gt;
        ///&lt;p&gt;Re: {patient.name} {patient.DOB} Please find attached a letter about this patient with regard to their participation in the Healthlines Service.&lt;/p&gt;
        ///&lt;p&gt;Kind regards &lt;br/&gt; Healthlines&lt;/p&gt;
        ///&lt;p&gt;&amp;nbsp;&lt;/p&gt;.
        /// </summary>
        public static string GPEmailText {
            get {
                return ResourceManager.GetString("GPEmailText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Please be advised that the information sent in this e-mail by the Healthlines Service is only for the person named in the message. If you are not this person, or you are not expecting information from us please disregard and delete the message.&lt;/p&gt;
        ///&lt;p&gt;Dear {patient.name},&lt;/p&gt;
        ///&lt;p&gt;Thank you for using the Healthlines Service. Following our recent discussion, we have pleasure in providing you with the information below.&lt;/p&gt;
        ///&lt;p&gt;&lt;strong&gt;&lt;span style=&quot;text-decoration: underline;&quot;&gt;Topic information &lt;br /&gt;&lt;/spa [rest of string was truncated]&quot;;.
        /// </summary>
        public static string PatientEmailTemplate {
            get {
                return ResourceManager.GetString("PatientEmailTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {patient.name}
        ///
        ///Please find attached a letter with regard to your participation in the Healthlines Service.
        ///
        ///Kind regards
        ///Healthlines.
        /// </summary>
        public static string PatientEmailText {
            get {
                return ResourceManager.GetString("PatientEmailText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patient information missing..
        /// </summary>
        public static string PatientInformationMissingError {
            get {
                return ResourceManager.GetString("PatientInformationMissingError", resourceCulture);
            }
        }
    }
}
