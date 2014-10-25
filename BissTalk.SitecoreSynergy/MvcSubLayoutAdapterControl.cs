#region licensing

// =============================================================
// <copyright file="MvcSubLayoutAdapterControl.cs" company="BissTalk">
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

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI;

namespace BissTalk.SitecoreSynergy
{
    /// <summary>
    ///     A <c>WebControl</c> that will output the results of a Sitecore MVC rendering.
    /// </summary>
    public class MvcSubLayoutAdapterControl : WebControl
    {
        /// <summary>
        ///     The Sitecore page context.
        /// </summary>
        private readonly PageContext _page;

        /// <summary>
        ///     The Sitecore rendering.
        /// </summary>
        private readonly Rendering _rendering;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MvcSubLayoutAdapterControl" /> class.
        /// </summary>
        /// <param name="rendering">The Sitecore rendering</param>
        /// <param name="page">The Sitecore page context.</param>
        public MvcSubLayoutAdapterControl(Rendering rendering, PageContext page)
        {
            _rendering = rendering;
            _page = page;
        }

        /// <summary>
        ///     Outputs the rendered html to a given <c>HtmlTextWriter</c>.
        /// </summary>
        /// <param name="output">The HtmlTextWriter output.</param>
        protected override void DoRender(HtmlTextWriter output)
        {
            var context = _page.RequestContext.HttpContext as HttpContextWrapper;
            Assert.IsNotNull(
                context,
                "Cannot execute rendering without a valid HTTP Context.  PageContext.RequestContext.HttpContext is null.");
            using (ContextService.Get().Push(_page))
            {
                var view = CreateViewContext(context, _page.RequestContext.RouteData, _rendering);

                using (ContextService.Get().Push(view))
                {
                    PipelineService.Get().RunPipeline(
                        "mvc.renderRendering",
                        new RenderRenderingArgs(_rendering, output));
                }
            }
        }

        /// <summary>
        ///     Create the view context for the Sitecore rendering.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="routeData">The HTTP Request route data.</param>
        /// <param name="rendering">The MVC Rendering that will be rendered.</param>
        /// <returns>The information that is needed to render a view.</returns>
        private static ViewContext CreateViewContext(HttpContextBase context, RouteData routeData, Rendering rendering)
        {
            var viewCtx = new ViewContext
            {
                ViewData = new ViewDataDictionary(),
                View = new RenderingView(rendering),
                TempData = new TempDataDictionary(),
                RouteData = routeData,
                HttpContext = context
            };
            return viewCtx;
        }
    }
}