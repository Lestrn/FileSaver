﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FileSaver.Domain.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ResourceMsgs {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceMsgs() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FileSaver.Domain.Resources.ResourceMsgs", typeof(ResourceMsgs).Assembly);
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
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div style=&quot;font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0;&quot;&gt;
        ///        &lt;div style=&quot;max-width: 600px; margin: 0 auto; padding: 20px; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);&quot;&gt;
        ///            &lt;h1 style=&quot;color: #333; font-size: 24px;&quot;&gt;Hello, {0},&lt;/h1&gt;
        ///            &lt;p style=&quot;font-size: 18px; color: #444;&quot;&gt;This is a recovery code on FileSaver:&lt;/p&gt;
        ///            &lt;p&gt;&lt;b style=&quot;font-weight: bold;&quot;&gt;The code  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string HtmlBodyEmailRecovery {
            get {
                return ResourceManager.GetString("HtmlBodyEmailRecovery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html&gt;
        ///&lt;head&gt;&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div style=&quot;font-family: Arial, sans-serif; background-color: #f5f5f5; margin: 0; padding: 0;&quot;&gt;
        ///        &lt;div style=&quot;max-width: 600px; margin: 0 auto; padding: 20px; background-color: #ffffff; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);&quot;&gt;
        ///            &lt;h1 style=&quot;color: #333; font-size: 24px;&quot;&gt;Hello, {0},&lt;/h1&gt;
        ///            &lt;p style=&quot;font-size: 18px; color: #444;&quot;&gt;This is a confirmation code on FileSaver:&lt;/p&gt;
        ///            &lt;p&gt;&lt;b style=&quot;font-weight: bold;&quot;&gt;The c [rest of string was truncated]&quot;;.
        /// </summary>
        public static string HtmlBodyEmailRegister {
            get {
                return ResourceManager.GetString("HtmlBodyEmailRegister", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Password must have a lowercase letter, a capital (uppercase letter, a number and minimum 8 characters.
        /// </summary>
        public static string InvalidPasswordMsg {
            get {
                return ResourceManager.GetString("InvalidPasswordMsg", resourceCulture);
            }
        }
    }
}