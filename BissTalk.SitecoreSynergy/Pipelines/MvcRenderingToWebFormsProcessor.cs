#region licensing

// =============================================================
// <copyright file="MvcRenderingToWebFormsProcessor.cs" company="BissTalk">
// Copyright (c) 2014 
// </copyright>
// <author>Joe Bissol</author>
// <date>10/21/2014 11:41 PM</date>
// This file is part of a BissTalk Sample Project
// 
// BissTalk Sample Projects are free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// BissTalk Sample Project are distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this BissTalk Sample Project. If not, see http://www.gnu.org/licenses/.
// =============================================================

#endregion

using System;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Xml.Linq;
using System.Xml.XPath;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines.InsertRenderings;
using PageContext = Sitecore.Mvc.Presentation.PageContext;

namespace BissTalk.SitecoreSynergy.Pipelines
{
    /// <summary>
    ///     An <c>InsertRenderingsProcessor</c> that will swap any MVC renderings with the <c>MvcSubLayoutAdapterControl</c> so
    ///     that you can render MVC renderings on a WebForm Page.
    /// </summary>
    public class MvcRenderingToWebFormsProcessor : InsertRenderingsProcessor
    {
        /// <summary>
        ///     A list of all MVC rendering types.
        /// </summary>
        private static readonly string[] MvcRendingTypeNameList =
        {
            "View rendering",
            "Controller rendering",
            "Item rendering"
        };

        /// <summary>
        ///     Processes the specified insert renderings arguments within the InsertRenderings pipeline.
        /// </summary>
        /// <param name="args">The insert rendering arguments.</param>
        public override void Process(InsertRenderingsArgs args)
        {
            Assert.IsNotNull(args, "Argument args of type InsertRenderingsArgs cannot be null.");
            var fieldValue = GetLayoutFieldValue(args);
            if (string.IsNullOrEmpty(fieldValue)) return;

            var page = CreatPageContext();
            var deviceId = Context.Device.ID.ToString();
            var layoutDoc = XDocument.Parse(fieldValue);
            var parser = new XmlBasedRenderingParser();

            foreach (var renderingReference in args.Renderings)
                ProcessRendering(renderingReference, layoutDoc, deviceId, parser, page);
        }

        /// <summary>
        ///     Creates the page context.
        /// </summary>
        /// <returns>The page context.</returns>
        private static PageContext CreatPageContext()
        {
            var page = new PageContext
            {
                RequestContext = CreateRequestContext(
                    new HttpContextWrapper(HttpContext.Current),
                    CreateRouteData())
            };
            return page;
        }

        /// <summary>
        ///     Processes the rendering.
        /// </summary>
        /// <param name="renderingReference">The rendering reference.</param>
        /// <param name="layoutDoc">The layout document.</param>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="parser">The XML rendering field parser.</param>
        /// <param name="page">The page context.</param>
        private static void ProcessRendering(RenderingReference renderingReference, XNode layoutDoc, string deviceId,
            XmlBasedRenderingParser parser, PageContext page)
        {
            var templateName = renderingReference.RenderingItem.InnerItem.TemplateName;
            if (!MvcRendingTypeNameList.Contains(templateName)) return;
            var xpath = string.Format("/r/d[@id='{0}']/r[@id='{1}']", deviceId, renderingReference.RenderingID);
            var renderingXml = layoutDoc.XPathSelectElement(xpath);
            var rendering = parser.Parse(renderingXml, false);
            renderingReference.SetControl(new MvcSubLayoutAdapterControl(rendering, page));
        }

        /// <summary>
        ///     Gets the layout field value.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The value of the layout (__Renderings) field.</returns>
        private static string GetLayoutFieldValue(InsertRenderingsArgs args)
        {
            string fieldValue;
            var layoutField = args.ContextItem.Fields[FieldIDs.LayoutField];
            if (layoutField == null || string.IsNullOrEmpty(fieldValue = LayoutField.GetFieldValue(layoutField)))
                return null;
            return fieldValue;
        }

        /// <summary>
        ///     Creates a RouteData with enough information for Sitecore to do what it needs.
        /// </summary>
        /// <returns>The Sitecore RouteData.</returns>
        private static RouteData CreateRouteData()
        {
            var routeData = new RouteData();
            routeData.Values["scLanguage"] = Context.Language.Name;
            routeData.Values["controller"] = MvcSettings.SitecoreControllerName;

            return routeData;
        }

        /// <summary>
        ///     Creates a request context for MVC renderings
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="routeData">The route data.</param>
        /// <returns>The request context.</returns>
        private static RequestContext CreateRequestContext(HttpContextBase context, RouteData routeData)
        {
            return new RequestContext
            {
                HttpContext = context,
                RouteData = routeData
            };
        }
    }
}