﻿

namespace MDocWriter.Templates
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Xml.Serialization;

    using ICSharpCode.SharpZipLib.Zip;

    public sealed class TemplateReader
    {
        private readonly List<Template> templates = new List<Template>();

        public TemplateReader()
            : this(Template.TemplatePath, Template.TemplateFileSearchPattern)
        {
        }

        public TemplateReader(string templatePath, string templateFileSearchPattern)
        {
            var templateFiles = Directory.GetFiles(templatePath, templateFileSearchPattern, SearchOption.TopDirectoryOnly);
            foreach (var templateFile in templateFiles)
            {
                try
                {
                    using (var fileStream = File.OpenRead(templateFile))
                    {
                        var zipFile = new ZipFile(fileStream);
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            if (zipEntry.IsFile && zipEntry.Name == Template.TemplateDefinitionFileName)
                            {
                                using (var zipStream = zipFile.GetInputStream(zipEntry))
                                {
                                    var xmlSerializer = new XmlSerializer(typeof(Template));
                                    var template = (Template)xmlSerializer.Deserialize(zipStream);
                                    template.TemplateFileName = templateFile;
                                    this.templates.Add(template);
                                }
                            }
                        }
                    }
                }
                    // ReSharper disable EmptyGeneralCatchClause
                catch
                    // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }
        }

        public string GetTemplateContent(Template template)
        {
            if (template==null)
                throw new ArgumentNullException("template");
            if (string.IsNullOrEmpty(template.TemplateFileName))
                throw new ArgumentNullException("template.TemplateFileName");
            using (var fileStream = File.OpenRead(template.TemplateFileName))
            {
                var zipFile = new ZipFile(fileStream);
                var cssEntry = zipFile.GetEntry(template.TemplateFile);
                using (var reader = new StreamReader(zipFile.GetInputStream(cssEntry)))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public Image GetPreviewImage(Template template)
        {
            if (template==null)
                throw new ArgumentNullException("template");
            if (string.IsNullOrEmpty(template.TemplateFileName))
                throw new ArgumentNullException("template.TemplateFileName");
            using (var fileStream = File.OpenRead(template.TemplateFileName))
            {
                var zipFile = new ZipFile(fileStream);
                var previewImageEntry = zipFile.GetEntry(template.Preview);
                return Image.FromStream(zipFile.GetInputStream(previewImageEntry));
            }
        }

        public IEnumerable<Template> Templates
        {
            get
            {
                return this.templates;
            }
        }

    }
}