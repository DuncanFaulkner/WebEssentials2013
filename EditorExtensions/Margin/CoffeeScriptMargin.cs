﻿using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Text;

namespace MadsKristensen.EditorExtensions
{
    internal class CoffeeScriptMargin : MarginBase
    {
        public const string MarginName = "CoffeeScriptMargin";
        private static NodeExecutorBase _compiler = new CoffeeScriptCompiler();

        protected virtual string ServiceName { get { return "CoffeeScript"; } }
        protected virtual NodeExecutorBase Compiler { get { return _compiler; } }

        public CoffeeScriptMargin(string contentType, string source, bool showMargin, ITextDocument document)
            : base(source, MarginName, contentType, showMargin, document)
        { }

        protected CoffeeScriptMargin(string contentType, string source, bool showMargin, ITextDocument document, string marginName)
            : base(source, marginName, contentType, showMargin, document)
        { }

        protected override async void StartCompiler(string source)
        {
            if (!CompileEnabled)
                return;

            string sourceFilePath = Document.FilePath;

            string jsFileName = GetCompiledFileName(sourceFilePath, ".js", CompileToLocation);

            if (IsFirstRun && File.Exists(jsFileName))
            {
                OnCompilationDone(File.ReadAllText(jsFileName), sourceFilePath);
                return;
            }

            Logger.Log(ServiceName + ": Compiling " + Path.GetFileName(sourceFilePath));

            var result = await Compiler.Compile(sourceFilePath, jsFileName);

            if (result.IsSuccess)
            {
                OnCompilationDone(result.Result, result.FileName);
            }
            else
            {
                result.Error.Message = ServiceName + ": " + result.Error.Message;

                CreateTask(result.Error);

                base.OnCompilationDone("ERROR:" + result.Error.Message, sourceFilePath);
            }
        }

        protected override void MinifyFile(string fileName, string source)
        {
            if (!CompileEnabled)
                return;

            if (WESettings.GetBoolean(WESettings.Keys.CoffeeScriptMinify))
            {
                FileHelpers.MinifyFile(fileName, source, ".js");
            }
        }

        public override bool CompileEnabled
        {
            get { return WESettings.GetBoolean(WESettings.Keys.CoffeeScriptEnableCompiler); }
        }

        public override string CompileToLocation
        {
            get { return WESettings.GetString(WESettings.Keys.CoffeeScriptCompileToLocation); }
        }

        public override bool IsSaveFileEnabled
        {
            get { return WESettings.GetBoolean(WESettings.Keys.GenerateJsFileFromCoffeeScript); }
        }

        protected override bool CanWriteToDisk(string source)
        {
            return !string.IsNullOrWhiteSpace(source);
        }
    }
}