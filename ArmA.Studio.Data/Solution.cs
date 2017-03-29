﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Utility;
using Utility.Collections;

namespace ArmA.Studio.Data
{
    public class Solution : IXmlSerializable
    {
        public ObservableSortedCollection<Project> Projects { get; private set; }
        public Uri FileUri { get; set; }

        public Solution()
        {
            this.Projects = new ObservableSortedCollection<Project>();
        }

        #region Xml Serialization
        public static void Serialize(Solution solution, System.IO.Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Solution));
            serializer.Serialize(stream, solution);
        }
        public static Solution Deserialize(System.IO.Stream stream, Uri fileUri)
        {
            var serializer = new XmlSerializer(typeof(Solution));
            var sol = serializer.Deserialize(stream) as Solution;
            sol.Deserialize_RepairReferences();
            sol.FileUri = fileUri;
            return sol;
        }

        private void Deserialize_RepairReferences()
        {
            foreach (var proj in this.Projects)
            {
                proj.OwningSolution = this;
                foreach (var pff in proj)
                {
                    pff.OwningSolution = this;
                    pff.OwningProject = proj;
                }
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            //throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            //throw new NotImplementedException();
        }
        #endregion

        public ProjectFileFolder FindFileFolder(Uri uri)
        {
            foreach (var proj in this.Projects)
            {
                var ff = proj.FindFileFolder(uri);
                if (ff != null)
                    return ff;
            }
            return null;
        }

        /// <summary>
        /// Tries to find a <see cref="ProjectFileFolder"/> for provided ArmA-Path.
        /// will return null object if nothing was found.
        /// </summary>
        /// <param name="armaPath">ArmA Path of the <see cref="ProjectFileFolder"/> to find.</param>
        /// <returns>The correct <see cref="ProjectFileFolder"/> instance or null if no corresponding file was found.</returns>
        public ProjectFileFolder GetProjectFileFolderFromArmAPath(string armaPath)
        {
            foreach (var project in this.Projects)
            {
                if (armaPath.StartsWith(project.ArmAPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var pff in project)
                    {
                        if (armaPath.Equals(pff.ArmAPath, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return pff;
                        }
                    }
                    break;
                }
            }
            return null;
        }
    }
}