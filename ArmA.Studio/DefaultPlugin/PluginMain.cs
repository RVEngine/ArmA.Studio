﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArmA.Studio.Data;
using ArmA.Studio.Data.Lint;
using ArmA.Studio.Data.UI;
using ArmA.Studio.Plugin;

namespace ArmA.Studio.DefaultPlugin
{
    internal sealed class PluginMain : IDocumentProviderPlugin, IHotKeyPlugin
    {
        public string Description => Properties.Localization.DefaultPluginDescription;
        public string Name => "ArmA.Studio";

        public static readonly FileType SqfFileType = new FileType("SQF", (ext) => true, ".sqf") { Linter = new SqfLintHelper(), FileTemplate = @"/*
 * @Author: 
 * 
 * @Description: 
 * 
 * @Arguments: -/-
 * @Return: -/-
 */
params [];" };
        public static readonly DocumentBase.DocumentDescribor SqfDocumentDescribor = new DocumentBase.DocumentDescribor(new[] { SqfFileType }, "SQF");

        public static readonly FileType ConfigFileType = new FileType("Config", (ext) => true, ".cpp", ".hpp", ".ext") { Linter = new ConfigLintHelper(), FileTemplate = @"/*
 * @Author: 
 * 
 * @Purpose: 
 *
 */" };
        public static readonly FileType ConfigCppFileType = new FileType("Config.cpp", (ext) => true, ".cpp") { Linter = new ConfigLintHelper(), StaticFileName = "config.cpp", FileTemplate = @"class CfgPatches
{
    class TAG_ModName
    {
        units[] = {};
        weapons[] = {};
        requiredVersion = 1.68;
        requiredAddons[] = {};
        author = "";
        mail = "";
        url = "";
    };
};" };
        public static readonly FileType DescriptionExtFileType = new FileType("description.ext", (ext) => true, ".ext") { Linter = new ConfigLintHelper(), StaticFileName = "description.ext" };
        public static readonly DocumentBase.DocumentDescribor ConfigDocumentDescribor = new DocumentBase.DocumentDescribor(new[] { ConfigFileType }, "Config");

        public static readonly FileType ImageViewerFileType = new FileType("Image", (ext) => true, ".png", ".paa", "jpeg", "jpe", "jpg", "tga") { CanCreate = false };
        public static readonly DocumentBase.DocumentDescribor ImageViewerDescribor = new DocumentBase.DocumentDescribor(new[] { ImageViewerFileType }, "Image");

        public IEnumerable<DataTemplate> DocumentDataTemplates => new[] { TextEditorBaseDataContext.TextEditorBaseDataTemplate, ImageViewerDocument.ImageViewerDocumentDataTemplate };
        public IEnumerable<DocumentBase.DocumentDescribor> Documents => new[] { SqfDocumentDescribor, ConfigDocumentDescribor, ImageViewerDescribor };
        public IEnumerable<FileType> FileTypes => new[] { SqfFileType, ConfigFileType, ConfigCppFileType, DescriptionExtFileType, ImageViewerFileType };

        public DocumentBase CreateDocument(DocumentBase.DocumentDescribor describor)
        {
            if (describor == SqfDocumentDescribor)
            {
                return new SqfDocument();
            }
            else if (describor == ConfigDocumentDescribor)
            {
                return new ConfigDocument();
            }
            else if(describor == ImageViewerDescribor)
            {
                return new ImageViewerDocument();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public DocumentBase CreateDocument(FileType type)
        {
            if (type == SqfFileType)
            {
                return new SqfDocument();
            }
            else if (type == ConfigFileType || type == ConfigCppFileType || type == DescriptionExtFileType)
            {
                return new ConfigDocument();
            }
            else if (type == ImageViewerFileType)
            {
                return new ImageViewerDocument();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<KeyContainer> GetGlobalHotKeys()
        {
            yield return new KeyContainer(Properties.Localization.Hotkey_SaveCurrentDocument, new Key[] { Key.LeftCtrl, Key.S }, (p) => Workspace.Instance.CmdSave.Execute(null));
        }
    }
}
