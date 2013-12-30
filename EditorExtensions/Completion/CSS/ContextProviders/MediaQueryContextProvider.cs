﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CSS.Core;
using Microsoft.CSS.Editor.Intellisense;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.EditorExtensions
{
    [Export(typeof(ICssCompletionContextProvider))]
    [Name("MediaQueryCompletionContextProvider")]
    internal class MediaQueryCompletionContextProvider : ICssCompletionContextProvider
    {
        public IEnumerable<Type> ItemTypes
        {
            get
            {
                return new Type[] { typeof(MediaQuery), };
            }
        }

        public CssCompletionContext GetCompletionContext(ParseItem item, int position)
        {
            MediaQuery query = (MediaQuery)item;

            var token = item.StyleSheet.ItemBeforePosition(position);

            // Don't handle expressions. MediaFeatureContextProvider.cs does that
            if (token.FindType<MediaExpression>() != null)
                return null;

            // Don't show completion for "and"
            if (query.MediaType != null && query.MediaType != token && token != query.Operation)
                return null;

            return new CssCompletionContext((CssCompletionContextType)612, token.Start, token.Length, null);
        }
    }
}
