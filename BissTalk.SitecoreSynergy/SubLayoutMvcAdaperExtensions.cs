#region licensing

// =============================================================
// <copyright file="SubLayoutMvcAdaperExtensions.cs" company="BissTalk">
// Copyright (c) 2014 
// </copyright>
// <author>Joe Bissol</author>
// <date>10/21/2014 11:42 PM</date>
// This file is part of a BissTalk Sample Project
// 
// BissTalk Sample Projects are free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// BissTalk Sample Project are distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this BissTalk Sample Project. If not, see http://www.gnu.org/licenses/.
// =============================================================

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Sitecore.Collections;
using Sitecore.Mvc.Presentation;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;

namespace BissTalk.SitecoreSynergy
{
    /// <summary>
    /// HTML Helper extension methods for integrating a Sublayout rendering with an MVC page.
    /// </summary>
    public static class SubLayoutMvcAdaperExtensions
    {
        /// <summary>
        /// The context key for indexing forms to avoid collisions.
        /// </summary>
        private const string CONTEXT_KEY = "BissTalk.Mvc.Webform.Count";

        /// <summary>
        /// Gets a value indicating whether the MVC web form view state enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the MVC web form view state enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool MvcWebFormViewStateEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Renders the sitecore sublayout and returns the HTML string.
        /// </summary>
        /// <param name="html">The HTML helper being extended.</param>
        /// <param name="path">The path of an ASCX file.</param>
        /// <param name="datasource">The datasource, if any.</param>
        /// <param name="parameters">The rendering parameters, if any.</param>
        /// <returns>The HTML results of rendering.</returns>
        public static IHtmlString RenderSitecoreSubLayout(this HtmlHelper html, string path, string datasource = null,
            RenderingParameters parameters = null)
        {
            var page = new DummyPage { ViewStateEncryptionMode = ViewStateEncryptionMode.Never };
            AddSublayoutToPage(page, path, datasource, parameters);
            var result = ExecutePage(page);
            return html.Raw(result);
        }

        /// <summary>
        /// Adds the sublayout to page.
        /// </summary>
        /// <param name="page">The page used for rendering.</param>
        /// <param name="path">The path of the UserControl/Sublayout.</param>
        /// <param name="datasource">The datasource, if any.</param>
        /// <param name="parameters">The parameters, if any.</param>
        private static void AddSublayoutToPage(TemplateControl page, string path, string datasource,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var dict = ToDict(parameters);
            var sublayout = CreateSublayout(page, path, datasource, dict);
            Control controlToAdd = sublayout;
            if (MvcWebFormViewStateEnabled)
            {
                controlToAdd = CreateForm();
                controlToAdd.Controls.Add(sublayout);
            }

            page.Controls.Add(controlToAdd);
        }

        /// <summary>
        /// Creates the HTML form for postbacks.
        /// </summary>
        /// <returns>The HTML form for postbacks</returns>
        private static HtmlForm CreateForm()
        {
            var formName = GetNextFormName();
            var form = new HtmlForm
            {
                Method = "POST",
                ID = formName,
                Name = formName
            };
            return form;
        }

        /// <summary>
        /// Gets the name of the next form.
        /// </summary>
        /// <returns>The form name to be used next.</returns>
        private static string GetNextFormName()
        {
            var formNo = 0;
            int countParse;
            if (HttpContext.Current.Items.Contains(CONTEXT_KEY) &&
                    int.TryParse(HttpContext.Current.Items[CONTEXT_KEY].ToString(), out countParse))
                formNo = countParse;
            var formName = string.Concat("Form", formNo);
            HttpContext.Current.Items[CONTEXT_KEY] = ++formNo;
            return formName;
        }

        /// <summary>
        /// Creates the sublayout.
        /// </summary>
        /// <param name="page">The page the Sublayout will be added to.</param>
        /// <param name="path">The path od the UserControl/Sublayout.</param>
        /// <param name="datasource">The datasource, if any.</param>
        /// <param name="dict">The parameter dictionary.</param>
        /// <returns>A newly created Sublayout WebControl.</returns>
        private static Sublayout CreateSublayout(TemplateControl page, string path, string datasource, SafeDictionary<string> dict)
        {
            var sublayout = new Sublayout
            {
                DataSource = datasource,
                Path = path,
                Parameters = WebUtil.BuildQueryString(dict, true, true)
            };
            var userControl = page.LoadControl(path);
            sublayout.Controls.Add(userControl);
            return sublayout;
        }

        /// <summary>
        /// Converts the parameters to a <c>SafeDictionary</c>.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <c>SafeDictionary</c> containing the given parameters.</returns>
        private static SafeDictionary<string> ToDict(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var dict = new SafeDictionary<string>();
            if (parameters != null)
                parameters.ToList().ForEach(i => dict.Add(i.Key, i.Value));
            return dict;
        }

        /// <summary>
        /// Executes the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>The rendered HTML string.</returns>
        private static string ExecutePage(DummyPage page)
        {
            page.ProcessRequest(HttpContext.Current);
            return page.Result;
        }

        /// <summary>
        /// A page used to render the controls.
        /// </summary>
        private class DummyPage : Page
        {
            /// <summary>
            /// Gets the rendered HTML result.
            /// </summary>
            /// <value>
            /// The resulting HTML.
            /// </value>
            public string Result { get; private set; }

            /// <summary>
            /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter" /> object and calls on the child controls of the <see cref="T:System.Web.UI.Page" /> to render.
            /// </summary>
            /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> that receives the page content.</param>
            protected override void Render(HtmlTextWriter writer)
            {
                var buffer = new StringWriter();
                var stringWriter = new HtmlTextWriter(buffer);
                base.Render(stringWriter);
                Result = buffer.ToString();
            }
        }
    }
}