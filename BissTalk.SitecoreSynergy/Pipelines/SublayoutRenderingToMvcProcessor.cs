#region licensing

// =============================================================
// <copyright file="SublayoutRenderingToMvcProcessor.cs" company="BissTalk">
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

using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.BuildPageDefinition;
using Sitecore.Mvc.Presentation;

namespace BissTalk.SitecoreSynergy.Pipelines
{
    /// <summary>
    ///     A <c>BuildPageDefinitionProcessor</c> that will swap any Sublayout renderings with a MVC razor view that can
    ///     execute it so
    ///     that you can render sublayout renderings on a MVC Page.
    /// </summary>
    public class SublayoutRenderingToMvcProcessor : BuildPageDefinitionProcessor
    {
        /// <summary>
        ///     Processes the specified BuildPageDefinitionArgs arguments.
        /// </summary>
        /// <param name="args">The BuildPageDefinitionArgs arguments.</param>
        public override void Process(BuildPageDefinitionArgs args)
        {
            foreach (var rendering in args.Result.Renderings.Where(IsSublayout))
                SwitchRenderings(args, rendering);
        }

        /// <summary>
        ///     Switches the renderings.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="rendering">The rendering.</param>
        private static void SwitchRenderings(MvcPipelineArgs args, Rendering rendering)
        {
            const string DEFAULT_VIEW_RENDERING_ID = "{9753D6AD-96A3-42CC-9591-9AA3D83889EE}";
            const string PROPERTY_KEY = "Mvc.SublayoutRendering";
            const string VIEW_RENDERING_TYPE_NAME = "View rendering";

            var id = Settings.GetSetting(PROPERTY_KEY, DEFAULT_VIEW_RENDERING_ID);
            var item = args.PageContext.Database.GetItem(ID.Parse(id));
            var renderingItem = new RenderingItem(item);
            AddControlPathRenderingParameter(rendering);
            rendering.RenderingItem = renderingItem;
            rendering.RenderingType = VIEW_RENDERING_TYPE_NAME;
        }

        /// <summary>
        ///     Determines whether the specified rendering is sublayout.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        /// <returns><c>true</c> if the specified rendering is sublayout, otherwise <c>false</c>.</returns>
        private static bool IsSublayout(Rendering rendering)
        {
            return rendering.RenderingItem != null && rendering.RenderingItem.TagName == "Sublayout";
        }

        /// <summary>
        ///     Adds the control path rendering parameter.
        /// </summary>
        /// <param name="rendering">The rendering.</param>
        private static void AddControlPathRenderingParameter(Rendering rendering)
        {
            const string PARAMETERS_FIELD_NAME = "Parameters";
            const string PATH_FIELD_NAME = "Path";

            var parameterString = rendering.RenderingItem.InnerItem[PARAMETERS_FIELD_NAME];
            if (parameterString.Length > 0)
                parameterString += "&";

            rendering.Parameters =
                new RenderingParameters(string.Concat(parameterString, "sublayout-rendering-path=",
                    rendering.RenderingItem.InnerItem[PATH_FIELD_NAME]));
        }
    }
}