﻿using System;

namespace MDocWriter.Application.Workspaces
{
    /// <summary>
    /// Represents the setting values for a workspace.
    /// </summary>
    public class WorkspaceSettings
    {
        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        /// <value>
        /// The document title.
        /// </value>
        public string DocumentTitle { get; set; }

        /// <summary>
        /// Gets or sets the author of the document.
        /// </summary>
        /// <value>
        /// The document author.
        /// </value>
        public string DocumentAuthor { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>
        /// The template identifier.
        /// </value>
        public Guid TemplateId { get; set; }
    }
}
