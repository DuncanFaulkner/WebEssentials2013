﻿using System.Text;
using MadsKristensen.EditorExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebEssentialsTests.Tests.IntellisenseGeneration
{
    [TestClass]
    public class IntellisenseTypescript_with_primitives
    {
        private readonly IntellisenseType _stringType = new IntellisenseType { CodeName = "String" };
        private readonly IntellisenseType _int32Type = new IntellisenseType { CodeName = "Int32" };
        private readonly IntellisenseType _int32ArrayType = new IntellisenseType { CodeName = "Int32", IsArray = true };
        private readonly IntellisenseType _simpleType = new IntellisenseType { CodeName = "Foo.Simple", ClientSideReferenceName = "server.Simple" };

        [TestMethod]
        public void TypeScript_with_on_string_property()
        {
            var result = new StringBuilder();

            var io = new IntellisenseObject(new[]
            {
                new IntellisenseProperty(_stringType, "AString")
            })
            {
                FullName = "Foo.Primitives",
                Name = "Primitives",
                Namespace = "server"
            };
            IntellisenseWriter.WriteTypeScript(new[] { io }, result);

            result.ShouldBeCode(@"
declare module server {
       interface Primitives {
        AString: string;
    }
}");
        }
        [TestMethod]
        public void TypeScript_with_a_string_an_int_and_and_int_arrayproperty()
        {
            var result = new StringBuilder();

            var io = new IntellisenseObject(new[]
            {
                new IntellisenseProperty(_stringType, "AString"),
                new IntellisenseProperty(_int32Type, "AnInt"),
                new IntellisenseProperty(_int32ArrayType, "AnIntArray") { Summary = "ASummary"},
                new IntellisenseProperty(_simpleType, "TheSimple")
            })
            {
                FullName = "Foo.Primitives",
                Name = "Primitives",
                Namespace = "server"
            };
            IntellisenseWriter.WriteTypeScript(new[] { io }, result);

            result.ShouldBeCode(@"
declare module server {
       interface Primitives {
        AString: string;
        AnInt: number;
/** ASummary */
        AnIntArray: number[];
        TheSimple: server.Simple;
}
}");
        }

    }
}