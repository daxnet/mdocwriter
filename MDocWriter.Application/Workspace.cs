﻿namespace MDocWriter.Application
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;

    using MDocWriter.Documents;

    public sealed class Workspace
    {
        private readonly string workingDirectory;
        private readonly Document document;
        private WorkspaceStatus status = WorkspaceStatus.NewlyCreated;
        private bool isModified;

        private Workspace()
            : this(Path.GetTempPath(), new Document())
        {
        }

        private Workspace(string workingDirectory, Document document, bool attachDocumentEvent = true)
        {
            this.workingDirectory = workingDirectory;
            this.document = document;
            if (attachDocumentEvent)
                this.document.PropertyChanged += (s, e) => this.OnModified();
        }

        public event EventHandler Modified;

        public event EventHandler Saved;

        public string WorkingDirectory
        {
            get
            {
                return this.workingDirectory;
            }
        }

        public Document Document
        {
            get
            {
                return this.document;
            }
        }

        public WorkspaceStatus Status
        {
            get
            {
                return this.status;
            }
        }

        public bool IsModified
        {
            get
            {
                return this.isModified;
            }
        }

        private void OnModified()
        {
            var handler = this.Modified;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            this.isModified = true;
        }

        private void OnSaved()
        {
            var handler = this.Saved;
            if (handler!=null)
            {
                handler(this, EventArgs.Empty);
            }
            this.isModified = false;
        }

        public static Workspace Open(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                var workingDirectory = Path.GetTempPath();
                if (!Directory.Exists(workingDirectory)) Directory.CreateDirectory(workingDirectory);
                var serializer = new BinaryFormatter();
                var document = (Document)serializer.Deserialize(fileStream);
                // Extract the resources
                if (document.Resources != null &&
                    document.Resources.Any())
                {
                    Parallel.ForEach(
                        document.Resources,
                        resource => File.WriteAllBytes(
                            Path.Combine(workingDirectory, resource.FileName),
                            Convert.FromBase64String(resource.Base64Data)));
                }
                return new Workspace(workingDirectory, document) { status = WorkspaceStatus.Existing };
            }
        }

        public static void Save(string fileName, Workspace workspace)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(fileStream, workspace);
                workspace.OnSaved();
            }
        }

        public static Workspace New()
        {
            var newWorkspace = new Workspace();
            if (!Directory.Exists(newWorkspace.WorkingDirectory)) Directory.CreateDirectory(newWorkspace.WorkingDirectory);
            return newWorkspace;
        }
    }
}
